using JULIAdotNET;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Julia.Init();
            Console.WriteLine(Julia.Eval("1+1"));
            Julia.Exit(1);
        }
    }
}