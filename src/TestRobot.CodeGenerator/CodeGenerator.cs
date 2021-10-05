using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TestRobot.CodeGenerator
{
    [Generator]
    internal class CodeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUGGENERATOR
            if (!Debugger.IsAttached) {
                Debugger.Launch();
            }
#endif

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if(context.SyntaxContextReceiver is not SyntaxReceiver receiver)
                return;

            var codeWriter = new CodeWriter();
            var mockedClassInfos = receiver.MockedClassInfos;
            var usingStatements = mockedClassInfos
                .Select(x => x.MockNamespace)
                .Concat(mockedClassInfos.Select(x => x.MockedInterfaceNamespace))
                .Distinct()
                .Select(x => $"using {x};");
            
            codeWriter.AppendLine("using System;");
            codeWriter.AppendLine("using System.Linq.Expressions;");
            codeWriter.AppendLine("using PCLMock;");
            codeWriter.AppendLines(usingStatements);
            codeWriter.AppendLine();
            codeWriter.AppendLine("namespace TestRobot");
            codeWriter.AppendLine("{");
            codeWriter.AppendLineWithIndent(1, "/// <summary>");
            codeWriter.AppendLineWithIndent(1, "/// This is the base class for applying the Robot Pattern that facilitates writing more stable, readable and maintainable tests by");
            codeWriter.AppendLineWithIndent(1, "/// following the AAA Pattern (Arrange-Act-Assert) and making use of the Builder Pattern. It autogenerates internal instances of mocks based");
            codeWriter.AppendLineWithIndent(1, "/// on classes that end with *mock available in your testing code base. ");
            codeWriter.AppendLineWithIndent(1, "/// </summary>");
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobot<TSut, TRobot, TRobotResult> : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"private {x.MockName} {x.NameOfMockedInterfaceAsField};"));
            codeWriter.AppendLine();
            codeWriter.AppendLines(mockedClassInfos.Select(CreateWithMockMethod));
            codeWriter.AppendLineWithIndent(2, "public override TRobot Build()");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLinesWithIndent(3, mockedClassInfos.Select(x => $"{x.NameOfMockedInterfaceAsProperty} = {x.NameOfMockedInterfaceAsField} ?? Create{x.MockName}();"));
            codeWriter.AppendLineWithIndent(3, "return base.Build();");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "protected override TRobotResult CreateResult()");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, "return (TRobotResult) Activator.CreateInstance(typeof(TRobotResult), Sut, this);");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLines(mockedClassInfos.Select(CreateMockCreationMethod));
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"internal {x.MockName} {x.NameOfMockedInterfaceAsProperty} {{ get; private set; }}"));
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(1, "/// <summary>");
            codeWriter.AppendLineWithIndent(1, "/// Autogenerated class that is the corresponding part to the <see cref=\"AutoTestRobot{TSut,TRobot,TRobotResult}\"/>. It is responsible");
            codeWriter.AppendLineWithIndent(1, "/// for the assertion and verification of the test's outcome by providing autogenerated <see cref=\"RobotVerifyContinuation{TSut,TRobot,TRobotResult}\"/>");
            codeWriter.AppendLineWithIndent(1, "/// methods which are based on classes that end with *Mock available in your testing code base.");
            codeWriter.AppendLineWithIndent(1, "/// </summary>");
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobotResult<TSut, TRobot, TRobotResult> : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLineWithIndent(2, "private readonly AutoTestRobot<TSut, TRobot, TRobotResult> _autoRobot;");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "/// <summary>Constructor</summary>");
            codeWriter.AppendLineWithIndent(2, "/// <param name=\"sut\">The System Under Test (SUT).</param>");
            codeWriter.AppendLineWithIndent(2, "/// <param name=\"robot\">The corresponding TestRobot.</param>");
            codeWriter.AppendLineWithIndent(2, "protected AutoTestRobotResult(TSut sut, TRobot robot)");
            codeWriter.AppendLineWithIndent(3, ": base(sut)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, "_autoRobot = robot as AutoTestRobot<TSut, TRobot, TRobotResult>;");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLines(mockedClassInfos.Select(CreateVerifyMockMethod));
            codeWriter.AppendLineWithIndent(2, "/// <summary>The TestRobot to access internal fields for assertion or verification of the test's outcome.</summary>");
            codeWriter.AppendLineWithIndent(2, "/// <exception cref=\"ArgumentException\">");
            codeWriter.AppendLineWithIndent(2, "/// Will be thrown when the TestRobot that was passed via the constructor fits to the TestRobot given");
            codeWriter.AppendLineWithIndent(2, "/// by the type parameter.");
            codeWriter.AppendLineWithIndent(2, "/// </exception>");
            codeWriter.AppendLineWithIndent(2, "protected TRobot Robot => _autoRobot as TRobot ?? throw new ArgumentException($\"Passed robot must be of type {nameof(AutoTestRobot<TSut, TRobot, TRobotResult>)}\");");
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine("}");

            context.AddSource("AutoTestRobot.cs", SourceText.From(codeWriter.ToString(), Encoding.UTF8));
        }

        private static string CreateWithMockMethod(MockedClassInfo classInfo)
        {
            var codeWriter = new CodeWriter();
            codeWriter.AppendLineWithIndent(2, $"/// <summary>Injects an particular instance of <see cref=\"{classInfo.MockNamespace}.{classInfo.MockName}\"/> that can be used for a specific test case if needed.</summary>");
            codeWriter.AppendLineWithIndent(2, $"/// <param name=\"value\">The instance of the <see cref=\"{classInfo.MockNamespace}.{classInfo.MockName}\"/>.</param>");
            codeWriter.AppendLineWithIndent(2, $"/// <returns>The current TestRobot.</returns>");
            codeWriter.AppendLineWithIndent(2, $"public TRobot With{classInfo.MockName}({classInfo.MockName} value) =>");
            codeWriter.AppendLineWithIndent(3, $"With(ref {classInfo.NameOfMockedInterfaceAsField}, value);");
            return codeWriter.ToString();
        }

        private static string CreateMockCreationMethod(MockedClassInfo classInfo)
        {
            var codeWriter = new CodeWriter();
            codeWriter.AppendLineWithIndent(2, $"/// <summary>Creates an instance of a <see cref=\"{classInfo.MockNamespace}.{classInfo.MockName}\"/>. Override this method to adapt the mocks behaviour to your specific needs.</summary>");
            codeWriter.AppendLineWithIndent(2, $"/// <returns>The default instance of the <see cref=\"{classInfo.MockNamespace}.{classInfo.MockName}\"/> with <see cref=\"MockBehavior\"/> set to <see cref=\"MockBehavior.Loose\"/>.</returns>");
            codeWriter.AppendLineWithIndent(2, $"protected virtual {classInfo.MockName} Create{classInfo.MockName}() => new {classInfo.MockName}(MockBehavior.Loose);");
            return codeWriter.ToString();
        }

        private static string CreateVerifyMockMethod(MockedClassInfo classInfo)
        {
            var codeWriter = new CodeWriter();
            codeWriter.AppendLineWithIndent(2, $"/// <summary>Enables verification of calls to the mocked instance of <see cref=\"{classInfo.MockedInterfaceNamespace}.{classInfo.MockedInterfaceName}\"/>.</summary>");
            codeWriter.AppendLineWithIndent(2, "/// <param name=\"selector\">An expression that resolves to the member being verified.</param>");
            codeWriter.AppendLineWithIndent(2, "/// <returns>A continuation object with which the verification can be specified.</returns>");
            codeWriter.AppendLineWithIndent(2, $"public RobotVerifyContinuation<TSut, TRobot, TRobotResult> Verify{classInfo.MockName}(Expression<Action<{classInfo.MockedInterfaceName}>> selector)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, $"var mock = ({classInfo.MockName}) _autoRobot.{classInfo.NameOfMockedInterfaceAsProperty};");
            codeWriter.AppendLineWithIndent(3, "return mock.Verify(this, selector);");
            codeWriter.AppendLineWithIndent(2, "}");
            return codeWriter.ToString();
        }
    }
}
