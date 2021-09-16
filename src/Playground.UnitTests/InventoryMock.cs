using System.Threading.Tasks;
using PCLMock;
using Playground.Features;
using TestRobot;

namespace Playground.UnitTests
{
    public sealed class InventoryMock : MockBase<IInventory>, IInventory
    {
        public InventoryMock(MockBehavior behavior = MockBehavior.Strict)
            : base(behavior)
        {
            if(behavior == MockBehavior.Loose) {
                ConfigureLooseBehavior();
            }
        }

        private void ConfigureLooseBehavior()
        {
            When(x => x.GetPokeBallCount()).Return(0);
            When(x => x.GetPokemonCount()).Return(0);
            When(x => x.UsePokeBallAsync()).ReturnsAsync();
            When(x => x.UseHealingPotionAsync()).ReturnsAsync();
        }

        public int GetPokeBallCount()
        {
            return Apply(x => x.GetPokeBallCount());
        }

        public int GetPokemonCount()
        {
            return Apply(x => x.GetPokemonCount());
        }

        public Task UsePokeBallAsync()
        {
            return Apply(x => x.UsePokeBallAsync());
        }

        public Task UseHealingPotionAsync()
        {
            return Apply(x => x.UseHealingPotionAsync());
        }
    }
}
