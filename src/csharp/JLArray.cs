using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using static JULIAdotNET.ObjectManager;

//Written by Johnathan Bizzano

namespace JULIAdotNET
{

    public class ArrayEnumerator : IEnumerator
    {
        private Array array;
        private int index;
        private int endIndex;
        private int startIndex;    // Save for Reset.
        private int[] _indices;    // The current position in a multidim array
        private bool _complete;


        public ArrayEnumerator(Array array)
        {
            this.array = array;
            this.index = -1;
            startIndex = 0;
            endIndex = array.Length;
            _indices = new int[array.Rank];
            int checkForZero = 1;  // Check for dimensions of size 0.
            for (int i = 0; i < array.Rank; i++)
            {
                _indices[i] = array.GetLowerBound(i);
                checkForZero *= array.GetLength(i);
            }
            // To make MoveNext simpler, decrement least significant index.
            _indices[_indices.Length - 1]--;
            _complete = (checkForZero == 0);
        }

        private void IncArray()
        {
            // This method advances us to the next valid array index,
            // handling all the multiple dimension & bounds correctly.
            // Think of it like an odometer in your car - we start with
            // the last digit, increment it, and check for rollover.  If
            // it rolls over, we set all digits to the right and including 
            // the current to the appropriate lower bound.  Do these overflow
            // checks for each dimension, and if the most significant digit 
            // has rolled over it's upper bound, we're done.
            //
            int rank = array.Rank;
            _indices[rank - 1]++;
            for (int dim = rank - 1; dim >= 0; dim--)
            {
                if (_indices[dim] > array.GetUpperBound(dim))
                {
                    if (dim == 0)
                    {
                        _complete = true;
                        break;
                    }
                    for (int j = dim; j < rank; j++)
                        _indices[j] = array.GetLowerBound(j);
                    _indices[dim - 1]++;
                }
            }
        }

        public bool MoveNext()
        {
            if (_complete)
            {
                index = endIndex;
                return false;
            }
            index++;
            IncArray();
            return !_complete;
        }

        public object Current {
            get { return array.GetValue(_indices); }
            set { array.SetValue(value, _indices); }
        }

        public int[] Index { get => _indices; }

        public void Reset()
        {
            index = startIndex - 1;
            int checkForZero = 1;
            for (int i = 0; i < array.Rank; i++)
            {
                _indices[i] = array.GetLowerBound(i);
                checkForZero *= array.GetLength(i);
            }
            _complete = (checkForZero == 0);
            // To make MoveNext simpler, decrement least significant index.
            _indices[_indices.Length - 1]--;
        }
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct JLArray
    {
        private static MethodInfo unmangedcopyfrom = typeof(JLArray).GetMethod("UnsafeCopyFrom`1");

        internal IntPtr ptr;

        public JLArray(JLType type, long length) : this(JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(type, 1), length)) { }
        public JLArray(JLType type, long row, long col) : this(JuliaCalls.jl_alloc_array_2d(JuliaCalls.jl_apply_array_type(type, 2), row, col)) { }
        public JLArray(JLType type, long row, long col, long depth) : this(JuliaCalls.jl_alloc_array_3d(JuliaCalls.jl_apply_array_type(type, 3), row, col, depth)) { }
        public JLArray(Array a) : this(BoxArray(a)) { }
        public JLArray(IntPtr ptr) => this.ptr = ptr;


        public static implicit operator IntPtr(JLArray value) => value.ptr;
        public static implicit operator JLArray(IntPtr ptr) => new JLArray(ptr);
        public static implicit operator JLArray(JLVal ptr) => new JLArray(ptr);
        public static implicit operator JLVal(JLArray ptr) => new JLVal(ptr);

        public static unsafe JLArray Alloc(JLType eltype, params int[] dimensions){
            fixed (void* p = dimensions){
                return JLFun._MakeArrayF.Invoke(eltype, new JLVal(p), dimensions.Length);
            }
        }

        public static bool operator ==(JLArray value1, IntPtr value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLArray value1, IntPtr value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();
        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();
        public long Length { get => new JLVal(this).Length; }
        public JLType ElType { get => new JLVal(this).ElType; }
        public JLArray Size { get => new JLVal(this).Size; }
        public void Add(JLVal val) => new JLVal(this).Add(val);
        public void Remove(JLVal val) => new JLVal(this).Remove(val);
        public void RemoveAt(JLVal idx) => new JLVal(this).RemoveAt(idx);
        public void RemoveAt(JLRange range) => new JLVal(this).RemoveAt(range);
        public void Clear() => new JLVal(this).Clear();

        public JLVal this[JLVal idx] {
            get => new JLVal(this)[idx];
            set => new JLVal(this).setEl(idx, value);
        }

        public unsafe T* UnsafeRef<T>() where T : unmanaged => (T*)JuliaCalls.jl_array_ptr(this);

        public unsafe T[] UnsafeCopyTo<T>(T[] dest) where T : unmanaged{
            if (ElType == JLType.JLAny)
                return (T[]) UnboxArray();
            return (T[]) UnsafeCopyTo(UnsafeRef<T>(), dest);
        }

        public unsafe T[,] UnsafeCopyTo<T>(T[,] dest) where T : unmanaged
        {
            if (ElType == JLType.JLAny)
                return (T[,])UnboxArray();
            return (T[,])UnsafeCopyTo(UnsafeRef<T>(), dest);
        }

        private unsafe Array UnsafeCopyTo<T>(T* src, Array dest) where T : unmanaged{
            Julia.PUSH_GC(this);
            var handle = GCHandle.Alloc(dest, GCHandleType.Pinned);
            Buffer.MemoryCopy(src, handle.AddrOfPinnedObject().ToPointer(), Length * sizeof(T), Length * sizeof(T));
            handle.Free();
            Julia.POP_GC();
            return dest;
        }

        public unsafe T[] UnsafeCopyTo<T>() where T : unmanaged => UnsafeCopyTo((T[]) Array.CreateInstance(typeof(T), Length));
      
        public unsafe JLArray UnsafeCopyFrom<T>(Array src) where T : unmanaged{
            Julia.PUSH_GC(this);
            var raw_ptr = UnsafeRef<T>();
            var handle = GCHandle.Alloc(src, GCHandleType.Pinned);
            Buffer.MemoryCopy(handle.AddrOfPinnedObject().ToPointer(), raw_ptr, Length * sizeof(T), Length * sizeof(T));
            handle.Free();    
            Julia.POP_GC();
            return this;
        }
        
        private static int[] getsize(Array a){
            int[] d = new int[a.Rank];
            for (int i = 0; i < a.Rank; ++i)
                d[i] = a.GetLength(i);
            return d;
        }

        private static JLArray BoxArray(Array a)
        {
            var jlarray = Alloc(JLType.JLAny, getsize(a));
            var iter = JLFun.EachIndexF.Invoke(jlarray);
            var next = JLFun.IterateF.Invoke(iter);
            var arriter = new ArrayEnumerator(a);
            while (arriter.MoveNext()){
                var state = next[2];
                jlarray[next[1]] = new JLVal(arriter.Current);
                next = JLFun.IterateF.Invoke(iter, state);
            }
            return jlarray;
        }

        public unsafe Array UnboxArray(){
            Array arr;
            if(sizeof(SizeT) == sizeof(Int64))
                arr = Array.CreateInstance(typeof(object), Size.UnboxNTuple<Int64>());
            else
                arr = Array.CreateInstance(typeof(object), Size.UnboxNTuple<Int32>());
            var iter = JLFun.EachIndexF.Invoke(this);
            var next = JLFun.IterateF.Invoke(iter);
            var arriter = new ArrayEnumerator(arr);

            while (arriter.MoveNext()){
                var state = next[2];
                arriter.Current = this[next[1]].Value;
                next = JLFun.IterateF.Invoke(iter, state);
            }
            return arr;
        }

        public unsafe T[] UnboxNTuple<T>() where T : unmanaged => (T[]) UnsafeCopyTo((T*) ptr, (T[]) Array.CreateInstance(typeof(T), Length));

        public unsafe T[] UnboxArray<T>() where T : unmanaged => UnsafeCopyTo<T>();

        public static JLArray CreateArray<T>(T[] src) where T : unmanaged => new JLArray(JLType.GetJLType(typeof(T)), src.Length).UnsafeCopyFrom<T>(src);

        public static JLArray CreateArray(Array a) => BoxArray(a);
    }
}