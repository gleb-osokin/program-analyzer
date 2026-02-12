using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class VariableDeclaration(string variableName) : Statement, IDeclaration
{
    public string VariableName { get; } = variableName;

    public bool IsConflict { get; set; }
    public bool IsVisited { get; set; }

    public override string ToString(int indent) => $"var {VariableName}";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        if (context.Declarations.TryAddDeclaration(VariableName, this, ParentScope!))
            return;

        IsConflict = true;
        context.AddIssue(KnownErrors.ConflictDeclaration, this);
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (IsConflict ||
            context.Declarations.TryAddDeclaration(VariableName, this, ParentScope!, setVisited: true))
            return;

        IsConflict = true;
        context.AddIssue(KnownErrors.ConflictDeclaration, this);
    }
}
