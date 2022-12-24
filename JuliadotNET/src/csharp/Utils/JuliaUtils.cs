using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace JULIAdotNET
{
    public static class JLUtils
    {
        internal static string GetJuliaDir()
        {
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
            Regex rg = new("JULIAPPPATH(.+)JULIAPPPATH");
            var match = rg.Match(location);

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] public static unsafe T* ToPointer<T>(this Span<T> s) where T: unmanaged => (T*) Unsafe.AsPointer(ref s.GetPinnableReference());
        
        public static void PrintExp(this Exception x, TextWriter tw = null) {
            tw ??= Console.Out;
            tw.WriteLine(x.GetBaseException());
            
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            
            for(int i = 0; i < frames.Length; i ++) {
                var frame = frames[i];
                if (frame.GetFileLineNumber() < 1)
                    continue;
                tw.Write("File: " + frame.GetFileName());
                tw.Write(", Method:" + frame.GetMethod().Name);
                tw.Write(", LineNumber: " + frame.GetFileLineNumber());
                if (i == frames.Length - 1) {
                    tw.WriteLine();
                    break; 
                }
                tw.WriteLine("  -->  ");
            }
        }

        public static void Print<T>(this T o) => Console.Write(o);
        public static void Println<T>(this T o) => Console.WriteLine(o);
    }
}
