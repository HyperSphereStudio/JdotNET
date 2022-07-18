using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JuliaInterface
{
    public class JLUtils
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
    }
}
