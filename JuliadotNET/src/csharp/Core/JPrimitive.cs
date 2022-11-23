using System;
using System.Collections.Generic;
using Base;

namespace JULIAdotNET
{
    public static class JPrimitive
    {
        public static Any BaseM, CoreM, MainM, MetaM;
        public static Any SprintF, ShowErrorF, StringF, GetPropertyF, SetPropertyF,
            CatchBackTraceF, MakeArrayF, GetIndexF, SetIndexF, NamesF, LengthF, IterateF, 
            
            EqualsF, NEqualsF, GreaterThenF, LessThenF, GreaterThenOrEqualF, LessThenOrEqualF,
            
            NotF, TildeF, CaretF, AmpersandF, PipeF, PercentF, MultF, AddF, SubF, DivF, RightShiftF, LeftShiftF,
            
            GetTypeF;

        public static Any ModuleT, TypeT, FunctionT, MethodT;
        private static readonly Dictionary<Type, Any> Sharp2Julia = new();
        private static readonly Dictionary<Any, Type> Julia2Sharp = new();

        internal static void RegisterPrimitive(Type t, Any type) {
            Sharp2Julia.Add(t, type);
            Julia2Sharp.Add(type, t);
        }
        
    }
}