using System;

namespace TestsGenerator.DataStructures
{
    public class ParameterInfo
    {
        public string Name
        { get; protected set; }

        public TypeInfo Type
        { get; protected set; }

        public ParameterInfo(string name, TypeInfo type)
        {
            Name = name ?? throw new ArgumentException("Name shouldn't be null");
            Type = type ?? throw new ArgumentException("Type shouldn't be null");
        }
    }
}
