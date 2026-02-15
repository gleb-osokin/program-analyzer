using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class IfStatement : Statement
{
    public required Statement ThenBody { get; init; }

    public long LastVisitedPosition { get; set; }

    public override string ToString(int indent) => "if (...) " + ThenBody.ToString(indent);

    public override bool HasNestedScope(AnalyzerContext context) => true;

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        ThenBody.ParentScope = ParentScope;
        ThenBody.ParentIfStatement = this;

        context.Queue.Enqueue(ThenBody);
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (!context.IsTraversingFunctionDeclaration)
        {
            LastVisitedPosition = context.Position++;
        }

        context.Stack.Push(ThenBody);
    }

    public override void OnCallStackExit(AnalyzerContext context)
    {
        if (context.IsTraversingFunctionDeclaration)
            return;

        var assignment = context.LastKnownAssignment;
        while (assignment != null && assignment.LastVisitedPosition > LastVisitedPosition)
        {
            assignment = assignment.PreviousAssignment;
        }

        context.LastKnownAssignment = assignment;
    }
}
