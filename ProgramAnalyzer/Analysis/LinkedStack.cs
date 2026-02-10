using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class LinkedStack
{
    public Statement? Head { get; set; }

    public void Push(Statement statement)
    {
        statement.NextStatement = Head ?? null;
        Head = statement;
    }

    public Statement? Pop()
    {
        if (Head == null)
            return null;

        var statement = Head;
        Head = statement.NextStatement;

        return statement;
    }
}
