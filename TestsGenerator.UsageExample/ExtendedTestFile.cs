﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClassNamespace.Class1
{
    public class TestClass1
    {
        public int GetInt()
        {
            return 0;
        }

        public void DoSomething(int param1, double param2)
        { }

        public TestClass1(int param)
        { }

        public TestClass1(int p1, double p2)
        { }

        public TestClass1()
        { }
    }
}

namespace TestClassNamespace.Class2
{
    public interface IMyTestInterface
    { }

    public class TestClass2
    {
        public IMyTestInterface GetInterface()
        {
            return null;
        }

        public void SetInterface(IMyTestInterface myInt)
        { }

        protected IMyTestInterface GetProtectedInterface()
        {
            return null;
        }

        public TestClass2(IMyTestInterface myTestInterface)
        { }
    }
}
