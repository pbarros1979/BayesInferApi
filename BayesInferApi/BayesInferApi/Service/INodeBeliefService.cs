using BayesInferApi.Models;
using BayesInferCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.Service
{
    public interface INodeBeliefService
    {
        int IdRede { get; set; }
        List<NodeBelief> NodesBelief { get; set; }
        //List<String> Nodes { get; set; }
    }
}
