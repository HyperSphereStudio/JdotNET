using System;

namespace JULIAdotNET
{
    public class JuliaFunction{
        private readonly JLVal _ptr;

        
        private JuliaFunction(IntPtr ptr) => _ptr = ptr;

        
        public static bool operator ==(JuliaFunction value1, JuliaFunction value2) => value1._ptr == value2._ptr;
        public static bool operator !=(JuliaFunction value1, JuliaFunction value2) => !(value1 == value2);
        
        
    }
}