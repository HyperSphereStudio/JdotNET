using System;
using System.Collections.Generic;
using System.Reflection;
using Base;

namespace JULIAdotNET
{
    public static class JPrimitive
    {
        public static Any Base, Core, Main, Meta;
        public static Any SprintF, ShowErrorF, StringF, GetPropertyF, SetPropertyF,
            CatchBackTraceF, MakeArrayF, GetIndexF, SetIndexF, NamesF, LengthF, IterateF;
        private static readonly Dictionary<Type, Any> Sharp2Julia = new();
        private static readonly Dictionary<Any, Type> Julia2Sharp = new();

        internal static void RegisterPrimitive(Type t, Any type) {
            Sharp2Julia.Add(t, type);
            Julia2Sharp.Add(type, t);
        }
        
        
    }
}