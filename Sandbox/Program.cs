using Base;
using System;
using JULIAdotNET;

namespace Sandbox;

class Program {
    static void Main(){
        try {
            var jo = new JuliaOptions();
            // jo.LoadSystemImage = "my_sys_image_path";
            Julia.Init(jo);
 
            JModule myModule = Julia.Eval(@"
                    module T 
                        add!(m1, m2) = m1 .+= m2
                    end
                    using Main.T
                    return T");

            var m1 = new[,] { {2, 3, 4}, {8, 9, 10} };
            var m2 = new[,] { {1, 2, 3}, {4, 5, 6} };
            var j1 = new Any(m1);
            var j2 = new Any(m2);
            
            Julia.PUSH_GC(j1, j2);
            myModule.GetFunction("add!").Invoke(j1, j2).Println();
            j1.Println();
            Julia.POP_GC();
            
            Julia.Exit();
        }catch (Exception e) {
            e.PrintExp();
        }
    }
}