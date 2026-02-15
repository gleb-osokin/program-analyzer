```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-10750H CPU 2.60GHz (Max: 2.59GHz), 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.310
  [Host]     : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method  | Length | Mean        | Error     | StdDev    | Median      | Allocated  |
|-------- |------- |------------:|----------:|----------:|------------:|-----------:|
| **Analyze** | **10**     |   **0.0209 ms** | **0.0004 ms** | **0.0004 ms** |   **0.0210 ms** |    **5.51 KB** |
| **Analyze** | **100**    |   **0.2495 ms** | **0.0025 ms** | **0.0025 ms** |   **0.2492 ms** |   **44.13 KB** |
| **Analyze** | **1000**   |   **6.2052 ms** | **0.1150 ms** | **0.2928 ms** |   **6.1154 ms** |  **415.14 KB** |
| **Analyze** | **10000**  | **579.0655 ms** | **5.9090 ms** | **5.5273 ms** | **577.8477 ms** | **4445.44 KB** |
