using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.TestUtils
{
    public abstract class TestRobotBase<TRobot, TRobotResult> : Genesis.TestUtil.IBuilder
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        internal TestScheduler _scheduler;

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
            return (TRobot)this;
        }

        public TRobotResult AdvanceUntilEmpty()
        {
            _scheduler.AdvanceUntilEmpty();
            return CreateResult();
        }

        protected abstract TRobotResult CreateResult();

        public TRobotResult AdvanceTo(TimeSpan time)
        {
            return AdvanceTo(time.Ticks);
        }

        public TRobotResult AdvanceTo(long time)
        {
            _scheduler.AdvanceTo(time);
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
