using BenchmarkDotNet.Attributes;
using ProgramAnalyzer.Analysis;
using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.PerformanceTests;

public class WideProgramBenchmarks
{
    [Params(10, 100, 1000, 10_000)]
    public int Length;

    private readonly ProgramBlock _program = [];
    private readonly Analyzer _analyzer = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var func = new FunctionDeclaration("a");
        _program.Add(func);

        for (var i = 0; i < Length; i++)
        {
            var name = "a" + Length;
            func.Add(new FunctionDeclaration(name));
            func.Add(new Invocation(name));
            func = (FunctionDeclaration)func.Body[0];
        }

        _program.Add(new Invocation("a"));
    }

    [Benchmark]
    public void Analyze() => _analyzer.Analyze(_program);
}
