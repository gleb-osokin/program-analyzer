using ProgramAnalyzer.Analysis;
using System.Text;

namespace ProgramAnalyzer.Statements;

public sealed class ProgramBlock : List<Statement>
{
    public string ToString(int indent)
    {
        var builder = new StringBuilder("{").AppendLine();

        foreach (var statement in this)
        {
            builder
              .Append(' ', indent + 4)
              .Append(statement.ToString(indent + 4))
              .AppendLine();
        }

        return builder.Append(' ', indent).Append("}").ToString();
    }

    public override string ToString() => ToString(indent: 0);

    public void OnEnter(PassMode mode, AnalyzerContext context, FunctionDeclaration? owner)
    {
        if (mode == PassMode.CollectDeclarations)
        {
            context.Declarations.PushScope(this);
        }

        context.BlocksStack.Push((Func: owner, Block: this));
        if (owner != null && context.PreviousAnalyzedStatement is IfStatement &&
            !context.Declarations.TryAddDeclaration(owner.FunctionName, owner))
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, owner);
        }

        context.AnalyzeStack.Push(ProgramBlockTerminator.Instance);

        for (var i = Count - 1; i >= 0; i--)
        {
            context.AnalyzeStack.Push(this[i]);
        }
    }
}
