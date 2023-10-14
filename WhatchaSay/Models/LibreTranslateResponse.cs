using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WhatchaSay.Model
{
    public class LibreTranslateResponse
    {

        public detectedLanguageT detectedLanguage { get; set; }
        public string translatedText { get; set; }

        public class detectedLanguageT
        {
            public JsonValue confidence { get; set; }
            public string language { get; set; }
        }
    }
}
