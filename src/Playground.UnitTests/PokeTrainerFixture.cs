using System;
using Xunit;

namespace Playground.UnitTests
{
    public class PokeTrainerFixture
    {
        [Theory]
        [InlineData(42, true)]
        [InlineData(0, false)]
        public void catches_pokemon_regarding_to_pokeball_amount(int pokeBallAmount, bool expectedIsPokemonCaught)
        {
            new PokeTrainerRobot()
                .WithIsPokemonDetectable(true)
                .WithPokeBallAmount(pokeBallAmount)
                .Build()
                .CatchPokemonAsync()
                .AdvanceUntilEmpty()
                .AssertIsPokemonCaught(expectedIsPokemonCaught)
                .VerifyPokeDexMock(x => x.DetectPokemonAsync())
                .WasCalledExactlyOnce()
                .VerifyInventoryMock(x => x.GetPokeBallCount())
                .WasCalledExactlyOnce()
                .VerifyInventoryMock(x => x.UsePokeBallAsync())
                .WasCalledExactly(expectedIsPokemonCaught ? 1 : 0);
        }

        [Fact]
        public void heals_pokemons_of_inventory()
        {
            var robot = new PokeTrainerRobot()
                .WithPokemonCount(6)
                .Build()
                .HealPokemonsAsync(atTime:250);

            robot
                .AdvanceTo(TimeSpan.FromMilliseconds(100))
                .VerifyInventoryMock(x => x.UseHealingPotionAsync())
                .WasNotCalled();
            
            robot
                .AdvanceTo(TimeSpan.FromMilliseconds(1500))
                .VerifyInventoryMock(x => x.UseHealingPotionAsync())
                .WasCalledExactlyOnce();
            
            robot
                .AdvanceTo(TimeSpan.FromMilliseconds(4500))
                .VerifyInventoryMock(x => x.UseHealingPotionAsync())
                .WasCalledExactly(4);
            
            robot
                .AdvanceUntilEmpty()
                .VerifyInventoryMock(x => x.UseHealingPotionAsync())
                .WasCalledExactly(6);
        }
    }
}
