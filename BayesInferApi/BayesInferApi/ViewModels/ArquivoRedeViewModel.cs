using BayesInferCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.ViewModels
{
    public class ArquivoRedeViewModel
    {
        public string NomeRede { get; set; }
        public string Descricao { get; set; }
        public FileBayesianNetwork RedeBayesianaJson { get; set; }
    }
}
