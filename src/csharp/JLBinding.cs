using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//Written by Johnathan Bizzano

namespace JULIAdotNET
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLBinding
    {
        internal IntPtr ptr;

        public JLBinding(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLBinding value) => value.ptr;
        public static implicit operator JLBinding(IntPtr ptr) => new JLBinding(ptr);
        public static implicit operator JLBinding(JLVal ptr) => new JLBinding(ptr);
        public static implicit operator JLVal(JLBinding ptr) => new JLVal(ptr.ptr);

        public static bool operator ==(JLBinding value1, IntPtr value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLBinding value1, IntPtr value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();
        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();

    }
}
