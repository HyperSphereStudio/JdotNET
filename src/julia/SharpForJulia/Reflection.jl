module Reflection
    export SharpMethod, @T_str, @P_str, @G_str, @R_str, sharptype, sharpbox, sharpunbox, @netusing, makearray
    export SharpField, SharpType, SharpConstructor, SharpObject

    using ..Native
	using ..MemoryManagement
	using ..Sharp: AbstractNativeType, AbstractSharpType


    Base.print(io::IO, ast::AbstractSharpType) = print(io, string(ast))
	Base.show(io::IO, ast::AbstractSharpType) = show(io, string(ast))
	Base.hash(ast::AbstractSharpType) = GetHashCode(ast.ptr)
	Base.string(ast::AbstractSharpType) = ToString(ast.ptr)
    Base.convert(::Type{NativeObject}, ast::AbstractSharpType) = ast.ptr

    "Container for a sharp field"
	struct SharpField <: AbstractSharpType
		ptr::NativeObject
		
		SharpField(ptr::NativeObject) = new(ptr)
		
		Base.getindex(sf::SharpField) = sf()
		Base.getindex(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, val) = sf(val)
		Base.setindex!(sf::SharpField, obj, val) = sf(obj, val)
		
		(sf::SharpField)() = GetFieldValue(sf, C_NULL)
		(sf::SharpField)(obj) = GetFieldValue(sf, obj)
		(sf::SharpField)(obj, v) = SetFieldValue(sf, obj, v)
	end
		
	"Container for a sharp type"
	struct SharpType <: AbstractSharpType
	    ptr::NativeObject
	
		SharpType(name::String) = GetType(name)
		SharpType(ptr::NativeObject) = new(ptr)
		
		function Base.getproperty(st::SharpType, sym::Symbol)
			if sym == :new
				return GetConstructor(st)
			elseif sym == :ptr
				return Base.getfield(st, :ptr)
			else 
				name = string(sym)
				sf = GetField(st, name) 
				(sf.ptr.ptr != C_NULL) && return sf
				return GetMethodByName(st, name)
			end
		end

		Base.getindex(st::SharpType, generic_types::SharpType...) = GetGenericType(st, collect(generic_types))
	end

	"Container for a sharp constructor. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpConstructor <: AbstractSharpType
		ptr::NativeObject
		
		SharpConstructor(ptr::NativeObject) = new(ptr)
		
		(sc::SharpConstructor)(parameters...) = InvokeConstructor(sc, collect(parameters))
	end

	"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpMethod <: AbstractSharpType
		ptr::NativeObject
		
		SharpMethod(ptr::NativeObject) = new(ptr)
		
		Base.getindex(sm::SharpMethod) = sm
		Base.getindex(sm::SharpMethod, generic_types::SharpType...) = GetGenericMethod(sm, collect(generic_types))
		(sm::SharpMethod)(owner, parameters...) = InvokeMethod(sm, owner, createnativeobject(collect(parameters)))
	end

	"Get the Sharp Type"
	sharptype(so::AbstractSharpType) = SharpType(_GetType(so))

	"Container for sharp object"
	struct SharpObject <: AbstractSharpType
		ptr::NativeObject
		
		SharpObject(ptr::NativeObject) = new(ptr)
		
		function Base.getproperty(st::SharpObject, sym::Symbol)
			if sym == :ptr
				return Base.getfield(st, :ptr)
			else 
				t = sharptype(st)
				s = string(sym)

				sf = GetField(t, s)
				(sf.ptr.ptr != C_NULL) && return SharpOwnerField(sf, st)

				return SharpOwnerMethod(GetMethod(t, s), st)
			end
		end
	end

	"Container for a sharp owner field"
	struct SharpOwnerField <: AbstractSharpType
		ptr::SharpField
		owner::SharpObject
		
		SharpOwnerField(field, owner) = new(field, owner)
		
		Base.getindex(sof::SharpOwnerField) = sof()
		Base.setindex!(sof::SharpOwnerField, val) = sof(val)
		
		(sof::SharpOwnerField)() = sof.ptr(sof.owner)
		(sof::SharpOwnerField)(val) = sof.ptr(sof.owner, val)
	end
	
	"Container for a sharp method. Use [T,...] to pass generics. Can be invoked after passing generics (if needed)"
	struct SharpOwnerMethod <: AbstractSharpType
		ptr::SharpMethod
		owner::SharpObject
		
		SharpOwnerMethod(som, owner) = new(som, owner)
		
		Base.getindex(som::SharpOwnerMethod) = som
		Base.getindex(som::SharpOwnerMethod, generic_types::SharpType...) = SharpOwnerMethod(som[generic_types...], som.owner) 
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
	sharpbox(v::Any) = Box(v)
	
	sharpunbox(ptr::SharpObject) = Unbox(ptr)
	
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

	function _init()
		@sharpfunction(GetType, name::AbstractString, name)
		@sharpfunction(GetGenericType, (type::SharpType, generic_types::Array{SharpType}), (type, generic_types))
		
		@sharpfunction(GetMethodByName, (type::SharpType, name::AbstractString), (type, name))
		@sharpfunction(GetMethodByNameAndTypes, (type::SharpType, name::AbstractString, types::Array), (type, name, types))
		@sharpfunction(GetGenericMethod, (method::SharpMethod, generic_types::Array{SharpType}), (method, generic_types))
		@sharpfunction(InvokeMethod, (method::SharpMethod, owner::SharpObject, args::Array), (method, owner, args))
		
		@sharpfunction(GetConstructor, type::SharpType, type)
		@sharpfunction(GetConstructorByTypes, (type::NativeObject, types::Array{SharpType}), (type), types)
		@sharpfunction(InvokeConstructor, (constructor::SharpConstructor, args::Array), args)

		@sharpfunction(GetField, (type::SharpType, name::AbstractString), (type, name))
		@sharpfunction(GetFieldValue, (field::SharpField, owner::SharpObject), (field, owner))
		@sharpfunction(SetFieldValue, (field::NativeObject, owner::SharpObject, value::SharpObject), (field, owner, value))

		@sharpfunction(FreeSharp4JuliaReference, ptr::Ptr{Cvoid}, ptr) 

		@sharpfunction(GetObjectType, object::SharpObject, object) 
		@sharpfunction(ToString, object::NativeObject, object) 
		@sharpfunction(GetHashCode, object::SharpObject, object)
		@sharpfunction(Box, @nospecialize(x), x) 
		@sharpfunction(Unbox, (obj1::SharpObject, obj2::SharpObject), (obj1, obj2)) 

		Core.eval(Native, quote using ..Reflection: FreeSharp4JuliaReference end)
	end
end
