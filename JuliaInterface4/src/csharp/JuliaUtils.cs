using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            Regex rg = new Regex("JULIAPPPATH(.+)JULIAPPPATH");
            var match = rg.Match(location);

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] public static unsafe T* ToPointer<T>(this Span<T> s) where T: unmanaged => (T*) Unsafe.AsPointer(ref s.GetPinnableReference());
    }
}
