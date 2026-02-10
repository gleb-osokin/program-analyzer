using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class VariableDeclaration(string variableName) : Statement
{
    public string VariableName { get; } = variableName;

    public override string ToString(int indent) => $"var {VariableName}";

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode != PassMode.CollectDeclarations)
            return;

        var isConflict = context.PreviousAnalyzedStatement is IfStatement
            ? context.Declarations.IsDeclared(VariableName, context.CurrentBlock)
            : !context.Declarations.TryAddDeclaration(VariableName, this);

        if (isConflict)
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }
    }
}
