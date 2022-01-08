using System;
using System.Runtime.InteropServices;

namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLVal
    {
        internal readonly IntPtr ptr;

        public JLVal(IntPtr ptr) => this.ptr = ptr;
        public JLVal(long l) : this(JuliaCalls.jl_box_int64(l)) { }
        public JLVal(int l) : this(JuliaCalls.jl_box_int32(l)) { }
        public JLVal(short l) : this(JuliaCalls.jl_box_int16(l)) { }
        public JLVal(sbyte l) : this(JuliaCalls.jl_box_int8(l)) { }
        public JLVal(ulong l) : this(JuliaCalls.jl_box_uint64(l)) { }
        public JLVal(uint l) : this(JuliaCalls.jl_box_uint32(l)) { }
        public JLVal(ushort l) : this(JuliaCalls.jl_box_uint16(l)) { }
        public JLVal(byte l) : this(JuliaCalls.jl_box_uint8(l)) { }
        public JLVal(bool l) : this(JuliaCalls.jl_box_bool(l)) { }
        public JLVal(double l) : this(JuliaCalls.jl_box_float64(l)) { }
        public JLVal(float l) : this(JuliaCalls.jl_box_float32(l)) { }
        public JLVal(string s) : this(JuliaCalls.jl_box_string(s)) { }


        public static implicit operator IntPtr(JLVal value) => value.ptr;
        public static implicit operator JLVal(IntPtr ptr) => new JLVal(ptr);

        public static bool operator ==(JLVal value1, JLVal value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLVal value1, JLVal value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLVal && ((JLVal)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();

        public long UnboxInt64() => JuliaCalls.jl_unbox_int64(this);
        public int UnboxInt32() => JuliaCalls.jl_unbox_int32(this);
        public short UnboxInt16() => JuliaCalls.jl_unbox_int16(this);
        public sbyte UnboxInt8() => JuliaCalls.jl_unbox_int8(this);
        public bool UnboxIntBool() => JuliaCalls.jl_unbox_bool(this);
        public ulong UnboxUInt64() => JuliaCalls.jl_unbox_uint64(this);
        public uint UnboxUInt32() => JuliaCalls.jl_unbox_uint32(this);
        public ushort UnboxUInt16() => JuliaCalls.jl_unbox_uint16(this);
        public byte UnboxUInt8() => JuliaCalls.jl_unbox_uint8(this);


        public double UnboxFloat64() => JuliaCalls.jl_unbox_float64(this);
        public float UnboxFloat32() => JuliaCalls.jl_unbox_float32(this);


        
    }
}
