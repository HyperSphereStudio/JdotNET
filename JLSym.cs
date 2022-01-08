using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLSym
    {
        internal IntPtr ptr;

        public JLSym(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLSym value) => value.ptr;
        public static implicit operator JLSym(IntPtr ptr) => new JLSym(ptr);
        public static bool operator ==(JLSym value1, JLSym value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLSym value1, JLSym value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLSym && ((JLSym)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();
    }
}
