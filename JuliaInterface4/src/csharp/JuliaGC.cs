using System;
using System.Runtime.InteropServices;

namespace JULIAdotNET
{
    public class JuliaGC {
        private static UnsafeStream jl_gc_frames = new();
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

        public static unsafe void JL_GC_PUSHARGS(Span<JuliaV> v) {
            jl_gc_frames.EnsureSizeWrite(jl_gc_frames.Count + v.Length * sizeof(nint) + sizeof(_jl_gcframe_t));
            jl_gc_frames.Write(new _jl_gcframe_t(*CurrentFrame, v.Length));
            var ptr = (IntPtr*) jl_gc_frames.Begin;
            for (int i = 0; i < v.Length; i++)
                *ptr++ = v[i];
        }

        public static unsafe void JL_GC_POP() => *CurrentFrame = (*CurrentFrame)->prev;
    }
}