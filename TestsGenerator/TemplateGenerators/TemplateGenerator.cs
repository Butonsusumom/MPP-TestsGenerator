using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using TestsGenerator.CodeAnalysis;
using TestsGenerator.DataStructures;

namespace TestsGenerator.TemplateGenerators
{
    public class TemplateGenerator : ITemplateGenerator
    {
        protected readonly SyntaxToken emptyLineToken;
        protected readonly ExpressionStatementSyntax failExpression;

        protected const string actualVariableName = "actual";
        protected const string expectedVariableName = "expected";

        protected readonly ICodeAnalyzer codeAnalyzer;

        public IEnumerable<PathContentPair> Generate(string source)
        {
            if (source == null)
            {
                throw new ArgumentException("Source shouldn't be null");
            }

            List<PathContentPair> resultList = new List<PathContentPair>();

            TestFileInfo fileInfo = codeAnalyzer.Analyze(source);
            List<UsingDirectiveSyntax> commonUsings = fileInfo.Usings.Select((usingStr) => UsingDirective(IdentifierName(usingStr))).ToList();
            commonUsings.Add(UsingDirective(IdentifierName("Microsoft.VisualStudio.TestTools.UnitTesting")));
            commonUsings.Add(UsingDirective(IdentifierName("Moq")));

            foreach (TestClassInfo typeInfo in fileInfo.Classes)
            {
                resultList.Add(new PathContentPair(typeInfo.Name + "Test.cs", CompilationUnit()
                    .WithUsings(
                        List<UsingDirectiveSyntax>(
                            CreateClassUsings(typeInfo, commonUsings)))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            CreateTestClassWithNamespaceDeclaration(typeInfo)))
                    .NormalizeWhitespace().ToFullString()));
            }

            return resultList;
        }

        protected UsingDirectiveSyntax[] CreateClassUsings(TestClassInfo typeInfo, List<UsingDirectiveSyntax> fileUsings)
        {
            return new List<UsingDirectiveSyntax>(fileUsings)
            {
                UsingDirective(IdentifierName(typeInfo.Namespace))
            }.ToArray();
        }

        protected MemberDeclarationSyntax CreateTestClassWithNamespaceDeclaration(TestClassInfo typeInfo)
        {
            return NamespaceDeclaration(
                IdentifierName(typeInfo.Namespace + ".Test"))
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    CreateClassDeclaration(typeInfo)));
        }

        protected ClassDeclarationSyntax CreateClassDeclaration(TestClassInfo typeInfo)
        {
            return ClassDeclaration(typeInfo.Name + "Test")
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestClass"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithMembers(
                List<MemberDeclarationSyntax>(
                    new MemberDeclarationSyntax[]{
                        CreateTestClassInstanceFieldDeclaration(typeInfo).WithSemicolonToken(emptyLineToken),
                        CreateTestInitializeMethodDeclaration(typeInfo)
                    }.Concat(typeInfo.Methods.Select((methodInfo) => CreateTestMethodDeclaration(methodInfo, typeInfo)))));
        }

        protected MethodDeclarationSyntax CreateTestMethodDeclaration(TestMethodInfo methodInfo, TestClassInfo classInfo)
        {
            List<LocalDeclarationStatementSyntax> arrangeBody = methodInfo.Parameters.Select((parameter) => CreateVariableInitializeExpression(parameter)).ToList();

            if (arrangeBody.Count != 0)
            {
                arrangeBody[arrangeBody.Count - 1] = arrangeBody[arrangeBody.Count - 1].WithSemicolonToken(emptyLineToken);
            }

            List<StatementSyntax> actAssertBody = new List<StatementSyntax>();

            if (methodInfo.ReturnType.Typename != "void")
            {
                actAssertBody.Add(CreateActualDeclaration(methodInfo, classInfo).WithSemicolonToken(emptyLineToken));
                actAssertBody.Add(CreateExpectedDeclaration(methodInfo.ReturnType));
                actAssertBody.Add(CreateAreEqualExpression(methodInfo.ReturnType));
            }
            actAssertBody.Add(failExpression);

            return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier(methodInfo.Name + "Test"))
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestMethod"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithBody(
                Block(
                    arrangeBody.Concat(actAssertBody)));
        }

        protected string CreateVariableName(string parameterName, bool isTestInstance = false)
        {
            return char.ToLower(parameterName[0]) + (parameterName.Length == 1 ? string.Empty : parameterName.Substring(1)) + (isTestInstance ? "TestInstance" : string.Empty);
        }

        protected FieldDeclarationSyntax CreateTestClassInstanceFieldDeclaration(TestClassInfo classInfo)
        {
            return FieldDeclaration(
                VariableDeclaration(
                    IdentifierName(classInfo.Name))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(CreateVariableName(classInfo.Name, true))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PrivateKeyword)));
        }

        protected MethodDeclarationSyntax CreateTestInitializeMethodDeclaration(TestClassInfo classInfo)
        {
            return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier("TestInitialize"))
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestInitialize"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithBody(
                Block(
                    classInfo.Constructor.Parameters.Select((parameter) => CreateVariableInitializeExpression(parameter))
                        .Concat(new StatementSyntax[] { CreateTestClassInitializeExpression(classInfo) })
                    ));
        }

        protected LocalDeclarationStatementSyntax CreateVariableInitializeExpression(ParameterInfo parameterInfo)
        {
            ExpressionSyntax initializer;
            if (parameterInfo.Type.IsInterface)
            {
                initializer = ObjectCreationExpression(
                    GenericName(
                        Identifier("Mock"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(parameterInfo.Type.Typename)))))
                .WithArgumentList(
                    ArgumentList());
            }
            else
            {
                initializer = DefaultExpression(IdentifierName(parameterInfo.Type.Typename));
            }

            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName(parameterInfo.Type.Typename))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(CreateVariableName(parameterInfo.Name)))
                        .WithInitializer(
                            EqualsValueClause(
                                initializer)))));
        }

        protected ExpressionStatementSyntax CreateTestClassInitializeExpression(TestClassInfo classInfo)
        {
            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(CreateVariableName(classInfo.Name, true)),
                    ObjectCreationExpression(
                        IdentifierName(classInfo.Name))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                CreateArguments(classInfo.Constructor.Parameters))))));
        }

        protected List<SyntaxNodeOrToken> CreateArguments(IList<ParameterInfo> parameters)
        {
            SyntaxToken commaToken = Token(SyntaxKind.CommaToken);
            List<SyntaxNodeOrToken> arguments = new List<SyntaxNodeOrToken>();

            if (parameters.Count > 0)
            {
                arguments.Add(CreateArgument(parameters[0]));
            }

            for (int i = 1; i < parameters.Count; ++i)
            {
                arguments.Add(commaToken);
                arguments.Add(CreateArgument(parameters[i]));
            }

            return arguments;
        }

        protected SyntaxNodeOrToken CreateArgument(ParameterInfo parameterInfo)
        {
            if (parameterInfo.Type.IsInterface)
            {
                return Argument(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(CreateVariableName(parameterInfo.Name)),
                        IdentifierName("Object")));
            }
            else
            {
                return Argument(IdentifierName(CreateVariableName(parameterInfo.Name)));
            }
        }

        protected LocalDeclarationStatementSyntax CreateActualDeclaration(TestMethodInfo methodInfo, TestClassInfo classInfo)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName(methodInfo.ReturnType.Typename))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(actualVariableName))
                        .WithInitializer(
                            EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(CreateVariableName(classInfo.Name, true)),
                                        IdentifierName(methodInfo.Name)))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            CreateArguments(methodInfo.Parameters)))))))));
        }

        protected LocalDeclarationStatementSyntax CreateExpectedDeclaration(DataStructures.TypeInfo methodReturnType)
        {
            return CreateVariableInitializeExpression(new ParameterInfo(expectedVariableName, methodReturnType));
        }

        protected ExpressionStatementSyntax CreateAreEqualExpression(DataStructures.TypeInfo methodReturnType)
        {
            return ExpressionStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"),
                        IdentifierName("AreEqual")))
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]{
                                CreateArgument(new ParameterInfo(expectedVariableName, methodReturnType)),
                                Token(SyntaxKind.CommaToken),
                                Argument(
                                    IdentifierName(actualVariableName))}))));
        }

        protected ExpressionStatementSyntax CreateFailExpression()
        {
            return ExpressionStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"),
                        IdentifierName("Fail")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal("autogenerated")))))));
        }

        protected SyntaxToken CreateEmptyLineToken()
        {
            return Token(
                TriviaList(),
                SyntaxKind.SemicolonToken,
                TriviaList(
                    Trivia(
                        SkippedTokensTrivia()
                        .WithTokens(
                            TokenList(
                                BadToken(
                                    TriviaList(),
                                    "\n",
                                    TriviaList()))))));
        }

        public TemplateGenerator(ICodeAnalyzer codeAnalyzer)
        {
            this.codeAnalyzer = codeAnalyzer ?? throw new ArgumentException("Code analyzer shouldn't be null");

            emptyLineToken = CreateEmptyLineToken();
            failExpression = CreateFailExpression();
        }
    }
}
