using System;
using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class TestMethodInfo
    {
        public string Name
        { get; protected set; }

        public List<ParameterInfo> Parameters
        { get; protected set; }

        public TypeInfo ReturnType
        { get; protected set; }

        public TestMethodInfo(string name, TypeInfo returnType)
        {
            if ((name == null) || (returnType == null))
            {
                throw new ArgumentException("Arguments shouldn't be null");
            }

            Name = name;
            ReturnType = returnType;
            Parameters = new List<ParameterInfo>();
        }
    }
}
