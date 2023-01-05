#Used for Parser Validation
SharpModule = quote
    export makearray, maketuple, writeSharpArray, maketuple, ieval, union_types, makentuple

    function method_argnames(m::Method)
    	 argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
    	 isempty(argnames) && return argnames
    	 return [string(sym) for sym in argnames[1:m.nargs][2:end]]
    end
    
    ieval(mod::Module, ex) = Core.eval(mod, Meta.parse(ex))
    maketuple(vals...) = tuple(vals...)
    makentuple(::Type{T}, n, p::Ptr{Cvoid}) where T = (p2 = convert(Ptr{T}, p); return ntuple(i -> unsafe_load(p2, i), n))
    linedEvaluation(s::String, file::String, m::Module) = Core.eval(m, Meta.parseall(s, filename=file))
    
    function writeSharpArray(p, arr)
        ptr = convert(Ptr{Any}, p)
        for i in eachindex(arr)
            unsafe_store!(ptr, arr[i], i)
        end	 
    end
end

eval(SharpModule)

symbols = [Base, Core, Main,
    sprint, showerror, catch_backtrace, string,                                	            
    getproperty, setproperty!, names,
    makentuple, writeSharpArray, maketuple, ieval,                              	            
    getindex, setindex!, length, iterate,
    Module, Type, Function, Method, Union, Integer, AbstractFloat, String, Ptr,
    ==, !=, >, <, >=, <=, !, ~, ^, &, |, %, *, +, -, /, >>, <<,
    typeof, hash,
    ismutable, isabstracttype, isimmutable, isprimitivetype, sizeof, parentmodule, nameof,
    fieldcount, fieldname, fieldoffset, fieldtype, PermutedDimsArray]
    
primitiveTypeConversions = [
    Bool => "bool",
    Char => "char",
    
    Float64 => "double",
    Float32 => "float",
    Float16 => "Half",
    
    Int64 => "long",
    Int32 => "int",
    Int16 => "short",
    Int8 => "sbyte",
    UInt64 => "ulong",
    UInt32 => "uint",
    UInt16 => "ushort",
    UInt8 => "byte",
    Array => "Array"
]

function generate_primitives(project_root, src_root, gen_root)

    function write_expression(x)
        buffer = IOBuffer()
        Base.show_unquoted(buffer, Base.remove_linenums!(x))
        return String(take!(buffer))
    end
    
    function fix_name(f)
        f = replace(string(f), 
            "+" => "Addition",
            "-" => "Subtraction",
            "*" => "Multiply",
            "/" => "Division",
            "%" => "Modulus",
            "rem" => "Modulus",
            "^" => "ExclusiveOr",
            "&" => "BitwiseAnd",
            "|" => "BitwiseOr",
            "<<" => "LeftShift",
            ">>" => "RightShift",
            "==" => "Equality",
            ">=" => "GreaterThanOrEqual",
            "<=" => "LessThanOrEqual",
            ">" => "GreaterThan",
            "<" => "LessThan",
            "!=" => "Inequality",
            "--" => "Decrement",
            "++" => "Increment",
            "~" => "OnesComplement",
            "!" => "Not")
        f = replace(f, "!" => "")    
        return f
    end
    
     count = 0
     modules = []
     functions = []
     types = []
     
     for s in symbols
        if s isa Module
           push!(modules, (s, count))
        elseif s isa Function
            push!(functions, (s, count))
        elseif s isa Type
            push!(types, (s, count)) 
        end   
        count += 1
     end
     
     count += 1
     for v in primitiveTypeConversions
         push!(symbols, v[1])
     end
     
     write_lines(kernel, array) = join(["$(kernel(m));" for m in array], "\n\t\t\t\t")
     
     open("$gen_root/JPrimitive.gen.cs", "w") do io    
         write(io,      """using System;
                           using System.Collections.Generic;
                           using Base;
                           
                           namespace JULIAdotNET{
                               public static partial class JPrimitive {
                                   public static JModule $(join(["$(m[1])M" for m in modules], ", "));
                                   public static JType $(join(["$(t[1])T" for t in types], ", "));
                                   public static JType $(join(["$(t)T" for t in symbols[count:end]], ", "));
                                   public static Any $(join(["$(fix_name(f[1]))F" for f in functions], ", "));
                                   
                                   internal static unsafe void primitive_init() {
                                   
                                       Julia.Eval(@"module SharpModule $(write_expression(SharpModule)) end; using .SharpModule");
                                       
                                       var writeSharpArray = Julia.Eval(\"writeSharpArray\");
                                       fixed (Any* values = new Any[$(length(symbols))]) {
                                           var syms = Julia.Eval(\"[$(join(symbols, ","))]\");
                                           writeSharpArray.Invoke(new Any(values), syms);
                                           $(write_lines(m -> "$(m[1])M = values[$(m[2])]", modules))
                                           $(write_lines(t -> "$(t[1])T = values[$(t[2])]", types))
                                           $(write_lines(f -> "$(fix_name(f[1]))F = values[$(f[2])]", functions))
                                           $(write_lines(i -> "$(symbols[i])T = values[$(i-1)]", count:length(symbols)))
                                            
                                           $(write_lines(t -> "RegisterPrimitive(typeof($(t[2])), $(t[1])T)", primitiveTypeConversions))
                                       }
                                   }
                               }
                           }""")
                         
     end
end