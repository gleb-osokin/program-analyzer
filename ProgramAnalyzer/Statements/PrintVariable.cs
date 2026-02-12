using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class PrintVariable(string variableName) : Statement
{
    public string VariableName { get; } = variableName;
    public bool IsDeclared { get; set; }

    public override string ToString(int indent) => $"print({VariableName})";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        if (context.Declarations.IsVariableDeclared(VariableName, ParentScope!))
        {
            IsDeclared = true;
        }
        else
        {
            context.AddIssue(KnownErrors.UseOfUndeclaredVariable, this);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (IsDeclared && 
            !context.IsTraversingFunctionDeclaration && 
            !context.Assignments.IsAssigned(VariableName))
        {
            context.AddIssue(KnownErrors.UseOfUnassignedVariable, this);
        }
    }
}
