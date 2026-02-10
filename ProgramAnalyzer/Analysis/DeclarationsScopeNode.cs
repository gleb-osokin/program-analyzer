using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class DeclarationsScopeNode
{
    public DeclarationsScopeNode(ProgramBlock block, DeclarationsScopeNode? parent)
    {
        Block = block;
        Parent = parent;
    }

    public ProgramBlock Block { get; }
    public DeclarationsScopeNode? Parent { get; }

    public Dictionary<string, Statement> Statements { get; } = [];
}
