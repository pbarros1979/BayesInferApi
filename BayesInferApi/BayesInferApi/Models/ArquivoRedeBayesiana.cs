using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BayesInferApi.Models
{
    public class ArquivoRedeBayesiana
    {
        [Key]
        public int ID { get; set; }
        public string NomeRede { get; set; }
        public string NomeArquivo { get; set; }
        public string ArquivoJson { get; set; }
        public string Descricao { get; set; }
        public DateTime DataUpload { get; set; }
    }
}
