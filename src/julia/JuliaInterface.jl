module JuliaInterface
	export method_argnames, SharpField, SharpType, SharpConstructor, SharpObject, SharpMethod, free, pin, @T_str, @P_str, @G_str, @R_str, sharptype, sharpbox, sharpunbox, @netusing, makearray

	_SharpGetMethod = nothing
	_SharpGetGenericMethod = nothing
	_SharpGetConstructor = nothing
	_SharpGetGenericConstructor = nothing
	_SharpClass = nothing
	_SharpField = nothing
	_SharpInvoke = nothing
	_SharpPin = nothing
	_SharpFree = nothing
	_SharpGetType = nothing
	_SharpToString = nothing
	_SharpGetHashCode = nothing
	_SharpEquals = nothing
	_SharpBox = nothing
	_SharpUnBox = nothing

	function method_argnames(m::Method)
       argnames = ccall(:jl_uncompress_argnames, Vector{Symbol}, (Any,), m.slot_syms)
       isempty(argnames) && return argnames
       return [string(sym) for sym in argnames[1:m.nargs][2:end]]
	end

	abstract type AbstractSharpType end

	Base.print(io::IO, ast::AbstractSharpType) = print(io, string(ast))
	Base.show(io::IO, ast::AbstractSharpType) = show(io, string(ast))
	Base.hash(ast::AbstractSharpType) = _SharpGetHashCode(ast.ptr)
	Base.string(ast::AbstractSharpType) = _SharpToString(ast.ptr)

	"Container for a sharp field"
	struct SharpField <: AbstractSharpType
		ptr::Int
		
		SharpField(ptr::Int) = new(ptr)
		
		Base.getindex(sf::SharpField) = sf()
		Base.getindex(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, obj, val) = sf(obj, val)
		
		(sf::SharpField)() = _SharpInvoke(sf.ptr, Any[])
		(sf::SharpField)(obj) = _SharpInvoke(sf.ptr, Any[obj])
		(sf::SharpField)(obj, newVal) = _SharpInvoke(sf.ptr, Any[obj, newVal])
	end
		
	"Container for a sharp type"
	struct SharpType <: AbstractSharpType
		ptr::Int
	
		SharpType(name::String) = _SharpClass(name)
		SharpType(ptr::Int) = new(ptr)
		
		function Base.getproperty(st::SharpType, sym::Symbol)
			if sym == :new
				return _SharpGetConstructor(st.ptr)
			elseif sym == :ptr
				return Base.getfield(st, :ptr)
			else 
				sm = _SharpGetMethod(st.ptr, string(sym))
				(sm.ptr != 0) && return sm
				return _SharpField(st.ptr, string(sym))
			end
		end
	end

	"Container for a sharp constructor. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpConstructor <: AbstractSharpType
		ptr::Int
		
		SharpConstructor(ptr::Int) = new(ptr)
		
		Base.getindex(sc::SharpConstructor) = sc
		Base.getindex(sc::SharpConstructor, generic_types::SharpType...) = _SharpGetGenericConstructor(sc.ptr, Any[gen_type.ptr for gen_type in generic_types])
		(sc::SharpConstructor)(parameters...) = _SharpInvoke(sc.ptr, Any[parameters...])
	end

		"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpMethod <: AbstractSharpType
		ptr::Int
		
		SharpMethod(ptr::Int) = new(ptr)
		
		Base.getindex(sm::SharpMethod) = sm
		Base.getindex(sm::SharpMethod, generic_types::SharpType...) = _SharpGetGenericMethod(sm.ptr, Any[gen_type.ptr for gen_type in generic_types])
		(sm::SharpMethod)(parameters...) = _SharpInvoke(sm.ptr, Any[parameters...])
	end

	"Get the Sharp Type"
	sharptype(so) = SharpType(_SharpGetType(so.ptr))

	"Container for sharp object"
	mutable struct SharpObject <: AbstractSharpType
		ptr::Int
		
		SharpObject(ptr::Int) = new(ptr)
		
		function Base.getproperty(st::SharpObject, sym::Symbol)
			if sym == :ptr
				return Base.getfield(st, :ptr)
			else 
			    sm = SharpOwnerMethod(_SharpGetMethod(sharptype(st).ptr, string(sym)), st)
				(sm.ptr.ptr != 0) && return sm
				return SharpOwnerField(_SharpField(sharptype(st).ptr, string(sym)), st)
			end
		end
	end

	"Container for a sharp owner field"
	struct SharpOwnerField <: AbstractSharpType
		ptr::SharpField
		owner::SharpObject
		
		SharpOwnerField(field::SharpField, owner::SharpObject) = new(field, owner)
		
		Base.getindex(sof::SharpOwnerField) = sof()
		Base.setindex!(sof::SharpOwnerField, val) = sof(val)
		
		(sof::SharpOwnerField)() = sof.ptr(sof.owner)
		(sof::SharpOwnerField)(obj) = sof.ptr(sof.owner, val)
	end
	
	"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpOwnerMethod <: AbstractSharpType
		ptr::SharpMethod
		owner::SharpObject
		
		SharpOwnerMethod(som::SharpMethod, owner::SharpObject) = new(som, owner)
		
		Base.getindex(som::SharpOwnerMethod) = som
		Base.getindex(som::SharpOwnerMethod, generic_types::SharpType...) = SharpOwnerMethod(som(generic_types...), som.owner) 
		(som::SharpOwnerMethod)(parameters...) = som.ptr(som.owner, parameters...)
	end
	
	"Container for sharp exceptions."
	struct SharpException <: Exception
		exp::SharpObject
		SharpException(so::SharpObject) = new(so)
		
		Base.showerror(io::IO, se::SharpException) = print(io, se.exp)
		Base.hash(se::SharpException) = hash(se.exp)
		Base.string(se::SharpException) = string(se.exp)
		Base.print(io::IO, se::SharpException) = print(io, se.exp)
		Base.show(io::IO, se::SharpException) = show(io, se.exp)
	end
	
	"Box the Julia type to sharp type"
	sharpbox(v::Any) = SharpObject(_SharpBox(v))
	
	"Unbox the sharp type to julia type"
	sharpunbox(ptr::Int) = _SharpUnBox(ptr)
	
	"Unbox the sharp container to julia type"
	sharpunbox(v::Any) = sharpunbox(v.ptr)

	"Free the Sharp GC Pin"
	function free(s)
			if !s.wasFreed
				s.wasFreed = true
				_SharpFree(s.ptr)
			end
	end
	
	"Pin the Sharp Object"
	pin(s) = _SharpPin(s.ptr)
	
	"Stub for the Sharp GC"
	mutable struct SharpStub <: AbstractSharpType
		ptr::Int
		wasFreed::Bool
		
		function SharpStub(ptr::Int)
			val = new(ptr, false)
			finalizer(free, val)
			val
		end
		
		Base.close(st::SharpStub) = free(st)
	end
	
	function typefind(mod, type) 
		if !hasproperty(mod, :NETNAMESPACE)
			Core.eval(mod, :(NETNAMESPACE = String[]))
		end
		storage = getproperty(mod, :NETNAMESPACE)

		st = SharpType(type)
		(st.ptr != 0) && return st

		for i in length(storage):-1:1
			st = SharpType("$(replace(storage[i])).$type")
			(st.ptr != 0) && return st
		end

		return st
	end

	macro netusing(namespace)
		namespace = string(namespace)
		if !hasproperty(__module__, :NETNAMESPACE)
			Core.eval(__module__, :(NETNAMESPACE = String[]))
		end
		storage = getproperty(__module__, :NETNAMESPACE)
		idx = findfirst(==(namespace), storage)
		namespace in storage || (push!(storage, namespace))
		return nothing
	end

    const TypeMap = Dict{String, SharpType}()

	"Perform a static lookup of the Sharp Type. If using the same type multiple times, use P,G & R version to store then remove from local storage"
	macro T_str(type)
		return typefind(__module__, string(type))
	end
	
	"Push local type to local storage for fast lookup"
	macro P_str(type)
		type = string(type)
		stype = typefind(__module__, type)
		TypeMap[type] = stype
		return stype
	end
		
	"Get type from local storage"
	macro G_str(type)
		return TypeMap[string(type)]
	end
	
	"Remove type from local storage"
	macro R_str(type)
		return pop!(TypeMap, string(type))
	end

	function gen_ccall(addr, ret, argtypes...)
		argSyms = [Symbol("arg$i") for i in 1:length(argtypes)]
		return Core.eval(Main.JuliaInterface, Expr(:(->), Expr(:tuple, argSyms...), Expr(:call, :ccall, addr, ret, Expr(:tuple, argtypes...), argSyms...)))
	end

	makearray(T, dims) = Array{T}(undef, dims...)

	function initialize_library(getClazz, getMethod, getGenericMethod, getConstructor, getGenericConstructor, getField, 
			sharpInvoke, sharpPinGC, sharpFreeGC, sharpGetType, sharpEquals, sharpToString,
			sharpGetHash, sharpBox, sharpUnBox)

		global _SharpClass = gen_ccall(getClazz, Any, Cstring)
		global _SharpGetMethod = gen_ccall(getMethod, Any, Int, Cstring)
		global _SharpGetGenericMethod = gen_ccall(getGenericMethod, Any, Int, Cstring)
		global _SharpGetConstructor = gen_ccall(getConstructor, Any, Int)
		global _SharpGetGenericConstructor = gen_ccall(getGenericConstructor, Any, Int, Any)
		global _SharpField = gen_ccall(getField, Any, Int, Cstring)
		global _SharpInvoke = gen_ccall(sharpInvoke, Any, Int, Any)
		global _SharpPin = gen_ccall(sharpPinGC, Any, Int)
		global _SharpFree = gen_ccall(sharpFreeGC, Cvoid, Int)
		global _SharpGetType = gen_ccall(sharpGetType, Int, Int)
		global _SharpToString = gen_ccall(sharpToString, String, Int)
		global _SharpGetHashCode = gen_ccall(sharpGetHash, Int, Int)
		global _SharpEquals = gen_ccall(sharpEquals, Bool, Int, Int)
		global _SharpBox = gen_ccall(sharpBox, Int, Any)
		global _SharpUnBox = gen_ccall(sharpUnBox, Any, Int)
	end
end