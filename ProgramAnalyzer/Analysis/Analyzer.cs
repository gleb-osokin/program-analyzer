using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public class Analyzer
{
    public List<Issue> Analyze(ProgramBlock program)
    {
        if (program.Count == 0)
            return [];

        var context = new AnalyzerContext(program);
        Analyze(program, context, PassMode.CollectDeclarations);
        Analyze(program, context, PassMode.AnalyzeCallStack);

        return context.GetAllIssues();
    }

    private static void Analyze(ProgramBlock program, AnalyzerContext context, PassMode mode)
    {
        program.OnEnter(mode, context, owner: null);

        while (context.AnalyzeStack.Count > 0)
        {
            var statement = context.AnalyzeStack.Pop();

            // We will only walk all nodes in the order of their appearance
            // during declarations collection.
            if (mode == PassMode.CollectDeclarations)
            {
                statement.Position = context.Position;
            }

            statement.OnEnter(mode, context);
            context.PreviousAnalyzedStatement = statement;

            if (statement is not ITerminator)
            {
                context.IncrementPosition();
            }
        }

        context.ResetPosition();
    }
}
