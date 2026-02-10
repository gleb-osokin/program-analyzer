using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Tests.Utils;

internal static class FunctionDeclarationExtensions
{
    public static FunctionDeclaration WithStatements(
        this FunctionDeclaration functionDeclaration, IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            functionDeclaration.Add(statement);
        }
        return functionDeclaration;
    }
}
