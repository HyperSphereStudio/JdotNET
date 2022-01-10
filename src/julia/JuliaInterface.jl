module JuliaInterface

	SharpGetMethod = 0
	SharpGetConstructor = 0
	SharpGetClass = 0
	SharpGetField = 0
	SharpGetFieldValue = 0
	SharpSetFieldValue = 0

	struct SharpObject
		ptr::Ptr
		SharpObject(ptr::Ptr) = new(ptr)
		
	end

	function gen_ccall(addr, ret, argtypes...)
		argSyms = [Symbol("arg$i") for i in 1:length(argtypes)]
		return Core.eval(Main.JuliaInterface, Expr(:(->), Expr(:tuple, argSyms...), Expr(:call, :ccall, addr, ret, Expr(:tuple, argtypes...), argSyms...)))
	end

	function initialize_library(getClazz, getMethod, getConstructor, getField, getFieldValue, setFieldValue)
		SharpGetClass = gen_ccall(getClazz, Clong, (Cstring,))
		SharpGetMethod = gen_ccall(getMethod, Clong, (Clong, Cstring))
		SharpGetConstructor = gen_ccall(getConstructor, Clong, (Clong, Cint))
		SharpGetField = gen_ccall(getField, Clong, (Clong, Cstring))
		SharpGetFieldValue = gen_ccall(getFieldValue, Any, (Clong, Clong))
		SharpSetFieldValue = gen_ccall(setFieldValue, Cvoid, (Clong, Clong, Clong))
	end
	

end

using Main.JuliaInterface