```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-10750H CPU 2.60GHz (Max: 2.59GHz), 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.310
  [Host]     : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method  | Length | Mean        | Error     | StdDev    | Median      | Allocated |
|-------- |------- |------------:|----------:|----------:|------------:|----------:|
| **Analyze** | **10**     |   **0.0109 ms** | **0.0002 ms** | **0.0002 ms** |   **0.0109 ms** |     **176 B** |
| **Analyze** | **100**    |   **0.1080 ms** | **0.0003 ms** | **0.0002 ms** |   **0.1080 ms** |     **176 B** |
| **Analyze** | **1000**   |   **2.1251 ms** | **0.0576 ms** | **0.1497 ms** |   **2.0734 ms** |     **176 B** |
| **Analyze** | **10000**  | **310.6461 ms** | **4.2007 ms** | **3.9293 ms** | **309.0133 ms** |     **176 B** |
