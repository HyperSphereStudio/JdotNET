using JuliaInterface;
using NUnit.Framework;
using System;

namespace TestJuliaInterface
{
    [SingleThreaded]
    public class Tests
    {
        
        [OneTimeTearDown]
        public void Finish() => Julia.Exit(0);

        [OneTimeSetUp]
        public void Setup() => Julia.Init();

        [Test]
        public void JuliaTypes() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLType.JLBool, "Julia Type Import Failure");

        [Test]
        public void JuliaFuns() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLFun.LengthF, "Julia Function Import Failure");

        [Test]
        public void JuliaEval() => Assert.AreEqual(4.0, (double) Julia.Eval("2.0 * 2.0"), "Julia Evaluation Failure");

        [Test]
        public void SharpInterface1() => Assert.IsFalse(((JLVal) JLType.SharpType).IsNull, "Julia Type Import Failure");

        [Test]
        public void SharpInterface2() => Assert.DoesNotThrow(() => Julia.Eval("using Main.JuliaInterface"), "Julia Sharp Import Failure");

        [Test]
        public void FunctionParamTest(){
            JLFun fun = Julia.Eval("t(x::Int) = Int32(x)");
            Assert.AreEqual((IntPtr) JLType.JLInt32, (IntPtr) fun.ReturnType, "Julia Function Return Type Failure");
            Assert.AreEqual((IntPtr) JLType.JLInt64, (IntPtr) fun.ParameterTypes[1], "Julia Function Parameter Type Failure");
        }

        [Test]
        public void JuliaPinGC()
        {
            JLArray fun = Julia.Eval("[2, 3, 4]");
            var pinHandle = fun.Pin();
            Assert.AreEqual(1, ObjectCollector.JLObjLen, "Object not pinned!");
            pinHandle.Free();
            Assert.AreEqual(0, ObjectCollector.JLObjLen, "Object not freed!");
        }


        [Test]
        public void Sharp(){
            
            Julia.Eval("using Main.JuliaInterface");
            Assert.IsTrue(Type.GetType("TestJuliaInterface.ReflectionTestClass") != null, "Cannot Load Reflection Class");

            JLVal sharpType = Julia.Eval("sharpyType = SharpType(\"TestJuliaInterface.ReflectionTestClass\")");
            
            Assert.IsFalse(sharpType.IsNull, "Failed to retrieve Sharp type via Reflection");
            JLVal sharpCon = Julia.Eval("sharpyCon = SharpConstructor(sharpyType)");
            
            Assert.IsFalse(sharpCon.IsNull, "Failed to retrieve Sharp Constructor via Reflection");
            JLVal sharpObj = Julia.Eval("sharpObj = sharpyCon()");
            
            Assert.IsFalse(sharpObj.IsNull, "Failed to invoke Sharp Constructor");

            JLVal pinHandle = Julia.Eval("sharpStub = pin(sharpObj)");
            Assert.IsFalse(pinHandle.IsNull, "Failed to Get GC Handle!");
            Assert.AreEqual(1, ObjectCollector.CSharpObjLen, "Object not pinned!");
            
            Julia.Eval("free(sharpStub)");
            Assert.AreEqual(0, ObjectCollector.CSharpObjLen, "Object not released!");
        }

    }

    public class ReflectionTestClass{
        public ReflectionTestClass() { }
    }
}