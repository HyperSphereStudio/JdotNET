using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JULIAdotNET;
using JULIAdotNET.Dynamics;

namespace Base {
    public struct JEnumerator : IEnumerator<Any> {
        private readonly Any _ptr;
        private Any _state;
        public Any Index => _state[2];

        public JEnumerator(Any ptr) {
            _ptr = ptr;
            _state = IntPtr.Zero;
        }

        public bool MoveNext() {
            _state = _state == IntPtr.Zero ? JPrimitive.IterateF.Invoke(_ptr) : JPrimitive.IterateF.Invoke(_ptr, Index);
            return Index != IntPtr.Zero;
        }

        public void Reset() => _state = IntPtr.Zero;
        public Any Current => _state[1];
        object IEnumerator.Current => Current;
        public void Dispose(){}
    }

    
    [StructLayout(LayoutKind.Sequential)]
    public struct Any : IDynamicMetaObjectProvider, IEnumerable<Any> {
        private readonly IntPtr ptr;

        public Any(IntPtr ptr) => this.ptr = ptr;

        #region Conversion

        public unsafe Any(void* l) : this(JuliaCalls.jl_box_voidpointer(new IntPtr(l))) {}

        public Any(long l) : this(JuliaCalls.jl_box_int64(l))
        {
        }

        public Any(int l) : this(JuliaCalls.jl_box_int32(l))
        {
        }

        public Any(short l) : this(JuliaCalls.jl_box_int16(l))
        {
        }

        public Any(sbyte l) : this(JuliaCalls.jl_box_int8(l))
        {
        }

        public Any(ulong l) : this(JuliaCalls.jl_box_uint64(l))
        {
        }

        public Any(uint l) : this(JuliaCalls.jl_box_uint32(l))
        {
        }

        public Any(ushort l) : this(JuliaCalls.jl_box_uint16(l))
        {
        }

        public Any(byte l) : this(JuliaCalls.jl_box_uint8(l))
        {
        }

        public Any(char l) : this(JuliaCalls.jl_box_int32(l))
        {
        }

        public Any(bool l) : this(JuliaCalls.jl_box_bool(l))
        {
        }

        public Any(double l) : this(JuliaCalls.jl_box_float64(l))
        {
        }

        public Any(float l) : this(JuliaCalls.jl_box_float32(l))
        {
        }

        public Any(string s) : this(JuliaCalls.jl_cstr_to_string(s))
        {
        }


        public static implicit operator IntPtr(Any value) => value.ptr;
        public static implicit operator Any(IntPtr ptr) => new Any(ptr);
        public static implicit operator Any(long l) => new Any(l);
        public static implicit operator Any(ulong l) => new Any(l);
        public static implicit operator Any(int l) => new(l);
        public static implicit operator Any(uint l) => new Any(l);
        public static implicit operator Any(short l) => new Any(l);
        public static implicit operator Any(ushort l) => new Any(l);
        public static implicit operator Any(string l) => new Any(l);
        public static implicit operator Any(double l) => new Any(l);
        public static implicit operator Any(float l) => new Any(l);
        public static implicit operator Any(char l) => new Any(l);
        public static implicit operator Any(bool l) => new Any(l);
        public static implicit operator Any(byte l) => new Any(l);
        public static implicit operator Any(sbyte l) => new Any(l);

        public static explicit operator long(Any value) => value.UnboxInt64();
        public static explicit operator ulong(Any value) => value.UnboxUInt64();
        public static explicit operator int(Any value) => value.UnboxInt32();
        public static explicit operator uint(Any value) => value.UnboxUInt32();
        public static explicit operator short(Any value) => value.UnboxInt16();
        public static explicit operator ushort(Any value) => value.UnboxUInt16();
        public static explicit operator byte(Any value) => value.UnboxUInt8();
        public static explicit operator sbyte(Any value) => value.UnboxInt8();
        public static explicit operator string(Any value) => value.UnboxString();
        public static explicit operator char(Any value) => value.UnboxChar();
        public static explicit operator bool(Any value) => value.UnboxBool();
        public static explicit operator double(Any value) => value.UnboxFloat64();
        public static explicit operator float(Any value) => value.UnboxFloat32();

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

        public Any Invoke()
        {
            var val = UnsafeInvoke();
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke(Any arg1)
        {
            var val = UnsafeInvoke(arg1);
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke1(Any arg1)
        {
            var val = UnsafeInvoke(arg1);
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke(DyAny arg1, Any arg2) => Invoke(arg1.Ptr, arg2);
        
        public Any Invoke(Any arg1, Any arg2) {
            Any val = UnsafeInvoke(arg1, arg2);
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke(Any arg1, Any arg2, Any arg3)
        {
            var val = UnsafeInvoke(arg1, arg2, arg3);
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke(params Any[] args)
        {
            var val = UnsafeInvoke(args);
            Julia.CheckExceptions();
            return val;
        }

        public Any InvokeSplat(params Any[] args)
        {
            var val = UnsafeInvokeSplat(args);
            Julia.CheckExceptions();
            return val;
        }

        public Any Invoke(Span<Any> args)
        {
            var val = UnsafeInvoke(args);
            Julia.CheckExceptions();
            return val;
        }

        public Any InvokeSplat(Span<Any> args)
        {
            var val = UnsafeInvokeSplat(args);
            Julia.CheckExceptions();
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Any UnsafeInvoke() => JuliaCalls.jl_call0(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Any UnsafeInvoke(Any arg1) => JuliaCalls.jl_call1(this, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Any UnsafeInvoke1(Any arg1) => JuliaCalls.jl_call1(this, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Any UnsafeInvoke(Any arg1, Any arg2) => JuliaCalls.jl_call2(this, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Any UnsafeInvoke(Any arg1, Any arg2, Any arg3) =>
            JuliaCalls.jl_call3(this, arg1, arg2, arg3);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvoke(Span<Any> args) =>
            UnsafeInvoke((Any*)Unsafe.AsPointer(ref args.GetPinnableReference()), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvoke(Any* args, int length) => JuliaCalls.jl_call(this, args, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvokeSplat(Span<Any> args) =>
            UnsafeInvokeSplat((Any*) Unsafe.AsPointer(ref args.GetPinnableReference()), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvokeSplat(Any* v, int length) => JuliaCalls.jl_call(this, v, length);

        #endregion
        #region Array
        public Any this[Any idx] {
            get => JPrimitive.GetIndexF.UnsafeInvoke(ptr, idx);
            set => JPrimitive.SetIndexF.UnsafeInvoke(ptr, idx, value);
        }
        public Any this[Any i1, Any i2] {
            get => JPrimitive.GetIndexF.UnsafeInvoke(ptr, i1, i2);
            set => JPrimitive.SetIndexF.UnsafeInvoke(stackalloc Any[]{ptr, value, i1, i2});
        }
        public Any this[Any i1, Any i2, Any i3] {
            get => JPrimitive.GetIndexF.UnsafeInvoke(stackalloc Any[]{ptr, i1, i2, i3});
            set => JPrimitive.SetIndexF.UnsafeInvoke(stackalloc Any[]{ptr, value, i1, i2, i3});
        }
        public Any this[Any i1, Any i2, Any i3, Any i4] {
            get => JPrimitive.GetIndexF.UnsafeInvoke(stackalloc Any[]{ptr, i1, i2, i3, i4});
            set => JPrimitive.SetIndexF.UnsafeInvoke(stackalloc Any[]{ptr, value, i1, i2, i3, i4});
        }
        #endregion
        #region UsefulFunctions
        public int Length => (int) JPrimitive.LengthF.UnsafeInvoke(this);
        public bool Is(Any ty) => Julia.Isa(this, ty);
        #endregion
        #region Comparison

        public static bool operator ==(Any v, IntPtr p) => v.ptr == p;
        public static bool operator !=(Any v, IntPtr p) => v.ptr != p;
        public static bool operator ==(IntPtr p, Any v) => v.ptr == p;
        public static bool operator !=(IntPtr p, Any v) => v.ptr != p;
        public static bool operator ==(Any v, Any v2) => (bool) JPrimitive.EqualsF.Invoke(v, v2);
        public static bool operator !=(Any v, Any v2) => (bool) JPrimitive.NEqualsF.Invoke(v, v2);
        public static bool operator >(Any v, Any v2) => (bool) JPrimitive.GreaterThenF.Invoke(v, v2);
        public static bool operator <(Any v, Any v2) => (bool) JPrimitive.LessThenF.Invoke(v, v2);
        public static bool operator >=(Any v, Any v2) => (bool) JPrimitive.GreaterThenOrEqualF.Invoke(v, v2);
        public static bool operator <=(Any v, Any v2) => (bool) JPrimitive.LessThenOrEqualF.Invoke(v, v2);
        public static Any operator !(Any v) => (bool)JPrimitive.NotF.Invoke(v);
        #endregion
        #region Math
        public static Any operator ~(Any v) => JPrimitive.TildeF.Invoke(v);
        public static Any operator ^(Any v, Any v2) => JPrimitive.CaretF.Invoke(v, v2);
        public static Any operator &(Any v, Any v2) => JPrimitive.AmpersandF.Invoke(v, v2);
        public static Any operator |(Any v, Any v2) =>  JPrimitive.PipeF.Invoke(v, v2);
        public static Any operator %(Any v, Any v2) =>  JPrimitive.PercentF.Invoke(v, v2);
        public static Any operator *(Any v, Any v2) =>  JPrimitive.MultF.Invoke(v, v2);
        public static Any operator +(Any v, Any v2) =>  JPrimitive.AddF.Invoke(v, v2);
        public static Any operator -(Any v, Any v2) => JPrimitive.SubF.Invoke(v, v2);
        public static Any operator /(Any v, Any v2) => JPrimitive.DivF.Invoke(v, v2);
        public static Any operator >>(Any v, int n) => JPrimitive.RightShiftF.Invoke(v, n);
        public static Any operator <<(Any v, int n) =>  JPrimitive.LeftShiftF.Invoke(v, n);

        #endregion

        public override string ToString() => ptr == IntPtr.Zero ? "null" : JPrimitive.StringF.Invoke1(this).UnboxString();
        IEnumerator IEnumerable.GetEnumerator() => new JEnumerator(this);
        public IEnumerator<Any> GetEnumerator() => new JEnumerator(this);
        public DynamicMetaObject GetMetaObject(Expression parameter) => new JuliaDynamic(parameter, new(this));
    }
}