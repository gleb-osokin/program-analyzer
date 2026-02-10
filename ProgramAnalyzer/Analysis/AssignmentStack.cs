using ProgramAnalyzer.Statements;
using System.Runtime.InteropServices;

namespace ProgramAnalyzer.Analysis;

public sealed class AssignmentStack
{
    private readonly Dictionary<string, bool> _assignments = [];

    private IfStatement? _current;
    private AssignVariable? _lastKnownAssignment;

    public void PushScope(IfStatement statement)
    {
        statement.ParentIfStatement = _current;
        _current = statement;
    }

    public void PopScope()
    {
        if (_current == null)
        {
            // if we're here, then we have a bug in our algorithm...
            throw new InvalidOperationException("Cannot pop beyond root scope");
        }

        PopAssignments(_current.CallStackPosition, _lastKnownAssignment);
    }

    public void TryAdd(AnalyzerContext context, AssignVariable assignment)
    {
        // ignore if already assigned
        ref var isAssigned = ref CollectionsMarshal.GetValueRefOrAddDefault(_assignments, assignment.VariableName, out _);
        if (isAssigned)
            return;

        assignment.InitialPosition = context.Position;
        assignment.PreviousAssignment = _lastKnownAssignment;
        _lastKnownAssignment = assignment;
        isAssigned = true;
    }

    public bool IsAssigned(string name) => _assignments.GetValueOrDefault(name, false);

    private void PopAssignments(long ifBlockStart, AssignVariable? assignment)
    {
        while (assignment != null && assignment.InitialPosition > ifBlockStart)
        {
            _assignments[assignment.VariableName] = false;
            assignment = assignment.PreviousAssignment;
        }
    }
}
