using System;
using System.Linq.Expressions;
using PCLMock;
using Playground.Features;
using Playground.UnitTests.Tests;
using TestRobot;

namespace Playground.UnitTests
{
    public abstract class AutoTestRobot<TRobot, TRobotResult> : TestRobotBase<TRobot, TRobotResult>
        where TRobot : TestRobotBase<TRobot, TRobotResult>
        where TRobotResult : TestRobotResultBase<TRobot, TRobotResult>
    {
        internal IPokedex _pokedex;

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