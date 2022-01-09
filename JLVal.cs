using System;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLVal
    {
        internal readonly IntPtr ptr;

        public JLVal(object o) : this(DotNotType(o).ptr) {}
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
        public JLVal(string s) : this(JuliaCalls.jl_cstr_to_string(s)) { }


        public static implicit operator IntPtr(JLVal value) => value.ptr;
        public static implicit operator JLVal(IntPtr ptr) => new JLVal(ptr);
        public static implicit operator JLVal(long l) => new JLVal(l);
        public static implicit operator JLVal(ulong l) => new JLVal(l);
        public static implicit operator JLVal(int l) => new JLVal(l);
        public static implicit operator JLVal(uint l) => new JLVal(l);
        public static implicit operator JLVal(short l) => new JLVal(l);
        public static implicit operator JLVal(ushort l) => new JLVal(l);
        public static implicit operator JLVal(string l) => new JLVal(l);
        public static implicit operator JLVal(double l) => new JLVal(l);
        public static implicit operator JLVal(float l) => new JLVal(l);
        public static implicit operator JLVal(bool l) => new JLVal(l);
        public static implicit operator JLVal(byte l) => new JLVal(l);
        public static implicit operator JLVal(sbyte l) => new JLVal(l);

        public static explicit operator long(JLVal value) => value.UnboxInt64();
        public static explicit operator ulong(JLVal value) => value.UnboxUInt64();
        public static explicit operator int(JLVal value) => value.UnboxInt32();
        public static explicit operator uint(JLVal value) => value.UnboxUInt32();
        public static explicit operator short(JLVal value) => value.UnboxInt16();
        public static explicit operator ushort(JLVal value) => value.UnboxUInt16();
        public static explicit operator byte(JLVal value) => value.UnboxUInt8();
        public static explicit operator sbyte(JLVal value) => value.UnboxInt8();
        public static explicit operator string(JLVal value) => value.UnboxString();
        public static explicit operator bool(JLVal value) => value.UnboxBool();
        public static explicit operator double(JLVal value) => value.UnboxFloat64();
        public static explicit operator float(JLVal value) => value.UnboxFloat32();



        public static bool operator ==(JLVal value1, JLVal value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLVal value1, JLVal value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLVal && ((JLVal)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();

        public long UnboxInt64() => JuliaCalls.jl_unbox_int64(this);
        public int UnboxInt32() => JuliaCalls.jl_unbox_int32(this);
        public short UnboxInt16() => JuliaCalls.jl_unbox_int16(this);
        public sbyte UnboxInt8() => JuliaCalls.jl_unbox_int8(this);
        public bool UnboxBool() => JuliaCalls.jl_unbox_bool(this);
        public ulong UnboxUInt64() => JuliaCalls.jl_unbox_uint64(this);
        public uint UnboxUInt32() => JuliaCalls.jl_unbox_uint32(this);
        public ushort UnboxUInt16() => JuliaCalls.jl_unbox_uint16(this);
        public byte UnboxUInt8() => JuliaCalls.jl_unbox_uint8(this);


        public double UnboxFloat64() => JuliaCalls.jl_unbox_float64(this);
        public float UnboxFloat32() => JuliaCalls.jl_unbox_float32(this);
        public string UnboxString() => Marshal.PtrToStringAnsi(JuliaCalls.jl_string_ptr(this));

        public uint Length() => JLFun.Length.Invoke(this).UnboxUInt32();

        public JLType GetJLType() => new JLType(JLFun.TypeOf.Invoke(this));

        public override string ToString() => JLFun.String.Invoke(this).UnboxString();

        public void Println() => JLFun.Println.Invoke(this);
        public void Print() => JLFun.Print.Invoke(this);


        public static JLVal DotNotType(object o){
            if (JLType.IsPointerType(o))
                return new JLVal((IntPtr) o);
            else{
                if (o is long)
                    return new JLVal((long)o);
                else if (o is ulong)
                    return new JLVal((ulong)o);
                else if (o is int)
                    return new JLVal((int)o);
                else if (o is uint)
                    return new JLVal((uint)o);
                else if (o is short)
                    return new JLVal((short)o);
                else if (o is ushort)
                    return new JLVal((ushort) o);
                else if (o is sbyte)
                    return new JLVal((sbyte) o);
                else if (o is byte)
                    return new JLVal((byte) o);
                else if (o is double)
                    return new JLVal((double) o);
                else if (o is float)
                    return new JLVal((float) o);
                else if (o is bool)
                    return new JLVal((bool) o);
                else if (o is string)
                    return new JLVal((string) o);
                else throw new Exception("Unable To Convert \"" + o + "\" To Julia Object!");
            }
        }

    }
}
