using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLModule
    {
        internal IntPtr ptr;

        public JLModule(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLModule value) => value.ptr;
        public static implicit operator JLModule(IntPtr ptr) => new JLModule(ptr);

        public static bool operator ==(JLModule value1, JLModule value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLModule value1, JLModule value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLModule && ((JLModule)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();
    }
}
