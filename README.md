Julia.NET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 

This is a very new library (created a couple days ago) so there is alot of things that can be added / fixed!


Evaluation:
```csharp
Julia.Init();
int v = (int) Julia.Eval("2 * 2");
Julia.Exit(0);
```

Function Handling:
```csharp
  Julia.Init();
  JLFun fun = Julia.Eval("t(x::Int) = Int32(x * 2)");
  JLSvec ParameterTypes = fun.ParameterTypes;
  JLType willbeInt64 = fun.ParameterTypes[1];
  JLType willBeInt32 = fun.ReturnType;
  
  int resultWillBe4 = (int) fun.Invoke(2);
  object willReturnNetBoxed4 = fun.Invoke(3).Value;
```

Exception Handling:
```csharp
  Julia.Init();
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  Julia.Exit(0);  
```



.NET Interface

The Julia.NET API also has a reverse calling API to call .NET from Julia. This also uses the C interface making it super fast (compared to message protocol based language interop systems. It depends on reflection which is the factor that slows it down compared to normal C# code).

WARNING: THIS FEATURE IS VERY EXPERIMENTAL AT THE MOMENT

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
sharpCon = SharpConstructor(sharpType, 0)  #Get Constructor at Index 0
o = sharpCon(6) #Create Instance
sharpField = SharpField(sharpType, "g") #Get Field
println(sharpField(o)) #Get Field Value
```
