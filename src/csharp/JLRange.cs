using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public class JLRange
    {
        internal IntPtr ptr;

        public JLRange(IntPtr ptr) => this.ptr = ptr;
        public JLRange(long start, long end) => JLType.JLUnitRange.Create(start, end);
        public JLRange(long start, long step, long end) => JLType.JLStepRange.Create(start, step, end);
        public JLRange(double start, double end) => JLType.JLUnitRange.Create(start, end);
        public JLRange(double start, double step, double end) => JLType.JLStepRange.Create(start, step, end);


        public static implicit operator IntPtr(JLRange value) => value.ptr;
        public static implicit operator JLRange(IntPtr ptr) => new JLRange(ptr);
        public static implicit operator JLRange(JLVal ptr) => new JLRange(ptr);
        public static implicit operator JLVal(JLRange ptr) => new JLVal(ptr.ptr);

        public static bool operator ==(JLRange value1, IntPtr value2) => new JLVal(value1.ptr) == new JLVal(value2);
        public static bool operator !=(JLRange value1, IntPtr value2) => new JLVal(value1.ptr) != new JLVal(value2);
        public override string ToString() => new JLVal(ptr).ToString();
        public override bool Equals(object o) => new JLVal(ptr).Equals(o);
        public override int GetHashCode() => new JLVal(ptr).GetHashCode();
        public void Println() => new JLVal(ptr).Println();
        public void Print() => new JLVal(ptr).Print();
    }
}
