using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class IfStatementTerminator : Statement, ITerminator
{
    private IfStatementTerminator()
    {
    }

    public static IfStatementTerminator Instance { get; } = new IfStatementTerminator();

    public override string ToString(int indent) => string.Empty;

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode == PassMode.AnalyzeCallStack)
        {
            context.Assignments.PopScope();
        }
    }
}
