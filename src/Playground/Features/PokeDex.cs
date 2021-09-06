using System.Threading.Tasks;

namespace Playground.Features
{
    public sealed class PokeDex : IPokeDex
    {
        public Task DetectPokemonAsync()
        {
            return Task.CompletedTask;
        }
    }
}