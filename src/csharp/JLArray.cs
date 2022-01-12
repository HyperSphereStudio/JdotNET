using System;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano

namespace JuliaInterface
{

    [StructLayout(LayoutKind.Sequential)]
    public struct JLArray
    {
        internal IntPtr ptr;

        public JLArray(JLType type, long length) : this(JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(type, 1), length)) { }
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
        public JLGCStub Pin() => new JLVal(this).Pin();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();
        public long Length { get => new JLVal(this).Length; }
        public JLType ElType { get => new JLVal(this).ElType; }
        public JLVal Size { get => new JLVal(this).Size; }
        public void Add(JLVal val) => new JLVal(this).Add(val);
        public void RemoveAt(JLVal idx) => new JLVal(this).RemoveAt(idx);

        public JLVal this[int idx] {
            get => new JLVal(this)[idx];
            set => new JLVal(this).setEl(idx, value);
        }

        public object[] LinearNetUnPack(){
            object[] arr = new object[Length];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = this[i + 1].Value;
            return arr;
        }
    }
}