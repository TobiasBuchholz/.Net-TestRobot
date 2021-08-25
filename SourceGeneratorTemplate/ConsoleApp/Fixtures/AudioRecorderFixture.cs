using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ConsoleApp.Fixtures
{
    public class AudioRecorderFixture
    {
        [Fact]
        public void test_some_stuff()
        {
            new AudioRecorderRobot()
                .Build()
                .AdvanceUntilEmpty()
                .VerifyAudioRecorderMock(x => x.StartRecording())
                .WasCalledExactlyOnce();
        }
    }
}
