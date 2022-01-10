using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLModule
    {
        internal IntPtr ptr;

        public JLModule(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLModule value) => value.ptr;
        public static implicit operator JLModule(IntPtr ptr) => new JLModule(ptr);
        public static implicit operator JLModule(JLVal ptr) => new JLModule(ptr);
        public static implicit operator JLVal(JLModule ptr) => new JLVal(ptr);

        public static bool operator ==(JLModule value1, JLModule value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLModule value1, JLModule value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();



        public static JLModule Base, Core, Main, JuliaInterface;

        internal static void init_mods(){
            Base = Julia.Eval("Base").ptr;
            Core = Julia.Eval("Core").ptr;
            Main = Julia.Eval("Main").ptr;
            JuliaInterface = Julia.Eval("Main.JuliaInterface").ptr;

        }
    }
}
