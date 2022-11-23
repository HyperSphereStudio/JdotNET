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
            primitive_init();
            Julia.CheckExceptions();
            Julia.CheckExceptions();
        }
        
        private static unsafe void primitive_init() {
            var sharp = Julia.Eval(@"
				module Sharp
					function _method_argnames(m::Method)
						argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
						isempty(argnames) && return argnames
						return [string(sym) for sym in argnames[1:m.nargs][2:end]]
					end

					_eval(mod, ex) = Core.eval(mod, Meta.parse(ex))
					_makearray(T::Type, dims::Ptr{Cvoid}, len::Int32) = (ptr = convert(Ptr{Int32}, dims); Array{T}(undef, [unsafe_load(ptr, i) for i in 1:len]...))
					_maketuple(vals) = tuple(vals...)
					_linedEvaluation(s::String, file::String, m::Module) = Core.eval(m, Meta.parseall(s, filename=file))
	
					function getprimitivetypes(p::Ptr{Cvoid})
						ptr = convert(Ptr{Any}, p)
						array = [Base, Core, Main, Meta,
							    sprint, showerror, catch_backtrace, string, 
                                 	            
							    getproperty, setproperty!, names,
                                 	            
							    _makearray, 
                                 	            
							    getindex, setindex!, length, iterate]
                 
						for i in eachindex(array)
							unsafe_store!(ptr, array[i], i)
						end
					end
				end
			");

            fixed (Any* values = stackalloc Any[16]) {
                Julia.GetGlobal(sharp, "getprimitivetypes").Invoke(new Any(values));
                
                JPrimitive.Base = values[0];
                JPrimitive.Core = values[1];
                JPrimitive.Main = values[2];
                JPrimitive.Meta = values[3];

                JPrimitive.SprintF = values[4];
                JPrimitive.ShowErrorF = values[5];
                JPrimitive.CatchBackTraceF = values[6];
                JPrimitive.StringF = values[7];
                JPrimitive.GetPropertyF = values[8];
                JPrimitive.SetPropertyF = values[9];
                JPrimitive.NamesF = values[10];
                
                JPrimitive.MakeArrayF = values[11];
                JPrimitive.GetIndexF = values[12];
                JPrimitive.SetIndexF = values[13];
                JPrimitive.LengthF = values[14];
                JPrimitive.IterateF = values[15];
            }
        }
    }
}