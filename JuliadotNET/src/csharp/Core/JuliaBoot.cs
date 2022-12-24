using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Base;

namespace JULIAdotNET
{
    public class JuliaOptions
    {
        public string JuliaDirectory;
        public List<string> Arguments = new();
        
        public int ThreadCount = 1;
        public int WorkerCount = 1;
        public int Optimize = 2;
        public string LoadSystemImage;
        public string EvaluationString;
        public bool UseSystemImageNativeCode = true;
        public bool HandleSignals = true;
        public bool PrecompileModules = true;

        public void Add(params object[] args) {
            foreach (var arg in args)
                Arguments.Add(arg.ToString());
        }

        private string AsJLString(bool b) => b ? "yes" : "no";

        internal void BuildArguments()
        {
            Add("");

            if (ThreadCount != 1)
                Add("-t", ThreadCount);

            if (WorkerCount != 1)
                Add("-p", WorkerCount);

            if (Optimize != 2)
                Add("-O", Optimize);

            if (EvaluationString != null)
                Add("-e", EvaluationString);

            if (LoadSystemImage != null)
                Add("-J", LoadSystemImage);

            if (!UseSystemImageNativeCode)
                Add("--sysimage-native-code=", AsJLString(UseSystemImageNativeCode));

            if (!PrecompileModules)
                Add("--compiled-modules=", AsJLString(PrecompileModules));

            if(!HandleSignals)
                Add("--handle-signals =", AsJLString(PrecompileModules));

            if (JuliaDirectory != null)
                Julia.JuliaDir = JuliaDirectory;
            else JuliaDirectory = Julia.JuliaDir;

            if (JuliaDirectory == null)
                throw new Exception("Julia Path Not Found");
        }
    }
    
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
            JPrimitive.primitive_init();
            JPrimitive.init_primitive_types();
            Julia.CheckExceptions();
            Julia.CheckExceptions();
        }
    }
}