using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public interface IDeclaration
{
    public bool IsConflict { get; set; }
    public ProgramBlock? ParentScope { get; }
}
