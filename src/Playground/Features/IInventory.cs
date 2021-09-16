using System;
using System.Threading.Tasks;

namespace Playground.Features
{
    public interface IInventory
    {
        int GetPokeBallCount();
        int GetPokemonCount();
        Task UsePokeBallAsync();
        Task UseHealingPotionAsync();
    }
}
