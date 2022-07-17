using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    public class JuliaOptions
    {
        public string JuliaDirectory;
        public List<string> Arguments = new List<string>();
        public int ThreadCount = 1;
        public string EvalStr;

        public void Add(params object[] args) {
            foreach (var arg in args)
                Arguments.Add(arg.ToString());
        }

        internal void BuildArguments()
        {
            Add("");

            if (ThreadCount != 1)
                Add("-t", ThreadCount);

            if (EvalStr != null)
                Add("-e", EvalStr);

            JuliaDirectory = JuliaDirectory == null ? Julia.JuliaDir : JuliaDirectory;

            if (JuliaDirectory == null)
                throw new Exception("Julia Path Not Found");
        }
    }

    public class Julia
    {
        private static volatile bool _IsInitialized = false;
        private static string _JuliaDir;
        private static volatile bool _PreviosulyLoaded = false;
        private static object _gclock = new object();
        private static unsafe JuliaCalls.JLGCFrame* lastFrame;

        public static string JuliaDir {
            get {
                if (_JuliaDir != null)
                    return _JuliaDir;

                var proc = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "julia",
                        Arguments = "-e \"println(\"\"JULIAPPPATH$(Sys.BINDIR)JULIAPPPATH\"\")\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                var location = proc.StandardOutput.ReadToEnd();
                Regex rg = new Regex("JULIAPPPATH(.+)JULIAPPPATH");
                var match = rg.Match(location);

                if (match.Success)
                    _JuliaDir = match.Groups[1].Value;

                return _JuliaDir;
            }
            set { _JuliaDir = value; }
        }


        public static bool IsInitialized { get => _IsInitialized; }
        public static string LibDirectory { get => MString(JuliaCalls.jl_get_libdir()); }
        public static bool IsInstalled { get => JuliaDir != null; }
        public static bool PreviouslyLoaded { get => _PreviosulyLoaded; }

        public static string WorkingDirectory {
            get => (string)JLFun.PWDF.Invoke();
            set => JLFun.CDF.Invoke(value.Replace("\\", "\\\\"));
        }


        public static void Init() => Init(new JuliaOptions());
        
        public static void Init(JuliaOptions options)
        {
            if (_IsInitialized) return;
            _IsInitialized = true;

            if (PreviouslyLoaded)
                throw new InvalidOperationException("Cannot Close And Reopen Julia in the Same Process");

            options.BuildArguments();
            var env = Environment.CurrentDirectory;
            Environment.CurrentDirectory = options.JuliaDirectory;
            JuliaDll.Open();
            jl_init_code(options);
            NativeSharp.init();
            ObjectCollector.init();
            Environment.CurrentDirectory = env;
            Eval("using Main.JuliaInterface");

            unsafe
            {
                lastFrame = JuliaCalls.jl_get_pgcstack();
            }
            
            _PreviosulyLoaded = true;
        }


        private static void jl_init_code(JuliaOptions options)
        {
            var arguments = options.Arguments.ToArray();
            if (arguments != null && arguments.Length != 0)
            {
                int len = arguments.Length;
                unsafe
                {
                    byte*[] stringBytes = new byte*[arguments.Length];
                    GCHandle[] handles = new GCHandle[arguments.Length];

                    for (int i = 0; i < arguments.Length; ++i)
                    {
                        handles[i] = GCHandle.Alloc(Encoding.ASCII.GetBytes(arguments[i]), GCHandleType.Pinned);
                        stringBytes[i] = (byte*)handles[i].AddrOfPinnedObject();
                    }

                    fixed (byte** ANSIPTR = stringBytes)
                        JuliaCalls.jl_parse_opts(ref len, &ANSIPTR);

                    foreach (var handle in handles)
                        handle.Free();
                }
            }

            JuliaCalls.jl_init();
            JuliaCalls.jl_eval_string(Encoding.UTF8.GetString(Resource1.JuliaInterface));
            JLModule.init_mods();
            JLType.init_types();
            JLFun.init_funs();
        }

        public static bool Isa(JLVal v, JLType t) => JuliaCalls.jl_isa(v, t) != 0;

        public static JLGCStub PinGC(IntPtr val) => ObjectCollector.PushJL(val);

        public static void SetGlobal(JLModule m, JLSym sym, JLVal val)
        {
            JuliaCalls.jl_set_global(m, sym, val);
            CheckExceptions();
        }

        public static JLVal GetGlobal(JLModule m, JLSym sym)
        {
            var val = JuliaCalls.jl_get_global(m, sym);
            CheckExceptions();
            return val;
        }

        public static void CheckExceptions()
        {
            if (JuliaCalls.jl_exception_occurred().ptr.ToInt64() != 0)
                throw new JuliaException(JuliaCalls.jl_exception_occurred());
        }

        public static void Exit(int code)
        {
            if (!IsInitialized) return;
            _IsInitialized = false;
            JuliaCalls.jl_atexit_hook(code);
            NativeSharp.destroy();
            JuliaDll.Close();
        }

        public static JLVal Eval(string str)
        {
            var val = JuliaCalls.jl_eval_string(str);
            CheckExceptions();
            return val;
        }

        public static JLFun GetFunction(JLModule mod, string fun) => GetGlobal(mod, fun);
        
        public static void PUSH_GC(params JLVal[] values){
            unsafe{
                lock (_gclock) lastFrame = JuliaCalls.JLGCFrame.createNewFrame(lastFrame, values);
            }
        }

        public static void POP_GC(){
            unsafe{
                lock (_gclock) lastFrame = lastFrame->pop();
            }
        }

        public static string UnboxString(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_string_ptr(val));
        public static string TypeNameStr(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typename_str(val));
        public static string TypeOfStr(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typeof_str(val));
        public static JLVal BoxPtr(IntPtr ptr) => new JLVal(JuliaCalls.jl_box_voidpointer(ptr));

        public static JLVal AllocStruct(JLType type, params JLVal[] vals) => JuliaCalls.jl_new_structv(type, vals, (uint)vals.Length);

        internal static string MString(IntPtr p)
        {
            CheckExceptions();
            return Marshal.PtrToStringAnsi(p);
        }
    }
}