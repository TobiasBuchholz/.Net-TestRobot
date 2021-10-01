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
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobot<TSut, TRobot, TRobotResult> : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"internal {x.MockedInterfaceName} {x.MockedInterfaceAsFieldName};"));
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "public override TRobot Build()");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLinesWithIndent(3, mockedClassInfos.Select(x => $"{x.MockedInterfaceAsFieldName} = Create{x.MockName}();"));
            codeWriter.AppendLineWithIndent(3, "return base.Build();");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "protected override TRobotResult CreateResult()");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, "return (TRobotResult) Activator.CreateInstance(typeof(TRobotResult), Sut, this);");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"protected virtual {x.MockName} Create{x.MockName}() => new {x.MockName}(MockBehavior.Loose);"));
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobotResult<TSut, TRobot, TRobotResult> : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLineWithIndent(2, "private readonly AutoTestRobot<TSut, TRobot, TRobotResult> _autoRobot;");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "protected AutoTestRobotResult(TSut sut, TRobot robot)");
            codeWriter.AppendLineWithIndent(3, ": base(sut)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, "_autoRobot = robot as AutoTestRobot<TSut, TRobot, TRobotResult>;");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLines(mockedClassInfos.Select(CreateVerifyMockMethod));
            codeWriter.AppendLineWithIndent(2, "protected TRobot Robot => _autoRobot as TRobot ?? throw new ArgumentException($\"Passed robot must be of type {nameof(AutoTestRobot<TSut, TRobot, TRobotResult>)}\");");
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine("}");

            context.AddSource("AutoTestRobot.cs", SourceText.From(codeWriter.ToString(), Encoding.UTF8));
        }

        private static string CreateVerifyMockMethod(MockedClassInfo classInfo)
        {
            var codeWriter = new CodeWriter();
            codeWriter.AppendLineWithIndent(2, $"public RobotVerifyContinuation<TSut, TRobot, TRobotResult> Verify{classInfo.MockName}(Expression<Action<{classInfo.MockedInterfaceName}>> selector)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, $"var mock = ({classInfo.MockName}) _autoRobot.{classInfo.MockedInterfaceAsFieldName};");
            codeWriter.AppendLineWithIndent(3, "return mock.Verify(this, selector);");
            codeWriter.AppendLineWithIndent(2, "}");
            return codeWriter.ToString();
        }
    }
}
