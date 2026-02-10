using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using ProgramAnalyzer.PerformanceTests;

var config = ManualConfig
    .CreateMinimumViable()
    .AddDiagnoser(MemoryDiagnoser.Default)
    .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond))
    .AddExporter(MarkdownExporter.GitHub);

BenchmarkRunner.Run<LengthyProgramBenchmarks>(config);
BenchmarkRunner.Run<WideProgramBenchmarks>(config);