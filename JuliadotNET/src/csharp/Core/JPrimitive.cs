using System;
using System.Collections.Generic;
using Base;

namespace JULIAdotNET
{
    public static partial class JPrimitive
    {
        private static readonly Dictionary<Type, Any> Sharp2Julia = new();
        private static readonly Dictionary<Any, Type> Julia2Sharp = new();

        internal static void RegisterPrimitive(Type t, Any type) {
            Sharp2Julia.Add(t, type);
            Julia2Sharp.Add(type, t);
        }
        
    }
}