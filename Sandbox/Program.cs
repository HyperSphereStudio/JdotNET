using System;
using JULIAdotNET;

namespace Sandbox
{
    class Program {
        static void Main(string[] args) {
            try {
                Julia.Init();
                
                Julia.Exit();
            }catch (Exception e) {
                e.Print();
            }
        }
    }
}