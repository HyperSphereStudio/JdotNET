using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

//Written by Johnathan Bizzano

namespace JuliaInterface
{

    internal class NativeSharp
    {
        private static BindingFlags MethodBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty;

        internal delegate JLVal Invokable(object[] args);
        delegate JLVal SharpInvoke(long invokable, JLArray parameters);
        delegate JLVal SharpConstructor(long clazz);
        delegate JLVal SharpGenericConstructor(long non_gen_constructor, JLArray generic_types);
        delegate JLVal SharpMethod(long clazz, string method);
        delegate JLVal SharpGenericMethod(long non_gen_method, JLArray generic_types);
        delegate JLVal SharpClass(string clazz);
        delegate JLVal SharpField(long clazz, string field);
        delegate JLVal SharpPINgc(long objectToPin);
        delegate void SharpFreegc(IntPtr val);
        delegate long SharpGetType(long obj);
        delegate JLVal SharpToString(long obj);
        delegate long SharpGetHashCode(long obj);
        delegate bool SharpEquals(long obj1, long obj2);
        delegate long SharpBox(JLVal v);
        delegate JLVal SharpUnBox(long obj);

        private static object ReadSharpVal(long l) => ReadSharpVal<object>(l);
        private static T ReadSharpVal<T>(long l) => AddressHelper.GetInstance<T>((IntPtr) l);
        private static long WriteSharpVal(object o) => AddressHelper.GetAddress(o).ToInt64();
        private static JLVal CreateJuliaVal(JLType t, object o) => Julia.AllocStruct(t, (JLVal) WriteSharpVal(o));


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

        private static JLVal GetPtr<T>(T del) => Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(del));
        private static JLVal throwExp(Exception e){
            JuliaCalls.jl_throw(JLType.SharpJLException.Create(CreateJuliaVal(JLType.SharpObject, e)));
            return new JLVal(IntPtr.Zero);
        }

        internal unsafe static void init()
        {
            SharpConstructor GetConstructor = (clazz) => {
                try{
                    Type t = ReadSharpVal<Type>(clazz);
                    return CreateJuliaVal(JLType.SharpConstructor, new InvokableObject((args) => {
                        return CreateJuliaVal(JLType.SharpObject, Activator.CreateInstance(t, args));
                    }, t, "Constructor for Type:" + t));
                }catch(Exception e){
                    return throwExp(e);
                }
            };

            SharpGenericConstructor GetGenericConstructor = (non_gen_constructor, generic_types) => {
                try
                {   
                    Type t = ((Type) ReadSharpVal<InvokableObject>(non_gen_constructor).Parent).MakeGenericType(((long[])generic_types.UnboxInt64Array()).Select(x => ReadSharpVal<Type>(x)).ToArray());
                    return CreateJuliaVal(JLType.SharpConstructor, new InvokableObject((args) => CreateJuliaVal(JLType.SharpObject, Activator.CreateInstance(t, args)), t, "Generic Constructor for Type:" + t));
                }catch(Exception e){
                    return throwExp(e);
                }
            };

            SharpMethod GetMethod = (clazz, method) => {
                try
                {
                    Type t = ReadSharpVal<Type>(clazz);
                    var m = t.GetMethod(method, MethodBindingFlags);
                    if (m == null)
                        return CreateJuliaVal(JLType.SharpMethod, null);

                    return CreateJuliaVal(JLType.SharpMethod, new InvokableObject((args) => {
                        if (m.IsStatic)
                            return CreateJuliaVal(JLType.SharpObject, m.Invoke(null, args));
                        var owner = args[1];
                        var tempArgs = new object[args.Length - 1];
                        Array.Copy(args, 1, tempArgs, 0, args.Length - 1);
                        return CreateJuliaVal(JLType.SharpObject, m.Invoke(owner, args));
                    }, m, m.ToString()));
                }catch (Exception e){
                    return throwExp(e);
                }
            };

            SharpGenericMethod GetGenericMethod = (method, generic_types) => {
                try
                {
                    var m = ((MethodInfo)ReadSharpVal<InvokableObject>(method).Parent).MakeGenericMethod(((long[])generic_types.UnboxInt64Array()).Select(x => ReadSharpVal<Type>(x)).ToArray());
                    return CreateJuliaVal(JLType.SharpMethod, new InvokableObject((args) => {
                        if (m.IsStatic)
                            return CreateJuliaVal(JLType.SharpObject, m.Invoke(null, args));
                        var owner = args[1];
                        var tempArgs = new object[args.Length - 1];
                        Array.Copy(args, 1, tempArgs, 0, args.Length - 1);
                        return CreateJuliaVal(JLType.SharpObject, m.Invoke(owner, args));

                    }, null, m.ToString()));
                }catch(Exception e){
                    return throwExp(e);
                }
            };

            SharpClass GetClass = (clazz) => {
                try
                {
                    return CreateJuliaVal(JLType.SharpType, FindType(clazz));
                }catch(Exception e){
                    return throwExp(e);
                }
            };

            SharpField GetField = (clazz, field) => {
                try
                {
                    var f = ReadSharpVal<Type>(clazz).GetField(field, MethodBindingFlags);
                    return CreateJuliaVal(JLType.SharpField, new InvokableObject((args) => {
                        if (f.IsStatic)
                        {
                            if (args.Length == 1)
                            {
                                f.SetValue(null, args[0]);
                                return IntPtr.Zero;
                            }
                            else if (args.Length == 0)
                            {
                                return CreateJuliaVal(JLType.SharpObject, f.GetValue(null));
                            }
                        }
                        else if (args.Length == 2)
                        {
                            f.SetValue(args[0], args[1]);
                            return IntPtr.Zero;
                        }
                        else if (args.Length == 1)
                        {
                            return CreateJuliaVal(JLType.SharpObject, f.GetValue(args[0]));
                        }
                        return throwExp(new Exception("Unknown Sharp Field Invokation"));
                    }, f, f.ToString()));
                }
                catch (Exception e)
                {
                    return throwExp(e);
                }
            };

            SharpInvoke SharpInvoke = (val, parameters) => {
                try
                {
                    return ReadSharpVal<InvokableObject>(val).invokable.Invoke((object[])parameters.UnboxObjectArray());
                }
                catch (Exception e)
                {
                    return throwExp(e);
                }
            };

            SharpPINgc sharpPINgc = (o) => {
                try
                {
                    return JLType.SharpStub.Create(ObjectCollector.PushCSharp(ReadSharpVal(o)));
                }catch(Exception e)
                {
                    return throwExp(e);
                }
            };

            SharpFreegc sharpFreegc = (o) => {
                try{
                    ObjectCollector.Ccollector.Remove(AddressHelper.GetInstance(o));
                }catch(Exception e){
                    throwExp(e);
                }
            };

            SharpGetType sharpGetType = (o) => {
                try
                {
                    if (o == 0)
                        return 0;
                    return WriteSharpVal(ReadSharpVal(o).GetType());
                }
                catch (Exception e)
                {
                    throwExp(e);
                    return 0;
                }
            };

            SharpToString sharpToString = (o) => {
                try {
                    return new JLVal(ReadSharpVal(o).ToString());
                }
                catch(Exception e)
                { 
                    return throwExp(e);
                }
            };

            SharpGetHashCode sharpGetHashCode = (o) => {
                try
                {
                    return ReadSharpVal(o).GetHashCode();
                }
                catch (Exception e)
                {
                    throwExp(e);
                    return 0;
                }
            };

            SharpEquals sharpEquals = (o1, o2) => {
                try
                {
                    return ReadSharpVal(1) == ReadSharpVal(o2);
                }catch(Exception e)
                {
                    throwExp(e);
                    return false;
                }
            };

            SharpBox sharpBox = (o) => {
                try
                {
                    return WriteSharpVal(o.Value);
                }
                catch (Exception e)
                {
                    throwExp(e);
                    return 0;
                }
            };

            SharpUnBox sharpUnBox = (o) => {
                try
                {
                    return new JLVal(ReadSharpVal(o));
                }
                catch (Exception e)
                {
                    return throwExp(e);
                }
            };

         
            Julia.GetFunction(JLModule.JuliaInterface, "initialize_library")
                   .Invoke(GetPtr(GetClass),
                           GetPtr(GetMethod),
                           GetPtr(GetGenericMethod),
                           GetPtr(GetConstructor),
                           GetPtr(GetGenericConstructor),
                           GetPtr(GetField),
                           GetPtr(SharpInvoke),
                           GetPtr(sharpPINgc),
                           GetPtr(sharpFreegc),
                           GetPtr(sharpGetType),
                           GetPtr(sharpEquals),
                           GetPtr(sharpToString),
                           GetPtr(sharpGetHashCode),
                           GetPtr(sharpBox),
                           GetPtr(sharpUnBox));

        }


        internal class InvokableObject{
            internal Invokable invokable;
            internal object Parent;
            internal string Message;

            public InvokableObject(Invokable i, object parent, string message){
                invokable = i;
                Message = message;
                Parent = parent;
            }

            public override string ToString() => Message;
        }
    }
}