using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PCLMock;

namespace TestRobot
{
	/// <summary>
	/// Extension methods to facilitate setting up the <see cref="WhenContinuation{TMock, TMember}"/> objects of a mocked object.
	/// </summary>
	public static class PCLMockExtension
	{
		/// <summary>
		/// Requests that the given exception will be thrown if the member is accessed.
		/// </summary>
		/// <param name="this">
		/// The object the method gets invoked on.
		/// </param>
		/// <param name="e">
		/// The exception that should be thrown.
		/// </param>
		/// <typeparam name="TMock">
		/// The type of the object being mocked.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type being returned by the member being specified.
		/// </typeparam>
		public static void ThrowObservable<TMock, TMember>(this WhenContinuation<TMock, IObservable<TMember>> @this, Exception e) 
		{
			@this.Return(Observable.Throw<TMember>(e));
		}

		/// <summary>
		/// Requests that a specified value will be returned as observable if the member is accessed. 
		/// </summary>
		/// <param name="this">
		/// The object the method gets invoked on.
		/// </param>
		/// <param name="value">
		/// The value to return.
		/// </param>
		/// <typeparam name="TMock">
		/// The type of the object being mocked.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type being returned by the member being specified.
		/// </typeparam>
		public static void ReturnsObservable<TMock, TMember>(this WhenContinuation<TMock, IObservable<TMember>> @this, TMember value) 
		{
			@this.Return(Observable.Return(value));
		}

        /// <summary>
        /// Requests that a unit as observable will be returned if the member is accessed.
        /// </summary>
		/// <param name="this">
		/// The object the method gets invoked on.
		/// </param>
		/// <typeparam name="TMock">
		/// The type of the object being mocked.
		/// </typeparam>
        public static void ReturnsUnit<TMock>(this WhenContinuation<TMock, IObservable<Unit>> @this) 
        {
            @this.Return(Observable.Return(Unit.Default));
        }

		/// <summary>
        /// Requests that a specified value will be returned as task if the member is accessed.
		/// </summary>
		/// <param name="this">
		/// The object the method gets invoked on.
		/// </param>
		/// <param name="value">
		/// The value to return.
		/// </param>
		/// <typeparam name="TMock">
		/// The type of the object being mocked.
		/// </typeparam>
		/// <typeparam name="TMember">
		/// The type being returned by the member being specified.
		/// </typeparam>
		public static void ReturnsAsync<TMock, TMember>(this WhenContinuation<TMock, Task<TMember>> @this, TMember value) 
		{
			@this.Return(Task.FromResult(value));
		}

		/// <summary>
        /// Requests that a completed task will be returned if the member is accessed.
		/// </summary>
		/// <param name="this">
		/// The object the method gets invoked on.
		/// </param>
		/// <typeparam name="TMock">
		/// The type of the object being mocked.
		/// </typeparam>
		public static void ReturnsAsync<TMock>(this WhenContinuation<TMock, Task> @this) 
		{
			@this.Return(Task.CompletedTask);
		}
	}
}
