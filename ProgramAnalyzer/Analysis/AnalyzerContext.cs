using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class AnalyzerContext
{
    public List<Issue> Issues { get; } = [];

    public long Position { get; set; }

    public LinkedQueue Queue { get; } = new();
    public LinkedStack Stack { get; } = new();

    public Statement? CurrentStatement { get; set; }
    public Invocation? CurrentInvocation { get; set; }
    public VariableDeclaration? LastVisitedVariableDeclaration { get; set; }
    public AssignVariable? LastKnownAssignment { get; set; }

    public int DeclarationDepth { get; set; }
    public bool IsTraversingFunctionDeclaration => DeclarationDepth > 0;

    public void AddIssue(string error, Statement statement) =>
        Issues.Add(new Issue(error, statement));

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

    public FunctionDeclaration? FindFunctionDeclaration(string name, Statement? start)
    {
        if (start == null)
            return null;

        // shortcut
        var owner = start.ParentScope!.Owner;
        if (owner?.FunctionName == name)
            return owner.OriginalDeclaration ?? owner;

        var nearestFunctionDeclaration = start.ParentScope!.LastFunctionDeclaration ??
                                         start.ParentScope!.ParentScope?.LastFunctionDeclaration;
        return start == null
            ? null
            : FindFunctionDeclaration(name, nearestFunctionDeclaration);
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
