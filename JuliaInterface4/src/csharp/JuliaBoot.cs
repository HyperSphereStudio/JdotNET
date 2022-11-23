using System;
using System.Runtime.InteropServices;
using System.Text;

namespace JULIAdotNET
{
    internal class JuliaBoot {
        internal static void jl_init_code(JuliaOptions options, bool sharpInit) {
            if (sharpInit) {
                var arguments = options.Arguments.ToArray();
                if (arguments.Length != 0) {
                    int len = arguments.Length;
                    unsafe {
                        var stringBytes = stackalloc byte*[arguments.Length];
                        Span<GCHandle> handles = stackalloc GCHandle[arguments.Length];

                        for (int i = 0; i < arguments.Length; ++i) {
                            handles[i] = GCHandle.Alloc(Encoding.ASCII.GetBytes(arguments[i]), GCHandleType.Pinned);
                            stringBytes[i] = (byte*) handles[i].AddrOfPinnedObject();
                        }
                        
                        JuliaCalls.jl_parse_opts(ref len, &stringBytes);

                        foreach(var handle in handles)
                            handle.Free();
                    }
                }
                JuliaCalls.jl_init();
            }
            JuliaPrimitive.init();
            Julia.CheckExceptions();
        }
    }
}