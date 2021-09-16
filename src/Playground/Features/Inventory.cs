using System;
using System.Threading.Tasks;

namespace Playground.Features
{
    public sealed class Inventory : IInventory
    {
        private readonly int _pokeBallCount;
        private readonly int _pokemonCount;

        public Inventory(int pokeBallCount, int pokemonCount)
        {
            _pokeBallCount = pokeBallCount;
            _pokemonCount = pokemonCount;
        }

        public int GetPokeBallCount()
        {
            return _pokeBallCount;
        }

        public int GetPokemonCount()
        {
            return _pokemonCount;
        }

        public Task UsePokeBallAsync()
        {
            Logger.Log("PokeBall was used");
            return Task.CompletedTask;
        }

        public Task UseHealingPotionAsync()
        {
            Logger.Log("Potion was used");
            return Task.CompletedTask;
        }
    }
}