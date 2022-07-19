module JULIAdotNET

export Init

using Libdl

const MinimumNetVersion = "4.0.1"
const DotNetCompileVersion = "net5.0"
const Debug = false

if Sys.iswindows()
    const CStr = Cwstring
    const Ccar = Cwchar_t
elseif Sys.islinux() || Sys.isunix()
    const CStr = Cstring 
    const Ccar = Cchar
end
const CStrPtr = Ptr{CStr}

if Debug 
    ENV["COREHOST_TRACE"] = 1
    ENV["COREHOST_TRACE_VERBOSITY"] = 4
end


@enum hostfxr_delegate_type begin 
    hdt_com_activation
    hdt_load_in_memory_assembly
    hdt_winrt_activation
    hdt_com_register
    hdt_com_unregister
    hdt_load_assembly_and_get_function_pointer
    hdt_get_function_pointer
end

include("julia/CSharpError.jl")
include("julia/JuliaInterface.jl")

using .JuliaInterface

get_sys_arch_name() = join(match(r"RID:\s+([^0-9]+)(?:[0-9]*)([^0-9])([^\n\r]*)(?:\n|\r)", read(`dotnet --info`, String)), "")

function expand_version(version)
    s = split(version, ".")
    v = 0.0
    for w in 1:length(s)
        v += parse(Float64, s[w]) ^ (1-w)
    end
    return v
end

function search_for_native_net_dir()
    min_version = expand_version(MinimumNetVersion)
    base_dotnet_dir = dirname(strip(read(`where dotnet`, String)))
    sys_arch = get_sys_arch_name()
    base_dotnet_dir = joinpath(base_dotnet_dir, "packs", "Microsoft.NETCore.App.Host.$sys_arch")

    versions = sort!([(expand_version(basename(sdk_version)), sdk_version) for sdk_version in readdir(base_dotnet_dir)])
    v = findfirst(x -> x[1] >= min_version, versions)
    base_dotnet_dir = joinpath(base_dotnet_dir, string(versions[v][2]), "runtimes", sys_arch, "native")

    return base_dotnet_dir
end

mutable struct NetCoreHost
    open_libraries::Array{Tuple{String, Ptr{Cvoid}}}

    function NetCoreHost()
        nch = new([])
        Base.finalizer(free, nch)
        return nch
    end
end

free(nch::NetCoreHost) = (length(nch.open_libraries) == 0 && return; foreach(x -> Libdl.dlclose(x[2]), nch.open_libraries); empty!(nch.open_libraries))
load_lib(nch::NetCoreHost, library_name::String) = (l = (basename(library_name), Libdl.dlopen(library_name)); (l == C_NULL) && throw(error("Unable to Load Library $library_name")); push!(nch.open_libraries, l); return l)

function Init()
    (isinteractive()) && throw(error("Currently Unable To Intialize .NET from REPL...."))

    #To have .NET Access all neccesary libraries around JuliaInterface.dll
    lastDIR = pwd()
    cd(joinpath(pkgdir(JULIAdotNET), "bin", "Release", DotNetCompileVersion))

    max_path = 260
    dll_path = pwd()

    runtime_json_config_path = joinpath(dll_path, "JuliaInterface.runtimeconfig.json")
    native_net_dir = search_for_native_net_dir()

    nch = NetCoreHost()
    load_f(library::Tuple{String, Ptr{Cvoid}}, function_name::String) = (f = Libdl.dlsym(library[2], function_name); (f == C_NULL) && throw(error("Unable to Load Function $function_name in $(library[1])")); return f)
    check(rc, msg) = (rc != Success) && throw(error("$msg Exit Code:$rc"))
    generate_buffer() = pointer(Array{Ccar}(undef, max_path))

    function load_runtime()
        net_host_lib = load_lib(nch, joinpath(native_net_dir, "nethost"))
        get_hostfxr_path = load_f(net_host_lib, "get_hostfxr_path")

        #Get Host Path
        host_dir = generate_buffer()
        host_dir_len = Ref{Csize_t}(max_path)
        check(ccall(get_hostfxr_path, StatusCode, (CStr, Ptr{Csize_t}, Ptr{Nothing}), host_dir, host_dir_len, C_NULL), 
             "Unable To Successfully Call get_hostfxr_path")

        #Initialize Init, Get, Close
        host_lib = load_lib(nch, unsafe_string(host_dir, host_dir_len[] - 1))
        init_fptr = load_f(host_lib, "hostfxr_initialize_for_runtime_config")
        get_delegate_fptr = load_f(host_lib, "hostfxr_get_runtime_delegate")
        close_fptr = load_f(host_lib, "hostfxr_close")
        
        #Initialize .NET
        cxt = Ref{Ptr{Cvoid}}(C_NULL)
        rc = ccall(init_fptr, StatusCode, (CStr, Ptr, Ref, ), runtime_json_config_path, C_NULL, cxt)
        (rc != 0 || cxt == C_NULL) && check(rc, "Unable To Initialize from Runtime Config.")

        #Obtain load asm and get function ptr
        load_assembly_and_get_function_pointer = Ref{Ptr{Cvoid}}(C_NULL)
        check(ccall(get_delegate_fptr, StatusCode, (Ptr{Cvoid}, hostfxr_delegate_type, Ref{Ptr{Cvoid}}, ), 
                cxt[], 
                hdt_load_assembly_and_get_function_pointer,
                load_assembly_and_get_function_pointer),
            "Unable to Retrieve Load Assembly and Get Function Pointer")
        
        #Close the .NET Context
        check(ccall(close_fptr, StatusCode, (Ptr{Cvoid}, ), cxt[]),
             "Unable to Close Context Handle")

        #Call CSharp.Init from JuliaInterface.dll
        complete_dll_path = joinpath(dll_path, "JuliaInterface.dll")
        dot_net_type = "JuliaInterface.CSharp, JuliaInterface"
        dot_net_method = "Init"
        unmanaged_function = Ptr{Cvoid}(-1)
        IntializeFromJulia = Ref{Ptr{Cvoid}}(C_NULL)
        rc = check(get_net_error(ccall(load_assembly_and_get_function_pointer[], Cint, (CStr, CStr, CStr, Ptr{Cvoid}, Ptr{Cvoid}, Ref{Ptr{Cvoid}}, ),
                   complete_dll_path,
                   dot_net_type,
                   dot_net_method,
                   unmanaged_function,
                   C_NULL,
                   IntializeFromJulia)), 
            "Unable to Obtain Init Handle")
        
        julia_bindir = Sys.BINDIR
        check(ccall(IntializeFromJulia[], StatusCode, (CStr, Cint), julia_bindir, sizeof(julia_bindir)),
            "Unable To Initialize Julia Interface")
    end

    try
        load_runtime()
        cd(lastDIR)
        return nch
    catch e
        free(nch)
        cd(lastDIR)
        throw(e)
    end
end

end