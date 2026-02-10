using ProgramAnalyzer.Analysis;
using System.Collections;

namespace ProgramAnalyzer.Statements;

public sealed class FunctionDeclaration(string functionName)
  : Statement, IEnumerable<Statement>
{
    public string FunctionName { get; } = functionName;

    public ProgramBlock Body { get; } = [];
    public bool IsInvocable { get; set; }

    public override string ToString(int indent) => $"func {FunctionName} " + Body.ToString(indent);

    public IEnumerator<Statement> GetEnumerator() => Body.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Body.GetEnumerator();
    public void Add(Statement statement) => Body.Add(statement);

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        IsInvocable = ParentIfStatement == null;
        var isConflict = IsInvocable
            ? !context.Declarations.TryAddDeclaration(FunctionName, this)
            : context.Declarations.GetFunctionDeclaration(FunctionName, ParentScope!) != null;

        if (isConflict)
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        if (Body.Count <= 0)            
            return;

        // check for conflicting/missing declarations even if we are in conflict
        Body.OnDeclarationsEnter(context);
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        // ignore function declarations completely during call stack analysis
    }
}
