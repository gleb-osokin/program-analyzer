using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class IfStatement : Statement
{
    public required Statement ThenBody { get; init; }

    public long CallStackPosition { get; set; }

    public override string ToString(int indent) => "if (...) " + ThenBody.ToString(indent);

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        ThenBody.ParentScope = ParentScope;
        ThenBody.ParentIfStatement = this;
        context.Queue.Enqueue(ThenBody);
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        CallStackPosition = context.Position;
        context.Assignments.PushScope(this);
        context.Stack.Push(ThenBody);
    }
}
