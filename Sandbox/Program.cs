using JuliadotNET;
using System;
using JULIAdotNET;

namespace Sandbox
{
    class Program {
        static void Main(string[] args) {
            try {
                Julia.Init();
                new JuliaStaticLibrary(JPrimitive.BaseM).Generate("Base.dll");
                Julia.Exit();
            }catch (Exception e) {
                e.PrintExp();
            }
        }
    }
}