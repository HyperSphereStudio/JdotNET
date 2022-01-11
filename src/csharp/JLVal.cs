using System;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano 
namespace JuliaInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JLVal
    {
        internal readonly IntPtr ptr;

        public object Value {
            get {
                if (IsFloat64())
                    return UnboxFloat64();
                else if (IsFloat32())
                    return UnboxFloat32();
                else if (IsString())
                    return UnboxString();
                else if (IsInt64())
                    return UnboxInt64();
                else if (IsInt32())
                    return UnboxInt32();
                else if (IsInt16())
                    return UnboxInt16();
                else if (IsInt8())
                    return UnboxInt8();
                else if (IsBool())
                    return UnboxBool();
                else if (IsUInt64())
                    return UnboxUInt64();
                else if (IsUInt32())
                    return UnboxUInt32();
                else if (IsUInt16())
                    return UnboxUInt16();
                else if (IsUInt8())
                    return UnboxUInt8();
                else if (IsSharpObject())
                    return UnboxSharpObject();
                else if (IsPtr())
                    return UnboxPtr();
                else return this;
            }
        }

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
        public override int GetHashCode() => JLFun.HashCodeF.Invoke(this).UnboxInt32();

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
        public IntPtr UnboxPtr() => JuliaCalls.jl_unbox_voidpointer(this);
        public string UnboxString() => Julia.UnboxString(this);

        public object UnboxSharpObject() => AddressHelper.GetInstance<object>((IntPtr) JLFun.GetFieldF.Invoke(this, (JLSym) "ptr").UnboxInt64());

        public bool IsFloat64() => JuliaCalls.jl_isa(this, JLType.JLFloat64) != 0;
        public bool IsFloat32() => JuliaCalls.jl_isa(this, JLType.JLFloat32) != 0;
        public bool IsInt64() => JuliaCalls.jl_isa(this, JLType.JLInt64) != 0;
        public bool IsInt32() => JuliaCalls.jl_isa(this, JLType.JLInt32) != 0;
        public bool IsInt16() => JuliaCalls.jl_isa(this, JLType.JLInt16) != 0;
        public bool IsInt8() => JuliaCalls.jl_isa(this, JLType.JLInt8) != 0;
        public bool IsUInt64() => JuliaCalls.jl_isa(this, JLType.JLUInt64) != 0;
        public bool IsUInt32() => JuliaCalls.jl_isa(this, JLType.JLUInt32) != 0;
        public bool IsUInt16() => JuliaCalls.jl_isa(this, JLType.JLUInt16) != 0;
        public bool IsUInt8() => JuliaCalls.jl_isa(this, JLType.JLUInt8) != 0;
        public bool IsString() => JuliaCalls.jl_isa(this, JLType.JLString) != 0;
        public bool IsBool() => JuliaCalls.jl_isa(this, JLType.JLBool) != 0;
        public bool IsPtr() => JuliaCalls.jl_isa(this, JLType.JLPtr) != 0;
        public bool IsSharpObject() => JuliaCalls.jl_isa(this, JLType.SharpObject) != 0;

        public JLVal GetFieldValue(JLSym fieldName) => JLFun.GetFieldF.Invoke(this, fieldName);
        public JLVal SetFieldValue(JLSym fieldName, JLVal v) => JLFun.SetField_F.Invoke(this, fieldName, v);

        public SizeT Length() => JLFun.LengthF.Invoke(this);
        public JLType GetJLType() => new JLType(JLFun.TypeOfF.Invoke(this));
        public override string ToString() => JLFun.StringF.Invoke(this).UnboxString();
        public void Println() => JLFun.PrintlnF.Invoke(this);
        public void Print() => JLFun.PrintF.Invoke(this);
        public bool IsNull() => ptr == IntPtr.Zero;

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
                    return new JLVal((ushort)o);
                else if (o is sbyte)
                    return new JLVal((sbyte)o);
                else if (o is byte)
                    return new JLVal((byte)o);
                else if (o is double)
                    return new JLVal((double)o);
                else if (o is float)
                    return new JLVal((float)o);
                else if (o is bool)
                    return new JLVal((bool)o);
                else if (o is string)
                    return new JLVal((string)o);
                else if (o is IntPtr)
                    return Julia.BoxPtr((IntPtr) o);
                else
                    return Julia.CreateStruct(JLType.SharpObject, AddressHelper.GetAddress(o).ToInt64());
            }
        }

    }
}
