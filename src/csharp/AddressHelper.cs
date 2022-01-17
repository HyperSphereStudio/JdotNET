using System;
using System.Runtime.InteropServices;

namespace JuliaInterface
{
    public static class AddressHelper
    {
        private static object mutualobject;
        private static objectReinterpreter reinterpreter;

        static AddressHelper()
        {
            AddressHelper.mutualobject = new object();
            AddressHelper.reinterpreter = new objectReinterpreter();
            AddressHelper.reinterpreter.Asobject = new ObjectWrapper();
        }

        public static IntPtr GetAddress(object obj)
        {
            lock (AddressHelper.mutualobject)
            {
                AddressHelper.reinterpreter.Asobject.Object = obj;
                IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
                AddressHelper.reinterpreter.Asobject.Object = null;
                return address;
            }
        }

        public static object GetInstance(IntPtr address) => GetInstance<object>(address);

        public static T GetInstance<T>(IntPtr address)
        {
            lock (AddressHelper.mutualobject)
            {
                AddressHelper.reinterpreter.AsIntPtr.Value = address;
                return (T)AddressHelper.reinterpreter.Asobject.Object;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct objectReinterpreter
        {
            [FieldOffset(0)] public ObjectWrapper Asobject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper
        {
            public object Object;
        }

        private class IntPtrWrapper
        {
            public IntPtr Value;
        }
    }
}
