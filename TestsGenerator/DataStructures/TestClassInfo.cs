using System;
using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class TestClassInfo
    {
        public List<TestMethodInfo> Methods
        { get; protected set; }

        public string Namespace
        { get; protected set; }

        public string Name
        { get; protected set; }

        public ConstructorInfo Constructor
        { get; protected set; }

        public TestClassInfo(string name, string @namespace, ConstructorInfo constructorInfo)
        {
            if ((name == null) || (@namespace == null) || (constructorInfo == null))
            {
                throw new ArgumentException("Arguments shouldn't be null");
            }

            Name = name;
            Namespace = @namespace;
            Methods = new List<TestMethodInfo>();
            Constructor = constructorInfo;
        }
    }
}
