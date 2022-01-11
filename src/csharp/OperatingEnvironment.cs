using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    public abstract class OperatingEnvironment
    {

        public OperatingEnvironment() { }

        public abstract string GetWhereExe();
        public abstract string TrimJuliaPath(string s);

        public static OperatingEnvironment GetEnvironment(){
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsEnvironment();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxEnvironment();
            else throw new Exception("Unsupported Operating System!");
        }
    }

    public class WindowsEnvironment : OperatingEnvironment
    {

        public override string GetWhereExe() => "where.exe";

        public override string TrimJuliaPath(string s) => s.Substring(0, s.Length - 10);
    }

    public class LinuxEnvironment : OperatingEnvironment
    {
        public override string GetWhereExe() => "which";

        public override string TrimJuliaPath(string s) => s;
    }

    
}
