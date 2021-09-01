using System.Threading.Tasks;
using PCLMock;
using Playground.Features;

namespace Playground.UnitTests.Tests
{
    public sealed class PokedexMock : MockBase<IPokedex>, IPokedex
    {
        public PokedexMock(MockBehavior behavior = MockBehavior.Strict)
            : base(behavior)
        {
            if(behavior == MockBehavior.Loose) {
                ConfigureLooseBehavior();
            }
        }

        private void ConfigureLooseBehavior()
        {
        }

        public Task DetectPokemonAsync()
        {
            return Apply(x => x.DetectPokemonAsync());
        }
    }
}
