using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Playground.Features;

namespace Playground
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var pokeTrainer = new PokeTrainer(new Inventory(pokeBallCount:42, pokemonCount:4), new PokeDex(), new EventLoopScheduler());
            await pokeTrainer.CatchPokemonAsync();
            await pokeTrainer.HealPokemonsAsync();
        }
    }
}