using BenchmarkDotNet.Attributes;
using ProgramAnalyzer.Analysis;
using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.PerformanceTests;

public class WideProgramBenchmarks
{
    [Params(10, 100, 1000, 10_000)]
    public int Length;

    private ProgramBlock? _program;
    private readonly Analyzer _analyzer = new();

    [IterationSetup]
    public void Setup()
    {
        _program =
        [
            new FunctionDeclaration("a"),
            new Invocation("a")
        ];

        var func = (FunctionDeclaration)_program[0];

        for (var i = 0; i < Length; i++)
        {
            var name = "a" + i;
            func.Add(new FunctionDeclaration(name));
            func.Add(new Invocation(name));
            func = (FunctionDeclaration)func.Body[0];
        }
    }

    [Benchmark]
    public void Analyze() => _analyzer.Analyze(_program!);
}
