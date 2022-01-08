using System;
using System.Runtime.InteropServices;

namespace JuliaInterface
{
    public class JuliaCalls
    {

        [DllImport("kernel32.dll")]
        public static extern bool SetDllDirectory(string path);

        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_init();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_eval_string(string c);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_atexit_hook(int hook);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double jl_unbox_float64(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float jl_unbox_float32(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long jl_unbox_int64(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_unbox_int32(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short jl_unbox_int16(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte jl_unbox_int8(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool jl_unbox_bool(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jl_unbox_uint64(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint jl_unbox_uint32(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort jl_unbox_uint16(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte jl_unbox_uint8(JLVal t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_unbox_voidpointer(JLVal v);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_float64(double t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_float32(float t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_int64(long t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_int32(int t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_int16(short t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_int8(sbyte t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_bool(bool t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_uint64(ulong t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_uint32(uint t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_uint16(ushort t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_uint8(byte t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_box_voidpointer(IntPtr x);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_call0(JLFun f);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_call1(JLFun f, JLVal arg1);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_call2(JLFun f, JLVal arg1, JLVal arg2);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_call3(JLFun f, JLVal arg1, JLVal arg2, JLVal arg3);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_get_global(JLModule m, JLSym var);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLSym jl_symbol(string sym);

        public static JLFun jl_get_function(JLModule mod, JLSym sym) => new JLFun(jl_get_global(mod, sym));
    }
}
