using System;
using System.Collections;
using Base;

//Written by Johnathan Bizzano

namespace JULIAdotNET
{

    public class ArrayEnumerator : IEnumerator {
        private Array array;
        private int index;
        private int endIndex;
        private int startIndex;
        private int[] _indices;
        private bool _complete;
        
        public ArrayEnumerator(Array array)
        {
            this.array = array;
            this.index = -1;
            startIndex = 0;
            endIndex = array.Length;
            _indices = new int[array.Rank];
            int checkForZero = 1;
            for (int i = 0; i < array.Rank; i++)
            {
                _indices[i] = array.GetLowerBound(i);
                checkForZero *= array.GetLength(i);
            }
            _indices[_indices.Length - 1]--;
            _complete = (checkForZero == 0);
        }

        private void IncArray()
        {
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
            get => array.GetValue(_indices);
            set => array.SetValue(value, _indices);
        }

        public int[] Index => _indices;

        public void Reset()
        {
            index = startIndex - 1;
            int checkForZero = 1;
            for (int i = 0; i < array.Rank; i++)
            {
                _indices[i] = array.GetLowerBound(i);
                checkForZero *= array.GetLength(i);
            }
            _complete = checkForZero == 0;
            _indices[_indices.Length - 1]--;
        }
    }
    
    public struct JArray
    {
        private Any _ptr;
        public int Length => (int) JPrimitive.LengthF.UnsafeInvoke(this);

        public JArray(JType type, long length) : this(JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(type, 1), (nint) length)) { }
        public JArray(JType type, long row, long col) : this(JuliaCalls.jl_alloc_array_2d(JuliaCalls.jl_apply_array_type(type, 2), (nint) row, (nint) col)) { }
        public JArray(JType type, long row, long col, long depth) : this(JuliaCalls.jl_alloc_array_3d(JuliaCalls.jl_apply_array_type(type, 3), (nint) row, (nint) col, (nint) depth)) { }
        public JArray(IntPtr ptr) => _ptr = ptr;
        
        public static implicit operator IntPtr(JArray value) => value._ptr;
        public static implicit operator JArray(IntPtr ptr) => new(ptr);
        public static implicit operator JArray(Any ptr) => new(ptr);
        public static implicit operator Any(JArray ptr) => new(ptr);

        public static unsafe JArray Alloc(JType eltype, params int[] dimensions){
            fixed (void* p = dimensions){
                return JPrimitive.MakeArrayF.Invoke(eltype, new Any(p), dimensions.Length);
            }
        }
        
        public override string ToString() => new Any(this).ToString();
        public override bool Equals(object o) => new Any(this).Equals(o);
        public override int GetHashCode() => new Any(this).GetHashCode();

        public Any this[Any idx] {
            get => _ptr[idx];
            set => _ptr[idx] = value;
        }
     
        public Any this[Any i1, Any i2] {
            get => _ptr[i1, i2];
            set => _ptr[i1, i2] = value;
        }
        
        public Any this[Any i1, Any i2, Any i3] {
            get => _ptr[i1, i2, i3];
            set => _ptr[i1, i2, i3] = value;
        }
        
        public Any this[Any i1, Any i2, Any i3, Any i4] {
            get => _ptr[i1, i2, i3, i4];
            set => _ptr[i1, i2, i3, i4] = value;
        }
        
        
    }
}