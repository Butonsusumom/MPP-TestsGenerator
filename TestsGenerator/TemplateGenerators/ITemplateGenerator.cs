using System.Collections.Generic;
using TestsGenerator.DataStructures;

namespace TestsGenerator.TemplateGenerators
{
    public interface ITemplateGenerator
    {
        IEnumerable<PathContentPair> Generate(string source);
    }
}
