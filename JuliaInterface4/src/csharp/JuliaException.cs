using System;

//Written by Johnathan Bizzano
namespace JULIAdotNET
{
    public class JuliaException : Exception
    {
        private readonly JuliaV _ptr;
        public JuliaException(JuliaV excep) { this._ptr = excep; }

        public override string ToString() => (string)JuliaPrimitive.SprintF.UnsafeInvoke(JuliaPrimitive.ShowErrorF, _ptr);
        public override string Message => ToString();
    }
}
