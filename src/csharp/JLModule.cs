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

        public JLArray Symbols { get { return JLFun.NamesF.Invoke(this); }  }

        public JLModule(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLModule value) => value.ptr;
        public static implicit operator JLModule(IntPtr ptr) => new JLModule(ptr);
        public static implicit operator JLModule(JLVal ptr) => new JLModule(ptr);
        public static implicit operator JLVal(JLModule ptr) => new JLVal(ptr);

        public static bool operator ==(JLModule value1, JLModule value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLModule value1, JLModule value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();

        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();
        public JLVal GetGlobal(JLSym name) => Julia.GetGlobal(this, name);
        public JLFun GetFunction(string name) => Julia.GetFunction(this, name);


        public static JLModule Base, Core, Main, JuliaInterface;

        internal static void init_mods(){
            Base = Julia.Eval("Base").ptr;
            Core = Julia.Eval("Core").ptr;
            Main = Julia.Eval("Main").ptr;
            JuliaInterface = Julia.Eval("Main.JuliaInterface").ptr;

        }
    }
}
