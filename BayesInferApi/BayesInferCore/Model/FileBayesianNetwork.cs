using Microsoft.ML.Probabilistic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BayesInferCore.Model
{
    public class FileBayesianNetwork : InferModel
    {
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("network")]
        public Network Network { get; set; }
        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; }

    }
}
