using System;
using System.Linq.Expressions;
using PCLMock;

namespace TestRobot
{
	/// <summary>
	/// Extension methods for the mocked object that return a <see cref="RobotVerifyContinuation{TRobot, TRobotResult}"/> so the calls can be chained.
	/// </summary>
	public static class VerifyContinuationExtension
	{
		/// <summary>
		/// Extension method that begins a verification specification.
		/// </summary>
		/// <param name="mock">
		/// The mock on which the method gets called.
		/// </param>
		/// <param name="robotResult">
		/// The robot result object that enables the chaining of calls.
		/// </param>
		/// <param name="selector">
		/// An expression that resolves to the member being verified.
		/// </param>
		/// <returns>
		/// A continuation object with which the verification can be specified.
		/// </returns>
		public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TRobot, TRobotResult>(
            this MockBase<T> mock,
            TestRobotResultBase<TRobot, TRobotResult> robotResult,
            Expression<Action<T>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
		{
			return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
		}

		/// <summary>
		/// Extension method that begins a verification specification.
		/// </summary>
		/// <typeparam name="T">
		/// The type being mocked.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type of the property.
		/// </typeparam>
		/// <typeparam name="TRobot"></typeparam>
		/// <typeparam name="TRobotResult"></typeparam>
		/// <param name="mock">
		/// The mock on which the method gets called.
		/// </param>
		/// <param name="robotResult">
		/// The robot result object that enables the chaining of calls.
		/// </param>
		/// <param name="selector">
		/// An expression that resolves to the member being verified.
		/// </param>
		/// <returns>
		/// A continuation object with which the verification can be specified.
		/// </returns>
		public static RobotVerifyContinuation<TRobot, TRobotResult> Verify<T, TMember, TRobot, TRobotResult>(
            this MockBase<T> mock, 
            TestRobotResultBase<TRobot, TRobotResult> robotResult,
            Expression<Func<T, TMember>> selector)
			where TRobot : TestRobotBase<TRobot, TRobotResult>
            where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
        {
            return new RobotVerifyContinuation<TRobot, TRobotResult>(mock.Verify(selector), robotResult);
        }

		/// <summary>
		/// Extension method that begins a verification specification for a property set.
		/// </summary>
		/// <typeparam name="T">
		/// The type being mocked.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type of the property.
		/// </typeparam>
		/// <typeparam name="TRobot"></typeparam>
		/// <typeparam name="TRobotResult"></typeparam>
		/// <param name="mock">
		/// The mock on which the method gets called.
		/// </param>
		/// <param name="robotResult">
		/// The robot result object that enables the chaining of calls.
		/// </param>
		/// <param name="propertySelector">
		/// An expression that resolves to the property being verified.
		/// </param>
		/// <param name="valueFilterSelector">
		/// An optional expression that can provide filtering against the property value being set.
		/// </param>
		/// <returns>
		/// A continuation object with which the verification can be specified.
		/// </returns>
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

	/// <summary>
	/// Facilitates the expression of verifications against a member in a <see cref="MockBase{T}"/>.
	/// </summary>
	public sealed class RobotVerifyContinuation<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
	{
		private readonly VerifyContinuation _verifyContinuation;
		private readonly TRobotResult _robotResult;

		internal RobotVerifyContinuation(
			VerifyContinuation verifyContinuation,
			TestRobotResultBase<TRobot, TRobotResult> robotResult)
		{
			_verifyContinuation = verifyContinuation;
			_robotResult = (TRobotResult) robotResult;
		}

		/// <summary>
		/// Verifies that the member was not called.
		/// </summary>
		public TRobotResult WasNotCalled()
		{
			_verifyContinuation.WasNotCalled();
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member called <paramref name="times"/> or fewer times.
		/// </summary>
		/// <param name="times">
		/// The maximum number of times the member must have been called.
		/// </param>
		public TRobotResult WasCalledAtMost(int times)
		{
			_verifyContinuation.WasCalledAtMost(times);
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member was called <paramref name="times"/> or more times.
		/// </summary>
		/// <param name="times">
		/// The minimum number of times the member must have been called.
		/// </param>
		public TRobotResult WasCalledAtLeast(int times)
		{
			_verifyContinuation.WasCalledAtLeast(times);
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member was called exactly <paramref name="times"/> time.
		/// </summary>
		/// <param name="times">
		/// The number of times the member must have been called.
		/// </param>
		public TRobotResult WasCalledExactly(int times)
		{
			_verifyContinuation.WasCalledExactly(times);
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member was either not called, or only called once.
		/// </summary>
		public TRobotResult WasCalledAtMostOnce()
		{
			_verifyContinuation.WasCalledAtMostOnce();
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member was called exactly one time.
		/// </summary>
		public TRobotResult WasCalledExactlyOnce()
		{
			_verifyContinuation.WasCalledExactlyOnce();
			return _robotResult;
		}

		/// <summary>
		/// Verifies that the member was called one or more times.
		/// </summary>
		public TRobotResult WasCalledAtLeastOnce()
		{
			_verifyContinuation.WasCalledAtLeastOnce();
			return _robotResult;
		}
	}
}
