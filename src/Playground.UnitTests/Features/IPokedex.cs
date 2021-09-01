using System.Threading.Tasks;

namespace Playground.Features
{
    public interface IPokedex
    {
        Task DetectPokemonAsync();
    }
}