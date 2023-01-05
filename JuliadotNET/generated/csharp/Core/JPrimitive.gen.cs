using System;
using System.Collections.Generic;
using Base;

namespace JULIAdotNET{
    public static partial class JPrimitive {
        public static JModule BaseM, CoreM, MainM;
        public static JType ModuleT, TypeT, FunctionT, MethodT, UnionT, IntegerT, AbstractFloatT, StringT, PtrT, PermutedDimsArrayT;
        public static JType BoolT, CharT, Float64T, Float32T, Float16T, Int64T, Int32T, Int16T, Int8T, UInt64T, UInt32T, UInt16T, UInt8T, ArrayT;
        public static Any sprintF, showerrorF, catch_backtraceF, stringF, getpropertyF, setpropertyNotF, namesF, makentupleF, writeSharpArrayF, maketupleF, ievalF, getindexF, setindexNotF, lengthF, iterateF, EqualityF, InequalityF, GreaterThanF, LessThanF, GreaterThanOrEqualF, LessThanOrEqualF, NotF, OnesComplementF, ExclusiveOrF, BitwiseAndF, BitwiseOrF, ModulusF, MultiplyF, AdditionF, SubtractionF, DivisionF, RightShiftF, LeftShiftF, typeofF, hashF, ismutableF, isabstracttypeF, isimmutableF, isprimitivetypeF, sizeofF, parentmoduleF, nameofF, fieldcountF, fieldnameF, fieldoffsetF, fieldtypeF;
        
        internal static unsafe void primitive_init() {
        
            Julia.Eval(@"module SharpModule begin
    export makearray, maketuple, writeSharpArray, maketuple, ieval, union_types, makentuple
    function method_argnames(m::Method)
        argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
        isempty(argnames) && return argnames
        return [string(sym) for sym = (argnames[1:m.nargs])[2:end]]
    end
    ieval(mod::Module, ex) = begin
            Core.eval(mod, Meta.parse(ex))
        end
    maketuple(vals...) = begin
            tuple(vals...)
        end
    (makentuple(::Type{T}, n, p::Ptr{Cvoid}) where T) = begin
            p2 = convert(Ptr{T}, p)
            return ntuple((i->begin
                            unsafe_load(p2, i)
                        end), n)
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
            fixed (Any* values = new Any[73]) {
                var syms = Julia.Eval("[Base,Core,Main,sprint,showerror,catch_backtrace,string,getproperty,setproperty!,names,makentuple,writeSharpArray,maketuple,ieval,getindex,setindex!,length,iterate,Module,Type,Function,Method,Union,Integer,AbstractFloat,String,Ptr,==,!=,>,<,>=,<=,!,~,^,&,|,rem,*,+,-,/,>>,<<,typeof,hash,ismutable,isabstracttype,isimmutable,isprimitivetype,sizeof,parentmodule,nameof,fieldcount,fieldname,fieldoffset,fieldtype,PermutedDimsArray,Bool,Char,Float64,Float32,Float16,Int64,Int32,Int16,Int8,UInt64,UInt32,UInt16,UInt8,Array]");
                writeSharpArray.Invoke(new Any(values), syms);
                BaseM = values[0];
				CoreM = values[1];
				MainM = values[2];
                ModuleT = values[18];
				TypeT = values[19];
				FunctionT = values[20];
				MethodT = values[21];
				UnionT = values[22];
				IntegerT = values[23];
				AbstractFloatT = values[24];
				StringT = values[25];
				PtrT = values[26];
				PermutedDimsArrayT = values[58];
                sprintF = values[3];
				showerrorF = values[4];
				catch_backtraceF = values[5];
				stringF = values[6];
				getpropertyF = values[7];
				setpropertyNotF = values[8];
				namesF = values[9];
				makentupleF = values[10];
				writeSharpArrayF = values[11];
				maketupleF = values[12];
				ievalF = values[13];
				getindexF = values[14];
				setindexNotF = values[15];
				lengthF = values[16];
				iterateF = values[17];
				EqualityF = values[27];
				InequalityF = values[28];
				GreaterThanF = values[29];
				LessThanF = values[30];
				GreaterThanOrEqualF = values[31];
				LessThanOrEqualF = values[32];
				NotF = values[33];
				OnesComplementF = values[34];
				ExclusiveOrF = values[35];
				BitwiseAndF = values[36];
				BitwiseOrF = values[37];
				ModulusF = values[38];
				MultiplyF = values[39];
				AdditionF = values[40];
				SubtractionF = values[41];
				DivisionF = values[42];
				RightShiftF = values[43];
				LeftShiftF = values[44];
				typeofF = values[45];
				hashF = values[46];
				ismutableF = values[47];
				isabstracttypeF = values[48];
				isimmutableF = values[49];
				isprimitivetypeF = values[50];
				sizeofF = values[51];
				parentmoduleF = values[52];
				nameofF = values[53];
				fieldcountF = values[54];
				fieldnameF = values[55];
				fieldoffsetF = values[56];
				fieldtypeF = values[57];
                BoolT = values[59];
				CharT = values[60];
				Float64T = values[61];
				Float32T = values[62];
				Float16T = values[63];
				Int64T = values[64];
				Int32T = values[65];
				Int16T = values[66];
				Int8T = values[67];
				UInt64T = values[68];
				UInt32T = values[69];
				UInt16T = values[70];
				UInt8T = values[71];
				ArrayT = values[72];
                 
                RegisterPrimitive(typeof(bool), BoolT);
				RegisterPrimitive(typeof(char), CharT);
				RegisterPrimitive(typeof(double), Float64T);
				RegisterPrimitive(typeof(float), Float32T);
				RegisterPrimitive(typeof(Half), Float16T);
				RegisterPrimitive(typeof(long), Int64T);
				RegisterPrimitive(typeof(int), Int32T);
				RegisterPrimitive(typeof(short), Int16T);
				RegisterPrimitive(typeof(sbyte), Int8T);
				RegisterPrimitive(typeof(ulong), UInt64T);
				RegisterPrimitive(typeof(uint), UInt32T);
				RegisterPrimitive(typeof(ushort), UInt16T);
				RegisterPrimitive(typeof(byte), UInt8T);
				RegisterPrimitive(typeof(Array), ArrayT);
            }
        }
    }
}