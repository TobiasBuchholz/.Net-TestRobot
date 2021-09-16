using System;
using System.Reactive.Concurrency;
using Playground.Features;
using TestRobot;
using Xunit;

namespace Playground.UnitTests
{
    public sealed class PokeTrainerRobot : AutoTestRobot<PokeTrainerRobot, PokeTrainerRobotResult>
    {
        internal PokeTrainer _sut;
        private bool _isPokemonDetectable;
        private int _pokeBallAmount;
        private int _pokemonCount;

        public PokeTrainerRobot()
        {
        }
        
        public PokeTrainerRobot WithIsPokemonDetectable(bool isDetectable) => 
            With(ref _isPokemonDetectable, isDetectable);

        public PokeTrainerRobot WithPokeBallAmount(int amount) => 
            With(ref _pokeBallAmount, amount);
        
        public PokeTrainerRobot WithPokemonCount(int count) => 
            With(ref _pokemonCount, count);

        protected override PokeTrainerRobot Build(IScheduler scheduler)
        {
            base.Build(scheduler);
            _sut = new PokeTrainer(_inventory, _pokeDex, scheduler);
            return this;
        }

        protected override InventoryMock CreateInventoryMock()
        {
            var mock =  base.CreateInventoryMock();
            mock.When(x => x.GetPokeBallCount()).Return(_pokeBallAmount);
            mock.When(x => x.GetPokemonCount()).Return(_pokemonCount);
            return mock;
        }

        protected override PokeDexMock CreatePokeDexMock()
        {
            var mock = base.CreatePokeDexMock();
            mock.When(x => x.DetectPokemonAsync()).ReturnsAsync(_isPokemonDetectable);
            return mock;
        }

        public PokeTrainerRobot CatchPokemonAsync()
        {
            Schedule(() => _sut.CatchPokemonAsync());
            return this;
        }

        public PokeTrainerRobot HealPokemonsAsync(long atTime = 0)
        {
            Schedule(TimeSpan.FromMilliseconds(atTime), () => _sut.HealPokemonsAsync());
            return this;
        }

        protected override PokeTrainerRobotResult CreateResult()
        {
            return new PokeTrainerRobotResult(this);
        }
    }

    public sealed class PokeTrainerRobotResult : AutoTestRobotResult<PokeTrainerRobot, PokeTrainerRobotResult>
    {
        public PokeTrainerRobotResult(PokeTrainerRobot robot)
            : base(robot)
        {
        }

        public PokeTrainerRobotResult AssertIsPokemonCaught(bool expected)
        {
            Assert.Equal(expected, Robot._sut.IsPokemonCaught);
            return this;
        }
    }
}
