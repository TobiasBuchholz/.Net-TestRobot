using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp.Mocks;
using ConsoleApp.TestUtils;

namespace ConsoleApp.Fixtures
{
    public class AudioRecorderRobot : TestRobotGenerated<AudioRecorderRobot, AudioRecorderRobotResult>
    {
        public override AudioRecorderRobot Build()
        {
            return base.Build();
        }

        protected override AudioRecorderMock CreateAudioRecorderMock()
        {
            return base.CreateAudioRecorderMock();
        }

        protected override AudioRecorderRobotResult CreateResult()
        {
            return new AudioRecorderRobotResult(this);
        }
    }


    public class AudioRecorderRobotResult : TestRobotResultGenerated<AudioRecorderRobot, AudioRecorderRobotResult>
    {
        public AudioRecorderRobotResult(AudioRecorderRobot robot)
            : base(robot)
        {
        }
    }
}
