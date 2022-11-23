using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano 
namespace JULIAdotNET {
    [StructLayout(LayoutKind.Sequential)]
    public struct JuliaV : IDynamicMetaObjectProvider
    {
        internal readonly IntPtr ptr;

        public JuliaV(IntPtr ptr) => this.ptr = ptr;

        #region Conversion

        public unsafe JuliaV(void* l) : this(JuliaCalls.jl_box_voidpointer(new IntPtr(l)))
        {
        }

        public JuliaV(long l) : this(JuliaCalls.jl_box_int64(l))
        {
        }

        public JuliaV(int l) : this(JuliaCalls.jl_box_int32(l))
        {
        }

        public JuliaV(short l) : this(JuliaCalls.jl_box_int16(l))
        {
        }

        public JuliaV(sbyte l) : this(JuliaCalls.jl_box_int8(l))
        {
        }

        public JuliaV(ulong l) : this(JuliaCalls.jl_box_uint64(l))
        {
        }

        public JuliaV(uint l) : this(JuliaCalls.jl_box_uint32(l))
        {
        }

        public JuliaV(ushort l) : this(JuliaCalls.jl_box_uint16(l))
        {
        }

        public JuliaV(byte l) : this(JuliaCalls.jl_box_uint8(l))
        {
        }

        public JuliaV(char l) : this(JuliaCalls.jl_box_int32(l))
        {
        }

        public JuliaV(bool l) : this(JuliaCalls.jl_box_bool(l))
        {
        }

        public JuliaV(double l) : this(JuliaCalls.jl_box_float64(l))
        {
        }

        public JuliaV(float l) : this(JuliaCalls.jl_box_float32(l))
        {
        }

        public JuliaV(string s) : this(JuliaCalls.jl_cstr_to_string(s))
        {
        }


        public static implicit operator IntPtr(JuliaV value) => value.ptr;
        public static implicit operator JuliaV(IntPtr ptr) => new JuliaV(ptr);
        public static implicit operator JuliaV(long l) => new JuliaV(l);
        public static implicit operator JuliaV(ulong l) => new JuliaV(l);
        public static implicit operator JuliaV(int l) => new(l);
        public static implicit operator JuliaV(uint l) => new JuliaV(l);
        public static implicit operator JuliaV(short l) => new JuliaV(l);
        public static implicit operator JuliaV(ushort l) => new JuliaV(l);
        public static implicit operator JuliaV(string l) => new JuliaV(l);
        public static implicit operator JuliaV(double l) => new JuliaV(l);
        public static implicit operator JuliaV(float l) => new JuliaV(l);
        public static implicit operator JuliaV(char l) => new JuliaV(l);
        public static implicit operator JuliaV(bool l) => new JuliaV(l);
        public static implicit operator JuliaV(byte l) => new JuliaV(l);
        public static implicit operator JuliaV(sbyte l) => new JuliaV(l);

        public static explicit operator long(JuliaV value) => value.UnboxInt64();
        public static explicit operator ulong(JuliaV value) => value.UnboxUInt64();
        public static explicit operator int(JuliaV value) => value.UnboxInt32();
        public static explicit operator uint(JuliaV value) => value.UnboxUInt32();
        public static explicit operator short(JuliaV value) => value.UnboxInt16();
        public static explicit operator ushort(JuliaV value) => value.UnboxUInt16();
        public static explicit operator byte(JuliaV value) => value.UnboxUInt8();
        public static explicit operator sbyte(JuliaV value) => value.UnboxInt8();
        public static explicit operator string(JuliaV value) => value.UnboxString();
        public static explicit operator char(JuliaV value) => value.UnboxChar();
        public static explicit operator bool(JuliaV value) => value.UnboxBool();
        public static explicit operator double(JuliaV value) => value.UnboxFloat64();
        public static explicit operator float(JuliaV value) => value.UnboxFloat32();

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
        public char UnboxChar() => (char)JuliaCalls.jl_unbox_int32(this);
        public IntPtr UnboxPtr() => JuliaCalls.jl_unbox_voidpointer(this);
        public string UnboxString() => Julia.UnboxString(this);

        #endregion

        #region Invokation

        public JuliaV Invoke()
        {
            var val = UnsafeInvoke();
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke(JuliaV arg1)
        {
            var val = UnsafeInvoke(arg1);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke1(JuliaV arg1)
        {
            var val = UnsafeInvoke(arg1);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke(JuliaV arg1, JuliaV arg2)
        {
            var val = UnsafeInvoke(arg1, arg2);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke(JuliaV arg1, JuliaV arg2, JuliaV arg3)
        {
            var val = UnsafeInvoke(arg1, arg2, arg3);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke(params JuliaV[] args)
        {
            var val = UnsafeInvoke(args);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV InvokeSplat(params JuliaV[] args)
        {
            var val = UnsafeInvokeSplat(args);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV Invoke(Span<JuliaV> args)
        {
            var val = UnsafeInvoke(args);
            Julia.CheckExceptions();
            return val;
        }

        public JuliaV InvokeSplat(Span<JuliaV> args)
        {
            var val = UnsafeInvokeSplat(args);
            Julia.CheckExceptions();
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JuliaV UnsafeInvoke() => JuliaCalls.jl_call0(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JuliaV UnsafeInvoke(JuliaV arg1) => JuliaCalls.jl_call1(this, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JuliaV UnsafeInvoke1(JuliaV arg1) => JuliaCalls.jl_call1(this, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JuliaV UnsafeInvoke(JuliaV arg1, JuliaV arg2) => JuliaCalls.jl_call2(this, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JuliaV UnsafeInvoke(JuliaV arg1, JuliaV arg2, JuliaV arg3) =>
            JuliaCalls.jl_call3(this, arg1, arg2, arg3);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe JuliaV UnsafeInvoke(Span<JuliaV> args) =>
            UnsafeInvoke((JuliaV*)Unsafe.AsPointer(ref args.GetPinnableReference()), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe JuliaV UnsafeInvoke(JuliaV* args, int length) => JuliaCalls.jl_call(this, (IntPtr*)args, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe JuliaV UnsafeInvokeSplat(Span<JuliaV> args) =>
            UnsafeInvokeSplat((JuliaV*)Unsafe.AsPointer(ref args.GetPinnableReference()), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe JuliaV UnsafeInvokeSplat(JuliaV* v, int length) => JuliaCalls.jl_call(this, (IntPtr*)v, length);

        #endregion
        
        public override string ToString() => JuliaPrimitive.StringF.Invoke1(this).UnboxString();
        
        public DynamicMetaObject GetMetaObject(Expression parameter) {
            return null;
        }
    }
    
    
}