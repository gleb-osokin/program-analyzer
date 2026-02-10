using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class AssignVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public AssignVariable? PreviousAssignment { get; set; }
    public ulong InitialPosition { get; set; } // position of the first assignment in the call stack

    public override string ToString(int indent) => $"{VariableName} = ...";

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode != PassMode.AnalyzeCallStack)
            return;

        var declaration = context.Declarations.GetDeclaration<VariableDeclaration>(VariableName, context.CurrentBlock);
        if (declaration == null || declaration.Position >= Position)
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
        else
        {
            context.Assignments.TryAdd(context, this);
        }
    }
}
