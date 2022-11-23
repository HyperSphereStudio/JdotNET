using System;
using JULIAdotNET;
using Base;

namespace Sandbox
{
    class Program {
        static void Main(string[] args) {
            try {
                Julia.Init();
                Console.WriteLine(new Any(24) * 4 + 8);
                Julia.Exit();
            }catch (Exception e) {
                e.Print();
            }
        }
    }
}