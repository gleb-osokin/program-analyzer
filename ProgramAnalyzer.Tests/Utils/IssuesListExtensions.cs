using ProgramAnalyzer.Analysis;

namespace ProgramAnalyzer.Tests.Utils;

public static class IssuesListExtensions
{
    public static List<Issue> SortIssues(this List<Issue> issues)
    {
        issues.Sort((issue1, issue2) => issue1.Statement.OriginalPosition.CompareTo(issue2.Statement.OriginalPosition));

        return issues;
    }
}
