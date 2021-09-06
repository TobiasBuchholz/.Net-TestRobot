using System.Threading.Tasks;
using PCLMock;
using Playground.Features;

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
        }

        public int GetPokeBallCount()
        {
            return Apply(x => x.GetPokeBallCount());
        }

        public Task UsePokeBallAsync()
        {
            return Apply(x => x.UsePokeBallAsync());
        }
    }
}
