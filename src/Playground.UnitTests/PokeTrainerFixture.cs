using System;
using Xunit;

namespace Playground.UnitTests
{
    public class PokeTrainerFixture
    {
        [Theory]
        [InlineData(42, true)]
        [InlineData(0, false)]
        public void catches_pokemon_regarding_to_pokeball_amount(int pokeBallAmount, bool isPokemonCaught)
        {
            new PokeTrainerRobot()
                .WithPokeBallAmount(pokeBallAmount)
                .Build()
                .UsePokeDexAsync()
                .ThrowPokeBallAsync(atTime:100)
                .AdvanceUntilEmpty()
                .AssertIsPokemonCaught(isPokemonCaught)
                .VerifyPokeDexMock(x => x.DetectPokemonAsync())
                .WasCalledExactlyOnce()
                .VerifyInventoryMock(x => x.GetPokeBallCount())
                .WasCalledExactlyOnce();
        }
    }
}
