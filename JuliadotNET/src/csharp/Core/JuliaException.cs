using System;
using Base;

//Written by Johnathan Bizzano
namespace JULIAdotNET
{
    public class JuliaException : Exception
    {
        private readonly Any _ptr;

        public JuliaException(Any excep){
            _ptr = excep;
        }

        public override string ToString() {
            try {
                return "JuliaException(\"" + JPrimitive.SprintF
                    .UnsafeInvoke(JPrimitive.ShowErrorF, _ptr, JPrimitive.CatchBackTraceF.UnsafeInvoke())
                    .UnboxString() + "\")";
            }catch (Exception e) {
                Console.WriteLine("Error Writing Exception To Console!");
                Console.WriteLine(e);
                Console.WriteLine(_ptr.ToString());
                throw;
            }
        }
        public override string Message => ToString();
    }
}
