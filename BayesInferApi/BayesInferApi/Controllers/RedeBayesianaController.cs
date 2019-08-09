using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayesInferApi.Data;
using BayesInferApi.Models;
using BayesInferApi.ViewModels;
using BayesInferApi.Service;

namespace BayesInferApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedeBayesianaController : ControllerBase
    {
        private readonly BayesInferContext _context;

        public RedeBayesianaController(BayesInferContext context)
        {
            _context = context;
        }

        // GET: api/ArquivoRedeBayesianas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArquivoRedeBayesiana>>> GetArquivosRedeBayesiana()
        {
            return await _context.ArquivosRedeBayesiana.ToListAsync();
        }

        // GET: api/ArquivoRedeBayesianas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArquivoRedeViewModel>> GetArquivoRedeBayesiana(int id)
        {
            ArquivoRedeViewModel arquivoRedeViewModel = new ArquivoRedeViewModel();
            ArquivoRedeService arquivoRedeService = new ArquivoRedeService(_context);

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(id);

            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }
            arquivoRedeViewModel.NomeRede = arquivoRedeBayesiana.NomeRede;
            arquivoRedeViewModel.Descricao = arquivoRedeBayesiana.Descricao;
            arquivoRedeViewModel.RedeBayesianaJson = await arquivoRedeService.GetRedeBayesiana(arquivoRedeBayesiana);

            return arquivoRedeViewModel;
        }

        // POST: api/ArquivoRedeBayesianas
        [HttpPost]
        public async Task<ActionResult<ArquivoRedeBayesiana>> PostArquivoRedeBayesiana(ArquivoRedeViewModel arg)
        {
            ArquivoRedeService arquivoRedeService = new ArquivoRedeService(_context);
            ArquivoRedeBayesiana arq = await arquivoRedeService.AddArquivoRedeBayesiana(arg);
            return CreatedAtAction("GetArquivoRedeBayesiana", new { id = arq.ID }, arq);
        }
    }
}
