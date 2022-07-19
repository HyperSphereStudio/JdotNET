using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    public class JuliaOptions
    {
        public string JuliaDirectory;
        public List<string> Arguments = new List<string>();
        
        public int ThreadCount = 1;
        public int WorkerCount = 1;
        public int Optimize = 2;
        public string LoadSystemImage;
        public string EvaluationString;
        public bool UseSystemImageNativeCode = true;
        public bool HandleSignals = true;
        public bool PrecompileModules = true;

        public void Add(params object[] args) {
            foreach (var arg in args)
                Arguments.Add(arg.ToString());
        }

        private string AsJLString(bool b) => b ? "yes" : "no";

        internal void BuildArguments()
        {
            Add("");

            if (ThreadCount != 1)
                Add("-t", ThreadCount);

            if (WorkerCount != 1)
                Add("-p", WorkerCount);

            if (Optimize != 2)
                Add("-O", Optimize);

            if (EvaluationString != null)
                Add("-e", EvaluationString);

            if (LoadSystemImage != null)
                Add("-J", LoadSystemImage);

            if (!UseSystemImageNativeCode)
                Add("--sysimage-native-code=", AsJLString(UseSystemImageNativeCode));

            if (!PrecompileModules)
                Add("--compiled-modules=", AsJLString(PrecompileModules));

            if(!HandleSignals)
                Add("--handle-signals =", AsJLString(PrecompileModules));

            if (JuliaDirectory != null)
                Julia.JuliaDir = JuliaDirectory;
            else JuliaDirectory = Julia.JuliaDir;

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
        internal static string juliaInterfaceModuleName = "Main.JuliaInterface";

        public static string JuliaDir {
            get {
                if (_JuliaDir != null)
                    return _JuliaDir;

                _JuliaDir = JLUtils.GetJuliaDir();
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

        public static void Init(JuliaOptions options) => Init(options, true);

        internal static void Init(JuliaOptions options, bool sharpInit)
        {
            if (_IsInitialized) return;
            _IsInitialized = true;

            if (PreviouslyLoaded)
                throw new InvalidOperationException("Cannot Close And Reopen Julia in the Same Process");

            options.BuildArguments();
            var env = Environment.CurrentDirectory;
            Environment.CurrentDirectory = options.JuliaDirectory;
            JuliaDll.Open();
            jl_init_code(options, sharpInit);
            NativeSharp.init();
            ObjectCollector.init();
            Environment.CurrentDirectory = env;

            if(sharpInit)
                Eval("using Main.JuliaInterface");

            unsafe
            {
                lastFrame = JuliaCalls.jl_get_pgcstack();
            }
            _PreviosulyLoaded = true;
        }


        private static void jl_init_code(JuliaOptions options, bool sharpInit)
        {
            if (sharpInit)
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
            }
            else
                juliaInterfaceModuleName = "JULIAdotNET.JuliaInterface";

            JLModule.init_mods();
            JLType.init_types();
            JLFun.init_funs();
        
            if(sharpInit)
                Eval(Encoding.UTF8.GetString(Resource1.JuliaInterface), "JuliaInterface.jl");

            JLModule.finish_init_mods();
            JLType.finish_init_types();
            JLFun.finish_init_funs();
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

        public static JLVal Eval(string str, string filename = null){
            JLVal val = filename == null ? JuliaCalls.jl_eval_string(str) : JLFun.LinedEval.Invoke(str, filename, JLModule.Main);
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

    public class CSharp
    {
        [UnmanagedCallersOnly]
        public static int Init(IntPtr julia_bindir){
            var jo = new JuliaOptions();
            jo.JuliaDirectory = Marshal.PtrToStringUni(julia_bindir);
            Julia.Init(jo, false);
            return 0;
        }
    }
}