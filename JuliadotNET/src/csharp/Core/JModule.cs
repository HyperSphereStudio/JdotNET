using Base;

namespace JULIAdotNET;

public struct JModule
{
    private readonly Any _ptr;
    public string Name => ToString();
    public JModule Parent => _ptr.Module;
    
    public JModule(Any ptr) => _ptr = ptr;

    public static implicit operator JModule(Any ptr) => new(ptr);
    public static implicit operator Any(JModule ptr) => new(ptr._ptr);
    
    #region NeededOverloadedOperators
    public static bool operator ==(JModule v, JModule p) => v._ptr == p._ptr;
    public static bool operator !=(JModule v, JModule p) => !(v == p);
    public override string ToString() => _ptr.ToString();
    public override int GetHashCode() => _ptr.GetHashCode();
    public override bool Equals(object o) => _ptr.Equals(o);
    #endregion

    public JType GetType(Any name) => Julia.GetGlobal(_ptr, name);
    public Any GetFunction(Any name) => Julia.GetGlobal(_ptr, name);
    public Any GetGlobal(Any name) => Julia.GetGlobal(_ptr, name);
    
    public JType GetType(string name) => Julia.GetGlobal(_ptr, Julia.Symbol(name));
    public Any GetFunction(string name) => Julia.GetGlobal(_ptr, Julia.Symbol(name));
    public Any GetGlobal(string name) => Julia.GetGlobal(_ptr, Julia.Symbol(name));
}