using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JULIAdotNET
{
    public static class JLUtils
    {
        private static Dictionary<Type, bool> cachedTypes = new Dictionary<Type, bool>();

        public static bool IsUnManaged(this Type t){
            var result = false;
            if (cachedTypes.ContainsKey(t))
                return cachedTypes[t];
            else if (t.IsPrimitive || t.IsPointer || t.IsEnum)
                result = true;
            else if (t.IsGenericType || !t.IsValueType)
                result = false;
            else
                result = t.GetFields(BindingFlags.Public |
                   BindingFlags.NonPublic | BindingFlags.Instance)
                    .All(x => IsUnManaged(x.FieldType));
            cachedTypes.Add(t, result);
            return result;
        }

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
