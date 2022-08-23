using System;
using System.Runtime.InteropServices;
using System.Reflection;

//Written by Johnathan Bizzano

namespace JULIAdotNET
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeString{
        IntPtr len;
        IntPtr data;

        public NativeString(IntPtr len, IntPtr data){
            this.len = len;
            this.data = data;
        }

        public static explicit operator string(NativeString ns) => ns.Value;
        public static unsafe implicit operator NativeString(IntPtr* ptr) => new NativeString(*ptr++, *ptr);

        public string Value => Marshal.PtrToStringUni(data, len.ToInt32());
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeArray<T>{
        public IntPtr len;
        public NativeObject<T>* data;

        public unsafe NativeArray(IntPtr len, NativeObject<T>* data){
            this.len = len;
            this.data = data;
        }

        public static explicit operator T[](NativeArray<T> na) => na.Value;
        public static explicit operator object[](NativeArray<T> na) => na.OValue;
        public static unsafe implicit operator NativeArray<T>(IntPtr* ptr) => new NativeArray<T>(*ptr++, (NativeObject<T>*) ptr);

        public object[] OValue {
            get {
                var l = len.ToInt32();
                var t = new object[l];
                for (int i = 0; i < l; ++i)
                    t[i] = (T)data[i];
                return t;
            }
        }

        public T[] Value {
            get {
                var l = len.ToInt32();
                var t = new T[l];
                for (int i = 0; i < l; ++i)
                    t[i] = (T) data[i];
                return t;
            }
        }
    }

    public struct NativeObject<T>{
        public IntPtr data;

        public NativeObject(T o) => data = new IntPtr(ObjectManager._CreateSharp4JuliaReference(o));

        public NativeObject(IntPtr data) => this.data = data;

        public static explicit operator T(NativeObject<T> no) => no.Value;
        public static unsafe implicit operator NativeObject<T>(IntPtr* ptr) => new NativeObject<T>(*ptr);

        public T Value => (T) ObjectManager._GetSharp4JuliaValue(data.ToInt32());
    }

    public class NativeSharp
    {
        private static readonly BindingFlags MethodBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty;
        private static JLFun RegisterSharpFunctionHandle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr JuliaNativeInterface(IntPtr* data);

        public static unsafe JLFun RegisterSharpFunction(string name, JuliaNativeInterface jns){
            JuliaNativeInterface wrappedInterface = data => {
                try {
                    return jns(data);
                }catch (Exception e){
                    Julia.Throw(new Exception("[Native Sharp Function::Sharp] Exception in " + name + " due to " + e.Message, e));
                    return IntPtr.Zero;
                }
            };
            ObjectManager._CreateSharp4JuliaReference(jns);
            var ptr = Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(wrappedInterface));
            var sym = new JLSym(name);
            return RegisterSharpFunctionHandle.Invoke(sym, ptr);
        }

        public static JLFun GetRegistedSharpFunction(JLSym sym) => RegisterSharpFunctionHandle.Invoke(sym, Julia.BoxPtr(new IntPtr(0)));

        public static JLVal GetType(Type t) => CreateJuliaVal(JLType.SharpType, t);
        public static JLVal GetType(string name) => GetType(FindType(name));
        public static JLVal GetType(NativeString name) => GetType(name.Value);

        public static JLVal GetGenericType(Type t, params Type[] generic_types) => GetType(t.MakeGenericType(generic_types));
        public static JLVal GetGenericType(NativeObject<Type> jltype, NativeArray<Type> generic_types) => GetGenericType(jltype.Value, generic_types.Value);

        public static JLVal GetMethod(MethodInfo m) => CreateJuliaVal(JLType.SharpMethod, m);
        public static JLVal GetMethod(Type t, string name) => GetMethod(t.GetMethod(name, MethodBindingFlags));
        public static JLVal GetMethod(Type t, string name, params Type[] types) => GetMethod(t.GetMethod(name, MethodBindingFlags, null, CallingConventions.Any, types, null));
        public static JLVal GetMethod(NativeObject<Type> t, NativeString name) => GetMethod(t.Value, name.Value);
        public static JLVal GetMethod(NativeObject<Type> t, NativeString name, NativeArray<Type> types) => GetMethod(t.Value, name.Value, types.Value);
        public static JLVal GetGenericMethod(MethodInfo m, params Type[] generic_types) => GetMethod(m.MakeGenericMethod(generic_types));
        public static JLVal GetGenericMethod(NativeObject<MethodInfo> m, NativeArray<Type> generic_types) => GetGenericMethod(m.Value, generic_types.Value);
        public static JLVal InvokeMethod(MethodInfo m, object owner, params object[] args) => BoxObject(m.Invoke(owner, args));
        public static JLVal InvokeMethod(NativeObject<MethodInfo> m, NativeObject<object> owner, NativeArray<object> args) => InvokeMethod(m.Value, owner.Value, args.OValue);

        public static JLVal BoxObject(object o) => CreateJuliaVal(JLType.SharpObject, o);
        public static JLVal BoxObject(JLVal v) => BoxObject(v.Value);
        public static JLVal UnBoxObject(object o) => new JLVal(o);
        public static JLVal UnBoxObject(NativeObject<object> o) => UnBoxObject(o.Value);

        public static JLVal GetConstructor(ConstructorInfo c) => CreateJuliaVal(JLType.SharpConstructor, new NativeObject<object>(c));
        public static JLVal GetConstructor(Type t) => GetConstructor(t.GetConstructor(Type.EmptyTypes));
        public static JLVal GetConstructor(NativeObject<Type> t) => GetConstructor(t.Value);
        public static JLVal GetConstructorByTypes(Type t, params Type[] types) => GetConstructor(t.GetConstructor(types));
        public static JLVal GetConstructorByTypes(NativeObject<Type> t, NativeArray<Type> args) => GetConstructorByTypes(t.Value, args.Value);
        
        public static JLVal InvokeConstructor(ConstructorInfo c, params object[] args) => BoxObject(c.Invoke(args));
        public static JLVal InvokeConstructor(NativeObject<ConstructorInfo> c, NativeArray<Type> types) => InvokeConstructor(c.Value, types.OValue);

        public static JLVal GetField(FieldInfo f) => CreateJuliaVal(JLType.SharpField, f);
        public static JLVal GetField(Type t, string name) => GetField(t.GetField(name, MethodBindingFlags));
        public static JLVal GetField(NativeObject<Type> t, NativeString name) => GetField(t.Value, name.Value);

        public static JLVal GetFieldValue(FieldInfo f, object owner) => BoxObject(f.GetValue(owner));
        public static JLVal GetFieldValue(NativeObject<FieldInfo> f, NativeObject<object> owner) => GetFieldValue(f.Value, owner.Value);
        public static IntPtr SetFieldValue(FieldInfo f, object owner, object value) {
            f.SetValue(owner, value);
            return IntPtr.Zero;
        }
        public static IntPtr SetFieldValue(NativeObject<FieldInfo> f, NativeObject<object> owner, NativeObject<object> value) => SetFieldValue(f.Value, owner.Value, value.Value);

        public static JLVal GetObjectType(object o) => GetType(o == null ? null : o.GetType());
        public static JLVal GetObjectType(NativeObject<object> o) => GetObjectType(o.Value);

        public static JLVal ToString(object o) => new JLVal(o == null ? "null" : o.ToString());
        public static JLVal ToString(NativeObject<object> o) => ToString(o.Value);

        public static JLVal GetHashCode(object o) => BoxObject(o == null ? 0 : o.GetHashCode());
        public static JLVal GetHashCode(NativeObject<object> o) => GetHashCode(o.Value);

        public static JLVal ObjectEquals(object o1, object o2) => new JLVal(Equals(o1, o2));
        public static JLVal ObjectEquals(NativeObject<object> o1, NativeObject<object> o2) => ObjectEquals(o1.Value, o2.Value);
        public static JLVal CreateJuliaVal(JLType t, NativeObject<object> o) => Julia.AllocStruct(t, GetNativeObject(o));
        public static JLVal CreateJuliaVal(JLType t, object o) => CreateJuliaVal(t, new NativeObject<object>(o));
        public static JLVal GetSharpMethodInstance(MethodInfo mi, object o) => Julia.AllocStruct(JLType.SharpOwnerMethod, GetMethod(mi), BoxObject(o));
        public static JLVal GetSharpFieldInstance(FieldInfo fi, object o) => Julia.AllocStruct(JLType.SharpOwnerField, GetField(fi), BoxObject(o));
        public static JLVal GetNativeObject(object o) => GetNativeObject(new NativeObject<object>(o));
        public static JLVal GetNativeObject(NativeObject<object> o) => JLType.NativeObject.Create(Julia.BoxPtr(o.data));
        public static JLVal FreeSharp4JuliaReference(IntPtr ptr){
            ObjectManager._FreeSharp4JuliaReference(ptr.ToInt32());
            return IntPtr.Zero;
        }

        private static Type FindType(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    Type[] exportedTypes = null;
                    try
                    {
                        exportedTypes = assembly.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        exportedTypes = e.Types;
                    }

                    if (exportedTypes != null)
                    {
                        foreach (var type in exportedTypes)
                        {
                            if (type.FullName == name)
                                return type;
                        }
                    }
                }
            }
            return Type.GetType(name);
        }

        internal unsafe static void init() {
            RegisterSharpFunctionHandle = JLModule.Sharp_Native.GetFunction("RegisterSharpFunction");

            RegisterSharpFunction("GetType", data => GetType(data));
            RegisterSharpFunction("GetGenericType", data => GetGenericType(data++, data));
            
            RegisterSharpFunction("GetMethodByName", data => GetMethod(data++, data));
            RegisterSharpFunction("GetMethodByNameAndTypes", data => GetMethod(data++, data++, ++data));
            RegisterSharpFunction("GetGenericMethod", data => GetGenericMethod(data++, data));
            RegisterSharpFunction("InvokeMethod", data => InvokeMethod(data++, data++, data));

            RegisterSharpFunction("GetConstructor", data => GetConstructor(data));
            RegisterSharpFunction("GetConstructorByTypes", data => GetConstructorByTypes(data++, data));
            RegisterSharpFunction("InvokeConstructor", data => InvokeConstructor(data++, data));

            RegisterSharpFunction("GetField", data => {
                
                return GetField(data++, data);
            });
            RegisterSharpFunction("GetFieldValue", data => GetFieldValue(data++, data));
            RegisterSharpFunction("SetFieldValue", data => SetFieldValue(data++, data++, data));

            RegisterSharpFunction("FreeSharp4JuliaReference", data => {
                Console.WriteLine("Test!");
                return FreeSharp4JuliaReference(*data);
            }
            );
            RegisterSharpFunction("GetObjectType", data =>GetObjectType(data));
            RegisterSharpFunction("ToString", data => ToString(data));
            RegisterSharpFunction("GetHashCode", data => GetHashCode(data));
            RegisterSharpFunction("Box", data => BoxObject(*data));
            RegisterSharpFunction("Unbox", data => UnBoxObject(*data));
            RegisterSharpFunction("Equals", data => ObjectEquals(data++, data));

            if (JLModule.Sharp.GetFunction("_init").Invoke().UnboxInt64() != 0)
                throw new Exception("Unable to initialize library!");
        }
    }
}