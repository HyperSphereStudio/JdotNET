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

            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };

            myModule.GetFunction("add!").Invoke(new Any(m1), new Any(m2));
            string.Join(",", m1).Println();

            Julia.Exit();
        }catch (Exception e) {
            e.PrintExp();
        }
    }
}