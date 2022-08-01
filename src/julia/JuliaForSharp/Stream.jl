using .Reflection:SharpOwnerMethod

struct SharpInputStream <: IO
    read_handle::SharpOwnerMethod
    close_handle::SharpOwnerMethod

    SharpInputStream(read_handle::SharpOwnerMethod, close_handle::SharpOwnerMethod) = new(read_handle, close_handle)

    Base.read(sis::SharpInputStream, ::Type{String}) = sis.read_handle()
    Base.close(sis::SharpInputStream) = sis.close_handle(true)
    Base.isopen(sis::SharpInputStream) = sis.close_handle(false)
end

struct SharpOutputStream <: IO
    write_handle::SharpOwnerMethod
    close_handle::SharpOwnerMethod

    SharpOutputStream(write_handle::SharpOwnerMethod, close_handle::SharpOwnerMethod) = new(write_handle, close_handle)
    Base.write(sos::SharpOutputStream, s) = sos.write_handle(sos.write_handle_obj, string(s)) 
    Base.close(sos::SharpOutputStream) = sos.close_handle(true)
    Base.isopen(sos::SharpOutputStream) = sos.close_handle(false)
end