using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public abstract class Statement
{
    public Statement? PreviousStatement { get; set; }
    public Statement? NextStatement { get; set; }

    public bool IsLastMember { get; set; }
    public Statement? NextDeclaration { get; set; } // in scope

    public abstract string ToString(int indent);
    public sealed override string ToString() => ToString(indent: 0);

    public long Position { get; set; } = -1; // For results ordering in tests
    public ProgramBlock? ParentScope { get; set; }
    public IfStatement? ParentIfStatement { get; set; } // immediate parent for IfStatement->ThenBody

    public abstract void OnDeclarationsEnter(AnalyzerContext context);
    public abstract void OnCallStackEnter(AnalyzerContext context);
}
