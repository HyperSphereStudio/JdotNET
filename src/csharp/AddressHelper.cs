using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JULIAdotNET
{
    public static class AddressHelper
    {
        private static object mutualObject;
        private static ObjectReinterpreter reinterpreter;

        static AddressHelper(){
            mutualObject = new object();
            reinterpreter = new ObjectReinterpreter();
            reinterpreter.AsObject = new ObjectWrapper();
        }

        public static IntPtr GetObjectAddress(object obj){
            lock (mutualObject)
            {
                reinterpreter.AsObject.Object = obj;
                IntPtr address = reinterpreter.AsIntPtr.Value;
                reinterpreter.AsObject.Object = null;
                return address;
            }
        }

        private static T GetObjectInstance<T>(IntPtr address){
            lock (mutualObject)
            {
                reinterpreter.AsIntPtr.Value = address;
                T obj = (T) reinterpreter.AsObject.Object;
                reinterpreter.AsObject.Object = null;
                return obj;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectReinterpreter{
            [FieldOffset(0)] public ObjectWrapper AsObject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper{ public object Object; }
        private class IntPtrWrapper{public IntPtr Value;}

    }
}
