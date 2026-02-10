using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class Invocation(string functionName) : Statement
{
    public string FunctionName { get; } = functionName;

    public override string ToString(int indent) => $"{FunctionName}()";

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode != PassMode.AnalyzeCallStack)
            return;

        var func = context.Declarations.GetDeclaration<FunctionDeclaration>(FunctionName, context.CurrentBlock);
        if (func == null)
        {
            context.AddIssue(KnownErrors.CallOfUndeclaredFunc, this);
            return;
        }

        // avoid recursive inspections
        foreach (var block in context.BlocksStack)
        {
            if (block.Func == func)
                return;
        }

        func.Body.OnEnter(mode, context, func);
    }
}
