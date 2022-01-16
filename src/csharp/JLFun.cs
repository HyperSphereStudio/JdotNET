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

        public JLVal InvokeSplat(params JLVal[] args)
        {
            var val = UnsafeInvokeSplat(args);
            Julia.CheckExceptions();
            return val;
        }

        public JLVal UnsafeInvoke() => JuliaCalls.jl_call0(this);
        public JLVal UnsafeInvoke(JLVal arg1) => JuliaCalls.jl_call1(this, arg1);
        public JLVal UnsafeInvoke(JLVal arg1, JLVal arg2) => JuliaCalls.jl_call2(this, arg1, arg2);
        public JLVal UnsafeInvoke(JLVal arg1, JLVal arg2, JLVal arg3) => JuliaCalls.jl_call3(this, arg1, arg2, arg3);
        public unsafe JLVal UnsafeInvoke(params JLVal[] args) => JuliaCalls.jl_call(this, args, args.Length);

        public unsafe JLVal UnsafeInvokeSplat(params JLVal[] args){
            var newArr = new JLVal[args.Length + args[args.Length - 1].Length - 1];
            Array.Copy(args, 0, newArr, 0, args.Length - 1);
            var splatObj = args[args.Length - 1];

            for (int d = args.Length - 1, s = 1, len = splatObj.Length; s <= len; ++s, ++d){
                newArr[d] = splatObj[s];
            }

            return JuliaCalls.jl_call(this, newArr, newArr.Length);
        }
        
        public static JLFun StringF, TypeOfF, PrintF, PrintlnF, 
                GetIndexF, SetIndex_F, Push_F, Deleteat_F, 
                CopyF, PointerF, LengthF, HashCodeF, SprintF, ShowErrorF,
                GetFieldF, SetField_F, NamesF, ElTypeF, SizeF, FirstF, LastF, 
                Delete_F, BroadCastF, IterateF, EachIndexF, SizeOfF;

        private static JLFun GetBFun(string name) => JLModule.Base.GetFunction(name);

        internal static void init_funs(){
            StringF = GetBFun("string");
            TypeOfF = GetBFun("typeof");
            PrintF = GetBFun("print");
            PrintlnF = GetBFun("println");
            GetIndexF = GetBFun("getindex");
            SetIndex_F = GetBFun("setindex!");
            Push_F = GetBFun("push!");
            Deleteat_F = GetBFun("deleteat!");
            Delete_F = GetBFun("delete!");
            CopyF = GetBFun("copy");
            PointerF = GetBFun("pointer");
            LengthF = GetBFun("length");
            HashCodeF = GetBFun("hash");
            SprintF = GetBFun("sprint");
            ShowErrorF = GetBFun("showerror");
            GetFieldF = GetBFun("getfield");
            SetField_F = GetBFun("setfield!");
            NamesF = GetBFun("names");
            ElTypeF = GetBFun("eltype");
            SizeF = GetBFun("size");
            FirstF = GetBFun("first");
            LastF = GetBFun("last");
            BroadCastF = GetBFun("broadcast");
            IterateF = GetBFun("iterate");
            EachIndexF = GetBFun("eachindex");
            SizeOfF = GetBFun("sizeof");
        }
    }
}
