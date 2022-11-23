module Sharp
	export method_argnames

	abstract type AbstractSharpType end
	abstract type AbstractNativeType end

	function _method_argnames(m::Method)
       argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
       isempty(argnames) && return argnames
       return [string(sym) for sym in argnames[1:m.nargs][2:end]]
	end

	_eval(mod, ex) = Core.eval(mod, Meta.parse(ex))
	_makearray(T::Type, dims::Ptr{Cvoid}, len::Int32) = (ptr = convert(Ptr{Int32}, dims); Array{T}(undef, [unsafe_load(ptr, i) for i in 1:len]...))
	_maketuple(vals) = tuple(vals...)
	_linedEvaluation(s::String, file::String, m::Module) = Core.eval(m, Meta.parseall(s, filename=file))

	include("SharpForJulia/Native.jl")
	include("JuliaForSharp/MemoryManagement.jl")
	include("SharpForJulia/Reflection.jl")
	include("SharpForJulia/NetCore.jl")
	include("JuliaForSharp/Stream.jl")

	using .Native
	using .Reflection

	function _init()::Int64
        Reflection._init()
		MemoryManagement._init()
		return 0
    end
end