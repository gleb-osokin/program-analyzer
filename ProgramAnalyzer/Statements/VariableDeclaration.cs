using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class VariableDeclaration(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public bool IsAssignable { get; set; }

    public override string ToString(int indent) => $"var {VariableName}";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        IsAssignable = ParentIfStatement == null;
        var isConflict = IsAssignable
            ? !context.Declarations.TryAddDeclaration(VariableName, this)
            : context.Declarations.IsVariableDeclared(VariableName, ParentScope!);

        if (isConflict)
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        // do nothing
    }
}
