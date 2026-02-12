using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public interface IDeclaration
{
    public bool IsVisited { get; set; }
    public ProgramBlock? ParentScope { get; }
    public IfStatement? ParentIfStatement { get; }
}
