using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUGGENERATOR
            if (!Debugger.IsAttached) {
                Debugger.Launch();
            }
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder(@"
using System;
using System.Linq.Expressions;
using PCLMock;
using ConsoleApp.Mocks;

namespace ConsoleApp.TestUtils
{
    public abstract class TestRobotGenerated<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        internal AudioRecorderMock _audioRecorder;

        public override TRobot Build()
        {
            _audioRecorder ??= CreateAudioRecorderMock();
            return base.Build();
        }

        protected virtual AudioRecorderMock CreateAudioRecorderMock() => new AudioRecorderMock(MockBehavior.Loose);
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

	public static class VerifyContinuationExtension
	{
		public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TRobot, TRobotResult>(
            this MockBase<T> mock,
            TestRobotResultGenerated<TRobot, TRobotResult> robotResult,
            Expression<Action<T>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
		{
			return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
		}

        public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TMember, TRobot, TRobotResult>(
            this MockBase<T> mock, 
            TestRobotResultGenerated<TRobot, TRobotResult> robotResult,
            Expression<Func<T, TMember>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
        {
            return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
        }

        public static RobotVerifyContinuation<TRobot, TRobotResult> VerifyPropertySet<T, TMember, TRobot, TRobotResult>(
            this MockBase<T> mock, 
            TestRobotResultGenerated<TRobot, TRobotResult> robotResult, 
            Expression<Func<T, TMember>> propertySelector, 
            Expression<Func<TMember>> valueFilterSelector = null)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
        {
            return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.VerifyPropertySet(propertySelector, valueFilterSelector), robotResult);
        }
	}

    public sealed class RobotVerifyContinuation<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
	{
		private readonly VerifyContinuation _verifyContinuation;
		private readonly TestRobotResultGenerated<TRobot, TRobotResult> _robotResult;

		public RobotVerifyContinuation(
			VerifyContinuation verifyContinuation,
			TestRobotResultGenerated<TRobot, TRobotResult> robotResult)
		{
			_verifyContinuation = verifyContinuation;
			_robotResult = robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasNotCalled()
		{
			_verifyContinuation.WasNotCalled();
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledAtMost(int times)
		{
			_verifyContinuation.WasCalledAtMost(times);
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledAtLeast(int times)
		{
			_verifyContinuation.WasCalledAtLeast(times);
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledExactly(int times)
		{
			_verifyContinuation.WasCalledExactly(times);
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledAtMostOnce()
		{
			_verifyContinuation.WasCalledAtMostOnce();
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledExactlyOnce()
		{
			_verifyContinuation.WasCalledExactlyOnce();
			return _robotResult;
		}

		public TestRobotResultGenerated<TRobot, TRobotResult> WasCalledAtLeastOnce()
		{
			_verifyContinuation.WasCalledAtLeastOnce();
			return _robotResult;
		}
	}
}");

            context.AddSource("generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private string CreateVerifyMethod()
        {
            return @"
        public RobotVerifyContinuation<TRobot, TRobotResult> VerifyAudioRecorderMock(Expression<Action<AudioRecorderMock>> selector)
        {
            return _autoRobot._audioRecorder.Verify(this, selector);
        }";
        }
    }
}
