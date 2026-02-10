using ProgramAnalyzer.Analysis;
using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Tests.Utils;

public static class IssuesListExtensions
{
    public static List<Issue> SortIssues(this List<Issue> issues)
    {
        issues.Sort((issue1, issue2) => issue1.Statement.Position.CompareTo(issue2.Statement.Position));

        return issues;
    }
}
