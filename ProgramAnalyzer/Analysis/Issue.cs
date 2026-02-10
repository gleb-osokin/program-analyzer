using ProgramAnalyzer.Statements;

namespace ProgramAnalyzer.Analysis;

public record Issue(string Message, Statement Statement);
