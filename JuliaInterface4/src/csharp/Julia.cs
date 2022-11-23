using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

//Written by Johnathan Bizzano 
namespace JULIAdotNET
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

    public class Julia {
        private static volatile bool _isInitialized;
        private static string _juliaDir;
        private static volatile bool _previosulyLoaded;
        private static object _gclock = new();

        public static string JuliaDir {
            get {
                if (_juliaDir != null)
                    return _juliaDir;

                _juliaDir = JLUtils.GetJuliaDir();
                return _juliaDir;
            }
            set { _juliaDir = value; }
        }

        public static bool IsInitialized { get => _isInitialized; }
        public static string LibDirectory { get => MString(JuliaCalls.jl_get_libdir()); }
        public static bool IsInstalled { get => JuliaDir != null; }
        public static bool PreviouslyLoaded { get => _previosulyLoaded; }

        public static void Init() => Init(new JuliaOptions());

        public static void Init(JuliaOptions options) => Init(options, true);

        internal static void Init(JuliaOptions options, bool sharpInit) {
            if (_isInitialized) return;
            _isInitialized = true;

            if (PreviouslyLoaded)
                throw new InvalidOperationException("Cannot Close And Reopen Julia in the Same Process");

            options.BuildArguments();
            var env = Environment.CurrentDirectory;
            Environment.CurrentDirectory = options.JuliaDirectory;
            JuliaDll.Open();
            JuliaBoot.jl_init_code(options, sharpInit);
            Environment.CurrentDirectory = env;
            _previosulyLoaded = true;
        }
        
        public static bool Isa(JuliaV v, JuliaV t) => JuliaCalls.jl_isa(v, t) != 0;

        public static void SetGlobal(JuliaV m, string sym, JuliaV val) => SetGlobal(m, JuliaCalls.jl_symbol(sym), val);
        public static void SetGlobal(JuliaV m, JuliaV sym, JuliaV val){
            JuliaCalls.jl_set_global(m, sym, val);
            CheckExceptions();
        }

        public static JuliaV GetGlobal(JuliaV m, JuliaV sym)
        {
            var val = JuliaCalls.jl_get_global(m, sym);
            CheckExceptions();
            return val;
        }
        
        public static JuliaV GetGlobal(JuliaV m, string sym) => GetGlobal(m, JuliaCalls.jl_symbol(sym));

        public static void CheckExceptions() {
            if (JuliaCalls.jl_exception_occurred() != IntPtr.Zero)
                throw new JuliaException(JuliaCalls.jl_exception_occurred());
        }

        public static void Exit(int code = 0)
        {
            if (!IsInitialized) return;
            _isInitialized = false;
            JuliaCalls.jl_atexit_hook(code);
            JuliaDll.Close();
        }

        public static IntPtr Eval(string str) {
            var val = JuliaCalls.jl_eval_string(str);
            CheckExceptions();
            return val;
        }

        public static void PUSH_GC(Span<JuliaV> values){
            lock (_gclock) JuliaGC.JL_GC_PUSHARGS(values);
        }

        public static void POP_GC(){
            lock (_gclock) JuliaGC.JL_GC_POP();
        }

        public static string UnboxString(JuliaV val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_string_ptr(val));
        public static string TypeNameStr(JuliaV val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typename_str(val));
        public static string TypeOfStr(JuliaV val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typeof_str(val));
        public static JuliaV BoxPtr(IntPtr ptr) => new(JuliaCalls.jl_box_voidpointer(ptr));
        public static unsafe JuliaV AllocStruct(JuliaV type, Span<JuliaV> vals) => JuliaCalls.jl_new_structv(type, vals.ToPointer(), (uint) vals.Length);

        internal static string MString(IntPtr p) {
            CheckExceptions();
            return Marshal.PtrToStringAnsi(p);
        }
        
    }


    public class CSharp{
#if NET
        [UnmanagedCallersOnly]
#endif
        public static int Init(IntPtr julia_bindir){
            var jo = new JuliaOptions();
            jo.JuliaDirectory = Marshal.PtrToStringUni(julia_bindir);
            Julia.Init(jo, false);
            return 0;
        }
    }
}