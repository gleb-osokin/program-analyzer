using ProgramAnalyzer.Analysis;
using System.Text;

namespace ProgramAnalyzer.Statements;

public sealed class ProgramBlock : List<Statement>
{
    public ProgramBlock? ParentScope { get; set; }

    public FunctionDeclaration? Owner {  get; set; }

    public FunctionDeclaration? LastFunctionDeclaration { get; set; }

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

    public bool IsChildOf(ProgramBlock other)
    {
        while (other.ParentScope != null)
        {
            if (other.ParentScope == this)
                return true;

            other = other.ParentScope;
        }
        return false;
    }

    public void OnDeclarationsEnter(AnalyzerContext context)
    {
        if (Count == 0)
            return;

        Owner = context.CurrentStatement as FunctionDeclaration;
        ParentScope = Owner?.ParentScope;

        var counter = 0;
        for (var i = 0; i < Count; i++)
        {
            var statement = this[i];
            statement.ParentScope = this;
            statement.ScopePosition = counter++;

            context.Queue.Enqueue(statement);
            if (statement is IfStatement ifStatement)
            {
                ifStatement.ScopePosition = counter++;
                context.Queue.Enqueue(ifStatement.ThenBody);
            }
        }
    }

    public void OnCallStackEnter(AnalyzerContext context)
    {
        if (Count == 0)
            return;

        context.LastVisitedVariableDeclaration = null;

        for (var i = Count - 1; i >= 0; i--)
        {
            context.Stack.Push(this[i]);
        }
    }
}
