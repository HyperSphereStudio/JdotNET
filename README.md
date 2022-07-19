Julia.NET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 

Launching Julia from C#
```csharp
JuliaOptions options = new JuliaOptions();
options.ThreadCount = 4;
Julia.Init(options);
```

Launching C# from Julia
```julia
using JULIAdotNET
using JULIAdotNET.JuliaInterface

handle = Init() #Keep handle alive as long as you want .NET to be alive

sharpList = T"System.Collections.Generic.List`1".new[T"System.Int64"]()
```


Evaluation:
```csharp
Julia.Init();
int v = (int) Julia.Eval("2 * 2");
Julia.Exit(0); //Even if your program terminates after you should call this. It runs the finalizers and stuff 
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
  JLFun fun = Julia.Eval("t(x::Int) = Int32(x * 2)");
  JLSvec ParameterTypes = fun.ParameterTypes;
  JLType willbeInt64 = fun.ParameterTypes[1];
  JLType willBeInt32 = fun.ReturnType;
  
  JLVal resultWillBe4 = fun.Invoke(2);
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
   JLArray arr = Julia.Eval("[2, 3, 4]")
   
   //Unpack to .net
   object[] o = arr.LinearNetUnPack();
   
   //Make own array
   var newArray = long[arr.Length];
   for(int i = 1; i <= arr.Length; ++i)
       newArray[i - 1] = (long) arr[i];
   
   JLType elementType = arr.ElType;
```

Exception Handling:
```csharp
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  Julia.Exit(0);  
```


Garbage Collection:
You are (at the current moment of this project) responsible for ensuring object safety on both .NET and Julia. When you make calls to either language, the GC could activate and invalidate the reference you hold in the other language unless you pin it!

There are two forms of Garbage Collector Pinning: Static & Stack.

Static pinning is meant for objects with a long life span (could exist forever).
Stack pinning is meant for objects with a short life span.

CSharp Static Garbage Collector Pinning:
```csharp
  JLArray myArr = new JLArray(JLType.Int64, 5);  //Allocate Int64 array of length 5
  
  var handle = myArr.Pin();    //Pin the Object 
  
  //Stuff calling Julia Functions
  
  handle.Free();   //Optional, handle destructor will auto call it. This is in case you want it freed earlier
```

CSharp Stack Garbage Collector Pinning:
```csharp
    JLVal v = Julia.Eval("2 * 2");
    JLVal v2 = Julia.Eval("Hi");
    Julia.GC_PUSH(v, v2);

    //Do Stuff with v without it being collected

    Julia.GC_POP();    
```

Julia Garbage Collector Pinning:
```julia
   handle = pin(sharpbox(5))
   #Stuff calling Sharp Functions
   free(handle) #Will also auto free. You can also treat it like stream and put it in do end block
```


.NET Interface

The Julia.NET API also has a reverse calling API to call .NET from Julia. This also uses the C interface making it super fast (compared to message protocol based language interop systems. It depends on reflection which is the factor that slows it down compared to normal C# code).

Lets say we have the following C# classes:
```csharp
namespace Test{
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
```

The Sharp Type object allows one to access .NET class fields, methods and constructors from julia
Accessing Sharp Types From Julia:
```julia
   myClass = T"Test.ReflectionTestClass"   #<= Perform Assembly Search and Return the Sharp Type
   
   myClass2 = P"Test.ReflectionTestClass"   #<= Perform one time assembly search and store the sharp type in a internal array (Reccommended for fast lookups)
   
   myClass3 = G"Test.ReflectionTestClass"   #Get from internal array
   
   myClass4 = R"Test.ReflectionTestClass"   #Remove from internal array
```


The using statement From Julia enables a user to shorten the length of a type name required
```julia
   myClass1 = T"System.Int64"   #<= Will Work But It is long to type
   myClass2 = T"Int64"   #<= Will Fail
   @netusing System
   myClass3 = T"Int64"   #<= Will Work
```

Field Invokation:
```julia
   @netusing Test
   shouldBe5 = T"ReflectionTestClass".TestStateField[]   #< Requires [] to actually get the field. If you dont put [] or () then it will just return the FieldInfo object
   
   T"ReflectionTestClass".TestStateField[] = 3 #To Set a Field. An error will occur if you dont put [].
```

Method Invokation:
```julia
   @netusing Test
   @netusing System
   shouldBe5 = T"ReflectionTestClass".TestStateField[]   #< Requires [] to actually get the field. If you dont put [] or () then it will just return the FieldInfo object
   shouldBe5 = T"ReflectionTestClass".StaticMethod[]() #To call a method. If you dont put [] or () then it will just return the MethodInfo object
   shouldBe3 = T"ReflectionTestClass".StaticGenericMethod[T"Int64"]() "To call a generic method, put the generic types in []
```

Constructor Invokation:
```julia
   @netusing Test
   @netusing System
   item = T""TestJuliaInterface.ReflectionTestClass"".new[](3)     #To call a constructor.  If you dont put [] or () then it will just return the ConstructorInfo object
   shouldBe5 = item.InstanceMethod[]();   #To call a instance method
   
   shouldBe3 = item.g[]       #To Access a instance field
   
   myGenericItem = T"TestJuliaInterface.ReflectionGenericTestClass`1".new[T"System.Int64"](3)    #To Create a generic instance of an object, put the generic types in [].
   
```

Boxing/Unboxing is converting a julia value from/to a sharp value from julia:
```julia
   boxed5 = sharpbox(5)   #Will return the sharp object of the long value "5"
   shouldBe5 = shapunbox(boxed5) #Will unbox the sharp object and return to native julia value
```


Library Written by Johnathan Bizzano
