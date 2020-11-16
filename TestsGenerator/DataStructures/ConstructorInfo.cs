using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class ConstructorInfo
    {
        public List<ParameterInfo> Parameters
        { get; protected set; }

        public ConstructorInfo()
        {
            Parameters = new List<ParameterInfo>();
        }
    }
}
