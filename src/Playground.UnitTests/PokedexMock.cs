using System.Threading.Tasks;
using PCLMock;

namespace Playground.UnitTests
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
