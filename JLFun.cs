using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLFun
    {
        internal IntPtr ptr;

        public JLFun(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLFun value) => value.ptr;
        public static implicit operator JLFun(IntPtr ptr) => new JLFun(ptr);

        public static bool operator ==(JLFun value1, JLFun value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLFun value1, JLFun value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLFun && ((JLFun)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();
    }
}
