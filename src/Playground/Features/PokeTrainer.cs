using System;
using System.Threading.Tasks;

namespace Playground.Features
{
    public class PokeTrainer
    {
        private readonly IInventory _inventory;
        private readonly IPokeDex _pokeDex;

        public PokeTrainer(IInventory inventory, IPokeDex pokeDex)
        {
            _inventory = inventory;
            _pokeDex = pokeDex;
        }

        public Task UsePokedexAsync()
        {
            Console.WriteLine("A wild Pikachu was detected!");
            return _pokeDex.DetectPokemonAsync();
        }

        public async Task ThrowPokeBallAsync()
        {
            Console.WriteLine("Attempting to throw a PokeBall!");
            
            if(_inventory.GetPokeBallCount() > 0) {
                await _inventory.UsePokeBallAsync();
                IsPokemonCaught = true;
                Console.WriteLine("A Pokemon was caught!");
            } else {
                IsPokemonCaught = false;
                Console.WriteLine("No Pokemon was caught!");
            }
        }
        
        public bool IsPokemonCaught { get; private set; }
    }
}