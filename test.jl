using Pkg
Pkg.activate(".")

#Pkg.build("JULIAdotNET")

using JULIAdotNET
using JULIAdotNET.JuliaInterface

handle = Init()

sharpList = T"System.Collections.Generic.List`1".new[T"System.Int64"]()
