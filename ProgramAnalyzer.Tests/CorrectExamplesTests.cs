using ProgramAnalyzer.Analysis;
using Shouldly;

namespace ProgramAnalyzer.Tests;

public class CorrectExamplesTests
{
    [Fact]
    public void Analyze_SimpleVariableUsage_NoIssues()
    {
        var result = new Analyzer().Analyze(CorrectExamples.SimpleVariableUsage);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Analyze_LocalFunctionInit_NoIssues()
    {
        var result = new Analyzer().Analyze(CorrectExamples.LocalFunctionInit);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Analyze_EmptyFunction_NoIssues()
    {
        var result = new Analyzer().Analyze(CorrectExamples.EmptyFunction);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Analyze_NestedFunctions_NoIssues()
    {
        var result = new Analyzer().Analyze(CorrectExamples.NestedFunctions);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Analyze_CyclicRecursiveInvocations_NoIssues()
    {
        var result = new Analyzer().Analyze(CorrectExamples.CyclicRecursiveInvocations);
        result.ShouldBeEmpty();
    }
}
