using BayesInferCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.ViewModels
{
    public class InferViewModel
    {
        public int IdRede { get; set; }
        public List<Belief> ListBelief { get; set; }
    }
}
