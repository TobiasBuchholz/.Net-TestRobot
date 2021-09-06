using System;
using Playground.Features;
using TestRobot;
using Xunit;

namespace Playground.UnitTests
{
    public sealed class PokeTrainerRobot : AutoTestRobot<PokeTrainerRobot, PokedexRobotResult>
    {
        internal PokeTrainer _sut;
        private int _pokeBallAmount;

        public PokeTrainerRobot()
        {
        }

        public PokeTrainerRobot WithPokeBallAmount(int amount) => 
            With(ref _pokeBallAmount, amount);

        public override PokeTrainerRobot Build()
        {
            base.Build();
            _sut = new PokeTrainer(_inventory, _pokeDex);
            return this;
        }

        protected override InventoryMock CreateInventoryMock()
        {
            var mock =  base.CreateInventoryMock();
            mock.When(x => x.GetPokeBallCount()).Return(_pokeBallAmount);
            mock.When(x => x.UsePokeBallAsync()).ReturnsAsync();
            return mock;
        }

        protected override PokeDexMock CreatePokeDexMock()
        {
            var mock = base.CreatePokeDexMock();
            mock.When(x => x.DetectPokemonAsync()).ReturnsAsync();
            return mock;
        }

        public PokeTrainerRobot UsePokeDexAsync()
        {
            Schedule(() => _sut.UsePokedexAsync());
            return this;
        }

        public PokeTrainerRobot ThrowPokeBallAsync(long atTime = 0)
        {
            Schedule(TimeSpan.FromMilliseconds(atTime), () => _sut.ThrowPokeBallAsync());
            return this;
        }

        protected override PokedexRobotResult CreateResult()
        {
            return new PokedexRobotResult(this);
        }
    }

    public sealed class PokedexRobotResult : AutoTestRobotResult<PokeTrainerRobot, PokedexRobotResult>
    {
        public PokedexRobotResult(PokeTrainerRobot robot)
            : base(robot)
        {
        }

        public PokedexRobotResult AssertIsPokemonCaught(bool expected)
        {
            Assert.Equal(expected, Robot._sut.IsPokemonCaught);
            return this;
        }
    }
}
