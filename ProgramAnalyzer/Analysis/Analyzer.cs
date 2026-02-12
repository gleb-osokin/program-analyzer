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

            var currentInvocation = context.CurrentInvocation;
            if (currentInvocation != null && statement.IsLastMember)
            {
                statement.ParentIfStatement = currentInvocation.ParentIfStatement;
                currentInvocation.ParentIfStatement = null;

                if (statement is Invocation invocation && !invocation.IsEmpty)
                {
                    // we need to go deeper...
                }
                else do
                {
                    context.CurrentInvocation = currentInvocation.ParentInvocation;
                    currentInvocation.ParentInvocation = null;
                } while (context.CurrentInvocation?.IsLastMember == true);
            }

            if (statement.ParentIfStatement != null)
            {
                context.Assignments.PopScope();
            }

            var nextStatement = context.Stack.Pop();
            context.CurrentStatement = nextStatement;
            context.Position++;
        }
    }
}
