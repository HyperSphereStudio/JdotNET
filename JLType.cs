using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//Written by Johnathan Bizzano 
namespace JuliaInterface
{

    [StructLayout(LayoutKind.Sequential)]
    public struct JLType
    {
        internal IntPtr ptr;

        public JLType(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLType value) => value.ptr;
        public static implicit operator JLType(IntPtr ptr) => new JLType(ptr);
        public static implicit operator JLType(JLVal ptr) => new JLType(ptr);
        public static implicit operator JLVal(JLType ptr) => new JLVal(ptr);

        public static bool operator ==(JLType value1, JLType value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLType value1, JLType value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLType && ((JLType)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();


        public static bool IsPointerType(object o) => o is JLVal || o is IntPtr || o is JLType || o is JLSym || o is JLModule || o is JLArray || o is JLFun;

        public static JLType JLInt64, JLInt32, JLInt16, JLInt8, JLUInt64, JLUInt32, JLUInt16, JLUInt8;
        public static JLType JLFloat64, JLFloat32;
        public static JLType JLString;


        internal static void init_types()
        {
            JLInt64 = JuliaCalls.jl_get_global(JLModule.Core, "Int64").ptr;
            JLInt32 = JuliaCalls.jl_get_global(JLModule.Core, "Int32").ptr;
            JLInt16 = JuliaCalls.jl_get_global(JLModule.Core, "Int16").ptr;
            JLInt8 = JuliaCalls.jl_get_global(JLModule.Core, "Int8").ptr;

            JLUInt64 = JuliaCalls.jl_get_global(JLModule.Core, "UInt64").ptr;
            JLUInt32 = JuliaCalls.jl_get_global(JLModule.Core, "UInt32").ptr;
            JLUInt16 = JuliaCalls.jl_get_global(JLModule.Core, "UInt16").ptr;
            JLUInt8 = JuliaCalls.jl_get_global(JLModule.Core, "UInt8").ptr;

            JLFloat64 = JuliaCalls.jl_get_global(JLModule.Core, "Float64").ptr;
            JLFloat32 = JuliaCalls.jl_get_global(JLModule.Core, "Float32").ptr;

            JLString = JuliaCalls.jl_get_global(JLModule.Core, "String").ptr;
        }
    }
}
