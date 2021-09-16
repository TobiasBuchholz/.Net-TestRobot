using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Playground.Features
{
    public class PokeTrainer
    {
        private readonly IInventory _inventory;
        private readonly IPokeDex _pokeDex;
        private readonly IScheduler _scheduler;

        public PokeTrainer(IInventory inventory, IPokeDex pokeDex, IScheduler scheduler)
        {
            _inventory = inventory;
            _pokeDex = pokeDex;
            _scheduler = scheduler;
        }

        public async Task CatchPokemonAsync()
        {
            if(await _pokeDex.DetectPokemonAsync()) {
                await ThrowPokeBallAsync();
            }
        }

        private async Task ThrowPokeBallAsync()
        {
            Logger.Log("Attempting to throw PokeBall");
            
            if(_inventory.GetPokeBallCount() > 0) {
                await _inventory.UsePokeBallAsync();
                IsPokemonCaught = true;
                Logger.Log("Pokemon was caught");
            } else {
                IsPokemonCaught = false;
                Logger.Log("No Pokemon was caught because there are no PokeBalls left");
            }
        }
        
        public Task HealPokemonsAsync()
        {
            Logger.Log("Healing all Pokemons of inventory");
            
            return Observable
                .Interval(TimeSpan.FromSeconds(1), _scheduler)
                .Take(_inventory.GetPokemonCount())
                .SelectMany(_ => _inventory.UseHealingPotionAsync().ToObservable())
                .Do(_ => Logger.Log("Pokemon was healed"))
                .ToTask();
        }

        public bool IsPokemonCaught { get; private set; }
    }
}