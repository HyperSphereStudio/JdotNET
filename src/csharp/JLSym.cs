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
    public struct JLSym
    {
        internal IntPtr ptr;

        public JLSym(IntPtr ptr) => this.ptr = ptr;
        public JLSym(string sym) : this(JuliaCalls.jl_symbol(sym).ptr) { }

        public static implicit operator JLSym(string sym) => new JLSym(sym);
        public static implicit operator string(JLSym sym) => new JLVal(sym.ptr).ToString();
        public static implicit operator IntPtr(JLSym value) => value.ptr;
        public static implicit operator JLSym(IntPtr ptr) => new JLSym(ptr);
        public static implicit operator JLSym(JLVal ptr) => new JLSym(ptr);
        public static implicit operator JLVal(JLSym ptr) => new JLVal(ptr.ptr);

        public static bool operator ==(JLSym value1, JLSym value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLSym value1, JLSym value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => new JLVal(ptr).Equals(o);
        public override int GetHashCode() => new JLVal(ptr).GetHashCode();
        public void Println() => new JLVal(ptr).Println();
        public void Print() => new JLVal(ptr).Print();
    }
}
