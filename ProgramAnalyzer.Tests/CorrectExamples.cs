using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Tests;

public static class CorrectExamples
{
    /*
     *  var foo
     *  foo = ...
     *  print(foo) // OK, initialized
     */
    public static readonly ProgramBlock SimpleVariableUsage =
    [
        new VariableDeclaration("foo"),
        new AssignVariable("foo"),
        new PrintVariable("foo")
    ];

    /*
     *  var foo
     *  func Boo {
     *    print(foo)
     *  }
     *
     *  Bar()
     *  print(foo) // OK, initialized
     *
     *  func Bar {
     *    foo = ...
     *    if (...) {
     *      Boo() // OK, 'foo' initialized
     *    }
     *  }
     */
    public static readonly ProgramBlock LocalFunctionInit =
    [
        new VariableDeclaration("foo"),
        new FunctionDeclaration("Boo")
        {
          new PrintVariable("foo")
        },

        new Invocation("Bar"),
        new PrintVariable("foo"),
        new FunctionDeclaration("Bar")
        {
            new AssignVariable("foo"),
            new IfStatement
            {
                ThenBody = new Invocation("Boo")
            }
        }
    ];

    /*
     * func Boo {
     * }
     */
    public static readonly ProgramBlock EmptyFunction =
    [
        new FunctionDeclaration("Boo")
    ];

    /*
     *  var foo
     *  func Boo {
     *    print(foo)
     *  }
     *
     *  Bar()
     *  print(foo) // OK, initialized
     *
     *  func Bar {
     *    foo = ...
     *    if (...) {
     *      Boo() // OK, 'foo' initialized
     *    }
     *    
     *    func Baz {
     *      print(foo) // OK, initialized
     *    }
     *    
     *    Baz() // OK, initialized
     *  }
     */
    public static readonly ProgramBlock NestedFunctions =
    [
        new VariableDeclaration("foo"),
        new FunctionDeclaration("Boo")
        {
          new PrintVariable("foo")
        },

        new Invocation("Bar"),
        new PrintVariable("foo"),
        new FunctionDeclaration("Bar")
        {
            new AssignVariable("foo"),
            new IfStatement
            {
                ThenBody = new Invocation("Boo")
            },
            new FunctionDeclaration("Baz")
            {
                new PrintVariable("foo")
            },
            new Invocation("Baz")
        }
    ];


    /*
     *  func Boo {
     *    Bar()
     *  }
     *  
     *  func Bar {
     *    Boo()
     *  }
     *
     *  Boo() // OK, should finish
     */
    public static readonly ProgramBlock CyclicRecursiveInvocations =
    [
        new FunctionDeclaration("Boo")
        {
            new Invocation("Bar")
        },
        new FunctionDeclaration("Bar")
        {
            new Invocation("Boo")
        },
        new Invocation("Boo")
    ];
}