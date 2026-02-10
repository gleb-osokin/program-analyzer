using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public sealed class LinkedQueue
{
    public Statement? Head { get; set; }
    public Statement? Tail { get; set; }

    public void Enqueue(Statement statement)
    {
        if (Tail == null)
        {
            Tail = statement;
            Head = statement;
            return;
        }

        Tail.NextStatement = statement;
        Tail = statement;
    }

    public Statement? Dequeue()
    {
        if (Head == null)
            return null;

        var statement = Head;
        Head = statement.NextStatement;
        return statement;
    }
}
