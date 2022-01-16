using JuliaInterface;
using NUnit.Framework;
using System;
using System.Linq;

namespace TestJuliaInterface
{
    public class Initialization{
        [OneTimeTearDown]
        public void Finish() => Julia.Exit(0);

        [OneTimeSetUp]
        public void Setup() => Julia.Init();

        [Test]
        public void JuliaTypes() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLType.JLBool, "Julia Type Import Failure");

        [Test]
        public void JuliaFuns() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLFun.LengthF, "Julia Function Import Failure");

        [Test]
        public void JuliaEval() => Assert.AreEqual(4.0, (double)Julia.Eval("2.0 * 2.0"), "Julia Evaluation Failure");
    }

    public class JuliaTest
    {
        [OneTimeTearDown]
        public void Finish() => Julia.Exit(0);

        [OneTimeSetUp]
        public void Setup() => Julia.Init();


        [Test]
        public void FunctionParamTest()
        {
            JLFun fun = Julia.Eval("t(x::Int) = Int32(x)");
            Assert.AreEqual((IntPtr)JLType.JLInt32, (IntPtr)fun.ReturnType, "Julia Function Return Type Failure");
            Assert.AreEqual((IntPtr)JLType.JLInt64, (IntPtr)fun.ParameterTypes[1], "Julia Function Parameter Type Failure");
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
        public void CopyArray()
        {
            JLArray arr = Julia.Eval("[2, 3, 4]");
            Assert.IsTrue(Enumerable.SequenceEqual((long[]) arr.UnboxInt64Array(), new long[] {2, 3, 4}), "Int Array copy failed");
            arr = Julia.Eval("[2.0, 3.0, 4.0]");
            Assert.IsTrue(Enumerable.SequenceEqual((double[]) arr.UnboxFloat64Array(), new double[] { 2, 3, 4 }), "Double Array copy failed");
        }

    }


    [SingleThreaded]
    public class SharpTest
    {
        
        [OneTimeTearDown]
        public void Finish() => Julia.Exit(0);

        [OneTimeSetUp]
        public void Setup()
        {
            Julia.Init();
            Julia.Eval("using Main.JuliaInterface");
        }

        [Test]
        public void SharpType() => Assert.IsFalse(((JLVal) JLType.SharpType).IsNull, "Julia Type Import Failure");

        [Test]
        public void SharpUsing() => Assert.DoesNotThrow(() => Julia.Eval("using Main.JuliaInterface"), "Julia Sharp Import Failure");


        [Test]
        public void Construction(){
            Assert.IsFalse(Julia.Eval(@"
                        item = T""TestJuliaInterface.ReflectionTestClass"".new[](3)
                        return item
                        ").IsNull, "Object instantiation failure");

            Assert.IsFalse(Julia.Eval(@"
                        itemG = T""TestJuliaInterface.ReflectionGenericTestClass`1"".new[T""System.Int64""](3)
                        return itemG
                        ").IsNull, "Generic Object instantiation failure");
        }

        [Test]
        public void GetField(){
            if (!Julia.Eval("@isdefined itemG").UnboxBool())
                Construction();
            Assert.AreEqual(3, Julia.Eval("itemG.g[]").Value, "Failed to Get Field");
            Assert.AreEqual(5, Julia.Eval(@"T""TestJuliaInterface.ReflectionTestClass"".TestStaticField[]").Value);
        }

        [Test]
        public void Method()
        {
            Assert.AreEqual(5, Julia.Eval(@"T""TestJuliaInterface.ReflectionTestClass"".StaticMethod[]()").Value);
            //Assert.AreEqual(3, Julia.Eval(@"T""TestJuliaInterface.ReflectionTestClass"".StaticGenericMethod[T""System.Int64""]()").Value);
        }

        [Test]
        public void BoxingTest(){
            Assert.IsTrue((long) Julia.Eval("sharpbox(5)").Value == 5, "Boxing Failed");
            //Assert.AreEqual(Julia.Eval("sharpunbox(T""TestJuliaInterface.ReflectionTestClass"".TestStaticField)"), 5, "Unboxing Failed")
        }

        [Test]
        public void UsingTest()
        {
            Julia.Eval("@netusing System");
            Julia.Eval(@"T""Int64""");
        }

        [Test]
        public void TypeMacros(){
            Julia.Eval(@"P""System.Int64""");
            Assert.AreEqual(1, Julia.Eval("length(Main.JuliaInterface.TypeMap)").UnboxInt64(), "Did not push to type map");
            Julia.Eval(@"G""System.Int64""");
            Julia.Eval(@"R""System.Int64""");
            Assert.AreEqual(0, Julia.Eval("length(Main.JuliaInterface.TypeMap)").UnboxInt64(), "Did not push to type map");
        }

        [Test]
        public void SharpGC()
        {
            if (!Julia.Eval("@isdefined itemG").UnboxBool())
                Construction();
            Julia.Eval("handle = pin(itemG)");
            Assert.AreEqual(1, ObjectCollector.CSharpObjLen, "Unable to pin object");
            Julia.Eval("free(handle)");
            Assert.AreEqual(0, ObjectCollector.CSharpObjLen, "Unable to free object");
        }
    }

    public class ReflectionTestClass{
        public long g;
        public static int TestStaticField = 5;
        public ReflectionTestClass(long g) { this.g = g; }
        public long InstanceMethod() => 5;
        public static long StaticMethod() => 5;
        public static long StaticGenericMethod<T>() => 3;
    }

    public class ReflectionGenericTestClass<T>
    {
        public T g;
        public ReflectionGenericTestClass(T g) { this.g = g; }
    }
}