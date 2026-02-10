using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class PrintVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public override string ToString(int indent) => $"print({VariableName})";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        if (!context.Declarations.IsVariableDeclared(VariableName, ParentScope!))
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (!context.Assignments.IsAssigned(VariableName))
        {
            context.AddIssue(KnownErrors.UseOfUnassignedVariable, this);
        }
    }
}
