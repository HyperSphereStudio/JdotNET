using System;
using System.Runtime.InteropServices;

//Written By Johnathan Bizzano
namespace JuliaInterface
{

    public class JuliaCalls
    {

        
        public enum JLIMAGESEARCH{
            JL_IMAGE_CWD = 0,
            JL_IMAGE_JULIA_HOME = 1,
            //JL_IMAGE_LIBJULIA = 2,
        }

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void julia_init(JLIMAGESEARCH rel);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_init();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_eval_string(string c);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_atexit_hook(int hook);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_string_ptr(JLVal v);


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
        public static extern JLVal jl_cstr_to_string(string s);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_call(JLFun f, JLVal[] args, Int32 arg_count);


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
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_types_equal(JLVal v1, JLVal v2);

        
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_exception_occurred();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_current_exception();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_exception_clear();
        
        

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH1(IntPtr p);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH2(IntPtr p, IntPtr p2);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH3(IntPtr p, IntPtr p2, IntPtr p3);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH4(IntPtr p, IntPtr p2, IntPtr p3, IntPtr p4);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH5(IntPtr p, IntPtr p2, IntPtr p3, IntPtr p4, IntPtr p5);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH6(IntPtr p, IntPtr p2, IntPtr p3, IntPtr p4, IntPtr p5, IntPtr p6);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_PUSH7(IntPtr p, IntPtr p2, IntPtr p3, IntPtr p4, IntPtr p5, IntPtr p6, IntPtr p7);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JL_GC_POP();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void _JL_GC_PUSHARGS(IntPtr[] p, SizeT count);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_typename_str(JLVal val);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_typeof_str(JLVal v);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLModule jl_new_module(JLSym name);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_nospecialize(JLModule  self, int on);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_optlevel(JLModule  self, int lvl);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_optlevel(JLModule  m);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_compile(JLModule  self, int value);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_compile(JLModule  m);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_infer(JLModule  self, int value);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_infer(JLModule  m);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLBinding jl_get_binding(JLModule m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLBinding jl_get_binding_or_error(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_module_globalref(JLModule  m, JLSym var);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLBinding jl_get_binding_wr(JLModule  m, JLSym var, int error);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLBinding jl_get_binding_for_method_def(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_boundp(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_defines_or_exports_p(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_binding_resolved_p(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_const(JLModule  m, JLSym var);
        
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_global(JLModule  m, JLSym var, JLVal val);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_const(JLModule  m, JLSym var, JLVal val);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_checked_assignment(JLBinding b, JLVal rhs);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_declare_constant(JLBinding b);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_using(JLModule  to, JLModule  from);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_use(JLModule  to, JLModule  from, JLSym s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_use_as(JLModule  to, JLModule  from, JLSym s, JLSym asname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_import(JLModule  to, JLModule  from, JLSym s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_import_as(JLModule  to, JLModule  from, JLSym s, JLSym asname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_export(JLModule  from, JLSym s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_imported(JLModule  m, JLSym s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_module_exports_p(JLModule  m, JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_add_standard_imports(JLModule  m);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLArray jl_eqtable_put(JLArray h, JLVal key, JLVal val, IntPtr inserted);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_eqtable_get(JLArray h, JLVal key, JLVal deflt);


        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_errno();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_errno(int e);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 jl_stat(string path, string statbuf);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_cpu_threads();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern long jl_getpagesize();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern long jl_getallocationgranularity();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_debugbuild();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLSym jl_get_UNAME();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLSym jl_get_ARCH();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_get_libllvm();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_environ(int i);


		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_error(string str);
		
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_too_few_args(string fname, int min);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_too_many_args(string fname, int max);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_type_error(string fname,
                                                    JLVal expected,
                                                    JLVal got);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_type_error_rt(string fname,
                                               string context,
                                               JLVal ty,
                                               JLVal got);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_undefined_var_error(JLSym var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_atomic_error(string str);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error(JLVal v,
                                                      JLVal t);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_v(JLVal v, JLVal[] idxs, SizeT nidxs);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_int(JLVal v, SizeT i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_tuple_int(JLVal[] v, SizeT nv, SizeT i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_unboxed_int(IntPtr v, JLVal vt, SizeT i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_ints(JLVal v, uint[] idxs, SizeT nidxs);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_eof_error();

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_libdir();
        

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_init_with_image(string julia_bindir, string image_relative_path);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_default_sysimg_path();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_initialized();
        
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_exit(int status);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_pathname_for_handle(IntPtr handle);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_deserialize_verify_header(IntPtr s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_preload_sysimg_so(string fname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_sysimg_so(IntPtr handle);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_create_system_image(IntPtr p);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_save_system_image(string fname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_restore_system_image(string fname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_restore_system_image_data(string buf, SizeT len);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_save_incremental(string fname, JLArray worklist);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_restore_incremental(string fname, JLArray depmods);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_restore_incremental_from_buf(string buf, SizeT sz, JLArray depmods);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_parse_all(string text, SizeT text_len, string filename, SizeT filename_len);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_parse_string(string text, SizeT text_len, int offset, int greedy);
    
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand(JLVal expr, JLModule  inmodule);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand_with_loc(JLVal expr, JLModule  inmodule, string file, int line);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand_with_loc_warn(JLVal expr, JLModule  inmodule, string file, int line);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand_in_world(JLVal expr, JLModule  inmodule, string file, int line, SizeT world);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand_stmt(JLVal expr, JLModule  inmodule);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern JLVal jl_expand_stmt_with_loc(JLVal expr, JLModule  inmodule, string file, int line);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_load_dynamic_library(string fname, SizeT flags, int throw_err);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_dlopen(string filename, SizeT flags);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_dlclose(IntPtr handle);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_dlsym(IntPtr handle, string symbol, IntPtr[] value, int throw_err);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_operator(string sym);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_unary_operator(string sym);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_unary_and_binary_operator(string sym);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_syntactic_operator(string sym);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_operator_precedence(string sym);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_yield();

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_new_task(JLFun f, JLVal v, SizeT p);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern void jl_switchto(IntPtr[] pt);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern int jl_set_task_tid(IntPtr[] task, int tid);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_throw(JLVal e);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_rethrow();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_sig_throw();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_rethrow_other(JLVal e);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_no_exc_handler(JLVal e);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_gc_enable(int on);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_gc_is_enabled();

        public enum JLGCCollection
        {
            JL_GC_AUTO = 0,         // use heuristics to determine the collection type
            JL_GC_FULL = 1,         // force a full collection
            JL_GC_INCREMENTAL = 2,  // force an incremental collection
        };

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_collect(JLGCCollection c);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_add_finalizer(JLVal v, JLFun f);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_add_ptr_finalizer(IntPtr ptls, JLVal v, IntPtr f);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_finalize(JLVal o);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_new_weakref(JLVal value);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_gc_alloc_0w();
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_gc_alloc_1w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_gc_alloc_2w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_gc_alloc_3w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_gc_allocobj(SizeT sz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern IntPtr jl_malloc_stack(uint* bufsz, IntPtr owner);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_free_stack(IntPtr stkbuf, SizeT bufsz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_use(JLVal a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_clear_malloc_data();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_managed_malloc(SizeT sz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_subtype(JLVal a, JLVal b);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_isa(JLVal a, JLType t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_types_equal(JLType a, JLType b);

        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_new_array(JLType atype, JLVal dims);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_reshape_array(JLType atype, JLArray data, JLVal dims);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_ptr_to_array_1d(JLType atype, IntPtr data, SizeT nel, int own_buffer);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_ptr_to_array(JLType atype, IntPtr data, JLVal dims, int own_buffer);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLType jl_apply_array_type(JLType type, SizeT dim);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_array_ptr(JLArray a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLType jl_array_eltype(JLArray a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_array_rank(JLArray a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SizeT jl_array_size(JLArray a, int d);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_alloc_array_1d(JLType atype, SizeT nr);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_alloc_array_2d(JLType atype, SizeT nr, SizeT nc);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_alloc_array_3d(JLType atype, SizeT nr, SizeT nc, SizeT z);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLVal jl_new_structv(JLType type, JLVal[] args, UInt32 na);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_apply_tuple_type_v(JLType[] types, SizeT nc);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JLArray jl_new_struct_uninit(JLType t);
    }
}
