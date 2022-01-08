using System;
using System.Diagnostics;

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
        }

        public static void Exit(int code) => JuliaCalls.jl_atexit_hook(code);


        public static JLVal Eval(string str) => JuliaCalls.jl_eval_string(str);

        public static JLFun GetFunction(JLModule mod, string fun) => JuliaCalls.jl_get_function(mod, JuliaCalls.jl_symbol(fun));
    }
}
