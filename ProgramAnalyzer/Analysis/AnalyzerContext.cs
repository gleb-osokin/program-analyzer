using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class AnalyzerContext
{
    private readonly Dictionary<Statement, List<string>> _issues = [];

    public AnalyzerContext(ProgramBlock root) => Declarations = new DeclarationScopeTree(root);

    public ulong Position { get; private set; }

    public Stack<Statement> AnalyzeStack { get; } = [];
    public Stack<(FunctionDeclaration? Func, ProgramBlock Block)> BlocksStack { get; } = [];

    public ProgramBlock CurrentBlock => BlocksStack.Peek().Block;
    public DeclarationScopeTree Declarations { get; }
    public AssignmentStack Assignments { get; } = new();
    public Statement? PreviousAnalyzedStatement { get; set; }
    public AssignVariable? LastAssignment { get; set; }

    public void IncrementPosition() => Position++;
    public void ResetPosition() => Position = 0;

    public List<Issue> GetAllIssues() =>
        _issues
            .OrderBy(pair => pair.Key.Position)
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
}
