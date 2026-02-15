using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class PrintVariable(string variableName) : Statement
{
    private bool _isUnassigned;

    public string VariableName { get; } = variableName;

    public override string ToString(int indent) => $"print({VariableName})";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        var lastKnownDeclaration = context.LastVisitedVariableDeclaration ?? ParentScope!.Owner?.PreviousVariableDeclaration;
        if (context.FindVariableDeclaration(VariableName, lastKnownDeclaration) == null)
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (_isUnassigned || // avoid double reports
            context.IsTraversingFunctionDeclaration ||
            IsAssigned(VariableName, context.LastKnownAssignment))
            return;

        _isUnassigned = true;
        context.AddIssue(KnownErrors.UseOfUnassignedVariable, this);
    }

    private static bool IsAssigned(string name, AssignVariable? lastKnownAssignment)
    {
        while (lastKnownAssignment != null)
        {
            if (lastKnownAssignment.VariableName == name)
                return true;

            lastKnownAssignment = lastKnownAssignment.PreviousAssignment;
        }

        return false;
    }
}
