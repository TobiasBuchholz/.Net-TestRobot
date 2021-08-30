using System;
using System.Linq.Expressions;
using PCLMock;

namespace Plugin.TestRobot
{
	public static class VerifyContinuationExtension
	{
		public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TRobot, TRobotResult>(
            this MockBase<T> mock,
            TestRobotResultBase<TRobot, TRobotResult> robotResult,
            Expression<Action<T>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
		{
			return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
		}

        public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TMember, TRobot, TRobotResult>(
            this MockBase<T> mock, 
            TestRobotResultBase<TRobot, TRobotResult> robotResult,
            Expression<Func<T, TMember>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
        {
            return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
        }

        public static RobotVerifyContinuation<TRobot, TRobotResult> VerifyPropertySet<T, TMember, TRobot, TRobotResult>(
            this MockBase<T> mock, 
            TestRobotResultBase<TRobot, TRobotResult> robotResult, 
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
		private readonly TestRobotResultBase<TRobot, TRobotResult> _robotResult;

		public RobotVerifyContinuation(
			VerifyContinuation verifyContinuation,
			TestRobotResultBase<TRobot, TRobotResult> robotResult)
		{
			_verifyContinuation = verifyContinuation;
			_robotResult = robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasNotCalled()
		{
			_verifyContinuation.WasNotCalled();
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledAtMost(int times)
		{
			_verifyContinuation.WasCalledAtMost(times);
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledAtLeast(int times)
		{
			_verifyContinuation.WasCalledAtLeast(times);
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledExactly(int times)
		{
			_verifyContinuation.WasCalledExactly(times);
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledAtMostOnce()
		{
			_verifyContinuation.WasCalledAtMostOnce();
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledExactlyOnce()
		{
			_verifyContinuation.WasCalledExactlyOnce();
			return _robotResult;
		}

		public TestRobotResultBase<TRobot, TRobotResult> WasCalledAtLeastOnce()
		{
			_verifyContinuation.WasCalledAtLeastOnce();
			return _robotResult;
		}
	}
}
