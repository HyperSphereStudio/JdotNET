using System;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano

namespace JuliaInterface
{

    [StructLayout(LayoutKind.Sequential)]
    public struct JLArray
    {
        internal IntPtr ptr;

        public long Length {get => JLFun.LengthF.Invoke(this).UnboxInt64();}
        


        public JLArray(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLArray value) => value.ptr;
        public static implicit operator JLArray(IntPtr ptr) => new JLArray(ptr);
        public static implicit operator JLArray(JLVal ptr) => new JLArray(ptr);
        public static implicit operator JLVal(JLArray ptr) => new JLVal(ptr);

        public static bool operator ==(JLArray value1, IntPtr value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLArray value1, IntPtr value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();
        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();

        public long Size(int idx) => JuliaCalls.jl_array_size(this, idx);
        public JLVal GetElement(JLVal idx) => JLFun.GetIndexF.Invoke(this, idx);
        public JLVal SetElement(JLVal idx, JLVal v) => JLFun.SetIndex_F.Invoke(this, idx, v);

        public object[] LinearNetUnPack(){
            object[] arr = new object[Length];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = JLFun.GetIndexF.Invoke(this, i + 1).Value;
            return arr;
        }
    }
}