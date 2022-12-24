using System;
using System.Collections.Generic;

namespace JULIAdotNET
{
    public static partial class JPrimitive
    {
        private static readonly Dictionary<Type, JType> Sharp2Julia = new();
        private static readonly Dictionary<JType, Type> Julia2Sharp = new();

        private static void RegisterPrimitive(Type t, JType type) {
            Sharp2Julia.Add(t, type);
            Julia2Sharp.Add(type, t);
        }

        public static JType FindJuliaPrimitiveEquivilent(Type t) {
            if(Sharp2Julia.TryGetValue(t, out var v))
                return v;
            throw new Exception("No primitive Type for " + t + " Found!");
        }

        public static void init_primitive_types() {
            
        }
    }
}