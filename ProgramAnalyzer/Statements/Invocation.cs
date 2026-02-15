using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public sealed class Invocation(string functionName) : Statement
{
    private bool _isUndeclared;

    public string FunctionName { get; } = functionName;

    public FunctionDeclaration? Declaration { get; set; }
    public Invocation? ParentInvocation { get; set; }

    public bool IsEmpty { get; set; }

    public override string ToString(int indent) => $"{FunctionName}()";

    public override bool HasNestedScope(AnalyzerContext context) =>
        !IsEmpty &&
        Declaration != null &&
        !context.IsTraversingFunctionDeclaration;

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        // do nothing, we don't have enough information at this point
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (_isUndeclared)
            return; // avoid double reports

        Declaration = context.FindFunctionDeclaration(FunctionName, ParentScope!.LastFunctionDeclaration);
        if (Declaration == null || Declaration.ParentIfStatement != null)
        {
            _isUndeclared = true;
            context.AddIssue(KnownErrors.CallOfUndeclaredFunc, this);
            return;
        }

        IsEmpty = Declaration.Body.Count == 0;
        if (context.IsTraversingFunctionDeclaration)
            return;

        // avoid recursive inspections
        var currentInvocation = context.CurrentInvocation;
        while (currentInvocation != null)
        {
            if (currentInvocation.FunctionName == FunctionName)
            {
                // hack to make consequent HasNestedScope() check return false
                // Declaration will be reset upon next enter
                Declaration = null;
                return;
            }

            currentInvocation = currentInvocation.ParentInvocation;
        }

        ParentInvocation = context.CurrentInvocation;
        context.CurrentInvocation = this;
        Declaration.Body.OnCallStackEnter(context);
    }

    public override void OnCallStackExit(AnalyzerContext context)
    {
        context.CurrentInvocation = ParentInvocation;
        ParentInvocation = null;
    }
}
