using ProgramAnalyzer.Statements;
using System.Runtime.InteropServices;

namespace ProgramAnalyzer.Analysis;

public sealed class AssignmentStack
{
    private readonly Dictionary<string, bool> _assignments = [];

    private IfStatement? _current;

    public Stack<(AssignVariable Statement, ulong Position)> AssignVariablesStack { get; } = [];

    public void PushScope(IfStatement statement)
    {
        _current = statement;
    }

    public void PopScope(AssignVariable? lastKnownAssignment)
    {
        if (_current == null)
        {
            // if we're here, then we have a bug in our algorithm...
            throw new InvalidOperationException("Cannot pop beyond root scope");
        }

        PopAssignments(_current.Position, lastKnownAssignment);
        _current = _current.ParentIfStatement;
    }

    public void TryAdd(AnalyzerContext context, AssignVariable assignment)
    {
        // ignore if already assigned
        ref var isAssigned = ref CollectionsMarshal.GetValueRefOrAddDefault(_assignments, assignment.VariableName, out _);
        if (isAssigned)
            return;

        assignment.InitialPosition = context.Position;
        assignment.PreviousAssignment = context.LastAssignment;
        context.LastAssignment = assignment;
        isAssigned = true;
    }

    public bool IsAssigned(string name) => _assignments.GetValueOrDefault(name, false);

    private void PopAssignments(ulong ifBlockStart, AssignVariable? assignment)
    {
        while (assignment != null && 
            assignment.InitialPosition > ifBlockStart)
        {
            _assignments[assignment.VariableName] = false;
            assignment = assignment.PreviousAssignment;
        }
    }
}
