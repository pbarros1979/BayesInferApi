using BayesInferApi.Data;
using BayesInferApi.Models;
using BayesInferApi.ViewModels;
using BayesInferCore.Model;
using BayesInferCore.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.Service
{
    public class ArquivoRedeService
    {
        private readonly BayesInferContext _context;


        public ArquivoRedeService(BayesInferContext bayesInferContext)
        {
            _context = bayesInferContext;
        }

        public async Task<ArquivoRedeBayesiana> AddArquivoRedeBayesiana(ArquivoRedeViewModel arg)
        {
            ArquivoRedeBayesiana arquivoRedeBayesiana = new ArquivoRedeBayesiana();
            arquivoRedeBayesiana.DataUpload = DateTime.Now;
            arquivoRedeBayesiana.Descricao = arg.Descricao;
            arquivoRedeBayesiana.NomeRede = string.IsNullOrEmpty(arg.NomeRede) ? arg.RedeBayesianaJson.Network.name : arg.NomeRede;
            arquivoRedeBayesiana.ArquivoJson = JsonConvert.SerializeObject(arg.RedeBayesianaJson);



            //Necessario salvar para obter id para usar no nome do arquivo
            _context.ArquivosRedeBayesiana.Add(arquivoRedeBayesiana);
            await _context.SaveChangesAsync();

            //Atualiza nome arquivo com Id e salva novamente
            arquivoRedeBayesiana.NomeArquivo = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + arquivoRedeBayesiana.ID.ToString("D8") + ".json";
            _context.ArquivosRedeBayesiana.Update(arquivoRedeBayesiana);
            await _context.SaveChangesAsync();

            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //ContractResolver = new CollectionAsObjectResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            var serializer = JsonSerializer.Create(serializerSettings);


            // write to disk  
            using (FileStream stream = new FileStream(@"RepoArquivos/" + @arquivoRedeBayesiana.NomeArquivo, FileMode.Create))
            {
                var streamWriter = new StreamWriter(stream);
                var jsonWriter = new JsonTextWriter(streamWriter);
                serializer.Serialize(jsonWriter, arg.RedeBayesianaJson);
                jsonWriter.Flush();
            }
            return arquivoRedeBayesiana;
        }
        public async Task<FileBayesianNetwork> GetRedeBayesiana(ArquivoRedeBayesiana arg)
        {
            
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //ContractResolver = new CollectionAsObjectResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            var serializer = JsonSerializer.Create(serializerSettings);

            FileBayesianNetwork redeBayesiana;

            // read from disk  
            using (FileStream stream = new FileStream(@"RepoArquivos/" + @arg.NomeArquivo, FileMode.Open))
            {
                var streamReader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(streamReader);
                redeBayesiana = serializer.Deserialize<FileBayesianNetwork>(jsonReader);
                
            }
            return redeBayesiana;
        }

        public async Task<List<String>> GetNodeRedeBayesiana(int idRede)
        {

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(idRede);


            ServiceRedeBayesiana serviceRede = new ServiceRedeBayesiana();
            FileBayesianNetwork redeBayesiana = JsonConvert.DeserializeObject<FileBayesianNetwork>(arquivoRedeBayesiana.ArquivoJson);

            List<String> nodeName = (from node in redeBayesiana.Nodes.ToList()
                                     select node.Id).ToList();

            return nodeName;
        }
    }
}
