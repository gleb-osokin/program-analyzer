using ProgramAnalyzer.Analysis;
using System.Collections;

namespace ProgramAnalyzer.Statements;

public sealed class FunctionDeclaration(string functionName)
  : Statement, IEnumerable<Statement>
{
    public string FunctionName { get; } = functionName;
    public ProgramBlock Body { get; } = [];

    public override string ToString(int indent) => $"func {FunctionName} " + Body.ToString(indent);

    public IEnumerator<Statement> GetEnumerator() => Body.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Body.GetEnumerator();
    public void Add(Statement statement) => Body.Add(statement);

    public override void OnEnter(PassMode mode, AnalyzerContext context)
    {
        if (mode != PassMode.CollectDeclarations)
            return;
            
        // we will put the declaration inside nested scope
        if (context.PreviousAnalyzedStatement is not IfStatement &&
            !context.Declarations.TryAddDeclaration(FunctionName, this))
        {
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        if (Body.Count <= 0)            
            return;

        // check for conflicting/missing declarations
        Body.OnEnter(mode, context, owner: this);
    }
}
