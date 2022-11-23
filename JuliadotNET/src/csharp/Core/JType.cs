using Base;

namespace JULIAdotNET {
    
    public struct JType {
        private readonly Any _ptr;
        public JType(Any ptr) => _ptr = ptr;
  
        public static implicit operator JType(Any ptr) => new(ptr);
        public static implicit operator Any(JType ptr) => new(ptr._ptr);
        
        public Any Create(params Any[] values) => _ptr.Invoke(values);
        public Any Create() => _ptr.Invoke();
        public Any Create(Any val) => _ptr.Invoke(val);
        public Any Create(Any val1, Any val2) => _ptr.Invoke(val1, val2);
        public Any Create(Any val1, Any val2, Any val3) => _ptr.Invoke(val1, val2, val3);

        public bool IsType(Any val) => Julia.Isa(val, this);
    }
    
}