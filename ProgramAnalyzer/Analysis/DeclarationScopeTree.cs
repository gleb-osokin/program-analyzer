using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class DeclarationScopeTree : DeclarationsScopeNode
{
    private readonly Dictionary<ProgramBlock, DeclarationsScopeNode> _fastTrackDictionary = [];
    private DeclarationsScopeNode _currentScope;

    public DeclarationScopeTree(ProgramBlock block)
        : base(block, parent: null)
    {
        _currentScope = this;
    }

    public ProgramBlock PeekScope() => _currentScope.Block;

    public void PushScope(ProgramBlock scope)
    {
        _currentScope = new DeclarationsScopeNode(scope, parent: _currentScope);
        _fastTrackDictionary.Add(scope, _currentScope);
    }

    public void PopScope()
    {
        if (_currentScope?.Parent != null)
        {
            // This should happen at the end of execution, so we don't care.
            _currentScope = _currentScope.Parent;
        }
    }

    public bool TryAddDeclaration(string name, Statement declaration)
    {
        if (GetDeclaration(name, _currentScope) != null)
            return false;

        _currentScope.Statements.Add(name, declaration);
        return true;
    }

    public bool IsDeclared(string name, ProgramBlock scope) =>
        GetDeclaration(name, _fastTrackDictionary.GetValueOrDefault(scope)) != null;

    public T? GetDeclaration<T>(string name, ProgramBlock scope)
        where T : Statement =>
        GetDeclaration(name, _fastTrackDictionary.GetValueOrDefault(scope)) as T;

    private static Statement? GetDeclaration(string name, DeclarationsScopeNode? scope)
    {
        while (scope != null)
        {
            if (scope.Statements.TryGetValue(name, out var statement))
                return statement;

            scope = scope.Parent;
        }

        return null;
    }
}
