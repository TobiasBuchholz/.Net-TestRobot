using System.Threading.Tasks;
using PCLMock;
using Playground.Features;

namespace Playground.UnitTests
{
    public sealed class PokeDexMock : MockBase<IPokeDex>, IPokeDex
    {
        public PokeDexMock(MockBehavior behavior = MockBehavior.Strict)
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
