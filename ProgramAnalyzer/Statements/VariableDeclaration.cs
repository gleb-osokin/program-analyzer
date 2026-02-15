using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class VariableDeclaration(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public bool IsConflict { get; set; }
    public VariableDeclaration? OriginalDeclaration { get; set; } // in case we're the conflicting one
    public VariableDeclaration? PreviousDeclaration { get; set; }

    public override string ToString(int indent) => $"var {VariableName}";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        // guarantee declarations chain from here to the top
        PreviousDeclaration = context.LastVisitedVariableDeclaration ?? ParentScope!.Owner?.PreviousVariableDeclaration;

        OriginalDeclaration = context.FindVariableDeclaration(VariableName, PreviousDeclaration);
        if (OriginalDeclaration != null &&
            OriginalDeclaration.ParentIfStatement == null || // vars in if statements don't affect anything below them
            context.FindFunctionDeclaration(VariableName, this) != null)
        {
            IsConflict = true;
        }

        context.LastVisitedVariableDeclaration = this;
        if (IsConflict)
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        // functions from the same scope below might still be conflicting
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        // do nothing
    }
}
