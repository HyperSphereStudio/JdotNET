using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static JuliaInterface.JLModule;

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
        public bool IsType(JLVal val) => Julia.Isa(val, this);

        public static bool IsPointerType(object o) => o is JLVal || o is IntPtr || o is JLType || o is JLSym || o is JLModule || o is JLArray || o is JLFun;

        public static JLType JLInt64, JLInt32, JLInt16, JLInt8, JLUInt64, JLUInt32, JLUInt16, JLUInt8;
        public static JLType JLFloat64, JLFloat32, JLChar;
        public static JLType JLString, JLBool, JLPtr, JLAny, JLRef, JLNothing, JLArray;
        public static JLType SharpObject, SharpMethod, SharpConstructor, SharpField, SharpType, SharpStub;
        public static JLType JLUnitRange, JLStepRange;
        public static JLType SharpJLException;

        private static JLType GetType(JLModule mod, string name) => JuliaCalls.jl_get_global(mod, name).ptr;



        internal static void init_types()
        {
            JLAny = GetType(Core, "Any");
            JLInt64 = GetType(Core, "Int64");
            JLInt32 = GetType(Core, "Int32");
            JLInt16 = GetType(Core, "Int16");
            JLInt8 = GetType(Core, "Int8");

            JLUInt64 = GetType(Core, "UInt64");
            JLUInt32 = GetType(Core, "UInt32");
            JLUInt16 = GetType(Core, "UInt16");
            JLUInt8 = GetType(Core, "UInt8");

            JLFloat64 = GetType(Core, "Float64");
            JLFloat32 = GetType(Core, "Float32");
           
            JLChar = GetType(Core, "Char");
            JLString = GetType(Core, "String");
            JLBool = GetType(Core, "Bool");
            JLPtr = GetType(Core, "Ptr");
            JLRef = GetType(Core, "Ref");
            JLNothing = GetType(Core, "Nothing");
            JLArray = GetType(Core, "Array");

            

            JLUnitRange = GetType(Core, "UnitRange");
            JLStepRange = GetType(Core, "StepRange");
        }

        internal static void finish_init_types(){
            SharpMethod = GetType(JLModule.JuliaInterface, "SharpMethod");
            SharpObject = GetType(JLModule.JuliaInterface, "SharpObject");
            SharpConstructor = GetType(JLModule.JuliaInterface, "SharpConstructor");
            SharpField = GetType(JLModule.JuliaInterface, "SharpField");
            SharpType = GetType(JLModule.JuliaInterface, "SharpType");
            SharpStub = GetType(JLModule.JuliaInterface, "SharpStub");
            SharpJLException = GetType(JLModule.JuliaInterface, "SharpException");
        }
    }
}
