using JuliaInterface;
using NUnit.Framework;
using System;

namespace TestJuliaInterface
{
    public class Tests
    {
        [NUnit.Framework.OneTimeTearDown]
        public void Finish() => Julia.Exit(0);

        [SetUp]
        public void Setup() => Julia.Init();

        [Test]
        public void JuliaTypes() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLType.JLBool, "Julia Type Import Failure");

        [Test]
        public void JuliaFuns() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLFun.LengthF, "Julia Function Import Failure");

        [Test]
        public void JuliaEval() => Assert.AreEqual(4.0, (double) Julia.Eval("2.0 * 2.0"), "Julia Evaluation Failure");


        [Test]
        public void SharpInterface1() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLType.SharpType, "Julia Type Import Failure");

        [Test]
        public void SharpInterface2() => Assert.DoesNotThrow(() => Julia.Eval("using Main.JuliaInterface"), "Julia Sharp Import Failure");
    }
}