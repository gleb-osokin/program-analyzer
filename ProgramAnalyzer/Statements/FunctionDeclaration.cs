using ProgramAnalyzer.Analysis;
using System.Collections;

namespace ProgramAnalyzer.Statements;

public sealed class FunctionDeclaration(string functionName)
  : Statement, IEnumerable<Statement>, IDeclaration
{
    public string FunctionName { get; } = functionName;

    public ProgramBlock Body { get; } = [];
    public bool IsConflict { get; set; }
    public bool IsVisited { get; set; }

    public override string ToString(int indent) => $"func {FunctionName} " + Body.ToString(indent);

    public override bool HasNestedScope(AnalyzerContext context) => Body.Count > 0;

    public IEnumerator<Statement> GetEnumerator() => Body.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Body.GetEnumerator();
    public void Add(Statement statement) => Body.Add(statement);

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        Body.ParentScope = ParentScope;
        
        if (!context.Declarations.TryAddDeclaration(FunctionName, this))
        {
            IsConflict = true;
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        if (!HasNestedScope(context))            
            return;

        // check for conflicting/missing declarations even if we are in conflict
        Body.OnDeclarationsEnter(context);
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        if (!IsConflict &&
            !context.Declarations.TryAddDeclaration(
                FunctionName, this, setVisited: true))
        {
            IsConflict = true;
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        context.DeclarationDepth++;
        Body.OnCallStackEnter(context);
    }

    public override void OnCallStackExit(AnalyzerContext context)
    {
        context.DeclarationDepth--;
    }
}
