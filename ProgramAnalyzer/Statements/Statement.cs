using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public abstract class Statement
{
    public abstract string ToString(int indent);
    public sealed override string ToString() => ToString(indent: 0);

    public ulong Position { get; set; } // For results ordering

    public abstract void OnEnter(PassMode mode, AnalyzerContext context);
}
