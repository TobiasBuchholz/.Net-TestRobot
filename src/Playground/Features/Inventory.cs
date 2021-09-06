using System;
using System.Threading.Tasks;

namespace Playground.Features
{
    public sealed class Inventory : IInventory
    {
        private readonly int _pokeBallAmount;

        public Inventory(int pokeBallAmount)
        {
            _pokeBallAmount = pokeBallAmount;
        }

        public int GetPokeBallCount()
        {
            return _pokeBallAmount;
        }

        public Task UsePokeBallAsync()
        {
            return Task.CompletedTask;
        }
    }
}