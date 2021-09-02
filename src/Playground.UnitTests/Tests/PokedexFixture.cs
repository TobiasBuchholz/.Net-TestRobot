using System;
using Playground.UnitTests.Tests;
using Xunit;

namespace Playground.UnitTests
{
    public class PokedexFixture
    {
        [Fact]
        public void detects_pokemon()
        {
            new PokedexRobot()
                .WithSomeProperty(1337)
                .Build()
                .AdvanceUntilEmpty()
                .AssertSomeProperty(1337)
                .VerifyPokedexMock(x => x.DetectPokemonAsync())
                .WasCalledExactlyOnce()
                .VerifyPokeCenterMock(x => x.HealPokemon())
                .WasCalledExactly(6);
        }
    }
}
