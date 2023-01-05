using System;
using System.Collections;
using Base;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        public int Length => (int) JPrimitive.lengthF.UnsafeInvoke(this);
        public int Rank => JuliaCalls.jl_array_rank(_ptr);
        public int Size(int d = 1) => (int) JuliaCalls.jl_array_size(_ptr, d);
        public JType ElementType => JuliaCalls.jl_array_eltype(_ptr);

        public JArray(JType type, long length) : this(JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(type, 1), (nint) length)) { }
        public JArray(JType type, long row, long col) : this(JuliaCalls.jl_alloc_array_2d(JuliaCalls.jl_apply_array_type(type, 2), (nint) row, (nint) col)) { }
        public JArray(JType type, long row, long col, long depth) : this(JuliaCalls.jl_alloc_array_3d(JuliaCalls.jl_apply_array_type(type, 3), (nint) row, (nint) col, (nint) depth)) { }

        public unsafe JArray(Array a, bool own = false) {
            var elType = JType.GetJuliaTypeFromNetType(a.GetType().GetElementType());
            var aType = JuliaCalls.jl_apply_array_type(elType, a.Rank);
            var ptr = GCHandle.Alloc(a, GCHandleType.Pinned).AddrOfPinnedObject();

            if (a.Rank == 1)
                _ptr = JuliaCalls.jl_ptr_to_array_1d(aType, ptr, a.Length, own ? 1 : 0);
            else {
                long* dims = stackalloc long[a.Rank];
                for (int i = 0; i < a.Rank; i++)
                    dims[i] = a.GetLength(a.Rank - (i + 1));
                var dimT = JPrimitive.makentupleF.Invoke(JPrimitive.Int64T, a.Rank, new(dims));
                _ptr = JuliaCalls.jl_ptr_to_array(aType, ptr, dimT, own ? 1 : 0);
                for (int i = 0; i < a.Rank; i++)
                    dims[i] = a.Rank - i;
                dimT = JPrimitive.makentupleF.Invoke(JPrimitive.Int64T, a.Rank, new(dims));
                _ptr = JPrimitive.PermutedDimsArrayT.Create(_ptr, dimT);
            }
        }

        public JArray(IntPtr ptr) => _ptr = ptr;
        
        public static implicit operator IntPtr(JArray value) => value._ptr;
        public static implicit operator JArray(IntPtr ptr) => new(ptr);
        public static implicit operator JArray(Any ptr) => new(ptr);
        public static implicit operator Any(JArray ptr) => new(ptr);

        public static JArray Alloc(JType elType, int n) => JuliaCalls.jl_alloc_array_1d(JuliaCalls.jl_apply_array_type(elType, 1), n);
        public static JArray Alloc(JType elType, int r, int c) => JuliaCalls.jl_alloc_array_2d(JuliaCalls.jl_apply_array_type(elType, 2), r, c);
        public static JArray Alloc(JType elType, int d, int r, int c) => JuliaCalls.jl_alloc_array_3d(JuliaCalls.jl_apply_array_type(elType, 3), d, r, c);
        public static unsafe JArray Alloc(JType elType, params int[] dimensions){
            var aType = JuliaCalls.jl_apply_array_type(elType, dimensions.Length);
            if (dimensions.Length == 1) 
                return JuliaCalls.jl_alloc_array_1d(aType, dimensions[0]);
            if (dimensions.Length == 2) return JuliaCalls.jl_alloc_array_2d(aType, dimensions[0], dimensions[1]);
            if (dimensions.Length == 3) return JuliaCalls.jl_alloc_array_3d(aType, dimensions[0], dimensions[1], dimensions[2]);
            fixed (int* p = dimensions) {
                var dimsTuple = JPrimitive.makentupleF.Invoke(JPrimitive.UInt32T, dimensions.Length, new(p));
                return JPrimitive.makentupleF.Invoke(aType, dimsTuple, dimensions.Length);
            }
        }
        
        #region NeededOverloadedOperators
        public static bool operator ==(JArray v, JArray p) => v._ptr == p._ptr;
        public static bool operator !=(JArray v, JArray p) => !(v == p);
        public override string ToString() => _ptr.ToString();
        public override int GetHashCode() => _ptr.GetHashCode();
        public override bool Equals(object o) => _ptr.Equals(o);
        #endregion

        public unsafe void Reshape(params int[] newDims) {
            var aType = JuliaCalls.jl_apply_array_type(ElementType, newDims.Length);
            fixed (int* dims = newDims) {
                var dimsTuple = JPrimitive.makentupleF.Invoke(JPrimitive.UInt32T, newDims.Length, new(dims));
                _ptr = JuliaCalls.jl_reshape_array(aType, _ptr, dimsTuple);
            }
        }
        
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

        public Any this[Span<Any> args] {
            get => _ptr[args];
            set => _ptr[args] = value;
        }
    }
}