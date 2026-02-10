using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class ProgramBlockTerminator : Statement, ITerminator
{
    private ProgramBlockTerminator()
    {
    }

    public static ProgramBlockTerminator Instance { get; } = new ProgramBlockTerminator();

    public override string ToString(int indent) => string.Empty;

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        context.BlocksStack.Pop();

        if (mode == PassMode.CollectDeclarations)
        {
            context.Declarations.PopScope();
        }
    }
}
