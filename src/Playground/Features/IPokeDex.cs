using System.Threading.Tasks;

namespace Playground.Features
{
    public interface IPokeDex
    {
        Task<bool> DetectPokemonAsync();
    }
}