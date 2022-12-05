using System;
using System.Collections.Generic;
using Base;

namespace JULIAdotNET{
    public static partial class JPrimitive {
        public static Any BaseM, CoreM, MainM;
        public static JType ModuleT, TypeT, FunctionT, MethodT, UnionT;
        public static Any sprintF, showerrorF, catch_backtraceF, stringF, getpropertyF, setpropertyNotF, namesF, makearrayF, writeSharpArrayF, maketupleF, ievalF, getindexF, setindexNotF, lengthF, iterateF, EqualityF, InequalityF, GreaterThanF, LessThanF, GreaterThanOrEqualF, LessThanOrEqualF, NotF, OnesComplementF, ExclusiveOrF, BitwiseAndF, BitwiseOrF, ModulusF, MultiplyF, AdditionF, SubtractionF, DivisionF, RightShiftF, LeftShiftF, typeofF, hashF, ismutableF, isabstracttypeF, isimmutableF, isprimitivetypeF, sizeofF, parentmoduleF, nameofF, fieldcountF, fieldnameF, fieldoffsetF, fieldtypeF;
        
        internal static unsafe void primitive_init() {
        
            Julia.Eval(@"module SharpModule begin
    export makearray, maketuple, writeSharpArray, maketuple, ieval
    function method_argnames(m::Method)
        argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
        isempty(argnames) && return argnames
        return [string(sym) for sym = (argnames[1:m.nargs])[2:end]]
    end
    ieval(mod::Module, ex) = begin
            Core.eval(mod, Meta.parse(ex))
        end
    makearray(T::Type, dims::Ptr{Cvoid}, len::Int32) = begin
            ptr = convert(Ptr{Int32}, dims)
            Array{T}(undef, [unsafe_load(ptr, i) for i = 1:len]...)
        end
    maketuple(vals...) = begin
            tuple(vals...)
        end
    linedEvaluation(s::String, file::String, m::Module) = begin
            Core.eval(m, Meta.parseall(s, filename = file))
        end
    function writeSharpArray(p, arr)
        ptr = convert(Ptr{Any}, p)
        for i = eachindex(arr)
            unsafe_store!(ptr, arr[i], i)
        end
    end
end end; using .SharpModule");

            var writeSharpArray = Julia.Eval("writeSharpArray");
            fixed (Any* values = new Any[54]) {
                var syms = Julia.Eval("[Base,Core,Main,sprint,showerror,catch_backtrace,string,getproperty,setproperty!,names,makearray,writeSharpArray,maketuple,ieval,getindex,setindex!,length,iterate,Module,Type,Function,Method,Union,==,!=,>,<,>=,<=,!,~,^,&,|,rem,*,+,-,/,>>,<<,typeof,hash,ismutable,isabstracttype,isimmutable,isprimitivetype,sizeof,parentmodule,nameof,fieldcount,fieldname,fieldoffset,fieldtype]");
                writeSharpArray.Invoke(new Any(values), syms);

                BaseM = values[0];
				CoreM = values[1];
				MainM = values[2];
                ModuleT = values[18];
				TypeT = values[19];
				FunctionT = values[20];
				MethodT = values[21];
				UnionT = values[22];
                sprintF = values[3];
				showerrorF = values[4];
				catch_backtraceF = values[5];
				stringF = values[6];
				getpropertyF = values[7];
				setpropertyNotF = values[8];
				namesF = values[9];
				makearrayF = values[10];
				writeSharpArrayF = values[11];
				maketupleF = values[12];
				ievalF = values[13];
				getindexF = values[14];
				setindexNotF = values[15];
				lengthF = values[16];
				iterateF = values[17];
				EqualityF = values[23];
				InequalityF = values[24];
				GreaterThanF = values[25];
				LessThanF = values[26];
				GreaterThanOrEqualF = values[27];
				LessThanOrEqualF = values[28];
				NotF = values[29];
				OnesComplementF = values[30];
				ExclusiveOrF = values[31];
				BitwiseAndF = values[32];
				BitwiseOrF = values[33];
				ModulusF = values[34];
				MultiplyF = values[35];
				AdditionF = values[36];
				SubtractionF = values[37];
				DivisionF = values[38];
				RightShiftF = values[39];
				LeftShiftF = values[40];
				typeofF = values[41];
				hashF = values[42];
				ismutableF = values[43];
				isabstracttypeF = values[44];
				isimmutableF = values[45];
				isprimitivetypeF = values[46];
				sizeofF = values[47];
				parentmoduleF = values[48];
				nameofF = values[49];
				fieldcountF = values[50];
				fieldnameF = values[51];
				fieldoffsetF = values[52];
				fieldtypeF = values[53];
            }
        }
    }
}