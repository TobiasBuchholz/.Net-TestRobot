using System;
using Playground.Features;
using TestRobot;
using Xunit;

namespace Playground.UnitTests
{
    public sealed class PokeTrainerRobot : AutoTestRobot<PokeTrainer, PokeTrainerRobot, PokeTrainerRobotResult>
    {
        private bool _isPokemonDetectable;
        private int _pokeBallAmount;
        private int _pokemonCount;

        public PokeTrainerRobot WithIsPokemonDetectable(bool isDetectable) => 
            With(ref _isPokemonDetectable, isDetectable);

        public PokeTrainerRobot WithPokeBallAmount(int amount) => 
            With(ref _pokeBallAmount, amount);
        
        public PokeTrainerRobot WithPokemonCount(int count) => 
            With(ref _pokemonCount, count);

        protected override PokeTrainer CreateSut()
        {
            return new PokeTrainer(
                InventoryMock,
                PokeDexMock,
                TestScheduler);
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
            Schedule(() => Sut.CatchPokemonAsync());
            return this;
        }

        public PokeTrainerRobot HealPokemonsAsync(long atTime = 0)
        {
            Schedule(TimeSpan.FromMilliseconds(atTime), () => Sut.HealPokemonsAsync());
            return this;
        }
    }

    public sealed class PokeTrainerRobotResult : AutoTestRobotResult<PokeTrainer, PokeTrainerRobot, PokeTrainerRobotResult>
    {
        public PokeTrainerRobotResult(PokeTrainer sut, PokeTrainerRobot robot)
            : base(sut, robot)
        {
        }

        public PokeTrainerRobotResult AssertIsPokemonCaught(bool expected)
        {
            Assert.Equal(expected, Sut.IsPokemonCaught);
            return this;
        }
    }
}
