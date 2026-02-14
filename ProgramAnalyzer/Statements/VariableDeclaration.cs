using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class VariableDeclaration(string variableName) : Statement, IDeclaration
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

        if (context.FindVariableDeclaration(VariableName, PreviousDeclaration) is { } conflictingVarDeclaration &&
            conflictingVarDeclaration.ParentIfStatement == null) // vars in if statements don't affect anything below them
        {
            // conflicting variable declaration can only be ABOVE this one
            IsConflict = true;
            OriginalDeclaration = conflictingVarDeclaration.OriginalDeclaration ?? conflictingVarDeclaration;
        }

        if (context.FindFunctionDeclaration(VariableName, this, out var isBelow) is { } conflictingFunc)
        {
            // conflicting function declaration can only be ABOVE or BELOW this one
            IDeclaration conflictingStatement = isBelow
                ? conflictingFunc
                : this;
            conflictingStatement.IsConflict = true;
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
