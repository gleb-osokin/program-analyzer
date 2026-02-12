using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class AssignVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public bool IsDeclared { get; set; }
    public AssignVariable? PreviousAssignment { get; set; }
    public long InitialPosition { get; set; } // position of the first assignment in the call stack

    public override string ToString(int indent) => $"{VariableName} = ...";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        if (!context.Declarations.IsVariableDeclared(VariableName, ParentScope!))
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
        else
        {
            IsDeclared = true;
        }
        return;
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (!context.Declarations.IsVariableDeclared(VariableName, ParentScope!, Position))
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }

        // It is not clear, if we want to ignore undeclared assignments.
        // Assume that we still want to consider them.
        // Otherwise uncomment the check below                
        // if (IsDeclared)
        {
            context.Assignments.TryAdd(context, this);
        }
    }
}
