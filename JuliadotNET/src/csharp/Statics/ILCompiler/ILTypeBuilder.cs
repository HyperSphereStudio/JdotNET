using System;
using System.Reflection;
using System.Reflection.Emit;

namespace runtime.ILCompiler
{
    public struct ILTypeBuilder
    {
        public readonly TypeBuilder InternalBuilder;
        public readonly IlExprBuilder TypeInitializer;

        internal ILTypeBuilder(TypeBuilder t) {
            TypeInitializer = new IlExprBuilder(t.DefineTypeInitializer());
            InternalBuilder = t;
        }

        private FieldBuilder CreateFieldImpl(string name, Type type, FieldAttributes extra) => InternalBuilder.DefineField(name, type, extra);

        public FieldBuilder CreateField(string name, Type type, bool isConst, bool isStatic = false, FieldAttributes extraAttribs = FieldAttributes.Public) {
            FieldAttributes fa = extraAttribs;
            if (isConst)
                fa |= FieldAttributes.InitOnly;
            if (isStatic)
                fa |= FieldAttributes.Static;
            return CreateFieldImpl(name, type, fa);
        }

        public PropertyBuilder CreateProperty(string name, Type t, PropertyAttributes attribs = PropertyAttributes.None) => InternalBuilder.DefineProperty(name, attribs, t, Type.EmptyTypes);

        public IlExprBuilder CreateMethod(string name, Type returnType, params Type[] parameters) => CreateMethod(name, returnType, MethodAttributes.Static | MethodAttributes.Public, parameters);

        public IlExprBuilder CreateMethod(string name, Type returnType, MethodAttributes attributes, params Type[] parameters) => new(InternalBuilder.DefineMethod(name, attributes, returnType, parameters));

        public IlExprBuilder CreateGetMethod(PropertyBuilder pb) {
            var mb = CreateMethod("get_" + pb.Name, pb.PropertyType, MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName, Type.EmptyTypes);
            pb.SetGetMethod(mb);
            return mb;
        }

        public IlExprBuilder CreateMethod(string name) => CreateMethod(name, typeof(void));
        public IlExprBuilder CreateMethod<TOut>(string name) => CreateMethod(name, typeof(TOut));
        public IlExprBuilder CreateMethod<TOut, T1>(string name) => CreateMethod(name, typeof(TOut), typeof(T1));
        public IlExprBuilder CreateMethod<TOut, T1, T2>(string name) => CreateMethod(name, typeof(TOut), typeof(T1), typeof(T2));
        public IlExprBuilder CreateMethod<TOut, T1, T2, T3>(string name) => CreateMethod(name, typeof(TOut), typeof(T1), typeof(T2), typeof(T3));
        public IlExprBuilder CreateMethod<TOut, T1, T2, T3, T4>(string name) => CreateMethod(name, typeof(TOut), typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public IlExprBuilder CreateMethod<TOut, T1, T2, T3, T4, T5>(string name) => CreateMethod(name, typeof(TOut), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        public IlExprBuilder CreateConstructor(params Type[] parameters) =>
            new(InternalBuilder.DefineConstructor(MethodAttributes.Static | MethodAttributes.Public, 
                CallingConventions.Any, parameters));

        public IlExprBuilder CreateConstructor<T1>() => CreateConstructor(typeof(T1));
        public IlExprBuilder CreateConstructor<T1, T2>() => CreateConstructor(typeof(T1), typeof(T2));
        public IlExprBuilder CreateConstructor<T1, T2, T3>() => CreateConstructor(typeof(T1), typeof(T2), typeof(T3));
        public IlExprBuilder CreateConstructor<T1, T2, T3, T4>() => CreateConstructor(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public IlExprBuilder CreateConstructor<T1, T2, T3, T4, T5>() => CreateConstructor(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        public Type Create() {
            var ti = TypeInitializer;
            ti.ReturnVoid();
            return InternalBuilder.CreateType();
        }

        public static bool IsAllowedName(string s) {
            if (!(char.IsLetter(s[0]) || s[0] == '_'))
                return false;
            foreach(var c in s)
                if (!(char.IsLetterOrDigit(c) || char.IsSeparator(c)))
                    return false;
            return true;
        }
    }
}