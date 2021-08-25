using ConsoleApp.Features.Audio;
using PCLMock;

namespace ConsoleApp.Mocks
{
    public class AudioRecorderMock : MockBase<AudioRecorderMock>, IAudioRecorder
    {
        public AudioRecorderMock(MockBehavior behavior = MockBehavior.Strict)
           : base(behavior)
        {
            if (behavior == MockBehavior.Loose)
            {
                ConfigureLooseBehavior();
            }
        }

        public void StartRecording()
        {
            Apply(x => x.StartRecording());
        }

        public void StopRecording()
        {
            Apply(x => x.StopRecording());
        }

        private void ConfigureLooseBehavior()
        {
        }
    }
}
