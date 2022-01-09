using System;
using System.Runtime.InteropServices;

namespace JuliaInterface
{

    [StructLayout(LayoutKind.Sequential)]
    public struct JLArray
    {
        internal IntPtr ptr;

        public JLArray(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLArray value) => value.ptr;
        public static implicit operator JLArray(IntPtr ptr) => new JLArray(ptr);

        public static bool operator ==(JLArray value1, JLArray value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLArray value1, JLArray value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLArray && ((JLArray)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();


    }
}