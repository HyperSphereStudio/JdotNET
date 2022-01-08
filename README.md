# JULIA.Net

Allows C# To Call Julia

Example USage:

```csharp
  Julia.Init();
  Console.WriteLine(Julia.Eval("2.0 * 2.0").UnboxFloat64());
  Julia.Exit(0);
```


WARNING: IN DEEP DEVELOPMENT AT THE MOMENT
