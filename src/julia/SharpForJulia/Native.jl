module Native
    export RegisterSharpFunction, AbstractNativeType, NativeString, NativeObject, NativeArray, @sharpfunction
    export GetSharpFunctionHandle

	using ..Sharp: AbstractNativeType, AbstractSharpType

    const sharp_function_handles = Dict{Symbol, Function}()
	GetSharpFunctionHandle(name::Symbol) = sharp_function_handles[name]

	@enum AllocationType <: UInt8 begin 
		JuliaPinned
		RequiresJuliaPin

		SharpPinned
		RequiresSharpPin

		External
    end
    
    function RegisterSharpFunction(name::Symbol, ptr::Ptr{Cvoid})
		haskey(sharp_function_handles, name) && return sharp_function_handles[name]
		ptr == C_NULL && return nothing

		f = function(data::AbstractNativeType...)
				try
					size = 0
					idx = 0
					foreach(function (x)
								if x.alloca == RequiresJuliaPin
									x.alloca = JuliaPinned
								end
								size += sizeof(x)
							end, data)
					arr = Array{Ptr{Cvoid}}(undef, size)
					GC.@preserve arr begin
						ptr = pointer(arr)
						for d in data
							unsafe_store!(ptr, d)
							idx += sizeof(d)
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
		alloc::AllocationType

		function NativeString(data::AbstractString)
			bytes = transcode(UInt16, data)
			return new(Ptr{Cvoid}(length(bytes)), pointer(bytes), RequiresJuliaPin)
		end

		NativeString(len::Ptr{Cvoid}, ptr::Ptr{Cvoid}, alloc::AllocationType) = new(len, convert(Ptr{UInt16}, ptr), alloc)	
		
		Base.length(::NativeString) = 2
		function Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, ns::NativeString)
			unsafe_store!(ptr, ns.len, 1)
			unsafe_store!(ptr, ns.ptr, 2)
		end
	end
	
	struct NativeObject <: AbstractNativeType
		ptr::Ptr{Cvoid}
		alloc::AllocationType

		NativeObject(ptr::Ptr{Cvoid}, alloc::AllocationType) = new(ptr, alloca)
		
		Base.length(::NativeObject) = 1
		Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, no::NativeObject) = unsafe_store!(ptr, no.ptr)
	end

	struct NativeArray <: AbstractNativeType
		len::Ptr{Cvoid}
		ptr::Ptr{NativeObject}
		alloc::AllocationType

		NativeArray(len::Ptr{Cvoid}, ptr::Ptr{NativeObject}, alloc::AllocationType) = new(len, ptr, alloca)

		Base.length(::NativeArray) = 2
		Base.unsafe_store!(ptr::Ptr{Ptr{Cvoid}}, na::NativeArray) = (unsafe_store!(ptr, na.len, 1); unsafe_store!(ptr, na.ptr, 2))
	end

	createnativeobject(x::AbstractString)  = NativeString(x)
	createnativeobject(x) = NativeObject(ismutabletype(typeof(x)) ? pointer_from_objref(x) : Base.unsafe_convert(Ptr{Cvoid}, Ref(x)), RequiresJuliaPin)
	createnativeobject(x::Array) = NativeArray(Ptr{Cvoid}(length(data)), pointer(x), RequiresJuliaPin)
	createnativeobject(nt::AbstractSharpType) = createnativeobject(nt.ptr)
	createnativeobject(nt::AbstractNativeType) = nt
	createnativeobject(x::Ptr{Cvoid}) = NativeObject(x, External)
	Base.convert(::Type{AbstractNativeType}, x) = createnativeobject(x)

    macro sharpfunction(name, args, funargs)
		args = (args isa Expr) && args.head == :tuple ? args.args : [args]
		funargs = (funargs isa Expr) && funargs.head == :tuple ? funargs.args : [funargs]
		return quote
				@eval($__module__, global $(name)($(args...)) = GetSharpFunctionHandle($(QuoteNode(name)))($(funargs...)))
			end
	end
end




