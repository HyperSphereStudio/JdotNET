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

        public JLVal Create(params JLVal[] values) => ((JLFun) ptr).Invoke(values);
        public JLVal Create() => ((JLFun)ptr).Invoke();
        public JLVal Create(JLVal val) => ((JLFun) ptr).Invoke(val);
        public JLVal Create(JLVal val1, JLVal val2) => ((JLFun)ptr).Invoke(val1, val2);
        public JLVal Create(JLVal val1, JLVal val2, JLVal val3) => ((JLFun)ptr).Invoke(val1, val2, val3);

        public static bool operator ==(JLType value1, IntPtr value2) => new JLVal(value1.ptr) == new JLVal(value2);
        public static bool operator !=(JLType value1, IntPtr value2) => new JLVal(value1.ptr) != new JLVal(value2);
        public override string ToString() => new JLVal(ptr).ToString();

        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();

        public static bool IsPointerType(object o) => o is JLVal || o is IntPtr || o is JLType || o is JLSym || o is JLModule || o is JLArray || o is JLFun;

        public static JLType JLInt64, JLInt32, JLInt16, JLInt8, JLUInt64, JLUInt32, JLUInt16, JLUInt8;
        public static JLType JLFloat64, JLFloat32;
        public static JLType JLString, JLBool, JLPtr, JLAny, JLRef;
        public static JLType SharpObject, SharpMethod, SharpConstructor, SharpField, SharpType, SharpStub;

        internal static void init_types()
        {
            JLAny = JuliaCalls.jl_get_global(JLModule.Core, "Any").ptr;
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
            JLBool = JuliaCalls.jl_get_global(JLModule.Core, "Bool").ptr;
            JLPtr = JuliaCalls.jl_get_global(JLModule.Core, "Ptr").ptr;
            JLRef = JuliaCalls.jl_get_global(JLModule.Core, "Ref").ptr;

            SharpMethod = JuliaCalls.jl_get_global(JLModule.Core, "SharpMethod").ptr;
            SharpObject = JuliaCalls.jl_get_global(JLModule.JuliaInterface, "SharpObject").ptr;
            SharpConstructor = JuliaCalls.jl_get_global(JLModule.JuliaInterface, "SharpConstructor").ptr;
            SharpField = JuliaCalls.jl_get_global(JLModule.JuliaInterface, "SharpField").ptr;
            SharpType = JuliaCalls.jl_get_global(JLModule.JuliaInterface, "SharpType").ptr;
            SharpStub = JuliaCalls.jl_get_global(JLModule.JuliaInterface, "SharpStub").ptr;

        }
    }
}
