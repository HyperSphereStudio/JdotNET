using System;

//Written by Johnathan Bizzano
namespace JULIAdotNET
{
    public class JuliaException : Exception
    {
        private JLVal excep;
        public JuliaException(JLVal excep) { this.excep = excep; }

        public override string ToString() => (string) JLFun.SprintF.Invoke(JLFun.ShowErrorF, excep);
        public override string Message => ToString();
    }
}
