module Native
    export RegisterSharpFunction, AbstractNativeType, NativeString, NativeObject, NativeArray, @sharpfunction
    export GetSharpFunctionHandle

	using ..Sharp: AbstractNativeType, AbstractSharpType

    const sharp_function_handles = Dict{Symbol, Function}()
	GetSharpFunctionHandle(name::Symbol) = sharp_function_handles[name]
    
    function RegisterSharpFunction(name::Symbol, ptr::Ptr{Cvoid})
		haskey(sharp_function_handles, name) && return sharp_function_handles[name]
		ptr == C_NULL && return nothing

		f = function(data::AbstractNativeType...)
				try
					s = 0
					foreach(x -> s += length(x), data)
					ptrarray = Array{Ptr{Cvoid}}(undef, s)
					GC.@preserve ptrarray begin
						ptrarrayptr = pointer(ptrarray)
						
						for d in data
							unsafe_store!(ptrarrayptr, d)
							ptrarrayptr += length(d)
						end

						return_value = ccall(ptr, Any, (Ptr{Ptr{Cvoid}},), pointer(ptrarray))
						return return_value
					end
				catch e
					println("[Native Sharp Function::Julia] Exception in $name due to $e")
					throw(e)
				end
			end

		sharp_function_handles[name] = f
		return f
	end

    struct NativeString <: AbstractNativeType
		len::Ptr{Cvoid}
		ptr::Ptr{UInt16}

		NativeString(data::AbstractString) = (bytes = transcode(UInt16, data); new(Ptr{Cvoid}(length(bytes)), pointer(bytes)))
		
		Base.length(::NativeString) = 2
		Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, ns::NativeString) = (unsafe_store!(ptr, ns.len, 1); unsafe_store!(ptr, ns.ptr, 2))
	end
	
	mutable struct NativeObject <: AbstractNativeType
		ptr::Ptr{Cvoid}

		function NativeObject(ptr::Ptr{Cvoid})
			o = new(ptr)
			Base.finalizer(x -> FreeSharp4JuliaReference(ptr), o)
			return o
		end
		
		NativeObject(data::Union{NativeObject, NativeString}) = new(data.ptr)
		NativeObject(nt::AbstractSharpType) = nt.ptr
		
		Base.length(::NativeObject) = 1
		Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, no::NativeObject) = unsafe_store!(ptr, no.ptr)
	end

	struct NativeArray <: AbstractNativeType
		len::Ptr{Cvoid}
		ptr::Ptr{NativeObject}

		NativeArray(data::Array{NativeObject}) = new(Ptr{Cvoid}(length(data)), pointer(data))

		Base.length(::NativeArray) = 2
		Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, na::NativeArray) = (unsafe_store!(ptr, na.len, 1); unsafe_store!(ptr, na.ptr, 2))
	end

    macro sharpfunction(name, args, funargs)
		args = (args isa Expr) && args.head == :tuple ? args.args : [args]
		funargs = (funargs isa Expr) && funargs.head == :tuple ? funargs.args : [funargs]
		return quote
				@eval($__module__, global $(name)($(args...)) = GetSharpFunctionHandle($(QuoteNode(name)))($(funargs...)))
			end
	end
end




