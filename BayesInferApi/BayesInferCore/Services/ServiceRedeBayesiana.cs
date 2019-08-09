using BayesInferCore.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BayesInferCore.Services
{
    public class ServiceRedeBayesiana
    {

        public FileBayesianNetwork LoadFileRedeBayesiana(string file)
        {
            string dataAsJson = File.ReadAllText(@file);
            FileBayesianNetwork redeBayesiana = JsonConvert.DeserializeObject<FileBayesianNetwork>(dataAsJson);
            redeBayesiana = LoadRedeBayesiana(redeBayesiana);

            return redeBayesiana;
        }

        public FileBayesianNetwork LoadRedeBayesiana(FileBayesianNetwork rede)
        {
            FileBayesianNetwork redeBayesiana = rede;
            foreach (var item in redeBayesiana.Nodes)
            {
                item.Cpts = GetCpt(item.CptsObject);
            }
            return redeBayesiana;
        }

        public List<Cpt> GetCpt(Object arg)
        {
            List<Cpt> _cpts = new List<Cpt>();
            if (arg is JArray)
            {
                _cpts = JsonConvert.DeserializeObject<List<Cpt>>(arg.ToString());
                foreach (var item in _cpts)
                {
                    item.WhenDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.WhenObject.ToString());
                    item.ThenDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ThenObject.ToString());
                }

            }
            else if (arg is JObject)
            {
                Cpt cpt = new Cpt();
                cpt.SingleCptDic = JsonConvert.DeserializeObject<Dictionary<string,string>>(arg.ToString());

                _cpts.Add(cpt);
            }          

            return _cpts;
        }
    }
}
