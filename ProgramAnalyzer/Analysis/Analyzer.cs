using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class Analyzer
{
    public List<Issue> Analyze(ProgramBlock program)
    {
        if (program.Count == 0)
            return [];

        var context = new AnalyzerContext();
        AnalyzeDeclarations(program, context);
        AnalyzeCallStack(program, context);

        return context.GetAllIssues();
    }

    private static void AnalyzeDeclarations(ProgramBlock program, AnalyzerContext context)
    {
        program.OnDeclarationsEnter(context);
        context.CurrentStatement = context.Queue.Dequeue();

        while (context.CurrentStatement != null)
        {
            var statement = context.CurrentStatement;
            statement.OnDeclarationsEnter(context);
            context.CurrentStatement = context.Queue.Dequeue();
        }
    }

    private static void AnalyzeCallStack(ProgramBlock program, AnalyzerContext context)
    {
        program.OnCallStackEnter(context);
        context.CurrentStatement = context.Stack.Pop();

        while (context.CurrentStatement != null)
        {
            var statement = context.CurrentStatement;
            if (statement.Position == 0)
            {
                statement.Position = context.Position;
            }

            statement.OnCallStackEnter(context);

            if (statement.HasNestedScope(context))
            {
                // We need to go deeper...
            }
            else
            {
                while (statement?.IsLastMember == true)
                {
                    statement = statement.GetParentStatement(context);
                    statement?.OnCallStackExit(context);
                }
            }

            context.CurrentStatement = context.Stack.Pop();
            context.Position++;
        }
    }
}
