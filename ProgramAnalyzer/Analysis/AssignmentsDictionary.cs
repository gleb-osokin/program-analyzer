using ProgramAnalyzer.Statements;
using System.Runtime.InteropServices;

namespace ProgramAnalyzer.Analysis;

public sealed class AssignmentsDictionary
{
    private readonly Dictionary<string, bool> _assignments = [];
    private AssignVariable? _lastKnownAssignment;

    public void RemoveNested(IfStatement statement) => 
        PopAssignments(statement.LastVisitedPosition, _lastKnownAssignment);

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
