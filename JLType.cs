using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLType
    {
        internal IntPtr ptr;

        public JLType(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLType value) => value.ptr;
        public static implicit operator JLType(IntPtr ptr) => new JLType(ptr);

        public static bool operator ==(JLType value1, JLType value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLType value1, JLType value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLType && ((JLType)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();
    }
}
