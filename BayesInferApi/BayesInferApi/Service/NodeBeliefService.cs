using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BayesInferApi.Models;

namespace BayesInferApi.Service
{
    public class NodeBeliefService : INodeBeliefService
    {
        public NodeBeliefService()
        {
            NodesBelief = new List<NodeBelief>();
            Nodes = new List<string>();
        }
        public int IdRede { get ; set ; }
        public List<NodeBelief> NodesBelief { get ; set; }
        public List<string> Nodes { get ; set ; }
    }
}
