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
            codeWriter.AppendLine("using System.Reactive.Concurrency;");
            codeWriter.AppendLines(usingStatements);
            codeWriter.AppendLine();
            codeWriter.AppendLine("namespace TestRobot");
            codeWriter.AppendLine("{");
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobot<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"internal {x.MockedInterfaceName} {x.MockedInterfaceAsFieldName};"));
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "protected override TRobot Build(IScheduler scheduler)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLinesWithIndent(3, mockedClassInfos.Select(x => $"{x.MockedInterfaceAsFieldName} ??= Create{x.MockName}();"));
            codeWriter.AppendLineWithIndent(3, "return base.Build(scheduler);");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"protected virtual {x.MockName} Create{x.MockName}() => new {x.MockName}(MockBehavior.Loose);"));
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobotResult<TRobot, TRobotResult> : TestRobotResultBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLineWithIndent(2, "private readonly AutoTestRobot<TRobot, TRobotResult> _autoRobot;");
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "protected AutoTestRobotResult(TRobot robot)");
            codeWriter.AppendLineWithIndent(3, ": base(robot)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, "_autoRobot = robot as AutoTestRobot<TRobot, TRobotResult>;");
            codeWriter.AppendLineWithIndent(2, "}");
            codeWriter.AppendLine();
            codeWriter.AppendLines(mockedClassInfos.Select(CreateVerifyMockMethod));
            codeWriter.AppendLineWithIndent(1, "}");
            codeWriter.AppendLine("}");

            context.AddSource("AutoTestRobot.cs", SourceText.From(codeWriter.ToString(), Encoding.UTF8));
        }

        private static string CreateVerifyMockMethod(MockedClassInfo classInfo)
        {
            var codeWriter = new CodeWriter();
            codeWriter.AppendLineWithIndent(2, $"public RobotVerifyContinuation<TRobot, TRobotResult> Verify{classInfo.MockName}(Expression<Action<{classInfo.MockedInterfaceName}>> selector)");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLineWithIndent(3, $"var mock = ({classInfo.MockName}) _autoRobot.{classInfo.MockedInterfaceAsFieldName};");
            codeWriter.AppendLineWithIndent(3, "return mock.Verify(this, selector);");
            codeWriter.AppendLineWithIndent(2, "}");
            return codeWriter.ToString();
        }
    }
}
