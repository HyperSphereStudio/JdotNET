using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
 

    public class Julia
    {
        public string LibDirectory { get => MString(JuliaCalls.jl_get_libdir()); }

        public static void Init()
        {
            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "where.exe",
                    Arguments = "Julia",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string location = proc.StandardOutput.ReadLine();
                if (location.Contains("julia.exe")){
                    Init(location.Substring(1, location.Length - 11));
                    return;
                }
                    
            }
            throw new Exception("Julia Path Not Found");
        }

        

        public static void Init(string dir){
            JuliaCalls.SetDllDirectory(dir);
            JuliaCalls.jl_init();
            JuliaCalls.jl_eval_string(System.Text.Encoding.UTF8.GetString(Resource1.JuliaInterface));
            JLModule.init_mods();
            JLType.init_types();
            JLFun.init_funs();
            NativeSharp.init();
        }

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

        public static void CheckExceptions(){
            if(JuliaCalls.jl_exception_occurred().ptr.ToInt64() != 0)
                throw new JuliaException(JuliaCalls.jl_exception_occurred());
        }

        public static void Exit(int code) => JuliaCalls.jl_atexit_hook(code);
        public static JLVal Eval(string str){
            var val = JuliaCalls.jl_eval_string(str);
            CheckExceptions();
            return val;
        }

        public static JLFun GetFunction(JLModule mod, string fun) => GetGlobal(mod, fun);

        public static void POPGC() => JuliaCalls.JL_GC_POP();
        public static void PUSHGC(IntPtr p) => JuliaCalls.JL_GC_PUSH1(p);
        public static void PUSHGC(IntPtr p, IntPtr p2) => JuliaCalls.JL_GC_PUSH2(p, p2);
        public static void PUSHGC(IntPtr p, IntPtr p2, IntPtr p3) => JuliaCalls.JL_GC_PUSH3(p, p2, p3);
        public static void PUSHGC(params IntPtr[] p) => JuliaCalls._JL_GC_PUSHARGS(p, p.Length);
        public static string UnboxString(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_string_ptr(val));
        public static string TypeNameStr(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typename_str(val));
        public static string TypeOfStr(JLVal val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_typeof_str(val));

        public static JLVal BoxPtr(IntPtr ptr) => new JLVal(JuliaCalls.jl_box_voidpointer(ptr));

        public static JLVal CreateStruct(JLType type, params JLVal[] vals) => JuliaCalls.jl_new_structv(type, vals, (uint) vals.Length);

        internal static string MString(IntPtr p){
            CheckExceptions();
            return Marshal.PtrToStringAnsi(p);
        }
    }
}
