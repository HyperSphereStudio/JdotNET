"Written by Johnathan Bizzano"
module JuliaInterface

	export method_argnames, SharpField, SharpType, SharpConstructor, SharpObject, SharpMethod, free, pin, @T_str, @P_str, @G_str, @R_str, sharptype, sharpbox, sharpunbox, @netusing

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

	"Container for a sharp field"
	struct SharpField
		ptr::Int
		
		SharpField(ptr::Int) = new(ptr)
		
		Base.getindex(sf::SharpField) = sf()
		Base.getindex(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, obj, val) = sf(obj, val)
		
		(sf::SharpField)() = _SharpInvoke(sf.ptr, Any[])
		(sf::SharpField)(obj) = _SharpInvoke(sf.ptr, Any[obj])
		(sf::SharpField)(obj, newVal) = _SharpInvoke(sf.ptr, Any[obj, newVal])
		
		Base.hash(so::SharpField) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpField) = _SharpToString(so.ptr)
		Base.print(io::IO, so::SharpField) = print(io, string(so))
		Base.show(io::IO, so::SharpField) = show(io, string(so))
	end

	"Container for a sharp type"
	struct SharpType
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
		
		Base.hash(so::SharpType) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpType) = _SharpToString(so.ptr)
		Base.print(io::IO, so::SharpType) = print(io, string(so))
		Base.show(io::IO, so::SharpType) = show(io, string(so))
	end
	
	"Container for a sharp constructor. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpConstructor
		ptr::Int
		
		SharpConstructor(ptr::Int) = new(ptr)
		
		Base.getindex(so::SharpConstructor) = so
		Base.getindex(so::SharpConstructor, generic_types::SharpType...) = _SharpGetGenericConstructor(so.ptr, Any[gen_type.ptr for gen_type in generic_types])
		(sc::SharpConstructor)(parameters...) = _SharpInvoke(sc.ptr, Any[parameters...])
		
		Base.hash(so::SharpConstructor) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpConstructor) = _SharpToString(so.ptr)
		Base.print(io::IO, so::SharpConstructor) = print(io, string(so))
		Base.show(io::IO, so::SharpConstructor) = show(io, string(so))
	end

	"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpMethod
		ptr::Int
		
		SharpMethod(ptr::Int) = new(ptr)
		
		Base.getindex(so::SharpMethod) = so
		Base.getindex(so::SharpMethod, generic_types::SharpType...) = _SharpGetGenericMethod(so.ptr, Any[gen_type.ptr for gen_type in generic_types])
		(sc::SharpMethod)(parameters...) = _SharpInvoke(sc.ptr, Any[parameters...])
		
		Base.hash(so::SharpMethod) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpMethod) = _SharpToString(so.ptr)
		Base.print(io::IO, so::SharpMethod) = print(io, string(so))
		Base.show(io::IO, so::SharpMethod) = show(io, string(so))
	end
	
	"Get the Sharp Type"
	sharptype(so) = SharpType(_SharpGetType(so.ptr))

	"Container for sharp object"
	mutable struct SharpObject
		ptr::Int
		
		SharpObject(ptr::Int) = new(ptr)
		
		function Base.getproperty(st::SharpObject, sym::Symbol)
			if sym == :ptr
				return Base.getfield(st, :ptr)
			else 
			    sm = SharpOwnerMethod(_SharpGetMethod(sharptype(st).ptr, string(sym)), st)
				(sm.method.ptr != 0) && return sm
				return SharpOwnerField(_SharpField(sharptype(st).ptr, string(sym)), st)
			end
		end
		
		Base.hash(so::SharpObject) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpObject) = _SharpToString(so.ptr)
		Base.print(io::IO, so::SharpObject) = print(io, string(so))
		Base.show(io::IO, so::SharpObject) = show(io, string(so))
	end
	
		
	"Container for a sharp owner field"
	struct SharpOwnerField
		field::SharpField
		owner::SharpObject
		
		SharpOwnerField(field::SharpField, owner::SharpObject) = new(field, owner)
		
		Base.getindex(sf::SharpOwnerField) = sf()
		Base.setindex!(sf::SharpOwnerField, val) = sf(val)
		
		(sf::SharpOwnerField)() = sf.field(sf.owner)
		(sf::SharpOwnerField)(obj) = sf.field(sf.owner, val)
		
		Base.hash(so::SharpOwnerField) = hash(so.field)
		Base.string(so::SharpOwnerField) = string(so.field)
		Base.print(io::IO, so::SharpOwnerField) = print(io, so.field)
		Base.show(io::IO, so::SharpOwnerField) = show(io, so.field)
	end
	
	"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpOwnerMethod
		method::SharpMethod
		owner::SharpObject
		
		SharpOwnerMethod(sm::SharpMethod, owner::SharpObject) = new(sm)
		
		Base.getindex(so::SharpOwnerMethod) = so
		Base.getindex(so::SharpOwnerMethod, generic_types::SharpType...) = SharpOwnerMethod(so(generic_types...), so.owner) 
		(sc::SharpOwnerMethod)(parameters...) = so.method(sc.owner, parameters...)
		
		Base.hash(so::SharpOwnerMethod) = hash(so.method)
		Base.string(so::SharpOwnerMethod) = string(so.method)
		Base.print(io::IO, so::SharpOwnerMethod) = print(io, so.method)
		Base.show(io::IO, so::SharpOwnerMethod) = show(io, so.method)
	end
	
	"Container for sharp exceptions."
	struct SharpException <: Exception
		exp::SharpObject
		SharpException(obj::SharpObject) = new(obj)
		
		Base.showerror(io::IO, se::SharpException) = print(io, se.exp)
		Base.hash(so::SharpException) = hash(so.exp)
		Base.string(so::SharpException) = string(so.exp)
		Base.print(io::IO, so::SharpException) = print(io, se.exp)
		Base.show(io::IO, so::SharpException) = show(io, so.exp)
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
	mutable struct SharpStub
		ptr::Int
		wasFreed::Bool
		
		function SharpStub(ptr::Int)
			val = new(ptr, false)
			finalizer(free, val)
			val
		end
		
		Base.close(st::SharpStub) = free(st)
		Base.hash(so::SharpStub) = _SharpGetHashCode(so.ptr)
		Base.string(so::SharpStub) = _SharpToString(so.ptr)
	end
	
	function typefind(mod, type) 
		if !hasproperty(mod, :NETNAMESPACE)
			Core.eval(mod, :(NETNAMESPACE = Symbol[]))
		end
		storage = getproperty(mod, :NETNAMESPACE)
		st = SharpType(string(type))
		for i in length(storage):-1:1
			(st.ptr != 0) && return st
			st = SharpType("$(storage[i]).$type") 
		end
		return st
	end

	macro netusing(namespace)
		if !hasproperty(__module__, :NETNAMESPACE)
			Core.eval(__module__, :(NETNAMESPACE = Symbol[]))
		end
		storage = getproperty(__module__, :NETNAMESPACE)
		idx = findfirst(==(namespace), storage)
		if idx != nothing
			deleteat!(storage, idx)
		end
		push!(storage, namespace)
	end
	
    TypeMap = Dict{Symbol, SharpType}()

	"Perform a static lookup of the Sharp Type. If using the same type multiple times, use P,G & R version to store then remove from local storage"
	macro T_str(type)
		type = Symbol(type)
		return typefind(__module__, type)
	end
	
	"Push local type to local storage for fast lookup"
	macro P_str(type)
		type = Symbol(type)
		stype = typefind(__module__, type)
		TypeMap[type] = stype
		return stype
	end
		
	"Get type from local storage"
	macro G_str(type)
		return TypeMap[Symbol(type)]
	end
	
	"Remove type from local storage"
	macro R_str(type)
		return pop!(TypeMap, Symbol(type))
	end

	function gen_ccall(addr, ret, argtypes...)
		argSyms = [Symbol("arg$i") for i in 1:length(argtypes)]
		return Core.eval(Main.JuliaInterface, Expr(:(->), Expr(:tuple, argSyms...), Expr(:call, :ccall, addr, ret, Expr(:tuple, argtypes...), argSyms...)))
	end

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