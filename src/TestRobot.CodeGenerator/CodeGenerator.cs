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
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            var mockedClassInfos = receiver.MockedClassInfos;
            var codeWriter = new CodeWriter();
            codeWriter.AppendLine("using System;");
            codeWriter.AppendLine("using System.Linq.Expressions;");
            codeWriter.AppendLine("using PCLMock;");
            codeWriter.AppendLines(mockedClassInfos.Select(x => $"using {x.MockNamespace};"));
            codeWriter.AppendLines(mockedClassInfos.Select(x => $"using {x.MockedInterfaceNamespace};"));
            codeWriter.AppendLine();
            codeWriter.AppendLine("namespace TestRobot");
            codeWriter.AppendLine("{");
            codeWriter.AppendLineWithIndent(1, "public abstract class AutoTestRobot<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobot : TestRobotBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(2, "where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>");
            codeWriter.AppendLineWithIndent(1, "{");
            codeWriter.AppendLinesWithIndent(2, mockedClassInfos.Select(x => $"internal {x.MockedInterfaceName} {x.MockedInterfaceAsFieldName};"));
            codeWriter.AppendLine();
            codeWriter.AppendLineWithIndent(2, "public override TRobot Build()");
            codeWriter.AppendLineWithIndent(2, "{");
            codeWriter.AppendLinesWithIndent(3, mockedClassInfos.Select(x => $"{x.MockedInterfaceAsFieldName} ??= Create{x.MockName}();"));
            codeWriter.AppendLineWithIndent(3, "return base.Build();");
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
