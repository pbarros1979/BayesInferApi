using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BayesInferCore.Model
{
    public class Cpt
    {
        [JsonProperty("when")]
        public Object WhenObject { get; set; }

        [JsonProperty("then")]
        public Object ThenObject { get; set; }


        public Dictionary<string,string> WhenDic { get; set; }
        public Dictionary<string, string> ThenDic { get; set; }
        public Dictionary<string, string> SingleCptDic { get; set; }


    }
}
