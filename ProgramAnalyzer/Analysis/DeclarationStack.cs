using ProgramAnalyzer.Statements;
using System.Runtime.InteropServices;

namespace ProgramAnalyzer.Analysis;

public sealed class DeclarationStack
{
    private readonly Dictionary<string, Statement> _declarations = [];
    private ProgramBlock? _current;

    public ProgramBlock CurrentScope => _current ??
        throw new InvalidOperationException("Root scope not initialized");

    public bool TryAddDeclaration(string name, Statement declaration)
    {
        ref var existingDeclaration = ref CollectionsMarshal.GetValueRefOrAddDefault(_declarations, name, out var exists);
        if (exists && AreIntersectingScopes(declaration.ParentScope!, existingDeclaration!.ParentScope!))
            return false;

        // existing declaration either doesn't exist or out of scope already
        existingDeclaration = declaration;
        return true;
    }

    public bool IsVariableDeclared(string name, ProgramBlock scope)
    {
        var decl = GetDeclaration(name, scope);
        return GetDeclaration(name, scope) is VariableDeclaration declaration &&
        declaration.IsAssignable;
    }

    public FunctionDeclaration? GetFunctionDeclaration(string name, ProgramBlock scope) =>
        GetDeclaration(name, scope) as FunctionDeclaration;

    private Statement? GetDeclaration(string name, ProgramBlock scope) =>
        _declarations.GetValueOrDefault(name) is { } declaration &&
            AreIntersectingScopes(declaration.ParentScope!, scope)
            ? declaration
            : null;

    private static bool AreIntersectingScopes(ProgramBlock scope1, ProgramBlock scope2) => 
        scope1.IsEqualOrChildOf(scope2) ||
        scope2.IsEqualOrChildOf(scope1);
}
