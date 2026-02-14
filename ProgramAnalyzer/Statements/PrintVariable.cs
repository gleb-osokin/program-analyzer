using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class PrintVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public override string ToString(int indent) => $"print({VariableName})";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        var lastKnownDeclaration = context.LastVisitedVariableDeclaration ?? ParentScope!.Owner?.PreviousVariableDeclaration;
        if (context.FindVariableDeclaration(VariableName, lastKnownDeclaration) == null)
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (context.IsTraversingFunctionDeclaration)
            return;

        if (!context.IsAssigned(VariableName))
        { 
            context.AddIssue(KnownErrors.UseOfUnassignedVariable, this);
        }
    }
}
