

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using TestsGenerator.IO;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;

namespace TestsGenerator.UnitTests
{
    [TestFixture]
    public class TestsGeneratorUnitTests
    {
        private CompilationUnitSyntax class1Root, class2Root;

        [SetUp]
        public void TestInit()
        {
            string testFilesDirectory = "../../";
            string testFilePath = testFilesDirectory + "TestFile.cs";
            string testClass1FilePath = testFilesDirectory + "TestClass1Test.cs";
            string testClass2FilePath = testFilesDirectory + "TestClass2Test.cs";

            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    testFilePath
                },
                Writer = new AsyncFileWriter()
                {
                    Directory = testFilesDirectory
                }
            };

            new TestsGenerator(config).Generate().Wait();

           // class1Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass1FilePath)).GetCompilationUnitRoot();
           // class2Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass2FilePath)).GetCompilationUnitRoot();
        }

       /* [Test]
        public void ExceptionThrowingTest()
        {
            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    "NonExistingFile.cs"
                }
            };

            Assert.ThrowsException<AggregateException>(() => new TestsGenerator(config).Generate().Wait());
        }*/

        [Test]
        public void UsingTests()
        {
            List<string> expected = new List<string>
            {
                "Microsoft.VisualStudio.TestTools.UnitTesting",
                "Moq",
                "System",
                "TestClassNamespace.Class1"
            };
            CollectionAssert.IsSubsetOf(expected, class1Root.Usings.Select(x => x.Name.ToString()).ToList());

            Assert.AreEqual(1, class2Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "TestClassNamespace.Class2").Count());
        }

        [Test]
        public void NamespaceTest()
        {
            IEnumerable<NamespaceDeclarationSyntax> namespaces;

            namespaces = class1Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual("TestClassNamespace.Class1.Test", namespaces.First().Name.ToString());

            namespaces = class2Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual("TestClassNamespace.Class2.Test", namespaces.First().Name.ToString());
        }

        [Test]
        public void ClTest()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ClassTest()
        {
            IEnumerable<ClassDeclarationSyntax> classes;

            classes = class1Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            Assert.AreEqual(1, classes.Count());
            Assert.AreEqual("TestClass1Test", classes.First().Identifier.ToString());

            classes = class2Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            Assert.AreEqual(1, classes.Count());
            Assert.AreEqual("TestClass2Test", classes.First().Identifier.ToString());
        }

        [Test]
        public void ClassAttributeTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>().Count(classDeclaration => Enumerable.Any<AttributeListSyntax>(classDeclaration.AttributeLists, (attributeList) => attributeList.Attributes
                    .Any((attribute) => attribute.Name.ToString() == "TestClass"))));
        }

        [Test]
        public void MethodsTest()
        {
            List<string> expected = new List<string>
            {
                "TestInitialize",
                "GetInterfaceTest",
                "SetInterfaceTest"
            };
            List<string> actual = class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>().Select((method) => method.Identifier.ToString()).ToList();

            CollectionAssert.AreEquivalent(expected, class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>().Select((method) => method.Identifier.ToString()).ToList());
            IsFalse(actual.Contains("GetProtectedInterfaceTest"));
        }

        [Test]
        public void MethodAttributeTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods = class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            Assert.AreEqual(2, methods.Where((methodDeclaration) => methodDeclaration.AttributeLists
                .Any((attributeList) => attributeList.Attributes.Any((attribute) => attribute.Name.ToString() == "TestMethod")))
                .Count());
            Assert.AreEqual(1, methods.Where((methodDeclaration) => methodDeclaration.AttributeLists
                .Any((attributeList) => attributeList.Attributes.Any((attribute) => attribute.Name.ToString() == "TestInitialize")))
                .Count());
        }

        [Test]
        public void MockTest()
        {
            Assert.AreEqual(1, class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Where((statement) => statement.ToFullString().Contains("new Mock")).Count());
        }

        [Test]
        public void ClassInitTest()
        {
            Assert.AreEqual(1, class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToFullString().Contains("new TestClass2")).Count());
        }

        [Test]
        public void ActualTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>().Count(statement => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "actual")));
        }

        [Test]
        public void ExpectedTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Where((statement) => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "expected")).Count());
        }

        [Test]
        public void AreEqualTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToString().Contains("Assert.AreEqual(expected, actual)")).Count());
        }

        [Test]
        public void FailTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToString().Contains("Assert.Fail")).Count());
        }

        [Test]
        public void ArgumentsInitializationTest()
        {
            List<string> expected = new List<string>()
            {
                "param1",
                "param2"
            };

            List<string> actual = class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "DoSomethingTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>().Select((declaration) => declaration.Declaration.Variables)
                .SelectMany((declaration) => declaration.ToList()).Select((variableDeclaration) => variableDeclaration.Identifier.ToString()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
