using Base;
using JULIAdotNET;
using runtime.core;
using runtime.ILCompiler;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JuliadotNET
{
    public partial class JuliaStaticLibrary
    {
        private readonly Any _baseModule;
        private ModuleBuilder _baseModuleBuilder;
        private ILTypeBuilder _baseModuleTypeBuilder;

        private FieldInfo UniversalConstantsFI;
        private readonly Dictionary<string, int> _universalConstants = new();

        private static readonly MethodInfo JuliaEvalMI =
            SharpReflect.GetMethod<Julia, string>("Eval");

        private static readonly FieldInfo WriteSharpEvalMI =
            SharpReflect.GetField(typeof(JPrimitive), "writeSharpArrayF");

        private static readonly MethodInfo jl_call_2mi = typeof(JuliaCalls).GetMethod("jl_call2");

        public JuliaStaticLibrary(Any module) => _baseModule = module;

        public void Generate(string libPath)
        {
            var modName = _baseModule.ToString();
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(modName), AssemblyBuilderAccess.Run);
            _baseModuleBuilder = asm.DefineDynamicModule(modName);
            _baseModuleTypeBuilder = new ILTypeBuilder(_baseModuleBuilder.DefineType("jl." + ConvertJuliaName(modName),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed));
            var mb = GenerateModule(new(_baseModule, _baseModuleTypeBuilder, null), true);
            WriteUniversalConstants(mb);
            mb.Create();
            new Lokad.ILPack.AssemblyGenerator().GenerateAssembly(asm, libPath);
        }

        private void WriteUniversalConstants(ILTypeBuilder mb)
        {
            if (UniversalConstantsFI == null)
                return;

            StringBuilder sb = new("[");
            var keys = _universalConstants.Keys.GetEnumerator();
            keys.MoveNext();
            sb.Append(keys.Current);
            while (keys.MoveNext())
                sb.Append(',').Append(keys.Current);
            sb.Append("]");
            keys.Dispose();

            var mi = mb.TypeInitializer;
            var ptr = mi.Create.Local(typeof(Any*), true);
            /*  CONSTANTS = new Any[len];
                fixed(Any* ptr = CONSTANTS){
                    Julia.Eval("[:sym1, ...]")
                }*/
            mi.Array.Create1d(typeof(Any), _universalConstants.Count);
            mi.Store.Field(UniversalConstantsFI);
            mi.Load.FieldValue(UniversalConstantsFI);
            mi.Store.Local(ptr);
            //   mi.Array.LoadElementArrayAddress(0);
            mi.Load.Local(ptr);
            mi.Load.Const(sb.ToString());
            mi.Function.Invoke(JuliaEvalMI);
            mi.Function.Invoke(jl_call_2mi);
        }

        private static void DecorateWithDefAttribute(TypeBuilder tb, ConstructorInfo ci)
        {
            CustomAttributeBuilder cab = new(ci, Array.Empty<object>());
            tb.SetCustomAttribute(cab);
        }

        private void LoadUniversalConstant(IlExprBuilder eb, int idx) {
            UniversalConstantsFI ??= _baseModuleTypeBuilder.CreateField(ConvertJuliaName("CONSTANTS"), typeof(Any[]), true, true);
            eb.Load.FieldValue(UniversalConstantsFI);
            eb.Array.LoadElement<Any>(idx);
        }

        private void LoadUniversalConstant(IlExprBuilder eb, string symbol) {
            if (_universalConstants.TryGetValue(symbol, out var v)) {
                LoadUniversalConstant(eb, v);
                return;
            }
            var cnt = _universalConstants.Count;
            _universalConstants.Add(symbol, cnt);
            LoadUniversalConstant(eb, cnt);
        }

        private void LoadSymbol(IlExprBuilder eb, string symbol) => LoadUniversalConstant(eb, ":" + symbol);

        private bool ConvertJuliaName(string s, out string v)
        {
            if (ILTypeBuilder.IsAllowedName(s))
            {
                v = s;
                return true;
            }

            v = s.Replace('!', '_');
            if (ILTypeBuilder.IsAllowedName(v))
                return true;

            v = null;
            return false;
        }

        private string ConvertJuliaName(string s)
        {
            if (ConvertJuliaName(s, out var v))
                return s;
            throw new Exception("Unable to convert Julia Name:" + s);
        }
    }
}