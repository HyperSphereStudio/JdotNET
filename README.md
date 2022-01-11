Julia.NET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 

This is a very new library (created a couple days ago) so there is alot of things that can be added / fixed!

Example Usage:

```csharp
  Julia.Init();
  Console.WriteLine(Julia.Eval("2.0 * 2.0").UnboxFloat64());   //Version 0.0.0
  
  
  //Version 0.0.1
  Julia.Eval("t(x) = x * 2");
  Julia.GetFunction(JLModule.Main, "t").Invoke(new JLVal(5)).Println();

  //Version 0.0.2
  JLFun fun = Julia.Eval("t(x) = x * 2");
  fun.Invoke(5).Println();

  //Version 0.0.3
  JLFun fun = Julia.Eval("t(x) = x * 2");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  
  //Version 0.0.4
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  double result = (double) fun.Invoke(2);
  object dotNetObject = fun.Invoke(3).Value;

  Julia.Exit(0);  
```

.NET Interface

The Julia.NET API also has a reverse calling API to call .NET from Julia. This also uses the C interface making it super fast (compared to message protocol based language interop systems. It depends on reflection which is the factor that slows it down compared to normal C# code).

Lets say we have the following C# class:
```csharp
public class TestClass{
    public long g;
    public TestClass(long g) => this.g = g;
}
```

We can then in Julia use this class with the following code:
```julia
sharpType = SharpType("TestClass")  #Create Type
sharpCon = SharpConstructor(sharpType, 0)  #Get Constructor
o = sharpCon(6) #Create Instance
sharpField = SharpField(sharpType, "g") #Get Field
println(sharpField(o)) #Get Field Value
```
