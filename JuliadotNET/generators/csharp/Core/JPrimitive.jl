#Used for Parser Validation
SharpModule = quote
    export makearray, maketuple, writeSharpArray, maketuple, ieval

    function method_argnames(m::Method)
    	 argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
    	 isempty(argnames) && return argnames
    	 return [string(sym) for sym in argnames[1:m.nargs][2:end]]
    end
    
    ieval(mod::Module, ex) = Core.eval(mod, Meta.parse(ex))
    makearray(T::Type, dims::Ptr{Cvoid}, len::Int32) = (ptr = convert(Ptr{Int32}, dims); Array{T}(undef, [unsafe_load(ptr, i) for i in 1:len]...))
    maketuple(vals...) = tuple(vals...)
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
    makearray, writeSharpArray, maketuple, ieval,                              	            
    getindex, setindex!, length, iterate,
    Module, Type, Function, Method, Union, 
    ==, !=, >, <, >=, <=, !, ~, ^, &, |, %, *, +, -, /, >>, <<,
    typeof, hash,
    ismutable, isabstracttype, isimmutable, isprimitivetype, sizeof, parentmodule, nameof,
    fieldcount, fieldname, fieldoffset, fieldtype]

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
     
     open("$gen_root/JPrimitive.gen.cs", "w") do io    
         write(io,      """using System;
                           using System.Collections.Generic;
                           using Base;
                           
                           namespace JULIAdotNET{
                               public static partial class JPrimitive {
                                   public static Any $(join(["$(m[1])M" for m in modules], ", "));
                                   public static JType $(join(["$(t[1])T" for t in types], ", "));
                                   public static Any $(join(["$(fix_name(f[1]))F" for f in functions], ", "));
                                   
                                   internal static unsafe void primitive_init() {
                                   
                                       Julia.Eval(@"module SharpModule $(write_expression(SharpModule)) end; using .SharpModule");
                                       
                                       var writeSharpArray = Julia.Eval(\"writeSharpArray\");
                                       fixed (Any* values = new Any[$count]) {
                                           var syms = Julia.Eval(\"[$(join(symbols, ","))]\");
                                           writeSharpArray.Invoke(new Any(values), syms);
                                           
                                           $(join(["$(m[1])M = values[$(m[2])];" for m in modules], "\n\t\t\t\t"))
                                           $(join(["$(t[1])T = values[$(t[2])];" for t in types], "\n\t\t\t\t"))
                                           $(join(["$(fix_name(f[1]))F = values[$(f[2])];" for f in functions], "\n\t\t\t\t"))
                                       }
                                   }
                               }
                           }""")
                         
     end
end