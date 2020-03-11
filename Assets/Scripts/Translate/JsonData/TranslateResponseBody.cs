using System;
using System.Collections.Generic;

namespace Scripts.Translate.JsonData
{
    [Serializable]
    class TranslateResponseBody
    {
        public List<TranslatedText> translations;
    }
}
