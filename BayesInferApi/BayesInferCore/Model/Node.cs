
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BayesInferCore.Model
{
    public class Node 
    {

		public Node()
		{
			NodeParents = new List<Node>();
		}
        public enum Estado  {Novo=0,Carregado=1,Finalizado=2}

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("states")]
        public List<string> States { get; set; }

        
        [JsonProperty("parents")]
        public List<string> Parents { get; set; }


        [JsonProperty("cpt")]
        public Object CptsObject { get; set; }
        
        public List<Cpt> Cpts { get; set; }

        //VAriaveçl para verificar situação no carregamento
        public int situacaoNodo { get; set; }
        public List<Node> NodeParents { get; set; }







    }
}
