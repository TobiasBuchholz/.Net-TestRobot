using System.Threading.Tasks;

namespace Playground.UnitTests
{
    public interface IPokedex
    {
        Task DetectPokemonAsync();
    }
}