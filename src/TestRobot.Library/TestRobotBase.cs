using System;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace TestRobot
{
    /// <summary>
    /// This is the base class for applying the Robot Pattern that facilitates writing more stable, readable and maintainable tests by
    /// following the AAA Pattern (Arrange-Act-Assert) and making use of the Builder Pattern.
    /// </summary>
    /// <typeparam name="TSut">
    /// The type of the System Under Test (SUT).
    /// </typeparam>
    /// <typeparam name="TRobot">
    /// The type of the TestRobot.
    /// </typeparam>
    /// <typeparam name="TRobotResult">
    /// The type of the TestRobotResult to the corresponding TestRobot.
    /// </typeparam>
    public abstract class TestRobotBase<TSut, TRobot, TRobotResult> : Genesis.TestUtil.IBuilder
        where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>
    {
        private TestScheduler _testScheduler;

        /// <summary>
        /// Constructor
        /// </summary>
        protected TestRobotBase()
        {
            _testScheduler = new TestScheduler();
        }

        /// <summary>
        /// Sets the scheduler that will be used in your tests.
        /// </summary>
        /// <param name="scheduler">
        /// The scheduler that will be used in the methods responsible for traveling through time in your tests,
        /// e.g. <see cref="AdvanceTo"/>, <see cref="AdvanceUntilEmpty"/> or <see cref="Schedule(System.Action)"/>.
        /// </param>
        /// <returns>The instance of the current TestRobot.</returns>
        public TRobot WithScheduler(TestScheduler scheduler) =>
            With(ref _testScheduler, scheduler);

        /// <summary>
        /// This method enables setting fields for the TestRobot by following the Builder Pattern.
        /// </summary>
        /// <param name="field">The field you want to set.</param>
        /// <param name="value">The value of the field to be set.</param>
        /// <returns>The instance of the current TestRobot.</returns>
        protected TRobot With<TField>(ref TField field, TField value)
        {
            field = value;
            return (TRobot) this;
        }

        /// <summary>
        /// Creates the System Under Test (SUT) and needs to be called after several <see cref="With{TField}"/> calls during the Arrange part of
        /// the AAA Pattern as final step.
        /// </summary>
        /// <returns>The instance of the current TestRobot.</returns>
        public virtual TRobot Build()
        {
            Sut = CreateSut();
            return (TRobot) this;
        }

        /// <summary>
        /// Creates the System Under Test (SUT).
        /// </summary>
        /// <returns>The System Under Test (SUT).</returns>
        protected abstract TSut CreateSut();

        /// <summary>
        /// Schedules an action to be executed on a default instance of the <see cref="Microsoft.Reactive.Testing.TestScheduler"/> or the instance that was
        /// given by <see cref="WithScheduler"/>.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        protected void Schedule(Action action)
        {
            _testScheduler.Schedule(action);
        }
        
        /// <summary>
        /// Schedules an action to be executed after the specified relative due time on a default instance of the <see cref="Microsoft.Reactive.Testing.TestScheduler"/> or the
        /// instance that was given by <see cref="WithScheduler"/>.
        /// </summary>
        /// <param name="dueTime">Relative time after which to execute the action.</param>
        /// <param name="action">Action to execute.</param>
        protected void Schedule(TimeSpan dueTime, Action action)
        {
            _testScheduler.Schedule(dueTime, action);
        }

        /// <summary>
        /// Advances the current instance of the <see cref="Microsoft.Reactive.Testing.TestScheduler"/> until the last scheduled action is executed and creates the
        /// TestRobotResult. This method should be called as the final step for the Act part of the AAA Pattern.
        /// </summary>
        /// <returns>The TestRobotResult to assert the test's outcome.</returns>
        public TRobotResult AdvanceUntilEmpty()
        {
            _testScheduler.AdvanceUntilEmpty();
            return CreateResult();
        }

        /// <summary>
        /// Creates the TestRobotResult that can be used for the assertion and verification of the test's outcome.
        /// </summary>
        protected virtual TRobotResult CreateResult()
        {
            return (TRobotResult) Activator.CreateInstance(typeof(TRobotResult), Sut);
        }

        /// <summary>
        /// Advances the clock of the current instance of <see cref="Microsoft.Reactive.Testing.TestScheduler"/>'s to the specified time, running all work till that point.
        /// </summary>
        /// <param name="time">Absolute time to advance the scheduler's clock to.</param>
        /// <returns>The TestRobotResult to assert the test's outcome.</returns>
        public TRobotResult AdvanceTo(TimeSpan time)
        {
            _testScheduler.AdvanceTo(time.Ticks);
            return CreateResult();
        }
        
        /// <summary>
        /// Instance of the System Under Test (SUT).
        /// </summary>
        protected TSut Sut { get; private set; }

        /// <summary>
        /// The scheduler that will be used in the methods responsible for travelling through time in your tests,
        /// e.g. <see cref="AdvanceTo"/>, <see cref="AdvanceUntilEmpty"/> or <see cref="Schedule(System.Action)"/>.
        /// Use <see cref="WithScheduler"/> to inject your a particular TestScheduler for a specific test case if needed.
        /// </summary>
        protected IScheduler TestScheduler => _testScheduler;
    }

    /// <summary>
    /// This class is the corresponding part to the TestRobot and is responsible for the assertion and verification of the test's outcome.
    /// </summary>
    /// <typeparam name="TSut">
    /// The type of the System Under Test (SUT).
    /// </typeparam>
    /// <typeparam name="TRobot">
    /// The type of the TestRobot.
    /// </typeparam>
    /// <typeparam name="TRobotResult">
    /// The type of the TestRobotResult to the corresponding TestRobot.
    /// </typeparam>
    public abstract class TestRobotResultBase<TSut, TRobot, TRobotResult>
        where TRobot : TestRobotBase<TSut, TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TSut, TRobot, TRobotResult>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sut">Instance of the System Under Test (SUT).</param>
        protected TestRobotResultBase(TSut sut)
        {
            Sut = sut;
        }

        /// <summary>
        /// Instance of the System Under Test (SUT).
        /// </summary>
        protected TSut Sut { get; }
    }
}