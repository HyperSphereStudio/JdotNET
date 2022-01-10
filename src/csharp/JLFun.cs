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
        public static implicit operator JLFun(JLVal ptr) => new JLFun(ptr);
        public static implicit operator JLVal(JLFun ptr) => new JLVal(ptr);

        public static bool operator ==(JLFun value1, JLFun value2) => value1.ptr == value2.ptr;
        public static bool operator !=(JLFun value1, JLFun value2) => value1.ptr != value2.ptr;

        public override bool Equals(object o) => new JLVal(this).Equals(o);
        public override int GetHashCode() => new JLVal(this).GetHashCode();
        public void Println() => new JLVal(this).Println();
        public void Print() => new JLVal(this).Print();

        public JLVal Invoke() {
            var val = UnsafeInvoke();
            Julia.CheckExceptions();
            return val;
        }

        public JLVal Invoke(JLVal arg1){
            var val = UnsafeInvoke(arg1);
            Julia.CheckExceptions();
            return val;
        }

        public JLVal Invoke(JLVal arg1, JLVal arg2){
            var val = UnsafeInvoke(arg1, arg2);
            Julia.CheckExceptions();
            return val;
        }
        public JLVal Invoke(JLVal arg1, JLVal arg2, JLVal arg3)
        {
            var val = UnsafeInvoke(arg1, arg2, arg3);
            Julia.CheckExceptions();
            return val;
        }

        public JLVal Invoke(params JLVal[] args)
        {
            var val = UnsafeInvoke(args);
            Julia.CheckExceptions();
            return val;
        }

        public JLVal UnsafeInvoke() => JuliaCalls.jl_call0(this);
        public JLVal UnsafeInvoke(JLVal arg1) => JuliaCalls.jl_call1(this, arg1);
        public JLVal UnsafeInvoke(JLVal arg1, JLVal arg2) => JuliaCalls.jl_call2(this, arg1, arg2);
        public JLVal UnsafeInvoke(JLVal arg1, JLVal arg2, JLVal arg3) => JuliaCalls.jl_call3(this, arg1, arg2, arg3);
        
        public unsafe JLVal UnsafeInvoke(params JLVal[] args) => JuliaCalls.jl_call(this, args.Select(x => x.ptr).ToArray(), args.Length);

        public static JLFun StringF, TypeOfF, PrintF, PrintlnF, 
                GetIndexF, SetIndex_F, Push_F, Deleteat_F, 
                CopyF, PointerF, LengthF, HashCodeF, SprintF, ShowErrorF,
                GetFieldF, SetField_F;

        

        internal static void init_funs(){
            StringF = Julia.GetFunction(JLModule.Base, "string");
            TypeOfF = Julia.GetFunction(JLModule.Base, "typeof");
            PrintF = Julia.GetFunction(JLModule.Base, "print");
            PrintlnF = Julia.GetFunction(JLModule.Base, "println");
            GetIndexF = Julia.GetFunction(JLModule.Base, "getindex");
            SetIndex_F = Julia.GetFunction(JLModule.Base, "setindex!");
            Push_F = Julia.GetFunction(JLModule.Base, "push!");
            Deleteat_F = Julia.GetFunction(JLModule.Base, "deleteat!");
            CopyF = Julia.GetFunction(JLModule.Base, "copy");
            PointerF = Julia.GetFunction(JLModule.Base, "pointer");
            LengthF = Julia.GetFunction(JLModule.Base, "length");
            HashCodeF = Julia.GetFunction(JLModule.Base, "hash");
            SprintF = Julia.GetFunction(JLModule.Base, "sprint");
            ShowErrorF = Julia.GetFunction(JLModule.Base, "showerror");
            GetFieldF = Julia.GetFunction(JLModule.Base, "getfield");
            SetField_F = Julia.GetFunction(JLModule.Base, "setfield!");
        }
    }
}
