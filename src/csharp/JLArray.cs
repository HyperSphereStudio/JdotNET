using System;
using System.Collections;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano

namespace JuliaInterface
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
        internal IntPtr ptr;

        public JLArray(JLType type, long length) : this(JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(type, 1), length)) { }
        public JLArray(JLType type, long row, long col) : this(JuliaCalls.jl_alloc_array_2d(JuliaCalls.jl_apply_array_type(type, 2), row, col)) { }
        public JLArray(JLType type, long row, long col, long depth) : this(JuliaCalls.jl_alloc_array_3d(JuliaCalls.jl_apply_array_type(type, 3), row, col, depth)) { }
        public JLArray(Array a) : this(BoxArray<object>(a, JLType.JLAny, v => new JLVal(v))) { }
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

        private long[] _UnboxLongArray(){
            long[] arr = new long[Length];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = this[i + 1].UnboxInt64();
            return arr;
        }

        public unsafe T* UnsafeRef<T>() where T : unmanaged => (T*)JuliaCalls.jl_array_ptr(this);

        public unsafe Array UnsafeCopy<T>() where T: unmanaged{
            var dims = Size._UnboxLongArray();
            var arr = Array.CreateInstance(typeof(T), dims);
            var raw_ptr = (T*) JuliaCalls.jl_array_ptr(this);
            GCHandle handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            Buffer.MemoryCopy(raw_ptr, AddressHelper.GetAddress(arr).ToPointer(), Length, Length);
            handle.Free();
            return arr;
        }

        private static unsafe JLArray BoxArray<T>(Array a, JLType elType, Func<T, JLVal> boxLambda){
            var dims = (JLVal) new JLArray(JLType.JLInt64, a.Rank);

            for (int i = 0; i < a.Rank; ++i)
                dims[i + 1] = (long) a.GetLength(i);

            var jlarray = JLFun.MakeArrayF.Invoke(elType, dims);
            var iter = JLFun.EachIndexF.Invoke(jlarray);
            var next = JLFun.IterateF.Invoke(iter);
            var arriter = new ArrayEnumerator(a);

            while (arriter.MoveNext())
            {
                var state = next[2];
                jlarray[next[1]] = boxLambda((T) arriter.Current);
                next = JLFun.IterateF.Invoke(iter, state);
            }

            return jlarray;
        }

        //TODO: If elType != Any && elType == this.GetJLType() => Copy Memory Directly
        private unsafe Array UnboxArray<T>(Func<JLVal, T> unboxLambda)
        {
            var dims = Size._UnboxLongArray();
            var arr = Array.CreateInstance(typeof(T), dims);
            var iter = JLFun.EachIndexF.Invoke(this);
            var next = JLFun.IterateF.Invoke(iter);
            var arriter = new ArrayEnumerator(arr);

            while (arriter.MoveNext()){
                var state = next[2];
                arriter.Current = unboxLambda(this[next[1]]);
                next = JLFun.IterateF.Invoke(iter, state);
            }

            return arr;
        }

        public Array UnboxInt64Array() => UnboxArray(v => v.UnboxInt64());
        public Array UnboxInt32Array() => UnboxArray(v => v.UnboxInt32());
        public Array UnboxInt16Array() => UnboxArray(v => v.UnboxInt16());
        public Array UnboxInt8Array() => UnboxArray(v => v.UnboxInt8());
        public Array UnboxBoolArray() => UnboxArray(v => v.UnboxBool());
        public Array UnboxFloat64Array() => UnboxArray(v => v.UnboxFloat64());
        public Array UnboxFloat32Array() => UnboxArray(v => v.UnboxFloat32());
        public Array UnboxPtrArray() => UnboxArray(v => v.UnboxPtr());
        public Array UnboxObjectArray() => UnboxArray(v => v.Value);
        public Array UnboxUInt64Array() => UnboxArray(v => v.UnboxUInt64());
        public Array UnboxUInt32Array() => UnboxArray(v => v.UnboxUInt32());
        public Array UnboxUInt16Array() => UnboxArray(v => v.UnboxUInt16());
        public Array UnboxUInt8Array() => UnboxArray(v => v.UnboxUInt8());
        public Array UnboxCharArray() => UnboxArray(v => v.UnboxChar());
        public Array UnboxStringArray() => UnboxArray(v => v.UnboxString());


    }
}