using JULIAdotNET;
using Base;
using runtime.ILCompiler;
using System.Reflection;
using System.Reflection.Emit;

namespace JuliadotNET.csharp.Statics {
    public class JuliaParser {
        
        public static void GenerateStaticLibrary(Any module, string libPath) {
            var modName = module.ToString();
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(modName), AssemblyBuilderAccess.Run);
            GenerateModule(asm.DefineDynamicModule(modName), module);
            new Lokad.ILPack.AssemblyGenerator().GenerateAssembly(asm, libPath);
        }


        private static void GenerateModule(ModuleBuilder mb, Any module) {
            var modTy = new ILTypeBuilder(mb.DefineType(module.ToString(), TypeAttributes.Class));
            
            foreach(var name in JPrimitive.NamesF.Invoke(module)) {
                 
            }
            
            modTy.Create();
        }
    }
}