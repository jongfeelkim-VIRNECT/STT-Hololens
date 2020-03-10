using System;

namespace Assets.Scripts.JsonData.TextToSpeech
{
    [Serializable]
    class TTSRequestBody
    {
        public Input input;
        public Voice voice;
        public AudioConfig audioConfig;
    }
}