using Base;
using System;

namespace JULIAdotNET {
    
    public enum JTypeType : byte{
        Struct,
        MutableStruct,
        Abstract,
        Primitive,
        Union
    }
    
    public struct JType {
        private readonly Any _ptr;
        public string Name => ToString();
        public JModule Module => _ptr.Module;
        public bool IsMutable => (bool) JPrimitive.ismutableF.Invoke(this);
        public bool IsImmutable => (bool) JPrimitive.isimmutableF.Invoke(this);
        public bool IsAbstract => (bool) JPrimitive.isabstracttypeF.Invoke(this);
        public bool IsPrimitive => (bool) JPrimitive.isprimitivetypeF.Invoke(this);
        public bool IsUnion => ((JType) JPrimitive.typeofF.Invoke(this) == JPrimitive.UnionT);
        public int SizeOf => (int) JPrimitive.sizeofF.Invoke(this);
        public int FieldCount => (int)JPrimitive.fieldcountF.Invoke(this);

        public JTypeType Type {
            get {
                if (IsPrimitive)
                    return JTypeType.Primitive;
                if (IsAbstract)
                    return JTypeType.Abstract;
                if (IsMutable)
                    return JTypeType.MutableStruct;
                if (IsImmutable)
                    return JTypeType.Struct;
                throw new Exception("Unable to determine typetype of " + this);
            }
        }
        
        public JType(Any ptr) => _ptr = ptr;
  
        public static implicit operator JType(Any ptr) => new(ptr);
        public static implicit operator Any(JType ptr) => new(ptr._ptr);
        
        public Any Create(params Any[] values) => _ptr.Invoke(values);
        public Any Create() => _ptr.Invoke();
        public Any Create(Any val) => _ptr.Invoke(val);
        public Any Create(Any val1, Any val2) => _ptr.Invoke(val1, val2);
        public Any Create(Any val1, Any val2, Any val3) => _ptr.Invoke(val1, val2, val3);

        public bool IsType(Any val) => Julia.Isa(val, this);

        public static bool operator ==(JType v, JType p) => JuliaCalls.jl_types_equal(v, p) != 0;
        public static bool operator !=(JType v, JType p) => !(v == p);

        #region NeededOverloadedOperators
        public override string ToString() => _ptr.ToString();
        public override int GetHashCode() => _ptr.GetHashCode();
        public override bool Equals(object o) => _ptr.Equals(o);
        #endregion

        public string FieldName(int i) => (string) JPrimitive.fieldnameF.Invoke(this, i);
        public int FieldOffset(int i) => (int) JPrimitive.fieldoffsetF.Invoke(this, i);
        public JType FieldType(int i) => JPrimitive.fieldtypeF.Invoke(this, i);


        public static JType GetJuliaTypeFromNetType(Type t) => JPrimitive.FindJuliaPrimitiveEquivilent(t);
    }
    
    
}