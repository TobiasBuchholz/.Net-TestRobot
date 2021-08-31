using System;
using System.Linq.Expressions;
using PCLMock;
using Plugin.TestRobot;

namespace Playground.UnitTests
{
    public abstract class AutoTestRobot<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        internal IPokedex _pokedex;

        protected Exception _exceptionToThrow;

        public TRobot WithThrowsException(Exception exception) =>
            With(ref _exceptionToThrow, exception);
        
		public override TRobot Build()
		{
            _pokedex ??= CreatePokedexMock();
		    return base.Build();
		}

        protected virtual PokedexMock CreatePokedexMock() => new PokedexMock(MockBehavior.Loose);
    }
    
    public abstract class AutoTestRobotResult<TRobot, TRobotResult> : TestRobotResultBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        private readonly AutoTestRobot<TRobot, TRobotResult> _autoRobot;
        
        protected AutoTestRobotResult(TRobot robot) 
            : base(robot)
        {
            _autoRobot = robot as AutoTestRobot<TRobot, TRobotResult>;
        }
        
        public RobotVerifyContinuation<TRobot, TRobotResult> VerifyPokedexMock(Expression<Action<IPokedex>> selector)
        {
            var mock = (PokedexMock) _autoRobot._pokedex;
            return mock.Verify(this, selector);
        }
    }
}