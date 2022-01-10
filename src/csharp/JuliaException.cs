using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    public class JuliaException : Exception
    {
        private JLVal excep;
        internal JuliaException(JLVal excep) { this.excep = excep; }

        public void Resolve() => JuliaCalls.jl_exception_clear();

        public override string ToString() => JLFun.SprintF.Invoke(JLFun.ShowErrorF, excep).UnboxString();
    }
}
