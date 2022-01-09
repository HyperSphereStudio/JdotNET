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
    public struct JLFun
    {
        internal IntPtr ptr;

        public JLFun(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLFun value) => value.ptr;
        public static implicit operator JLFun(IntPtr ptr) => new JLFun(ptr);

        public static bool operator ==(JLFun value1, JLFun value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLFun value1, JLFun value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => o is JLFun && ((JLFun)o).ptr == ptr;
        public override int GetHashCode() => ptr.GetHashCode();

        public JLVal Invoke() => JuliaCalls.jl_call0(this);
        public JLVal Invoke(IntPtr arg1) => JuliaCalls.jl_call1(this, arg1);
        public JLVal Invoke(IntPtr arg1, IntPtr arg2) => JuliaCalls.jl_call2(this, arg1, arg2);
        public JLVal Invoke(IntPtr arg1, IntPtr arg2, IntPtr arg3) => JuliaCalls.jl_call3(this, arg1, arg2, arg3);
        
        public unsafe JLVal Invoke(params IntPtr[] args)
        {
            fixed (void** arr = Julia.ConvertPointerArray(args))
                return JuliaCalls.jl_call(this, new IntPtr(arr), args.Length);
            
        }

        public static JLFun String, TypeOf, Print, Println, GetIndex, SetIndex, Push, Deleteat, Copy, Pointer, Length;
        
        internal static void init_funs(){
            String = Julia.GetFunction(JLModule.Base, "string");
            TypeOf = Julia.GetFunction(JLModule.Base, "typeof");
            Print = Julia.GetFunction(JLModule.Base, "print");
            Println = Julia.GetFunction(JLModule.Base, "println");
            GetIndex = Julia.GetFunction(JLModule.Base, "getindex");
            SetIndex = Julia.GetFunction(JLModule.Base, "setindex!");
            Push = Julia.GetFunction(JLModule.Base, "push!");
            Deleteat = Julia.GetFunction(JLModule.Base, "deleteat!");
            Copy = Julia.GetFunction(JLModule.Base, "copy");
            Pointer = Julia.GetFunction(JLModule.Base, "pointer");
            Length = Julia.GetFunction(JLModule.Base, "length");
        }
    }
}
