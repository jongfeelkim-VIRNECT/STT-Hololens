using System;

namespace Assets.Scripts.TextToSpeech.JsonData
{
    [Serializable]
    class TTSRequestBody
    {
        public Input input;
        public Voice voice;
        public AudioConfig audioConfig;
    }
}