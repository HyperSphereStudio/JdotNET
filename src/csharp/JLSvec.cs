using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct JLSvec
    {
        private JLVal valPtr;
        public readonly SizeT Length;
        private readonly JLVal* data;
        
        public JLSvec(IntPtr ptr){
            valPtr = ptr;
            Length = *((SizeT*)ptr);
            data = (JLVal*) (ptr + sizeof(SizeT));
        }

        public JLVal this[int idx] {
            get => data[idx];
        }

        public static bool operator ==(JLSvec value1, IntPtr value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLSvec value1, IntPtr value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();
        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();


        public static implicit operator IntPtr(JLSvec value) => value.valPtr;
        public static implicit operator JLSvec(IntPtr ptr) => new JLSvec(ptr);
        public static implicit operator JLSvec(JLVal ptr) => new JLSvec(ptr);
        public static implicit operator JLVal(JLSvec ptr) => new JLVal(ptr.valPtr);

    }
}
