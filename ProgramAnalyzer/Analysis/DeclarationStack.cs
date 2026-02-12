using ProgramAnalyzer.Statements;
using System.Runtime.InteropServices;

namespace ProgramAnalyzer.Analysis;

public sealed class DeclarationStack
{
    private readonly Dictionary<string, IDeclaration> _declarations = [];

    public bool TryAddDeclaration(
        string name, IDeclaration declaration, ProgramBlock scope, bool setVisited = false)
    {
        ref var existingDeclaration = ref CollectionsMarshal.GetValueRefOrAddDefault(_declarations, name, out var exists);
        if (exists &&
            existingDeclaration!.IsVisited == setVisited && // can become true only during analysis pass
            AreConflictingDeclarations(declaration, existingDeclaration))
            return false;

        // existing declaration either doesn't exist or out of scope already
        existingDeclaration = declaration;
        declaration.IsVisited = setVisited;
        return true;
    }

    public bool IsVariableDeclared(string name, ProgramBlock scope, long? position = null)
    {
        var statement = GetDeclaration(name, scope);
        return statement is VariableDeclaration declaration &&
               declaration.ParentIfStatement == null &&
               declaration.IsVisited == position.HasValue &&
               declaration.Position <= position.GetValueOrDefault();
    }

    public FunctionDeclaration? GetFunctionDeclaration(string name, ProgramBlock scope) =>
        GetDeclaration(name, scope) as FunctionDeclaration;

    public IDeclaration? GetDeclaration(string name, ProgramBlock scope) =>
        _declarations.GetValueOrDefault(name) is { } declaration &&
            AreIntersectingScopes(declaration.ParentScope!, scope)
            ? declaration
            : null;

    private static bool AreIntersectingScopes(ProgramBlock scope1, ProgramBlock scope2) =>
        scope1 == scope2 ||
        scope1.IsChildOf(scope2) ||
        scope2.IsChildOf(scope1);

    private static bool AreConflictingDeclarations(IDeclaration declaration1, IDeclaration declaration2)
    {
        if (declaration1 == declaration2)
            return false;

        if (declaration1.ParentScope == declaration2.ParentScope)
            return declaration1.ParentIfStatement == null &&
                   declaration2.ParentIfStatement == null;

        if (declaration1.ParentIfStatement == null &&
            declaration2.ParentIfStatement == null)
            return AreIntersectingScopes(declaration1.ParentScope!, declaration2.ParentScope!);

        if (declaration1.ParentIfStatement == null) // declaration2 is in if block
            return declaration1.ParentScope!.IsChildOf(declaration2.ParentScope!);

        if (declaration2.ParentIfStatement == null) // declaration1 is in if block
            return declaration2.ParentScope!.IsChildOf(declaration1.ParentScope!);

        // since there can be only one statement in an if block and both statements are in different ifs
        return false;
    }
}
