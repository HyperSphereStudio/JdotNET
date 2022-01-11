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

        public JLType ReturnType { get { return ((JLArray) JLModule.Base.GetFunction("return_types").Invoke(this))[1]; } }
        
        //Start at index 2
        public JLSvec ParameterTypes { get { return GetFieldF.Invoke(GetFieldF.Invoke(((JLArray) JLModule.Base.GetFunction("methods").Invoke(this))[1], (JLSym) "sig"), (JLSym) "parameters"); } }

        public JLFun(IntPtr ptr) => this.ptr = ptr;

        public static implicit operator IntPtr(JLFun value) => value.ptr;
        public static implicit operator JLFun(IntPtr ptr) => new JLFun(ptr);
        public static implicit operator JLFun(JLVal ptr) => new JLFun(ptr);
        public static implicit operator JLVal(JLFun ptr) => new JLVal(ptr);

        public static bool operator ==(JLFun value1, IntPtr value2) => new JLVal(value1) == new JLVal(value2);
        public static bool operator !=(JLFun value1, IntPtr value2) => new JLVal(value1) != new JLVal(value2);
        public override string ToString() => new JLVal(this).ToString();
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
                GetFieldF, SetField_F, NamesF;

        

        internal static void init_funs(){
            StringF = JLModule.Base.GetFunction("string");
            TypeOfF = JLModule.Base.GetFunction("typeof");
            PrintF = JLModule.Base.GetFunction("print");
            PrintlnF = JLModule.Base.GetFunction("println");
            GetIndexF = JLModule.Base.GetFunction("getindex");
            SetIndex_F = JLModule.Base.GetFunction("setindex!");
            Push_F = JLModule.Base.GetFunction("push!");
            Deleteat_F = JLModule.Base.GetFunction("deleteat!");
            CopyF = JLModule.Base.GetFunction("copy");
            PointerF = JLModule.Base.GetFunction("pointer");
            LengthF = JLModule.Base.GetFunction("length");
            HashCodeF = JLModule.Base.GetFunction("hash");
            SprintF = JLModule.Base.GetFunction("sprint");
            ShowErrorF = JLModule.Base.GetFunction("showerror");
            GetFieldF = JLModule.Base.GetFunction("getfield");
            SetField_F = JLModule.Base.GetFunction("setfield!");
            NamesF = JLModule.Base.GetFunction("names");

        }
    }
}
