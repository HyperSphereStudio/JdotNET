"Written by Johnathan Bizzano"
module JuliaInterface

	export SharpField, SharpType, SharpConstructor, SharpObject, SharpMethod

	SharpGetMethod = 0
	SharpGetConstructor = 0
	SharpGetClass = 0
	SharpGetField = 0
	SharpInvoke = 0

	struct SharpField
		ptr::Int
		
		SharpField(type, name::String) = SharpGetField(type, name)
		SharpField(ptr::Int) = new(ptr)
		
		(sf::SharpField)() = SharpInvoke(sf, Any[])					#Static Field Get
		(sf::SharpField)(obj) = SharpInvoke(sf, Any[obj])			#Get / Static Field Set
		(sf::SharpField)(obj, newVal) = SharpInvoke(sf, Any[obj, newVal])   #Set
	end

	struct SharpConstructor
		ptr::Int
		
		SharpConstructor(ptr::Int) = new(ptr)
		SharpField(type, idx::Int32) = SharpGetConstructor(type, idx)
		(sc::SharpConstructor)(parameters...) = SharpInvoke(sc, Any[parameters...])
	end

	struct SharpMethod
		ptr::Int
		
		SharpMethod(ptr::Int) = new(ptr)
		SharpField(type, name::String) = SharpGetMethod(type, name)
		(sm::SharpMethod)(parameters...) = SharpInvoke(sm, Any[parameters...])
	end

	struct SharpType
		ptr::Int
	
		SharpType(name::String) = SharpGetClass(name)
		SharpType(ptr::Int) = new(ptr)
		GetField(st::SharpType, name::String) = SharpField(st, name)
		GetConstructor(st::SharpType, idx::Int32) = SharpConstructor(st, idx)
		GetMethod(st::SharpType, name::String) = SharpMethod(st, name)
	end

	struct SharpObject
		ptr::Int
		
		SharpObject(ptr::Int) = new(ptr)
	end

	function gen_ccall(addr, ret, argtypes...)
		argSyms = [Symbol("arg$i") for i in 1:length(argtypes)]
		return Core.eval(Main.JuliaInterface, Expr(:(->), Expr(:tuple, argSyms...), Expr(:call, :ccall, addr, ret, Expr(:tuple, argtypes...), argSyms...)))
	end

	function initialize_library(getClazz, getMethod, getConstructor, getField, sharpInvoke)
		SharpGetClass = gen_ccall(getClazz, Any, Cstring)
		SharpGetMethod = gen_ccall(getMethod, Any, Any, Cstring)
		SharpGetConstructor = gen_ccall(getConstructor, Any, Any, Cint)
		SharpGetField = gen_ccall(getField, Any, Any, Cstring)
		SharpInvoke = gen_ccall(sharpInvoke, Any, Any, Any)
	end
end

using Main.JuliaInterface