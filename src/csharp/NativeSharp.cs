using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace JuliaInterface
{
    internal class NativeSharp
    {
        delegate long SharpConstructor(long clazz, int idx);
        delegate long SharpMethod(long clazz, string method);
        delegate long SharpClass(string clazz);
        delegate long SharpField(long clazz, string field);
        delegate JLVal SharpGetFieldValue(long field, long owner);
        delegate void SharpSetFieldValue(long field, long owner, long val);

        internal unsafe static void init(){
            SharpConstructor GetConstructor = (clazz, idx) => AddressHelper.GetInstance<Type>((IntPtr) clazz).GetConstructors()[idx].MethodHandle.GetFunctionPointer().ToInt64();
            SharpMethod GetMethod = (clazz, method) => AddressHelper.GetInstance<Type>((IntPtr) clazz).GetMethod(method).MethodHandle.GetFunctionPointer().ToInt64();
            SharpClass GetClass = clazz => AddressHelper.GetAddress(Type.GetType(clazz)).ToInt64();
            SharpField GetField = (clazz, field) => AddressHelper.GetAddress(AddressHelper.GetInstance<Type>((IntPtr) clazz).GetField(field)).ToInt64();
            SharpGetFieldValue GetFieldValue = (field, owner) => (JLVal) AddressHelper.GetInstance<FieldInfo>((IntPtr) field).GetValue(owner);
            SharpSetFieldValue SetFieldValue = (field, owner, value) => AddressHelper.GetInstance<FieldInfo>((IntPtr) field).SetValue(owner, AddressHelper.GetInstance<object>((IntPtr) value));

            Console.WriteLine("Alloc Pointer!");
            Julia.GetFunction(JLModule.JuliaInterface, "initialize_library")
                   .Invoke(Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetClass)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetMethod)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetConstructor)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetField)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(GetFieldValue)),
                           Julia.BoxPtr(Marshal.GetFunctionPointerForDelegate(SetFieldValue)));
           // Julia.GetFunction(JLModule.JuliaInterface, "testCall").Invoke();
        }
    }
}
