# JdotNET

JdotNET is an API designed to go between .NET and the Julia Language. It utilizes C Intefaces of both languages to allow super efficient transfers between languages (type conversion overhead is expected in using nonspecialized (Any types for instance) functions). I removed the API that allowed Julia launch & interact with .NET. It was hard to maintain and I don't think it was used much compared to the other way around. I may add it back in the future.


## Static Library Generator
**In Development**
This project generates a .NET dll that statically creates a snapshot of a julia Module and all its submodules and allows for static analysis from csharp. This library will use the most efficient techniques (caching constants, specializing functions and using cfunctions when it can) to provide for the most efficient calls to julia.

Example using Base.dll:
```csharp
   using Base;
   
   Base.println("My Function");   //This function will be statically available
```

## Dynamics
**In Development**
Some code may be dynamically generated in julia and thus cannot be represented using the static library generator. This project will provide an abstract layer to the underlying JdotNET net-julia interface to provide more seamless usage.

```csharp
   dynamic Base = JPrimitive.BaseM;
   Base.println("My Function");  //JdotNET will dynamically resolve and cache this function. It is extremely recommended to locally cache functions outside loops.
```

##JdotNet Julia-Net Interface#

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

### Any
The Any class represents a boxed Julia value. There is many built in default conversions from native NET types to the Any type which can be utilized via the Any constructor.

```csharp
   Julia.Init();
   var v25 = new Any(5) * 5; //The second 5 will be auto converted to Any then the operator '*' will be invoked on both arguments
   
   //Arrays are passed by pointer value so there is no copying cost. Keep in mind that if julia mutates the array, it will also be affected in c#
   var myNetArray = new []{1, 2, 3, 4, 5};
   var myJuliaArray = new Any(myNetArray);
   Julia.Eval("f(x) = x .* 2").Invoke(myJuliaArray);
   //myNetArray will now be {2, 4, 6, 8, 10}
   
   //GC
   Julia.GC_PUSH(myJuliaArray); //Will not be GC'd
   Julia.GC_POP();              //Can now be GC'd
    
   //Search for a Julia Module lets say its called Main.MyModule:
   var myModule = JPrimitive.MainM.GetGlobal("MyModule");
   
   //Search for Function add! in MyModule
   var add = myModule.GetFunction("add!");
   
   //You have several choices of invoking add
   
   //The first is using Invoke (SLOWEST). This is the safest way to invoke as it provides features like exception handling
   add.Invoke(2, 2);
   
   //The second is using Unsafe Invoke. This is unsafe and should only be used in time critcal code that is stable.
   add.UnsafeInvoke(2, 2);
   
   //Native Pointer of Specialized Methods. (FASTEST) This is unsafe so you should periodically check for exceptions
    var f = Julia.Eval(@"add(a, b) = a + b");   //Gets the julia function pointer add
    
    public delegate int Add(int a, int b); /*Declared somewhere*/
    
    Add add = f.GetDelegate<Add>();             //Compile f to a sharp function pointer Add
    var aAdd = f.GetDelegate<int, int, int>();  //Compile f to a sharp anonymous function pointer. Does not require a declared delegate
    
    int answer = add(3, 4);
    int answer2 = aAdd(4, 5);
   Julia.CheckExceptions();
   
   Julia.Exit(0);
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Author
Library Written by Johnathan Bizzano
