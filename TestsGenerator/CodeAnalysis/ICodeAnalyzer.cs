using TestsGenerator.DataStructures;

namespace TestsGenerator.CodeAnalysis
{
    public interface ICodeAnalyzer
    {
        TestFileInfo Analyze(string code);
    }
}
