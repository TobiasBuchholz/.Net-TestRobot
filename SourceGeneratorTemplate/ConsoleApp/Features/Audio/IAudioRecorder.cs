using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Features.Audio
{
    public interface IAudioRecorder
    {
        void StartRecording();
        void StopRecording();
    }
}
