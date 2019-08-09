using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.ViewModels
{
    public class ProbBeliefViewModel
    {
        public int ID { get; set; }

        public String[] InferModelStates { get; set; }
        public String[] InferModelResult { get; set; }

    }
}
