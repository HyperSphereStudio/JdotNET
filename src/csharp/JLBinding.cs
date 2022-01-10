using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
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

        public static bool operator ==(JLBinding value1, JLBinding value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLBinding value1, JLBinding value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();

    }
}
