using System.Threading.Tasks;

namespace Playground.UnitTests
{
    public sealed class Pokedex : IPokedex
    {
        public Task DetectPokemonAsync()
        {
            return Task.CompletedTask;
        }
    }
}