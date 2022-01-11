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
        delegate JLVal SharpInvoke(JLVal invokable, JLArray parameters);
        delegate JLVal SharpConstructor(JLVal clazz, int idx);
        delegate JLVal SharpMethod(JLVal clazz, string method);
        delegate JLVal SharpClass(string clazz);
        delegate JLVal SharpField(JLVal clazz, string field);

        private static T ReadSharpVal<T>(long l) => AddressHelper.GetInstance<T>((IntPtr) l);
        private static T GetSharpVal<T>(JLVal sharpObject) => ReadSharpVal<T>((long) JLFun.GetFieldF.Invoke(sharpObject, (JLSym)"ptr"));
        private static long WriteSharpVal(object o) => AddressHelper.GetAddress(o).ToInt64();
        private static JLVal CreateJuliaVal(JLType t, object o) => Julia.CreateStruct(t, WriteSharpVal(o));

        internal unsafe static void init(){
            SharpConstructor GetConstructor = (clazz, idx) => CreateJuliaVal(JLType.SharpConstructor, GetSharpVal<Type>(clazz).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[idx]);
            SharpMethod GetMethod = (clazz, method) => CreateJuliaVal(JLType.SharpMethod, GetSharpVal<Type>(clazz).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));
            SharpClass GetClass = clazz => CreateJuliaVal(JLType.SharpType, Type.GetType(clazz));
            SharpField GetField = (clazz, field) => CreateJuliaVal(JLType.SharpField, GetSharpVal<Type>(clazz).GetField(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));
            
            SharpInvoke SharpInvoke = (val, parameters) => {
                var invokable = GetSharpVal<object>(val);
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

            Julia.GetFunction(JLModule.JuliaInterface, "initialize_library")
                   .Invoke(Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetClass)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetMethod)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetConstructor)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetField)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(SharpInvoke)));
     
        }
    }
}