using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class AssignVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public VariableDeclaration? Declaration { get; set; }
    public AssignVariable? PreviousAssignment { get; set; }
    public long LastVisitedPosition { get; set; } // position of the first assignment in the call stack

    public override string ToString(int indent) => $"{VariableName} = ...";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        var lastKnownDeclaration = context.LastVisitedVariableDeclaration ?? ParentScope!.Owner?.PreviousVariableDeclaration;
        Declaration = context.FindVariableDeclaration(VariableName, lastKnownDeclaration);
        
        if (Declaration == null)
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (context.IsTraversingFunctionDeclaration ||
            ParentIfStatement != null)
            return;

        LastVisitedPosition = context.Position++;
        PreviousAssignment = context.LastKnownAssignment;
        context.LastKnownAssignment = this;
    }
}
