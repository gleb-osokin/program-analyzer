using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class AssignmentStack
{
    private readonly Dictionary<string, AssignVariable> _fastTrackDictionary = [];

    public Stack<ulong> IfStatementPositionsStack { get; } = [];
    public Stack<(AssignVariable Statement, ulong Position)> AssignVariablesStack { get; } = [];

    public void PushScope(ulong position) => IfStatementPositionsStack.Push(position);

    public void PopScope()
    {
        var position = IfStatementPositionsStack.Pop();

        while (AssignVariablesStack.TryPeek(out var tuple) && 
               tuple.Position > position)
        {
            _fastTrackDictionary.Remove(
                AssignVariablesStack.Pop().Statement.VariableName);
        }
    }

    public bool TryAdd(AssignVariable assignment, ulong position)
    {
        if (!_fastTrackDictionary.TryAdd(assignment.VariableName, assignment))
            return false;

        AssignVariablesStack.Push((assignment, position));
        return true;
    }

    public bool Exists(string name) => _fastTrackDictionary.ContainsKey(name);
}
