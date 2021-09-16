using System;
using System.Threading.Tasks;

namespace Playground.Features
{
    public sealed class PokeDex : IPokeDex
    {
        public Task<bool> DetectPokemonAsync()
        {
            Logger.Log("Wild Pokemon was detected");
            return Task.FromResult(true);
        }
    }
}