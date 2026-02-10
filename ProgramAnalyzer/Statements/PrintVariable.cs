using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class PrintVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public override string ToString(int indent) => $"print({VariableName})";

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode != PassMode.AnalyzeCallStack)
            return;

        var declaration = context.Declarations.GetDeclaration<VariableDeclaration>(VariableName, context.Declarations.PeekScope());
        if (declaration == null || declaration.Position >= Position)
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
        else if (!context.Assignments.Exists(VariableName))
        {
            context.AddIssue(KnownErrors.UseOfUnassignedVariable, this);
        }
    }
}
