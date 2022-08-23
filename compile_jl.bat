git commit -m "DEBUG"
julia -e "using Pkg; Pkg.add(path = pwd())"
git reset --soft "HEAD^"