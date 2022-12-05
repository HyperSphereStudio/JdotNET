include("csharp/Core/JPrimitive.jl")

project_root = pwd()

generate_primitives(project_root, "$project_root/src/csharp/Core", "$project_root/generated/csharp/Core")