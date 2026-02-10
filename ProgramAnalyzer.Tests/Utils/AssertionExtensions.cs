using ProgramAnalyzer.Analysis;
using Shouldly;

namespace ProgramAnalyzer.Tests.Utils;

internal static class AssertionExtensions
{
    public static void ShouldHaveError<T>(this Issue issue, string message, ulong position)
    {
        issue.Message.ShouldBe(message);
        issue.Statement.ShouldBeOfType<T>();
        issue.Statement.Position.ShouldBe(position);
    }
}
