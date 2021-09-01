using PCLMock;
using Playground.UnitTests.Features;

namespace Playground.UnitTests.Tests
{
    public sealed class PokeCenterMock : MockBase<IPokeCenter>, IPokeCenter
    {
        public PokeCenterMock(MockBehavior behavior = MockBehavior.Strict)
            : base(behavior)
        {
            if(behavior == MockBehavior.Loose) {
                ConfigureLooseBehavior();
            }
        }

        private void ConfigureLooseBehavior()
        {
        }
    }
}
