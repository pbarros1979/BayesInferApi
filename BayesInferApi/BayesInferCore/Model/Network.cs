using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
    public class Network
    {
        public string name { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public List<object> selectedNodes { get; set; }
        public bool propertiesPanelVisible { get; set; }
        public string kind { get; set; }
        public string id { get; set; }
        public List<object> subnetworks { get; set; }

    }
}
