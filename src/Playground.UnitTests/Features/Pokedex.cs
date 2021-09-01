using System.Threading.Tasks;

namespace Playground.Features
{
    public sealed class Pokedex : IPokedex
    {
        public Task DetectPokemonAsync()
        {
            return Task.CompletedTask;
        }
    }
}