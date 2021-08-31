﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PCLMock;

namespace Ablaze.UnitTests.Extensions
{
	public static class PCLMockExtension
	{
		public static void ThrowObservable<TMock, TMember>(this WhenContinuation<TMock, IObservable<TMember>> @this, Exception e) 
		{
			@this.Return(Observable.Throw<TMember>(e));
		}

		public static void ReturnsObservable<TMock, TMember>(this WhenContinuation<TMock, IObservable<TMember>> @this, TMember member) 
		{
			@this.Return(Observable.Return(member));
		}

        public static void ReturnsUnit<TMock>(this WhenContinuation<TMock, IObservable<Unit>> @this) 
        {
            @this.Return(Observable.Return(Unit.Default));
        }

		public static void ReturnsAsync<TMock, TMember>(this WhenContinuation<TMock, Task<TMember>> @this, TMember member) 
		{
			@this.Return(Task.FromResult(member));
		}

		public static void ReturnsAsync<TMock>(this WhenContinuation<TMock, Task> @this) 
		{
			@this.Return(Task.FromResult(1));
		}
	}
}