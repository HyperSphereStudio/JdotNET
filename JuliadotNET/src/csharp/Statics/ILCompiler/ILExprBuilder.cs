using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using JULIAdotNET;
using runtime.core;

namespace runtime.ILCompiler
{
    public struct IlExprBuilder
    {
        private readonly ILGenerator _il;
        public readonly object InternalMethod;
        
        public static implicit operator MethodBuilder(IlExprBuilder b) => (MethodBuilder) b.InternalMethod;
        public static implicit operator ConstructorBuilder(IlExprBuilder b) => (ConstructorBuilder) b.InternalMethod;

        #region BuilderExtensions

        public struct ILFunctionExprBuilder
        {
            private readonly IlExprBuilder _eb;

            internal ILFunctionExprBuilder(IlExprBuilder eb) => _eb = eb;

            public void Invoke<T>(string name, params Type[] parameters) =>
                Invoke(SharpReflect.GetMethod<T>(name, parameters));

            public void Invoke(Type t, string name, params Type[] parameters) =>
                Invoke(SharpReflect.GetMethod(t, name, parameters));

            public void Invoke<T>(params Type[] parameters) => Invoke(SharpReflect.GetConstructor<T>(parameters));
            public void Invoke(Type t, params Type[] parameters) => Invoke(SharpReflect.GetConstructor(t, parameters));

            public void Invoke(MethodInfo info) =>
                _eb._il.EmitCall(info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, info, null);

            public void Invoke(IlExprBuilder fb, bool requiresConstructor = false, bool requiresMethod = false)
            {
                if (fb.InternalMethod is MethodBuilder mb && !requiresConstructor)
                    Invoke(mb);
                else if (fb.InternalMethod is ConstructorBuilder cb && !requiresMethod)
                    Invoke(cb);
                else throw new Exception("Unable To Invoke Unknown Internal Method!");
            }

            public void Invoke(ConstructorInfo ci) => _eb._il.Emit(OpCodes.Newobj, ci);

            public void Println(string s) => _eb._il.EmitWriteLine(s);
        }

        public struct ILLoadExprBuilder
        {
            private static readonly MethodInfo GET_RUNTIME_TYPE = typeof(System.Type).GetMethod("GetTypeFromHandle");

            private readonly IlExprBuilder _eb;
            internal ILLoadExprBuilder(IlExprBuilder eb) => _eb = eb;

            public void Arg(uint i)
            {
                if (i < 4)
                {
                    if (i == 0)
                        _eb.Emit(OpCodes.Ldarg_0);
                    else if (i == 1)
                        _eb.Emit(OpCodes.Ldarg_1);
                    else if (i == 2)
                        _eb.Emit(OpCodes.Ldarg_2);
                    else _eb.Emit(OpCodes.Ldarg_3);
                }
                else if (i < 255)
                    _eb._il.Emit(OpCodes.Ldarg_S, i);
                else
                    _eb._il.Emit(OpCodes.Ldarg, i);
            }

            public void String(string str) => _eb._il.Emit(OpCodes.Ldstr, str);
            public void Bool(bool b) => _eb._il.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            public void Int64(Int64 v) => _eb._il.Emit(OpCodes.Ldc_I8, v);
            public void Int32(Int32 v) => _eb._il.Emit(OpCodes.Ldc_I4, v);
            public void Int16(Int16 v) => Int32(v);
            public void Int8(sbyte v) => Int32(v);
            public void UInt64(UInt64 v) => _eb._il.Emit(OpCodes.Ldc_I8, v);
            public void UInt32(UInt32 v) => _eb._il.Emit(OpCodes.Ldc_I4, v);
            public void UInt16(UInt16 v) => UInt32(v);
            public void UInt8(byte v) => UInt32(v);
            public void Float32(float v) => _eb._il.Emit(OpCodes.Ldc_R4, v);
            public void Float64(double v) => _eb._il.Emit(OpCodes.Ldc_R8, v);

            public void Type(System.Type t) {
                _eb._il.Emit(OpCodes.Ldtoken, t);
                _eb.Function.Invoke(GET_RUNTIME_TYPE);
            }

            public void FieldValue(FieldInfo f) => _eb._il.Emit(f.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, f);
            public void FieldValue(FieldBuilder fb) => _eb._il.Emit(fb.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fb);

            public void FieldAddr(FieldInfo f) => _eb._il.Emit(f.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldsfld, f);
            public void FieldAddr(FieldBuilder fb) => _eb._il.Emit(fb.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldsfld, fb);

            public void OptFloat(double v)
            {
                if (v > float.MaxValue)
                    Float32((float)v);
                else
                    Float64(v);
            }

            public void OptInt(Int64 v)
            {
                bool less = v < 0;
                if (less) v *= -1;
                if (v < System.Int32.MaxValue)
                    Int32((Int32)v * (less ? -1 : 1));
                else
                    Int64(v * (less ? -1 : 1));
            }

            public void OptUInt(UInt64 v)
            {
                if (v < System.Int32.MaxValue)
                    UInt32((UInt32)v);
                else
                    UInt64(v);
            }

            public void Const(UInt64 v) => OptUInt(v);
            public void Const(Int64 v) => OptInt(v);
            public void Const(double v) => OptFloat(v);
            public void Const(string s) => String(s);
            public void Const(bool b) => Bool(b);
            public void Local(LocalBuilder lb) => _eb._il.Emit(OpCodes.Ldloc, lb);

            public void FromPointerInt8() => _eb.Emit(OpCodes.Ldind_I1);
            public void FromPointerInt16() => _eb.Emit(OpCodes.Ldind_I2);
            public void FromPointerInt32() => _eb.Emit(OpCodes.Ldind_I4);
            public void FromPointerInt64() => _eb.Emit(OpCodes.Ldind_I8);
            public void FromPointerFloat32() => _eb.Emit(OpCodes.Ldind_R4);
            public void FromPointerFloat64() => _eb.Emit(OpCodes.Ldind_R8);
            public void FromPointerRef() => _eb.Emit(OpCodes.Ldind_Ref);

            public void FromPointer(Type t)
            {
                if (t.IsPrimitive)
                {
                    if (t == typeof(byte) || t == typeof(sbyte))
                        FromPointerInt8();
                    else if (t == typeof(Int16) || t == typeof(UInt16))
                        FromPointerInt16();
                    else if (t == typeof(Int32) || t == typeof(UInt32))
                        FromPointerInt32();
                    else if (t == typeof(Int64) || t == typeof(UInt64))
                        FromPointerInt64();
                    else if (t == typeof(float))
                        FromPointerFloat32();
                    else if (t == typeof(double))
                        FromPointerFloat64();
                    else throw new JuliaException("Unable to Determine Pointer Type T");
                }
                else if (t.IsByRef)
                {
                    FromPointerRef();
                }
                else throw new JuliaException("Unable to Determine Pointer Type T");
            }
        }

        public struct ILStoreExprBuilder
        {
            private readonly IlExprBuilder _eb;
            internal ILStoreExprBuilder(IlExprBuilder eb) => _eb = eb;

            public void Field(FieldInfo f) => _eb._il.Emit(f.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, f);
            public void Field(FieldBuilder fb) => _eb._il.Emit(fb.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fb);

            public void Local(LocalBuilder lb) => _eb._il.Emit(OpCodes.Stloc, lb);

            public void ToPointerInt8() => _eb.Emit(OpCodes.Stind_I1);
            public void ToPointerInt16() => _eb.Emit(OpCodes.Stind_I2);
            public void ToPointerInt32() => _eb.Emit(OpCodes.Stind_I4);
            public void ToPointerInt64() => _eb.Emit(OpCodes.Stind_I8);
            public void ToPointerFloat32() => _eb.Emit(OpCodes.Stind_R4);
            public void ToPointerFloat64() => _eb.Emit(OpCodes.Stind_R8);
            public void ToPointerRef() => _eb.Emit(OpCodes.Stind_Ref);

            public void ToPointer(Type t)
            {
                if (t.IsPrimitive)
                {
                    if (t == typeof(byte) || t == typeof(sbyte))
                        ToPointerInt8();
                    else if (t == typeof(Int16) || t == typeof(UInt16))
                        ToPointerInt16();
                    else if (t == typeof(Int32) || t == typeof(UInt32))
                        ToPointerInt32();
                    else if (t == typeof(Int64) || t == typeof(UInt64))
                        ToPointerInt64();
                    else if (t == typeof(float))
                        ToPointerFloat32();
                    else if (t == typeof(double))
                        ToPointerFloat64();
                    else throw new JuliaException("Unable to Determine Pointer Type T");
                }
                else if (t.IsByRef)
                {
                    ToPointerRef();
                }
                else throw new JuliaException("Unable to Determine Pointer Type T");
            }
        }

        public struct ILCreateExprBuilder
        {
            private readonly IlExprBuilder _eb;
            internal ILCreateExprBuilder(IlExprBuilder eb) => _eb = eb;
            public LocalBuilder Local(Type t, bool pinned = false) => _eb._il.DeclareLocal(t, pinned);
            public LocalBuilder Local<T>() => Local(typeof(T));

            public LocalBuilder LocalAndStore(Type t)
            {
                var lb = Local(t);
                _eb.Store.Local(lb);
                return lb;
            }

            public LocalBuilder LocalAndStore<T>() => LocalAndStore(typeof(T));

            public void Object(ConstructorInfo ci) => _eb.Function.Invoke(ci);
            public void Object(IlExprBuilder cb) => _eb.Function.Invoke(cb, true);
            public void Object<T>(params Type[] parameters) => _eb.Function.Invoke<T>(parameters);
            public void Object(Type t, params Type[] parameters) => _eb.Function.Invoke(t, parameters);
        }

        public struct ILArrayExprBuilder
        {
            private readonly IlExprBuilder _eb;
            internal ILArrayExprBuilder(IlExprBuilder eb) => _eb = eb;

            public void LoadElement(int i, Type t) {
                _eb.Load.Const(i);
                LoadElement(t);
            }
            
            public void LoadElement<T>(int i) => LoadElement(i, typeof(T));

            public void LoadElement(Type t) => _eb._il.Emit(OpCodes.Ldelem, t);
            public void LoadElement<T>() => LoadElement(typeof(T));
            
            public void StoreElement(Type t) => _eb._il.Emit(OpCodes.Stelem, t);
            public void StoreElement<T>() => StoreElement(typeof(T));
            public void StoreElement<T>(int i) => StoreElement(i, typeof(T));

            public void LoadElementArrayAddress() => _eb._il.Emit(OpCodes.Ldelema);

            public void LoadElementArrayAddress(int i) {
                _eb.Load.Const(i);
                LoadElementArrayAddress();
            }
            
            public void StoreElement(int i, Type t) {
                _eb.Load.Const(i);
                StoreElement(t);
            }

            public void Create1d(Type elType) => _eb._il.Emit(OpCodes.Newarr, elType);

            public void Create1d(Type elType, int size) {
                _eb.Load.Const(size);
                Create1d(elType);
            }
        }

        public struct ILUnsafeExprBuilder
        {
            private readonly IlExprBuilder _eb;
            internal ILUnsafeExprBuilder(IlExprBuilder eb) => _eb = eb;

            public void StackAlloc() => _eb._il.Emit(OpCodes.Localloc);

            public void StackAlloc(Type t, int size)
            {
                _eb.Load.UInt32((uint)(Marshal.SizeOf(t) * size));
                StackAlloc();
            }

            public void StackAlloc<T>(int size) => StackAlloc(typeof(T), size);
            public void StoreToPointer(Type t) => _eb.Store.ToPointer(t);
            public void StoreToPointer<T>() => StoreToPointer(typeof(T));
            public void LoadFromPointer(Type t) => _eb.Load.FromPointer(t);
            public void LoadFromPointer<T>() => LoadFromPointer(typeof(T));

            public void CopyBlock(int numBytes)
            {
                _eb.Load.Int32(numBytes);
                CopyBlock();
            }

            public void CopyBlock() => _eb._il.Emit(OpCodes.Cpblk);
        }

        #endregion

        #region BuilderExtensionImpl
        public ILFunctionExprBuilder Function => new(this);
        public ILLoadExprBuilder Load => new(this);
        public ILStoreExprBuilder Store => new(this);
        public ILCreateExprBuilder Create => new(this);
        public ILArrayExprBuilder Array => new(this);
        public ILUnsafeExprBuilder Unsafe => new(this);
        
        #endregion

        private IlExprBuilder(ILGenerator il, object internalMethod) {
            _il = il;
            InternalMethod = internalMethod;
        }

        public IlExprBuilder(MethodBuilder mb) : this(mb.GetILGenerator(), mb) {}

        public IlExprBuilder(ConstructorBuilder cb) : this(cb.GetILGenerator(), cb) {}
        
        #region HelperFunctions
        public void Emit(OpCode code) => _il.Emit(code);

        public void Return() => Emit(OpCodes.Ret);
        public void Add() => Emit(OpCodes.Add);
        public void Sub() => Emit(OpCodes.Sub);
        public void Duplicate() => Emit(OpCodes.Dup);
        public void Box(Type t) => _il.Emit(OpCodes.Box, t);
        public void Box<T>() => Box(typeof(T));

        public void ReturnVoid() {
            _il.Emit(OpCodes.Nop);
            Return();
        }

        public static IlExprBuilder CreateDynamicFunction(out DynamicMethod m, string name = "<Eval>")
        {
            m = new DynamicMethod(name, typeof(object), Type.EmptyTypes, typeof(string).Module);
            return new(m.GetILGenerator(), m);
        }
        
        #endregion
    }
}