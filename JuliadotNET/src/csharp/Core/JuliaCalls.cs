using System;
using System.Runtime.InteropServices;
using Base;

//Written By Johnathan Bizzano
namespace JULIAdotNET
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
        public static extern unsafe void jl_parse_opts(ref int argc, byte*** argvp);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_eval_string(string c);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_atexit_hook(int hook);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_string_ptr(IntPtr v);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double jl_unbox_float64(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float jl_unbox_float32(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long jl_unbox_int64(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_unbox_int32(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short jl_unbox_int16(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte jl_unbox_int8(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool jl_unbox_bool(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jl_unbox_uint64(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint jl_unbox_uint32(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort jl_unbox_uint16(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte jl_unbox_uint8(IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_unbox_voidpointer(IntPtr v);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_float64(double t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_float32(float t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_int64(long t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_int32(int t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_int16(short t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_int8(sbyte t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_bool(bool t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_uint64(ulong t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_uint32(uint t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_uint16(ushort t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_uint8(byte t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_box_voidpointer(IntPtr x);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_cstr_to_string(string s);
        

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Any jl_call(Any f, Any* args, Int32 arg_count);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_call0(Any f);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_call1(Any f, Any arg1);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_call2(Any f, Any arg1, Any arg2);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_call3(Any f, Any arg1, Any arg2, Any arg3);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_get_global(Any m, Any var);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Any jl_symbol(string sym);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_types_equal(JType v1, JType v2);

        
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_exception_occurred();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_current_exception();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_exception_clear();


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_typename_str(IntPtr val);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_typeof_str(IntPtr v);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_new_module(IntPtr name);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_nospecialize(IntPtr  self, int on);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_optlevel(IntPtr  self, int lvl);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_optlevel(IntPtr  m);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_compile(IntPtr  self, int value);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_compile(IntPtr  m);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_module_infer(IntPtr  self, int value);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_get_module_infer(IntPtr  m);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_binding(IntPtr m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_binding_or_error(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_module_globalref(IntPtr  m, IntPtr var);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_binding_wr(IntPtr  m, IntPtr var, int error);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_binding_for_method_def(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_boundp(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_defines_or_exports_p(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_binding_resolved_p(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_const(Any m, Any sym);
        
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_global(IntPtr  m, IntPtr var, IntPtr val);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_set_const(IntPtr  m, IntPtr var, IntPtr val);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_checked_assignment(IntPtr b, IntPtr rhs);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_declare_constant(IntPtr b);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_using(IntPtr  to, IntPtr  from);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_use(IntPtr  to, IntPtr  from, IntPtr s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_use_as(IntPtr  to, IntPtr  from, IntPtr s, IntPtr asname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_import(IntPtr  to, IntPtr  from, IntPtr s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_import_as(IntPtr  to, IntPtr  from, IntPtr s, IntPtr asname);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_module_export(IntPtr  from, IntPtr s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_is_imported(IntPtr  m, IntPtr s);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_module_exports_p(IntPtr  m, IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_add_standard_imports(IntPtr  m);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_eqtable_put(IntPtr h, IntPtr key, IntPtr val, IntPtr inserted);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_eqtable_get(IntPtr h, IntPtr key, IntPtr deflt);


        
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
		public static extern IntPtr jl_get_UNAME();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_ARCH();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_get_libllvm();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_environ(int i);


		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_error(string str);
		
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_too_few_args(string fname, int min);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_too_many_args(string fname, int max);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_type_error(string fname,
                                                    IntPtr expected,
                                                    IntPtr got);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_type_error_rt(string fname,
                                               string context,
                                               IntPtr ty,
                                               IntPtr got);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_undefined_var_error(IntPtr var);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_atomic_error(string str);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error(IntPtr v,
                                                      IntPtr t);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_v(IntPtr v, IntPtr[] idxs, nint nidxs);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_int(IntPtr v, nint i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_tuple_int(IntPtr[] v, nint nv, nint i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_unboxed_int(IntPtr v, IntPtr vt, nint i);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_bounds_error_ints(IntPtr v, uint[] idxs, nint nidxs);
        
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
		public static extern void jl_restore_system_image_data(string buf, nint len);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int jl_save_incremental(string fname, IntPtr worklist);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_restore_incremental(string fname, IntPtr depmods);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_restore_incremental_from_buf(string buf, nint sz, IntPtr depmods);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand(IntPtr expr, IntPtr  inmodule);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand_with_loc(IntPtr expr, IntPtr  inmodule, string file, int line);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand_with_loc_warn(IntPtr expr, IntPtr  inmodule, string file, int line);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand_in_world(IntPtr expr, IntPtr  inmodule, string file, int line, nint world);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand_stmt(IntPtr expr, IntPtr  inmodule);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_expand_stmt_with_loc(IntPtr expr, IntPtr  inmodule, string file, int line);

        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_load_dynamic_library(string fname, nint flags, int throw_err);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jl_dlopen(string filename, nint flags);
        
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
		public static extern IntPtr jl_new_task(IntPtr f, IntPtr v, nint p);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern void jl_switchto(IntPtr[] pt);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern int jl_set_task_tid(IntPtr[] task, int tid);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_throw(IntPtr e);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_rethrow();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_sig_throw();
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_rethrow_other(IntPtr e);
        
		[DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void jl_no_exc_handler(IntPtr e);

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
        public static extern void jl_gc_add_finalizer(IntPtr v, IntPtr f);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_add_ptr_finalizer(IntPtr ptls, IntPtr v, IntPtr f);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_finalize(IntPtr o);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_new_weakref(IntPtr value);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_alloc_0w();
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_alloc_1w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_alloc_2w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_alloc_3w();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_allocobj(nint sz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern IntPtr jl_malloc_stack(uint* bufsz, IntPtr owner);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_free_stack(IntPtr stkbuf, nint bufsz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_gc_use(IntPtr a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void jl_clear_malloc_data();

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_gc_managed_malloc(nint sz);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_subtype(IntPtr a, IntPtr b);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_isa(IntPtr a, IntPtr t);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_new_array(JType atype, JArray dims);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_reshape_array(JType atype, JArray data, JArray dims);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_ptr_to_array_1d(JType atype, IntPtr data, nint nel, int ownBuffer);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_ptr_to_array(JType atype, IntPtr data, Any dims, int ownBuffer);
        
        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JType jl_apply_array_type(JType type, nint dim);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_array_ptr(JArray a);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JType jl_array_eltype(IntPtr a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int jl_array_rank(IntPtr a);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern nint jl_array_size(IntPtr a, int d);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_alloc_array_1d(JType atype, nint nr);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_alloc_array_2d(JType atype, nint nr, nint nc);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern JArray jl_alloc_array_3d(JType atype, nint nr, nint nc, nint z);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr jl_new_structv(IntPtr type, Any* args, UInt32 na);

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr jl_apply_tuple_type_v(IntPtr* types, nint nc);


        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jl_new_struct_uninit(IntPtr t);
        

        [DllImport("libjulia.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern IntPtr* jl_get_pgcstack();
    }
}
