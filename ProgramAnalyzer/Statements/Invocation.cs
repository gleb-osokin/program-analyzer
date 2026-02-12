using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class Invocation(string functionName) : Statement
{
    public string FunctionName { get; } = functionName;

    public Invocation? ParentInvocation { get; set; }

    public bool IsEmpty { get; set; }

    public override string ToString(int indent) => $"{FunctionName}()";

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        // do nothing, we don't have enough information at this point
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        var declaration = context.Declarations.GetFunctionDeclaration(FunctionName, ParentScope!);
        if (declaration == null || declaration.ParentIfStatement != null)
        {
            context.AddIssue(KnownErrors.CallOfUndeclaredFunc, this);
            return;
        }

        IsEmpty = declaration.Body.Count == 0;

        // avoid recursive inspections
        var currentInvocation = context.CurrentInvocation;
        while (currentInvocation != null)
        {
            if (currentInvocation.FunctionName == FunctionName)
                return;

            currentInvocation = currentInvocation.ParentInvocation;
        }

        ParentInvocation = context.CurrentInvocation;
        context.CurrentInvocation = this;
        declaration.Body.OnCallStackEnter(context);
    }
}
