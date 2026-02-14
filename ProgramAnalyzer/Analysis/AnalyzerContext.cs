using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class AnalyzerContext
{
    private readonly Dictionary<Statement, List<string>> _issues = [];

    public long Position { get; set; }

    public LinkedQueue Queue { get; } = new();
    public LinkedStack Stack { get; } = new();

    public Statement? CurrentStatement { get; set; }
    public Invocation? CurrentInvocation { get; set; }
    public VariableDeclaration? LastVisitedVariableDeclaration { get; set; }
    public AssignVariable? LastKnownAssignment { get; set; }

    public int DeclarationDepth { get; set; }
    public bool IsTraversingFunctionDeclaration => DeclarationDepth > 0;

    public VariableDeclaration? FindVariableDeclaration(string name, VariableDeclaration? start)
    {
        while (start != null && start.VariableName != name)
        {
            start = start.PreviousDeclaration;
        }

        return start?.OriginalDeclaration ?? start;
    }

    public FunctionDeclaration? FindFunctionDeclaration(string name, FunctionDeclaration? start)
    {
        var current = start;
        while (current != null && 
               (current.FunctionName != name || !IsConflicting(current, start!)))
        {
            current = current.PreviousFunctionDeclaration;
        }

        return current?.OriginalDeclaration ?? current;
    }

    public FunctionDeclaration? FindFunctionDeclaration(string name, Statement? start, out bool isBelow)
    {
        isBelow = false;
        if (start == null)
            return null;

        // shortcut
        var owner = start.ParentScope!.Owner;
        if (owner?.FunctionName == name)
            return owner.OriginalDeclaration ?? owner;

        var nearestFunctionDeclaration = start.ParentScope!.LastFunctionDeclaration ??
                                         start.ParentScope!.ParentScope?.LastFunctionDeclaration;
        var result = start == null
            ? null
            : FindFunctionDeclaration(name, nearestFunctionDeclaration);

        if (result == null)
            return null;

        while (start != null && start.ParentScope != result!.ParentScope)
        {
            // we're walking up the tree, so this is guaranteed to converge
            start = start.ParentScope!.Owner;
        }

        isBelow = start!.ScopePosition < result.ScopePosition;
        return result;
    }

    public bool IsAssigned(string name)
    {
        var assignment = LastKnownAssignment;
        while (assignment != null)
        {
            if (assignment.VariableName == name)
                return true;

            assignment = assignment.PreviousAssignment;
        }

        return false;
    }

    public List<Issue> GetAllIssues() =>
        _issues
            .SelectMany(pair =>
                pair.Value.Select(err => new Issue(err, pair.Key)))
            .ToList();

    public void AddIssue(string error, Statement statement)
    {
        var errors = _issues.TryGetValue(statement, out var list)
            ? list
            : _issues[statement] = [];

        // the list of errors will be small, a few items at most
        foreach (var existingError in errors)
        {
            if (existingError == error)
                return;
        }

        errors.Add(error);
    }

    private static bool IsConflicting(FunctionDeclaration higherScopedFunc, FunctionDeclaration lowerScopedFunc)
    {
        // Function in IfStatement never conflicts with another function in IfStatement
        if (lowerScopedFunc.ParentIfStatement != null)
            return higherScopedFunc.ParentIfStatement == null;

        // Function in IfStatement only conflicts with functions in same or parent scopes
        return higherScopedFunc.ParentIfStatement == null ||
               higherScopedFunc.ParentScope == lowerScopedFunc.ParentScope;
    }
}
