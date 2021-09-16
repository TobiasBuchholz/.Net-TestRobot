using System;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace TestRobot
{
    public abstract class TestRobotBase<TRobot, TRobotResult> : Genesis.TestUtil.IBuilder
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
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

        public TRobot Build()
        {
            return Build(_scheduler);
        }

        protected virtual TRobot Build(IScheduler scheduler)
        {
            return (TRobot)this;
        }

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

        protected abstract TRobotResult CreateResult();

        public TRobotResult AdvanceTo(TimeSpan time)
        {
            _scheduler.AdvanceTo(time.Ticks);
            return CreateResult();
        }
    }

    public abstract class TestRobotResultBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        protected TestRobotResultBase(TRobot robot)
        {
            Robot = robot;
        }

        protected TRobot Robot { get; }
    }
}