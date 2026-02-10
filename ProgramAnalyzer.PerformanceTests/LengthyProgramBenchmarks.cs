using BenchmarkDotNet.Attributes;
using ProgramAnalyzer.Analysis;
using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.PerformanceTests;

public class LengthyProgramBenchmarks
{
    [Params(10, 100, 1000, 100_000, 1_000_000)]
    public int Length;

    private readonly ProgramBlock _program = [];
    private readonly Analyzer _analyzer = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        for (var i = 0; i < Length; i++)
        {
            var name = "a" + Length;
            _program.Add(new VariableDeclaration(name));
            _program.Add(new AssignVariable(name));
            _program.Add(new FunctionDeclaration(name));
            _program.Add(new Invocation(name));
        }
    }

    [Benchmark]
    public void Analyze() => _analyzer.Analyze(_program);
}
