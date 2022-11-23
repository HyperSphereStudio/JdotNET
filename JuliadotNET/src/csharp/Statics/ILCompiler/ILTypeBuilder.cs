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

        internal FieldBuilder CreateFieldImpl(string name, Type type, FieldAttributes extra = 0) => InternalBuilder.DefineField(name, type, FieldAttributes.Public | extra);

        public FieldBuilder CreateField(string name, Type type, bool isConst) => CreateFieldImpl(name, type, isConst ? FieldAttributes.InitOnly : 0);
        public IlExprBuilder CreateMethod(string name, Type returnType, params Type[] parameters) =>
            new(InternalBuilder.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Public, 
                returnType, parameters));

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
    }
}