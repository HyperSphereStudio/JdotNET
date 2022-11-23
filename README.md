# JdotNET

JdotNET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 


##Julia Interface from C#

### Launching Julia
```csharp
JuliaOptions options = new JuliaOptions();
options.ThreadCount = 4;
Julia.Init(options);
```

### Evaluation
```csharp
Julia.Init();
int v = (int) Julia.Eval("2 * 2");
Julia.Exit(0); //Even if your program terminates after you should call this. It runs the finalizers and stuff 
```

### Structs
```csharp

   #You have two choices, allocate a struct or create a struct.
   #Allocating directly sets the memory, creating will call a constructor of the struct
   
   var myAllocatedStruct = Julia.AllocStruct(JLType.JLRef, 3);   //Will throw error
   var myCreatedStuct = JLType.JLRef.Create(3);   //Will call constructor
```

### Functions
```csharp
  JLFun fun = Julia.Eval("t(x::Int) = Int32(x * 2)");
  JLSvec ParameterTypes = fun.ParameterTypes;
  JLType willbeInt64 = fun.ParameterTypes[1];
  JLType willBeInt32 = fun.ReturnType;
  
  JLVal resultWillBe4 = fun.Invoke(2);
```

### Values
```csharp
   //Auto alloc to Julia
   var val = new JLVal(3);

   //Manual Type Unboxing
   long netVal = val.UnboxInt64();
   
   //Auto Unboxing
   object newVal2 = val.Value;
```

### Arrays
```csharp
   JLArray arr = Julia.Eval("[2, 3, 4]")
   
   //Unpack to .net
   object[] o = arr.UnboxArray();
   
   var a = new int[]{2, 3, 4};
   
   //Copy to a Julia Array. Dont use this method if you know an object is an array though. There are faster methods!
   var v = new JLVal(a);
   
   //Fast Array Copy From .NET. This will deal with direct memory transfer rather then boxing/unboxing for unmanaged types
   var v2 = JLArray.CreateArray(a);
   
   //Fast Array Copy From Julia. This will deal with direct memory transfer rather then boxing/unboxing for unmanaged types
   int[] v2 = v2.UnboxArray<int>();
   
   JLType elementType = arr.ElType;
```

### Exception Handling
```csharp
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  Julia.Exit(0);  
```


### Garbage Collection
You are (at the current moment of this project) responsible for ensuring object safety on both .NET and Julia. When you make calls to either language, the GC could activate and invalidate the reference you hold in the other language unless you pin it!

There are two forms of Garbage Collector Pinning: Static & Stack.

Static pinning is meant for objects with a long life span (could exist forever).
Stack pinning is meant for objects with a short life span.

### CSharp Static Garbage Collector Pinning
```csharp
  JLArray myArr = new JLArray(JLType.Int64, 5);  //Allocate Int64 array of length 5
  
  var handle = myArr.Pin();    //Pin the Object 
  
  //Stuff calling Julia Functions
  
  handle.Free();   //Optional, handle destructor will auto call it. This is in case you want it freed earlier
```

### CSharp Stack Garbage Collector Pinning
```csharp
    JLVal v = Julia.Eval("2 * 2");
    JLVal v2 = Julia.Eval("Hi");
    Julia.GC_PUSH(v, v2);

    //Do Stuff with v without it being collected

    Julia.GC_POP();    
```

## C# from Julia

The Julia.NET API also has a reverse calling API to call .NET from Julia. This also uses the C interface making it super fast (compared to message protocol based language interop systems. It depends on reflection which is the factor that slows it down compared to normal C# code). Drew ideas of syntax from https://github.com/azurefx/DotNET.jl

### Launching C# from Julia
```julia
using JULIAdotNET
using JULIAdotNET.JuliaInterface

handle = Init() #Keep handle alive as long as you want .NET to be alive

sharpList = T"System.Collections.Generic.List`1".new[T"System.Int64"]()
```

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

### Accessing Sharp Types From Julia
The Sharp Type object allows one to access .NET class fields, methods and constructors from julia
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

### Field Invokation
```julia
   @netusing Test
   shouldBe5 = T"ReflectionTestClass".TestStateField[]   #< Requires [] to actually get the field. If you dont put [] or () then it will just return the FieldInfo object
   
   T"ReflectionTestClass".TestStateField[] = 3 #To Set a Field. An error will occur if you dont put [].
```

### Method Invokation
```julia
   @netusing Test
   @netusing System
   shouldBe5 = T"ReflectionTestClass".TestStateField[]   #< Requires [] to actually get the field. If you dont put [] or () then it will just return the FieldInfo object
   shouldBe5 = T"ReflectionTestClass".StaticMethod[]() #To call a method. If you dont put [] or () then it will just return the MethodInfo object
   shouldBe3 = T"ReflectionTestClass".StaticGenericMethod[T"Int64"]() "To call a generic method, put the generic types in []
```

### Constructor Invokation
```julia
   @netusing Test
   @netusing System
   item = T""TestJuliaInterface.ReflectionTestClass"".new[](3)     #To call a constructor.  If you dont put [] or () then it will just return the ConstructorInfo object
   shouldBe5 = item.InstanceMethod[]();   #To call a instance method
   
   shouldBe3 = item.g[]       #To Access a instance field
   
   myGenericItem = T"TestJuliaInterface.ReflectionGenericTestClass`1".new[T"System.Int64"](3)    #To Create a generic instance of an object, put the generic types in [].
   
```

### Boxing/Unboxing
Boxing/Unboxing is converting a julia value from/to a sharp value from julia
```julia
   boxed5 = sharpbox(5)   #Will return the sharp object of the long value "5"
   shouldBe5 = shapunbox(boxed5) #Will unbox the sharp object and return to native julia value
```

### Julia Garbage Collector Pinning
```julia
   handle = pin(sharpbox(5))
   #Stuff calling Sharp Functions
   free(handle) #Will also auto free. You can also treat it like stream and put it in do end block
```

## Exposing custom functions as native functions to julia
### From C#
```csharp
   public unsafe delegate IntPtr JuliaNativeInterface(IntPtr* data);
   
   //Example from NativeSharp.cs that we want to expose to julia
   public static JLVal /*Use this to manipulate julia objects to/from julia*/ GetMethod(NativeObject<Type> /*Use this to manipulate sharp objects to/from julia*/ t, NativeString /*Transfer strings*/ name) => GetMethod(t.Value, name.Value);
   
   //Register the method. You must increment the pointer for each argument left to right
   NativeSharp.RegisterSharpFunction("GetMethodByName", data => GetMethod(data++, data));
```
### From julia
```julia
   //Generate the function and insert it into current module
                  Function Name     Function Arguments Exposed to Julia       Convert to Native Objects
   @sharpfunction(GetMethodByName, (type::SharpType, name::AbstractString), (NativeObject(type), NativeString(name)))
   
   //Using the function
   someRandomType::SharpType
   sharpMethod = GetMethodByName(someRandomType, "MyMethod")
```


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Author
Library Written by Johnathan Bizzano
