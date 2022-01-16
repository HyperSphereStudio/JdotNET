using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Written by Johnathan Bizzano
namespace JuliaInterface
{
    public class JuliaException : Exception
    {
        private JLVal excep;
        public JuliaException(JLVal excep) { this.excep = excep; }

        public override string ToString() => JLFun.SprintF.Invoke(JLFun.ShowErrorF, excep).UnboxString();
    }
}
