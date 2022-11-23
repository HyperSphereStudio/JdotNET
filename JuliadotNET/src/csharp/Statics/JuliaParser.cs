using System;
using JULIAdotNET;
using Base;

namespace JuliadotNET.csharp.Statics
{
    public class JuliaParser {
        
        public static void GenerateStaticLibrary(Any module, string libPath) {
            
            var names = JPrimitive.NamesF.Invoke(module);
            foreach(var name in names) {
                Console.WriteLine(name);
            }
            
        }
        
    }
}