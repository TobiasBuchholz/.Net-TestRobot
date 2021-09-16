using System.Threading.Tasks;
using PCLMock;
using Playground.Features;
using TestRobot;

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
            When(x => x.DetectPokemonAsync()).ReturnsAsync(false);
        }

        public Task<bool> DetectPokemonAsync()
        {
            return Apply(x => x.DetectPokemonAsync());
        }
    }
}
