using Shouldly;

namespace ProgramAnalyzer.Tests.Utils;

public class ParserTests
{
    [Fact]
    public void Parse_SimpleVariableUsage_MatchesExample()
    {
        const string programText =
            """
            var foo
            foo = ...
            print(foo) // OK, initialized
            """;
        
        var result = Parser.Parse(programText);

        result.ToString().ShouldBe(CorrectExamples.SimpleVariableUsage.ToString());
    }

    [Fact]
    public void Parse_LocalFunctionInit_MatchesExample()
    {
        const string programText =
            """
            var foo
            func Boo {
              print(foo)
            }

            Bar()
            print(foo) // OK, initialized

            func Bar {
              foo = ...
              if (...) {
                Boo() // OK, 'foo' initialized
              }
            }
            """;

        var result = Parser.Parse(programText);

        result.ToString().ShouldBe(CorrectExamples.LocalFunctionInit.ToString());
    }

    [Fact]
    public void Parse_NestedFunctions_MatchesExample()
    {
        const string programText =
            """
            var foo
            func Boo {
              print(foo)
            }

            Bar()
            print(foo) // OK, initialized

            func Bar {
              foo = ...
              if (...) {
                Boo() // OK, 'foo' initialized
              }

              func Baz {
                print(foo) // OK, initialized
              }
              Baz() // OK, initialized
            }
            """;

        var result = Parser.Parse(programText);

        result.ToString().ShouldBe(CorrectExamples.NestedFunctions.ToString());
    }

    [Fact]
    public void Parse_EmptyFunction_MatchesExample()
    {
        const string programText = 
            """
            func Boo {
            }
            """;

        var result = Parser.Parse(programText);

        result.ToString().ShouldBe(CorrectExamples.EmptyFunction.ToString());
    }
}
