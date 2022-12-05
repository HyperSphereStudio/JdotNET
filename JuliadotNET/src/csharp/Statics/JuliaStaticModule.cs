using Base;
using JULIAdotNET;
using runtime.core;
using runtime.ILCompiler;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JuliadotNET
{
    internal class JuliaStaticModule {
        public Any Module;
        public ILTypeBuilder ModuleBuilder;
        public readonly FieldBuilder JModuleField, JParentModule, ParentModuleTy;
        private static readonly FieldInfo MainModuleField = SharpReflect.GetField(typeof(JPrimitive), "MainM");

        public JuliaStaticModule(Any module, ILTypeBuilder moduleBuilder, FieldBuilder parentModuleField) {
            var ti = moduleBuilder.TypeInitializer;
            Module = module;
            ModuleBuilder = moduleBuilder;
            JModuleField = moduleBuilder.CreateField("Module", typeof(Any), true, true);
            JParentModule = moduleBuilder.CreateField("ParentModule", typeof(Any), true, true);
            ParentModuleTy = moduleBuilder.CreateField("ParentModuleType", typeof(Type), true, true);

            if (parentModuleField != null) {
                //ParentModule = ParentModuleType.Module;
                //ParentModuleTy = typeof(ParentModuleType);
                ti.Load.FieldValue(parentModuleField);
                ti.Load.Type(parentModuleField.DeclaringType);
            }else{
                //ParentModule = JPrimitive.MainM;
                //ParentModuleTy = typeof(thistype);
                ti.Load.FieldValue(MainModuleField);
                ti.Load.Type(MainModuleField.DeclaringType);
            }
           
            ti.Store.Field(ParentModuleTy);
            ti.Store.Field(JParentModule);

           //  ti.Load.Const(module.ToString());
           // ti.Function.Invoke(getglobalstr);
           // ti.Store.Field(JModuleField);
        }
    }
    
    public partial class JuliaStaticLibrary
    {
        
        private ILTypeBuilder GenerateModule(JuliaStaticModule mod, bool isFirst)
        {
            try {
                DecorateWithDefAttribute(mod.ModuleBuilder.InternalBuilder, JModuleAttribute.DEF_CON);

                foreach (var name in JPrimitive.namesF.Invoke(mod.Module)) {
                    if (JuliaCalls.jl_is_const(mod.Module, name) != 0) {
                        var val = Julia.GetGlobal(mod.Module, name);
                        if (val.Is(JPrimitive.FunctionT))
                            GenerateFunction(mod, name, val);
                        else if (val.Is(JPrimitive.ModuleT) && val != mod.Module) {
                            var mb = mod.ModuleBuilder.InternalBuilder.DefineNestedType(ConvertJuliaName(name.ToString()), TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);
                            GenerateModule(new(val, new(mb), mod.JModuleField), false);
                        }
                        else if (val.Is(JPrimitive.TypeT) && IsNETRepresentable(val))
                             GenerateType(mod, name, val);
                        else
                            GenerateVar(mod, name, true);
                    }
                    else
                        GenerateVar(mod, name, false);
                }
                
                if(!isFirst)
                    mod.ModuleBuilder.Create();
                return mod.ModuleBuilder;
            }
            catch (Exception) {
                Console.WriteLine("Caught Error While Generating Module:" + mod.Module);
                throw;
            }
        }
        
    }
}