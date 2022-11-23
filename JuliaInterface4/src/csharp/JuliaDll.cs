using System;
using System.Runtime.InteropServices;

namespace JULIAdotNET
{
    public class JuliaDll
    {
        private static IntPtr JuliaLib;
        
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lib);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void FreeLibrary(IntPtr module);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr module, string proc);


        internal static void Open() => LoadLibrary("libjulia.dll");

        public static IntPtr GetFunction(string name) => JuliaLib == IntPtr.Zero ? IntPtr.Zero : GetProcAddress(JuliaLib, name);

        internal static void Close() {
            FreeLibrary(JuliaLib);
            JuliaLib = IntPtr.Zero;
        }

    }
}