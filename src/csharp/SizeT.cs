using System;
using System.Runtime.InteropServices;

//Written By Johnathan Bizzano
namespace JuliaInterface
{
    public struct SizeT
    {
        public UIntPtr p;

        public SizeT(long i) => p = new UIntPtr((ulong) i);
        public SizeT(int i) => p = new UIntPtr((uint) i);
        public SizeT(uint i) => p = new UIntPtr(i);
        public SizeT(ulong i) => p = new UIntPtr(i);
        public SizeT(JLVal v){
            if (v.IsUInt64)
                p = new UIntPtr((ulong) v);
            else if (v.IsUInt32)
                p = new UIntPtr((uint) v);
            else if (v.IsInt32)
                p = new UIntPtr((uint) v);
            else if (v.IsInt64)
                p = new UIntPtr((ulong) v);
            else
                throw new Exception("Unknown Size T Value!");
        }

        public static implicit operator SizeT(uint i) => new SizeT(i);
        public static implicit operator SizeT(ulong i) => new SizeT(i);
        public static implicit operator SizeT(int i) => new SizeT(i);
        public static implicit operator SizeT(long i) => new SizeT(i);
        public static implicit operator SizeT(JLVal v) => new SizeT(v);
        public static implicit operator uint(SizeT i) => i.p.ToUInt32();
        public static implicit operator ulong(SizeT i) => i.p.ToUInt64();
        public static implicit operator int(SizeT i) => (int)i.p.ToUInt32();
        public static implicit operator long(SizeT i) => (long)i.p.ToUInt64();
        public static unsafe implicit operator JLVal(SizeT i) => sizeof(void*) == sizeof(long) ? (ulong) i : (uint) i;
    }
}

   