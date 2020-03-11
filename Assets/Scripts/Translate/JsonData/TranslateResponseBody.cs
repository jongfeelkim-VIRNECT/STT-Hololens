using System;
using System.Collections.Generic;

namespace Assets.Scripts.Translate.JsonData
{
    [Serializable]
    class TranslateResponseBody
    {
        public List<TranslatedText> translations;
    }
}
