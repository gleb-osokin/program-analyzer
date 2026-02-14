using ProgramAnalyzer.Analysis;
using ProgramAnalyzer.Statements;
using ProgramAnalyzer.Tests.Utils;
using Shouldly;

namespace ProgramAnalyzer.Tests;

public class ErrorsTests
{
    public const string ConflictDeclaration = "Conflicting declaration";
    public const string UseOfUndeclaredVariable = "Use of undeclared variable";
    public const string UseOfUnassignedVariable = "Use of unassigned variable";
    public const string CallOfUndeclaredFunc = "Call of undeclared func";

    [Fact]
    public void Analyze_ConflictingDeclarations_IssuesReported()
    {
        var program = Parser.Parse(
            """
            var a // OK, first declaration
            var a // error
            func a { // error
              var a // error
            }
            func b { // OK, first declaration
              var b // error
            }
            func c { // OK, first declaration
            }
            func c { // error
            }
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(5),
            res => res[0].ShouldHaveError<VariableDeclaration>(ConflictDeclaration, 1),
            res => res[1].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 2),
            res => res[2].ShouldHaveError<VariableDeclaration>(ConflictDeclaration, 3),
            res => res[3].ShouldHaveError<VariableDeclaration>(ConflictDeclaration, 5),
            res => res[4].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 7));
    }

    [Fact]
    public void Analyze_UndeclaredUse_IssuesReported()
    {
        var program = Parser.Parse(
            """
            var a
            b = ... // error
            print(b) // error

            func C {
              print(b) // error
              D() // error
              f = ... // error
              var g
            }
            C()
            E() // error
            var f
            f = ...
            print(f)
            print(g) // error
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(8),
            res => res[0].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 1),
            res => res[1].ShouldHaveError<PrintVariable>(UseOfUndeclaredVariable, 2),
            res => res[2].ShouldHaveError<PrintVariable>(UseOfUndeclaredVariable, 4),
            res => res[3].ShouldHaveError<Invocation>(CallOfUndeclaredFunc, 5),
            res => res[4].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 6),
            res => res[5].ShouldHaveError<Invocation>(CallOfUndeclaredFunc, 9),
            res => res[6].ShouldHaveError<PrintVariable>(UseOfUndeclaredVariable, 13),
            res => res[7].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 13));
    }

    [Fact]
    public void Analyze_PrintUnassigned_IssuesReported()
    {
        var program = Parser.Parse(
            """
            var a
            a = ...
            print(a)

            var b
            print(b) // error

            C()

            func C {
              print(a)
              print(b) // error
              
              C()
              D()
            }

            func D {
              print(a)
              print(b) // error
            }

            func E {
              print(a)
              print(b) // OK, function is never called
            }
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(3),
            res => res[0].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 4),
            res => res[1].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 8),
            res => res[2].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 13));
    }

    [Fact]
    public void Analyze_NestedDeclarationScopes_ReportsFirstOnly()
    {
        var program = Parser.Parse(
            """
            var a

            func A {
              var a // error
              a = ...
            }

            A()
            print(a) // OK, a is still in scope
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(1),
            res => res[0].ShouldHaveError<VariableDeclaration>(ConflictDeclaration, 2));
    }

    [Fact]
    public void Analyze_CallBeforeAssignment_ReportsIssue()
    {
        var program = Parser.Parse(
            """
            A()

            var a
            a = ...

            B()

            func A {
              print(a) // error, call before assignment
            }

            func B {
              print(a) // OK, call after assignment
            }

            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(1),
            res => res[0].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 5));
    }

    [Fact]
    public void Analyze_AssignmentBeforeDeclaration_ReportsIssue()
    {
        var program = Parser.Parse(
            """
            a = ... // error
            var a
            print(a) // OK. Current requirements don't state expected behavior.
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(1),
            res => res[0].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 0));
    }

    [Fact]
    public void Analyze_DuplicateFunctionDeclarations_ReportsAllIssue()
    {
        var program = Parser.Parse(
            """
            var a
            func A {
              print(a) // error, call before assignment
              a = ...
            }

            func A { // error, duplicate declaration
              print(a) // OK, never called
            }

            A()
            print(a) // OK
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(2),
            res => res[0].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 2),
            res => res[1].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 4));
    }

    [Fact]
    public void Analyze_CallAfterConditional_ReportIssues()
    {
        var program = Parser.Parse(
            """
            var a
            if (...) {
              a = ...
            }
            print(a) // error

            if (...) {
              A()
            }
            b = ... // error
            print(b) // error

            func A {
              var b
              b = ...
            }
            """);

        var result = new Analyzer().Analyze(program).SortIssues().SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(3),
            res => res[0].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 3),
            res => res[1].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 6),
            res => res[2].ShouldHaveError<PrintVariable>(UseOfUndeclaredVariable, 7));
    }

    [Fact]
    public void Analyze_ConditionalAssignment_ReportsIssues()
    {
        var program = Parser.Parse(
           """
           var a

           func A {
             a = ...
           }

           func B {
             A()
           }

           func C {
             if (...) {
               A()
             }
           }

           C()
           print(a) // error, assignment within if block

           B()
           print(a) // OK
           """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(1),
            res => res[0].ShouldHaveError<PrintVariable>(UseOfUnassignedVariable, 9));
    }

    [Fact]
    public void Analyze_NonIntersectingScopes_ReportsIssues()
    {
        var program = Parser.Parse(
           """
           if (...) {
             var a
           }

           if (...) {
             var a // OK, independent scope
           }

           if (...) {
             func A {
                var a // OK, independent scope
             }
           }

           if (...) {
             func A {
                var a // OK, independent scope
             }
           }

           if (...) {
             func a { // OK, independent scope
                var a // error
             }
           }
           """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(1),
            res => res[0].ShouldHaveError<VariableDeclaration>(ConflictDeclaration, 12));
    }

    [Fact]
    public void Analyze_ConflictingDeclarations_IssuesStillInspected()
    {
        var program = Parser.Parse(
           """
           func A {
             a = ... // error
           }

           func A { // error
             a = ... // error
           }

           func B {
             b = ... // error
           }

           func B { // error
             var b
             b = ...
           }

           B()
           print(b) // error, duplicate func not called
           """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(6),
            res => res[0].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 1),
            res => res[1].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 2),
            res => res[2].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 3),
            res => res[3].ShouldHaveError<AssignVariable>(UseOfUndeclaredVariable, 5),
            res => res[4].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 6),
            res => res[5].ShouldHaveError<PrintVariable>(UseOfUndeclaredVariable, 10));
    }

    [Fact(Skip = "Nice to have, but there are no requirements for this, so not implemented at the moment")]
    public void Analyze_ConflictingDeclarations_UseNearestDeclaration()
    {
        var program = Parser.Parse(
           """
           var a
           func A {
           }

           func A { // error
             a = ... // error
           }

           print(a) // OK, nearest declaration has assignment
           """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(2),
            res => res[0].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 2),
            res => res[0].ShouldHaveError<AssignVariable>(UseOfUnassignedVariable, 3));
    }

    [Fact]
    public void Analyze_IntersectingConflictingVariablesAndDeclarations_AreStillAssignableInvocable()
    {
        var program = Parser.Parse(
            """
            var a
            func a { // error
            }
            a = ... // OK, var declared

            var b
            b = ...

            a() // OK, func ceclared
            b() // OK, func declared

            func b { // error
            }
            """);

        var result = new Analyzer().Analyze(program).SortIssues();

        result.ShouldSatisfyAllConditions(
            res => res.Count.ShouldBe(2),
            res => res[0].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 1),
            res => res[1].ShouldHaveError<FunctionDeclaration>(ConflictDeclaration, 7));
    }
}