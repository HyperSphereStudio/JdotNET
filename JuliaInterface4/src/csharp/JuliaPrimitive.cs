using System;
using System.Collections.Generic;

namespace JULIAdotNET
{
    public static class JuliaPrimitive
    {
        public static JuliaV Base, Core, Main, Meta;
        public static JuliaV SprintF, ShowErrorF, StringF;
        private static readonly Dictionary<Type, JuliaV> Sharp2Julia = new();
        private static readonly Dictionary<JuliaV, Type> Julia2Sharp = new();

        internal static void RegisterPrimitive(Type t, JuliaV type) {
            Sharp2Julia.Add(t, type);
            Julia2Sharp.Add(type, t);
        }

        internal static void init() {
            Base = Julia.Eval("Base");
            Core = Julia.Eval("Core");
            Main = Julia.Eval("Main");
            Meta = Julia.Eval("Meta");

            SprintF = Julia.GetGlobal(Base, "sprint");
            ShowErrorF = Julia.GetGlobal(Base, "showerror");
            StringF = Julia.GetGlobal(Base, "string");
        }
    }
}