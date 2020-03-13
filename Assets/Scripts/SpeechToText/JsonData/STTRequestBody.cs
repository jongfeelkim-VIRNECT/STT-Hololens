using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpeechToText.JsonData
{
    [Serializable]
    class STTRequestBody
    {
        public Config config;
        public Audio audio;
    }
}