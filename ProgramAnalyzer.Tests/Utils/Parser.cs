using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Tests.Utils;

internal static class Parser
{
    // This parser is for simplifying the test cases declarations.
    //
    // Assumptions:
    // 1) There won't be incorrect programs.
    // 2) Every statement is expected to be on it's own line (no "if (...) { Fn() }")
    //    and we only have \n endings.
    // 3) According to the task, if-statements, although declared with curlies, can only have a single statement inside
    //    and not a program block.
    // 4) Since original assignment provides the AST already, we don't need to care about
    //    variables/keywords overlapping (e.g., "var var", "if = ..." etc).
    // 5) Assume no funcs with name "print"
    // 7) Input programs must not have excessive whitespaces (e.g., "print( varName )")
    // 6) We don't care about speed/large programs support, this is only for our home-made test programs.

    public static ProgramBlock Parse(string input)
    {
        var lineStart = 0;
        var position = 0L;
        return ParseInternal(input.ReplaceLineEndings("\n"), ref lineStart, ref position);
    }

    private static ProgramBlock ParseInternal(ReadOnlySpan<char> input, ref int nextLineStart, ref long position)
    {
        // at least one program block, even for empty program
        var programBlock = new ProgramBlock();
        var lineStart = nextLineStart;
        
        while (true)
        {
            var relativeLineEnd = input[lineStart..].IndexOf("\n");
            var absoluteLineEnd = relativeLineEnd >= 0
                ? lineStart + relativeLineEnd
                : input.Length;
            var line = input[lineStart..absoluteLineEnd];              
            nextLineStart = absoluteLineEnd + 1;
            
            // trim comments
            var commentsPos = line.IndexOf("//", StringComparison.Ordinal);
            if (commentsPos >= 0 && (relativeLineEnd < 0 || relativeLineEnd > commentsPos))
            {
                line = line[..commentsPos].Trim();
            }

            // We've reached the end of the program block
            if (line.IndexOf('}') >= 0)
                break;

            ProcessLine(programBlock, input, line, ref nextLineStart, ref position);

            if (nextLineStart >= input.Length)
                break;

            lineStart = nextLineStart;
        }

        return programBlock;
    }

    private static void ProcessLine(
        ProgramBlock programBlock,
        ReadOnlySpan<char> input, 
        ReadOnlySpan<char> line, 
        ref int nextLineStart,
        ref long position)
    {
        var assignmentPos = line.IndexOf(" = ...", StringComparison.Ordinal);
        if (assignmentPos >= 0)
        {
            var varName = line[0..assignmentPos].Trim().ToString();
            programBlock.Add(new AssignVariable(varName) { Position = position++ });
            return;
        }

        var funcPos = line.IndexOf("func ");
        if (funcPos >= 0)
        {
            if (nextLineStart >= input.Length)
                throw new InvalidOperationException("Cannot parse function declaration on last line");

            var openingPos = line.IndexOf('{');
            var funcName = line[(funcPos + "func ".Length)..openingPos].Trim().ToString();
            programBlock.Add(
                new FunctionDeclaration(funcName) { Position = position++ }
                .WithStatements(ParseInternal(input, ref nextLineStart, ref position)));
            return;
        }

        if (line.Contains("if (...) {", StringComparison.Ordinal))
        {
            if (nextLineStart >= input.Length)
                throw new InvalidOperationException("Cannot parse function declaration on last line");

            programBlock.Add(new IfStatement()
            {
                Position = position++,
                // if-block can contain only a single statement and not an program block
                ThenBody = ParseInternal(input, ref nextLineStart, ref position).First(),
            });
            return;
        }

        var invocationPos = line.IndexOf("()", StringComparison.Ordinal);
        if (invocationPos >= 0)
        {
            var funcName = line[0..invocationPos].Trim().ToString();
            programBlock.Add(new Invocation(funcName) { Position = position++ });
            return;
        }

        var printPos = line.IndexOf("print(");
        if (printPos >= 0)
        {
            var closingPos = line.IndexOf(')');
            var varName = line[(printPos + "print(".Length)..closingPos].ToString();
            programBlock.Add(new PrintVariable(varName) { Position = position++ });
            return;
        }

        var varNamePos = line.IndexOf("var ", StringComparison.Ordinal);
        if (varNamePos >= 0)
        {
            var varName = line[(varNamePos + "var ".Length)..].Trim().ToString();
            programBlock.Add(new VariableDeclaration(varName) { Position = position++ });
            return;
        }
    }
}
