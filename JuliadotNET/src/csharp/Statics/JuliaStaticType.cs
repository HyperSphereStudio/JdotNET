using Base;
using JULIAdotNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JuliadotNET
{
    public partial class JuliaStaticLibrary {
        
        private void GenerateType(JuliaStaticModule mod, Any name, JType type) {
            try
            {
                var mb = mod.ModuleBuilder;
                var typename = name.ToString();
                if (mb.InternalBuilder.Module.GetType(typename) != null) return;
                var typetype = type.Type;
                TypeBuilder tb;

                switch (typetype)
                {
                    case JTypeType.MutableStruct:
                        int field_count = type.FieldCount;
                        tb = mb.InternalBuilder.DefineNestedType(typename,
                            TypeAttributes.NestedPublic | TypeAttributes.Class);
                        DecorateWithDefAttribute(tb, JMutableStructAttribute.DEF_CON);
                        for (int i = 0; i < field_count; i++)
                        {
                            type.FieldType(i).Name.Println();
                            var fb = tb.DefineField(type.FieldName(i),
                                mb.InternalBuilder.Module.GetType(type.FieldType(i).Name), FieldAttributes.Public);
                        }

                        break;
                    case JTypeType.Abstract:
                        tb = mb.InternalBuilder.DefineNestedType(typename,
                            TypeAttributes.NestedPublic | TypeAttributes.Abstract);
                        JuliaStaticLibrary.DecorateWithDefAttribute(tb, JAbstractTypeAttribute.DEF_CON);
                        break;
                    case JTypeType.Primitive:
                        tb = mb.InternalBuilder.DefineNestedType(typename,
                            TypeAttributes.NestedPublic | TypeAttributes.SequentialLayout,
                            typeof(ValueType), type.SizeOf);
                        JuliaStaticLibrary.DecorateWithDefAttribute(tb, JPrimitiveAttribute.DEF_CON);
                        break;
                    default:
                        int fieldcount = type.FieldCount;
                        tb = mb.InternalBuilder.DefineNestedType(typename,
                            TypeAttributes.NestedPublic | TypeAttributes.SequentialLayout,
                            typeof(ValueType));
                        JuliaStaticLibrary.DecorateWithDefAttribute(tb, JStructAttribute.DEF_CON);
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Caught Error While Generating Type:" + type + " (" + type.Type + ")");
                throw;
            }
        }
        
        private bool IsNETRepresentable(JType t) {
            return false;
        }

        private bool ResolveLocalType(JuliaStaticModule mod, JType ty, out Type t) {
            List<Any> modules = new();
            var w = ty.Module;
            while (w != w.Module) {
                modules.Add(w);
                w = w.Module;
            }

            if (modules.Count > 0) {
                var tb = (TypeBuilder) _baseModuleBuilder.GetType(modules[modules.Count - 1].ToString());
                for (int i = modules.Count - 2; i > -1; i--)
                    tb = (TypeBuilder) tb.GetNestedType(modules[i].ToString());
                t = tb.GetNestedType(ty.Name);
            }else t = _baseModuleBuilder.GetType(ty.Name);

            return true;
        }
        
        private Type ResolveType(JuliaStaticModule mod, Any shortName) {
            var jty = JPrimitive.getpropertyF.Invoke(mod.Module, shortName).Type;
            try {
                if (IsNETRepresentable(jty)) {
                    if (ResolveLocalType(mod, jty, out var t))
                        return t;
                    throw new Exception("Unable To Resolve Type:" + jty);
                }
                return typeof(Any);
            }catch (Exception) {
                Console.WriteLine("Caught Error While Resolving Type:" + jty);
                throw;
            }
        }
    }
}