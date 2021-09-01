using System;
using Playground.Features;
using TestRobot;
using Xunit;

namespace Playground.UnitTests.Tests
{
    public sealed class PokedexRobot : TestRobotGenerated<PokedexRobot, PokedexRobotResult>
    {
        private IPokedex _sut;
        private long _something;

        public PokedexRobot()
        {
        }

        public PokedexRobot WithSomeProperty(long something) => 
            With(ref _something, something);

        public override PokedexRobot Build()
        {
            base.Build();
            _sut = new Pokedex();

            return this;
        }

        protected override PokedexMock CreatePokedexMock()
        {
            var mock = base.CreatePokedexMock();
            mock.When(x => x.DetectPokemonAsync()).ReturnsAsync();
            return mock;
        }

        public PokedexRobot DetectPokemonAsync()
        {
            Schedule(TimeSpan.FromMilliseconds(100), () => _sut.DetectPokemonAsync());
            return this;
        }

        protected override PokedexRobotResult CreateResult()
        {
            return new PokedexRobotResult(this);
        }
    }

    public sealed class PokedexRobotResult : TestRobotResultGenerated<PokedexRobot, PokedexRobotResult>
    {
        public PokedexRobotResult(PokedexRobot robot)
            : base(robot)
        {
        }

        public PokedexRobotResult AssertSomeProperty(long expected)
        {
            Assert.Equal(expected, 1337);
            return this;
        }
    }
}