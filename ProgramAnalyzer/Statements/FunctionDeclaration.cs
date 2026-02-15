using ProgramAnalyzer.Analysis;
using System.Collections;

namespace ProgramAnalyzer.Statements;

public sealed class FunctionDeclaration(string functionName)
  : Statement, IEnumerable<Statement>
{
    public string FunctionName { get; } = functionName;

    public ProgramBlock Body { get; } = [];

    public bool IsConflict { get; set; }
    public FunctionDeclaration? OriginalDeclaration { get; set; } // in case we're the conflicting one

    public VariableDeclaration? PreviousVariableDeclaration { get; set; }
    public FunctionDeclaration? PreviousFunctionDeclaration { get; set; }

    public override string ToString(int indent) => $"func {FunctionName} " + Body.ToString(indent);

    public override bool HasNestedScope(AnalyzerContext context) => Body.Count > 0;

    public IEnumerator<Statement> GetEnumerator() => Body.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Body.GetEnumerator();
    public void Add(Statement statement) => Body.Add(statement);

    public override void OnDeclarationsEnter(AnalyzerContext context)
    {
        Body.ParentScope = ParentScope;
        PreviousVariableDeclaration = context.LastVisitedVariableDeclaration ?? ParentScope!.Owner?.PreviousVariableDeclaration;

        PreviousFunctionDeclaration = ParentScope!.LastFunctionDeclaration ??
                                      ParentScope!.ParentScope?.LastFunctionDeclaration; // we're first function in the scope

        ParentScope!.LastFunctionDeclaration = this;

        OriginalDeclaration = context.FindFunctionDeclaration(FunctionName, PreviousFunctionDeclaration);
        if (OriginalDeclaration != null ||
            context.FindVariableDeclaration(FunctionName, PreviousVariableDeclaration) is { } varDeclaration &&
            varDeclaration.ParentIfStatement == null) // vars in if statements don't affect anything below them
        {
            IsConflict = true;
            context.AddIssue(KnownErrors.ConflictDeclaration, this);
        }

        if (Body.Count > 0)
        {
            // check for conflicting/missing declarations even if we are in conflict
            Body.OnDeclarationsEnter(context);
        }
    }

    public override void OnCallStackEnter(AnalyzerContext context)
    {
        context.DeclarationDepth++;
        Body.OnCallStackEnter(context);
    }

    public override void OnCallStackExit(AnalyzerContext context)
    {
        context.DeclarationDepth--;
    }
}
