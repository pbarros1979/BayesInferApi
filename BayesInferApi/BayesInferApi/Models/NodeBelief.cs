﻿using BayesInferApi.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.Models
{
    public class NodeBelief
    {
        public string Id { get; set; }
        public int BeliefType { get; set; }
        public string BeliefTypeName { get; set; }
    }
}
