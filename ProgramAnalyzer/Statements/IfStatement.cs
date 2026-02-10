using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class IfStatement : Statement
{
    public required Statement ThenBody { get; init; }

    public override string ToString(int indent) => "if (...) " + ThenBody.ToString(indent);

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode == PassMode.AnalyzeCallStack)
        {
            context.Assignments.PushScope(context.Position);
        }

        context.AnalyzeStack.Push(IfStatementTerminator.Instance);
        context.AnalyzeStack.Push(ThenBody);
    }
}
