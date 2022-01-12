"Written by Johnathan Bizzano"
module JuliaInterface

	export SharpField, SharpType, SharpConstructor, SharpObject, SharpMethod, free, pin

	_SharpStringMethod = nothing
	_SharpIntMethod = nothing
	_SharpConstructor = nothing
	_SharpClass = nothing
	_SharpField = nothing
	_SharpInvoke = nothing
	_SharpPin = nothing
	_SharpFree = nothing

	struct SharpField
		ptr::Int
		
		SharpField(type, name::String) = _SharpField(type, name)
		SharpField(ptr::Int) = new(ptr)
		
		(sf::SharpField)() = _SharpInvoke(sf.ptr, Any[])					#Static Field Get
		(sf::SharpField)(obj) = _SharpInvoke(sf.ptr, Any[obj])			#Get / Static Field Set
		(sf::SharpField)(obj, newVal) = _SharpInvoke(sf.ptr, Any[obj, newVal])   #Set
	end

	struct SharpConstructor
		ptr::Int
		
		SharpConstructor(ptr::Int) = new(ptr)
		SharpConstructor(type, idx) = _SharpConstructor(type.ptr, Int32(idx))
		SharpConstructor(type) = SharpConstructor(type, 0)
		(sc::SharpConstructor)(parameters...) = _SharpInvoke(sc.ptr, Any[parameters...])
	end

	struct SharpMethod
		ptr::Int
		
		SharpMethod(ptr::Int) = new(ptr)
		SharpMethod(type, name::String) = _SharpStringMethod(type.ptr, name)
		SharpMethod(type, idx) = _SharpIntMethod(type.ptr, Int32(idx))
		(sm::SharpMethod)(parameters...) = _SharpInvoke(sm.ptr, Any[parameters...])
	end

	struct SharpType
		ptr::Int
	
		SharpType(name::String) = _SharpClass(name)
		function SharpType(ptr::Int)
			(ptr == 0) && error("Type does not exist")
			new(ptr)
		end
		GetField(st::SharpType, name::String) = SharpField(st, name)
		GetConstructor(st::SharpType, idx) = SharpConstructor(st, idx)
		GetMethod(st::SharpType, name::String) = SharpMethod(st, name)
	end

	mutable struct SharpObject
		ptr::Int
		
		SharpObject(ptr::Int) = new(ptr)
	end
	
	
	function free(s)
			if !s.wasFreed
				s.wasFreed = true
				_SharpFree(s.ptr)
			end
	end
	
	pin(s) = _SharpPin(s.ptr)
	
	mutable struct SharpStub
		ptr::Int
		wasFreed::Bool
		
		function SharpStub(ptr::Int)
			val = new(ptr, false)
			finalizer(free, val)
			val
		end
		
		Base.close(st::SharpStub) = free(st)
	end

	function gen_ccall(addr, ret, argtypes...)
		argSyms = [Symbol("arg$i") for i in 1:length(argtypes)]
		return Core.eval(Main.JuliaInterface, Expr(:(->), Expr(:tuple, argSyms...), Expr(:call, :ccall, addr, ret, Expr(:tuple, argtypes...), argSyms...)))
	end

	function initialize_library(getClazz, getIntMethod, getStringMethod, getConstructor, getField, sharpInvoke, sharpPinGC, sharpFreeGC)
		global _SharpClass = gen_ccall(getClazz, Any, Cstring)
		global _SharpIntMethod = gen_ccall(getIntMethod, Any, Int, Cint)
		global _SharpStringMethod = gen_ccall(getStringMethod, Any, Int, Cstring)
		global _SharpConstructor = gen_ccall(getConstructor, Any, Int, Cint)
		global _SharpField = gen_ccall(getField, Any, Int, Cstring)
		global _SharpInvoke = gen_ccall(sharpInvoke, Any, Int, Any)
		global _SharpPin = gen_ccall(sharpPinGC, Any, Int)
		global _SharpFree = gen_ccall(sharpFreeGC, Cvoid, Int)
	end
end