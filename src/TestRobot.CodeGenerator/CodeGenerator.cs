using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TestRobot.CodeGenerator
{
    [Generator]
    public class CodeGenerator : ISourceGenerator
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
            foreach(var classInfo in mockedClassInfos) {
                Debug.WriteLine($"Write code for mocked class: {classInfo}");

            }

            var sourceBuilder = new StringBuilder(@"
using System;
using System.Linq.Expressions;
using PCLMock;
using Playground.Features;
using Playground.UnitTests.Tests;

namespace TestRobot
{
    public abstract class TestRobotGenerated<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        internal IPokedex _pokedex;

        public override TRobot Build()
        {
            _pokedex ??= CreatePokedexMock();
            return base.Build();
        }

        protected virtual PokedexMock CreatePokedexMock() => new PokedexMock(MockBehavior.Loose);
    }
    
    public abstract class TestRobotResultGenerated<TRobot, TRobotResult> : TestRobotResultBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        private readonly TestRobotGenerated<TRobot, TRobotResult> _autoRobot;

        protected TestRobotResultGenerated(TRobot robot)
            : base(robot)
        {
            _autoRobot = robot as TestRobotGenerated<TRobot, TRobotResult>;
        }
 ");
            sourceBuilder.Append(CreateVerifyMethod());
            sourceBuilder.Append(@"
    }
}");

            context.AddSource("TestRobotGenerated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private string CreateVerifyMethod()
        {
            return @"
        public RobotVerifyContinuation<TRobot, TRobotResult> VerifyPokedexMock(Expression<Action<IPokedex>> selector)
        {
            var mock = (PokedexMock) _autoRobot._pokedex;
            return mock.Verify(this, selector);
        }";
        }
    }
}
