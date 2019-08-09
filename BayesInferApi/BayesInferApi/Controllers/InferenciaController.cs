using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BayesInferApi.Data;
using BayesInferApi.Models;
using BayesInferApi.ViewModels;
using BayesInferCore.Model;
using BayesInferCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BayesInferApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InferenciaController : ControllerBase
    {
        private readonly BayesInferContext _context;

        public InferenciaController(BayesInferContext context)
        {
            _context = context;
        }

        // POST: api/Inferencia
        [HttpPost]
        public async Task<ActionResult<FileBayesianNetwork>> Inferencia([FromBody] InferViewModel data )
        {

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(data.IdRede);

            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }
            ServiceRedeBayesiana serviceRede = new ServiceRedeBayesiana();
            FileBayesianNetwork redeBayesiana = JsonConvert.DeserializeObject<FileBayesianNetwork>(arquivoRedeBayesiana.ArquivoJson);
            redeBayesiana = serviceRede.LoadRedeBayesiana(redeBayesiana);
            BayesInfer bayesInfer = new BayesInfer(redeBayesiana);

            redeBayesiana = bayesInfer.InferModel(data.ListBelief);


            return redeBayesiana;
        }

        
    }
}
