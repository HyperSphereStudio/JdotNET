using System;
using System.Runtime.InteropServices;
using Base;

namespace JULIAdotNET;

public class JuliaGC {
    private static readonly UnsafeStream jl_gc_frames = new();
    public static unsafe _jl_gcframe_t** CurrentFrame = (_jl_gcframe_t**) JuliaCalls.jl_get_pgcstack();

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct _jl_gcframe_t {
        public nint nroots;
        public _jl_gcframe_t* prev;

        public _jl_gcframe_t(_jl_gcframe_t* lastFrame, int len) {
            nroots = len << 2;
            prev = lastFrame;
        }
    }

    public static unsafe void JL_GC_PUSHARGS(Span<Any> v) {
        jl_gc_frames.EnsureSizeWrite(jl_gc_frames.Count + v.Length * sizeof(nint) + sizeof(_jl_gcframe_t));
        var f = new _jl_gcframe_t(*CurrentFrame, v.Length);
        *CurrentFrame = (_jl_gcframe_t*) jl_gc_frames.Begin;
        jl_gc_frames.Write(f);
        var ptr = (IntPtr*) jl_gc_frames.Begin;
        foreach (var t in v)
            *ptr++ = t;
    }

    public static unsafe void JL_GC_POP() => *CurrentFrame = (*CurrentFrame)->prev;
}