using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Statements;

public abstract class Statement
{
    public Statement? PreviousStatement { get; set; }
    public Statement? NextStatement { get; set; }

    public Statement? NextDeclaration { get; set; } // in scope

    public bool IsLastMember => ParentIfStatement != null ||
                                  this == ParentScope![^1];


    public abstract string ToString(int indent);
    public sealed override string ToString() => ToString(indent: 0);

    public long OriginalPosition { get; set; } = -1; // For results ordering in tests
    public long Position { get; set; } // set during call analysis pass

    public ProgramBlock? ParentScope { get; set; }
    public IfStatement? ParentIfStatement { get; set; } // immediate parent for IfStatement->ThenBody

    public abstract void OnDeclarationsEnter(AnalyzerContext context);
    public abstract void OnCallStackEnter(AnalyzerContext context);
    public virtual void OnCallStackExit(AnalyzerContext context)
    {
    }

    public virtual bool HasNestedScope(AnalyzerContext context) => false;

    public Statement? GetParentStatement(AnalyzerContext context)
    {
        if (ParentIfStatement != null)
            return ParentIfStatement;

        var owner = ParentScope!?.Owner;
        if (owner is not FunctionDeclaration func)
            return null;

        return context.CurrentInvocation?.Declaration == func
            ? context.CurrentInvocation
            : func;
    }
}
