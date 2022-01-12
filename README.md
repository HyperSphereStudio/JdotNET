Julia.NET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 

This is a very new library (created a couple days ago) so there is alot of things that can be added / fixed!


Evaluation:
```csharp
Julia.Init();
int v = (int) Julia.Eval("2 * 2");
Julia.Exit(0);
```

Struct Handling:
```csharp

   #You have two choices, allocate a struct or create a struct.
   #Allocating directly sets the memory, creating will call a constructor of the struct
   
   var myAllocatedStruct = Julia.AllocStruct(JLType.JLRef, 3);   //Will throw error
   var myCreatedStuct = JLType.JLRef.Create(3);   //Will call constructor
```

Function Handling:
```csharp
  Julia.Init();
  JLFun fun = Julia.Eval("t(x::Int) = Int32(x * 2)");
  JLSvec ParameterTypes = fun.ParameterTypes;
  JLType willbeInt64 = fun.ParameterTypes[1];
  JLType willBeInt32 = fun.ReturnType;
  
  JLVal resultWillBe4 = fun.Invoke(2);
  Julia.Exit(0);
```

Value Handling:
```csharp
   //Auto alloc to Julia
   var val = new JLVal(3);

   //Manual Type Unboxing
   long netVal = val.UnboxInt64();
   
   //Auto Unboxing
   object newVal2 = val.Value;
```

Array Handling:
```csharp
   Julia.Init();
   JLArray arr = Julia.Eval("[2, 3, 4]")
   
   //Unpack to .net
   object[] o = arr.LinearNetUnPack();
   
   //Make own array
   var newArray = long[arr.Length];
   for(int i = 1; i < arr.Length; ++i)
       newArray[i - 1] = (long) arr[i];
   
   JLType elementType = arr.ElType;
   
   Julia.Exit(0); 
```

Exception Handling:
```csharp
  Julia.Init();
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  Julia.Exit(0);  
```


Garbage Collection:
You are (at the current moment of this project) responsible for ensuring object safety on both .NET and Julia. When you make calls to either language, the GC could activate and invalidate the reference you hold in the other language unless you pin it!

CSharp Garbage Collector Pinning:
```csharp
  Julia.Init();
  JLArray myArr = new JLArray(JLType.Int64, 5);  //Allocate Int64 array of length 5
  
  var handle = myArr.Pin();    //Pin the Object 
  
  //Stuff calling Julia Functions
  
  handle.Free();   //Optional, handle destructor will auto call it. This is in case you want it freed earlier
  
  Julia.Exit(0);
```
Keep In mind that there is another way to pin Julia objects using Julia.PUSHGC() and Julia.POPGC(). (The Julian way)


Julia Garbage Collector Pinning:
```julia
   myBasicType = SharpType("MyBasicType")
   myBasicCon = SharpConstructor(myBasicType)
   myBasicObj = myBasicCon()
   handle = pin(myBasicObj)
   
   #Stuff calling Sharp Functions
   
   free(handle) #Will also auto free. You can also treat it like stream and put it in do end block
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



Library Written by Johnathan Bizzano
