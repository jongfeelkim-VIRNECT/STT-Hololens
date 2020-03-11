using System;
using System.Collections.Generic;

namespace Assets.Scripts.Translate.JsonData
{
    [Serializable]
    class TranslateRequestBody
    {
        public List<string> contents;
        public string mimeType;
        public string sourceLanguageCode;
        public string targetLanguageCode;
    }
}