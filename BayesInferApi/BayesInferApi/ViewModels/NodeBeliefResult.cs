using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.ViewModels
{
    public class NodeBeliefResult
    {

        [JsonProperty("NodeName")]
        public string NodeName { get; set; }
        [JsonProperty("ResultPresente")]
        public double ResultPresente { get; set; }
        [JsonProperty("ResultAusente")]
        public double ResultAusente { get; set; }
    }
}
