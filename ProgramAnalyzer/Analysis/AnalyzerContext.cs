using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class AnalyzerContext
{
    private readonly Dictionary<Statement, List<string>> _issues = [];

    public long Position { get; set; }

    public LinkedQueue Queue { get; } = new();
    public LinkedStack Stack { get; } = new();

    public DeclarationStack Declarations { get; } = new();
    public AssignmentStack Assignments { get; } = new();
    public Statement? CurrentStatement { get; set; }
    public Invocation? CurrentInvocation { get; set; }

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
}
