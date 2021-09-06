using System;
using System.Threading.Tasks;
using Playground.Features;

namespace Playground
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var pokeTrainer = new PokeTrainer(new Inventory(pokeBallAmount:42), new PokeDex());
            await pokeTrainer.UsePokedexAsync();
            await pokeTrainer.ThrowPokeBallAsync();
        }
    }
}