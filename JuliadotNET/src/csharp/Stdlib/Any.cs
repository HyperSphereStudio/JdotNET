using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JULIAdotNET;
using JULIAdotNET.Dynamics;

namespace Base
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Any : IDynamicMetaObjectProvider, JEnumerable<Any, Any, Any, Any>, JVal<Any>
    {
        private readonly IntPtr ptr;
        Any JVal<Any>.This => this;

        public Any(IntPtr ptr) => this.ptr = ptr;

        #region Conversion

        public unsafe Any(void* l) : this(JuliaCalls.jl_box_voidpointer(new IntPtr(l)))
        {
        }

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

        public Any(Half l) : this(JuliaCalls.jl_box_float32((float)l))
        {
        }

        public Any(string s) : this(JuliaCalls.jl_cstr_to_string(s))
        {
        }

        public Any(Array a) : this(new JArray(a))
        {
        }
        public Any(Array a, bool own) : this(new JArray(a, own))
        {
        }
        public Any(object o) : this(CreateFromObject(o)){}
        
        private static Any CreateFromObject(object o) {
            if (o.GetType().IsPrimitive)
                switch (o) {
                    case bool i: return i;
                    case char i: return i;
                    
                    case sbyte i: return i;
                    case short i: return i;
                    case int i: return i;
                    case long i: return i;
                    
                    case byte i: return i;
                    case ushort i: return i;
                    case uint i: return i;
                    case ulong i: return i;
                    
                    case Half i: return i;
                    case float i: return i;
                    case double i: return i;
                }
            else if (o is string s)
                return s;
            else if (o is Array a)
                return new(a);
            else if (o is IntPtr i)
                return i;
            throw new Exception("Unable To Box:" + o + " To Julia");
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
        public static implicit operator Any(Half l) => new Any(l);
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
        public static explicit operator Half(Any value) => value.UnboxFloat16();
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
        public Half UnboxFloat16() => (Half)JuliaCalls.jl_unbox_float32(this);
        public char UnboxChar() => (char)JuliaCalls.jl_unbox_int32(this);
        public IntPtr UnboxPtr() => JuliaCalls.jl_unbox_voidpointer(this);
        public string UnboxString() => Julia.UnboxString(this);
        public object UnboxObject(bool throwOnError = false) {
            if (Is(JPrimitive.IntegerT))
                switch (this.SizeOf)
                {
                    case 1: return Is(JPrimitive.Int8T) ? (sbyte)this : (byte)this;
                    case 2: return Is(JPrimitive.Int16T) ? (short)this : (ushort)this;
                    case 4: return Is(JPrimitive.Int32T) ? (int)this : (uint)this;
                    case 8: return Is(JPrimitive.Int64T) ? (long)this : (ulong)this;
                }
            else if (Is(JPrimitive.AbstractFloatT))
                switch (this.SizeOf)
                {
                    case 2: return (Half)this;
                    case 4: return (float)this;
                    case 8: return (double)this;
                }
            else if (Is(JPrimitive.BoolT))
                return (bool)this;
            else if (Is(JPrimitive.CharT))
                return (char)this;
            else if (Is(JPrimitive.PtrT))
                return UnboxPtr();
            else if (Is(JPrimitive.StringT))
                return (string)this;
            else if(Is(JPrimitive.ArrayT))
                return (JArray) this;

            if (throwOnError) 
                throw new Exception("Unable To Unbox Object " + this);
            
            return null;
        }

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

        public Any Invoke(Any arg1, Any arg2)
        {
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
        public unsafe Any UnsafeInvoke(Span<Any> args) => UnsafeInvoke(args.ToPointer(), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvoke(Any* args, int length) => JuliaCalls.jl_call(this, args, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvokeSplat(Span<Any> args) =>
            UnsafeInvokeSplat((Any*)Unsafe.AsPointer(ref args.GetPinnableReference()), args.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe Any UnsafeInvokeSplat(Any* v, int length) => JuliaCalls.jl_call(this, v, length);

        #endregion

        #region Array

        public Any this[Any idx] {
            get => JPrimitive.getindexF.UnsafeInvoke(ptr, idx);
            set => JPrimitive.setindexNotF.UnsafeInvoke(ptr, idx, value);
        }

        public Any this[Any i1, Any i2] {
            get => JPrimitive.getindexF.UnsafeInvoke(ptr, i1, i2);
            set => JPrimitive.setindexNotF.UnsafeInvoke(stackalloc Any[] { ptr, value, i1, i2 });
        }

        public Any this[Any i1, Any i2, Any i3] {
            get => JPrimitive.getindexF.UnsafeInvoke(stackalloc Any[] { ptr, i1, i2, i3 });
            set => JPrimitive.setindexNotF.UnsafeInvoke(stackalloc Any[] { ptr, value, i1, i2, i3 });
        }

        public Any this[Any i1, Any i2, Any i3, Any i4] {
            get => JPrimitive.getindexF.UnsafeInvoke(stackalloc Any[] { ptr, i1, i2, i3, i4 });
            set => JPrimitive.setindexNotF.UnsafeInvoke(stackalloc Any[] { ptr, value, i1, i2, i3, i4 });
        }

        public unsafe Any this[Span<Any> args] {
            get {
                Span<Any> a = stackalloc Any[args.Length + 1];
                a[0] = ptr;
                Buffer.MemoryCopy(args.ToPointer(), a.ToPointer() + 1, a.Length, args.Length);
                return JPrimitive.getindexF.UnsafeInvoke(a);
            }

            set {
                Span<Any> a = stackalloc Any[args.Length + 2];
                a[0] = ptr;
                a[1] = value;
                Buffer.MemoryCopy(args.ToPointer(), a.ToPointer() + 2, a.Length, args.Length);
                JPrimitive.setindexNotF.UnsafeInvoke(a);
            }
        }

        #endregion

        #region UsefulFunctions

        public Any Module => JPrimitive.parentmoduleF.Invoke(this);
        public int Length => (int)JPrimitive.lengthF.UnsafeInvoke(this);
        public int SizeOf => (int)JPrimitive.sizeofF.Invoke(this);
        public JType Type => JPrimitive.typeofF.Invoke(this);
        public bool Is(JType ty) => Julia.Isa(this, ty);
        public static string operator +(string s, Any v) => s + v.ToString();
        public static string operator +(Any v, string s) => v.ToString() + s;

        #endregion

        #region Comparison

        public static bool operator ==(Any v, IntPtr p) => v.ptr == p;
        public static bool operator !=(Any v, IntPtr p) => v.ptr != p;
        public static bool operator ==(IntPtr p, Any v) => v.ptr == p;
        public static bool operator !=(IntPtr p, Any v) => v.ptr != p;
        public static bool operator ==(Any v, Any v2) => (bool)JPrimitive.EqualityF.Invoke(v, v2);
        public static bool operator !=(Any v, Any v2) => (bool)JPrimitive.InequalityF.Invoke(v, v2);
        public static bool operator >(Any v, Any v2) => (bool)JPrimitive.GreaterThanF.Invoke(v, v2);
        public static bool operator <(Any v, Any v2) => (bool)JPrimitive.LessThanF.Invoke(v, v2);
        public static bool operator >=(Any v, Any v2) => (bool)JPrimitive.GreaterThanOrEqualF.Invoke(v, v2);
        public static bool operator <=(Any v, Any v2) => (bool)JPrimitive.LessThanOrEqualF.Invoke(v, v2);
        public static Any operator !(Any v) => (bool)JPrimitive.NotF.Invoke(v);

        #endregion

        #region Math

        public static Any operator ~(Any v) => JPrimitive.OnesComplementF.Invoke(v);
        public static Any operator ^(Any v, Any v2) => JPrimitive.ExclusiveOrF.Invoke(v, v2);
        public static Any operator &(Any v, Any v2) => JPrimitive.BitwiseAndF.Invoke(v, v2);
        public static Any operator |(Any v, Any v2) => JPrimitive.BitwiseOrF.Invoke(v, v2);
        public static Any operator %(Any v, Any v2) => JPrimitive.ModulusF.Invoke(v, v2);
        public static Any operator *(Any v, Any v2) => JPrimitive.MultiplyF.Invoke(v, v2);
        public static Any operator +(Any v, Any v2) => JPrimitive.AdditionF.Invoke(v, v2);
        public static Any operator -(Any v, Any v2) => JPrimitive.SubtractionF.Invoke(v, v2);
        public static Any operator /(Any v, Any v2) => JPrimitive.DivisionF.Invoke(v, v2);
        public static Any operator >> (Any v, int n) => JPrimitive.RightShiftF.Invoke(v, n);
        public static Any operator <<(Any v, int n) => JPrimitive.LeftShiftF.Invoke(v, n);

        #endregion

        #region Enumerable

        void JEnumerable<Any, Any, Any, Any>.EnumerationReset(Any s, out Any ns) => ns = IntPtr.Zero;
        Any JEnumerable<Any, Any, Any, Any>.EnumerationCurrent(Any s) => s[1];

        void JEnumerable<Any, Any, Any, Any>.EnumerationDispose()
        {
        }

        bool JEnumerable<Any, Any, Any, Any>.EnumerationMoveNext(Any s, out Any ns)
        {
            ns = s == IntPtr.Zero ? JPrimitive.iterateF.Invoke(this) : JPrimitive.iterateF.Invoke(this, s[2]);
            return ns[2] != IntPtr.Zero;
        }

        Any JEnumerable<Any, Any, Any, Any>.EnumerationIndex(Any s) => s[2];

        #endregion

        #region Builtin

        public override string ToString() =>
            ptr == IntPtr.Zero ? "null" : JPrimitive.stringF.Invoke1(this).UnboxString();

        public override int GetHashCode() => (int)JPrimitive.hashF.Invoke1(this);
        public DynamicMetaObject GetMetaObject(Expression parameter) => new JuliaDynamic(parameter, new(this));
        public override bool Equals(object o) => o is Any a ? a == (Any) o : false;

        #endregion
    }
}