using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
    public class Belief
    {
        public string NodeName { get; set; }
        public int? BeliefValue { get; set; }
    }
}
