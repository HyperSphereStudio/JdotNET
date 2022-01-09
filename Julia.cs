using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    public class Julia
    {
        private static string GetJuliaDir()
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
                if (location.Contains("julia.exe"))
                    return location.Substring(1, location.Length - 11);
            }
            throw new Exception("Julia Path Not Found");
        }

        public static void Init(){
            JuliaCalls.SetDllDirectory(GetJuliaDir());
            JuliaCalls.jl_init();

            JLModule.init_mods();
            JLType.init_types();
            JLFun.init_funs();
        }

        public static void Exit(int code) => JuliaCalls.jl_atexit_hook(code);


        public static JLVal Eval(string str) => JuliaCalls.jl_eval_string(str);

        public static JLFun GetFunction(JLModule mod, string fun) => JuliaCalls.jl_get_function(mod, JuliaCalls.jl_symbol(fun));

        public static void POPGC() => JuliaCalls.JL_GC_POP();
        public static void PUSHGC(IntPtr p) => JuliaCalls.JL_GC_PUSH1(p);
        public static void PUSHGC(IntPtr p, IntPtr p2) => JuliaCalls.JL_GC_PUSH2(p, p2);
        public static void PUSHGC(IntPtr p, IntPtr p2, IntPtr p3) => JuliaCalls.JL_GC_PUSH3(p, p2, p3);
        public static unsafe void PUSHGC(params IntPtr[] p) {
            fixed(void** arr = ConvertPointerArray(p)){
                JuliaCalls._JL_GC_PUSHARGS(new IntPtr(arr), CUintptr(p.Length));
            }
        }





        internal static UIntPtr CUintptr<T>(T t) => new UIntPtr((uint)(object)t);

        internal static unsafe void*[] ConvertPointerArray(params IntPtr[] p)
        {
            var data = new void*[p.Length];
            for (var i = 0; i < p.Length; ++i)
                data[i] = p[i].ToPointer();
            return data;
        }

        internal static unsafe void*[] ConvertPointerArray(params JLVal[] p)
        {
            var data = new void*[p.Length];
            for (var i = 0; i < p.Length; ++i)
                data[i] = p[i].ptr.ToPointer();
            return data;
        }
    }
}
