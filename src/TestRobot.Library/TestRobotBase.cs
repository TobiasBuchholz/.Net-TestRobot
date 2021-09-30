using System;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace TestRobot
{
    public abstract class TestRobotBase<TSut, TRobot, TRobotResult> : Genesis.TestUtil.IBuilder
        where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>
    {
        private TestScheduler _scheduler;

        protected TestRobotBase()
        {
            _scheduler = new TestScheduler();
        }

        public TRobot WithScheduler(TestScheduler scheduler) =>
            With(ref _scheduler, scheduler);

        protected TRobot With<TField>(ref TField field, TField value)
        {
            field = value;
            return (TRobot)this;
        }

        public virtual TRobot Build()
        {
            Sut = CreateSut(_scheduler);
            return (TRobot) this;
        }

        protected abstract TSut CreateSut(TestScheduler scheduler);

        protected void Schedule(Action action)
        {
            _scheduler.Schedule(action);
        }
        
        protected void Schedule(TimeSpan dueTime, Action action)
        {
            _scheduler.Schedule(dueTime, action);
        }

        public TRobotResult AdvanceUntilEmpty()
        {
            _scheduler.AdvanceUntilEmpty();
            return CreateResult();
        }

        protected virtual TRobotResult CreateResult()
        {
            return (TRobotResult) Activator.CreateInstance(typeof(TRobotResult), Sut);
        }

        public TRobotResult AdvanceTo(TimeSpan time)
        {
            _scheduler.AdvanceTo(time.Ticks);
            return CreateResult();
        }
        
        protected TSut Sut { get; private set; }
    }

    public abstract class TestRobotResultBase<TSut, TRobot, TRobotResult>
        where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>
    {
        protected TestRobotResultBase(TSut sut)
        {
            Sut = sut;
        }

        protected TSut Sut { get; }
    }
}