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
        delegate JLVal SharpInvoke(long invokable, JLArray parameters);
        delegate JLVal SharpConstructor(long clazz, int idx);
        delegate JLVal SharpIntMethod(long clazz, int idx);
        delegate JLVal SharpStringMethod(long clazz, string method);
        delegate JLVal SharpClass(string clazz);
        delegate JLVal SharpField(long clazz, string field);
        delegate JLVal SharpPINgc(long objectToPin);
        delegate void SharpFreegc(long idx);

        private static T ReadSharpVal<T>(long l) => AddressHelper.GetInstance<T>((IntPtr) l);
        private static long WriteSharpVal(object o) => AddressHelper.GetAddress(o).ToInt64();
        private static JLVal CreateJuliaVal(JLType t, object o) => Julia.AllocStruct(t, WriteSharpVal(o));


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

        private static T pass<T>(T t)
        {
            Console.WriteLine(t);
            return t;
        }

        internal unsafe static void init(){
            SharpConstructor GetConstructor = (clazz, idx) => CreateJuliaVal(JLType.SharpConstructor, ReadSharpVal<Type>(clazz).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[idx]);
            SharpIntMethod GetIntMethod = (clazz, idx) => CreateJuliaVal(JLType.SharpMethod, ReadSharpVal<Type>(clazz).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)[idx]);
            SharpStringMethod GetStringMethod = (clazz, method) => CreateJuliaVal(JLType.SharpMethod, ReadSharpVal<Type>(clazz).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));
            SharpClass GetClass = clazz => CreateJuliaVal(JLType.SharpType, FindType(clazz));
            SharpField GetField = (clazz, field) => CreateJuliaVal(JLType.SharpField, ReadSharpVal<Type>(clazz).GetField(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));

            SharpInvoke SharpInvoke = (val, parameters) => {
                var invokable = ReadSharpVal<object>(val);
                var p = parameters.LinearNetUnPack();
                if (invokable is MethodInfo)
                {
                    var met = (MethodInfo)invokable;
                    if (met.IsStatic)
                        return (JLVal)met.Invoke(null, p);
                    object[] result = new object[p.Length - 1];
                    Array.Copy(p, 1, result, 0, p.Length - 1);
                    return new JLVal(met.Invoke(p[1], result));
                }
                else if (invokable is ConstructorInfo)
                    return new JLVal(((ConstructorInfo)invokable).Invoke(p));
                else if (invokable is FieldInfo){
                    var fi = ((FieldInfo)invokable);
                    if (fi.IsStatic && p.Length == 0)
                        return new JLVal(fi.GetValue(null));
                    else if(fi.IsStatic && p.Length == 1)
                    {
                        fi.SetValue(null, p[0]);
                        return new JLVal(0);
                    }else if(!fi.IsStatic && p.Length == 1){
                        return new JLVal(fi.GetValue(p[0]));
                    }else if (!fi.IsStatic && p.Length == 2){
                        fi.SetValue(p[0], p[1]);
                        return new JLVal(0);
                    }
                    throw new Exception("Invalid Field Invokation!");
                }
                else throw new Exception("Unknown Object Invokation");                  
            };

            SharpPINgc sharpPINgc = (o) => { return JLType.SharpStub.Create((long) ObjectCollector.PushCSharp(ReadSharpVal<object>(o))); };
            SharpFreegc sharpFreegc = (idx) => { ObjectCollector.Ccollector.RemoveAt((int) idx); };

            Julia.GetFunction(JLModule.JuliaInterface, "initialize_library")
                   .Invoke(Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetClass)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetIntMethod)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetStringMethod)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetConstructor)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetField)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(SharpInvoke)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(sharpPINgc)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(sharpFreegc)));
     
        }
    }
}