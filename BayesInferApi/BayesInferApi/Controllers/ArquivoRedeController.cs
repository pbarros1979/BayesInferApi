using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayesInferApi.Data;
using BayesInferApi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BayesInferApi.ViewModels;
using Newtonsoft.Json;
using BayesInferApi.Service;
using BayesInferCore.Model;

namespace BayesInferApi.Controllers
{
    public class ArquivoRedeController : Controller
    {
        private readonly BayesInferContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ArquivoRedeController(BayesInferContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: ArquivoRede
        public async Task<IActionResult> Index()
        {
            return View(await _context.ArquivosRedeBayesiana.ToListAsync());
        }

        // GET: ArquivoRede/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana
                .FirstOrDefaultAsync(m => m.ID == id);
            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }

            return View(arquivoRedeBayesiana);
        }

        // GET: ArquivoRede/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, string nomeRede, string descricaoRede)
        {
            var result = string.Empty;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                result = reader.ReadToEnd();
            }

            ArquivoRedeViewModel arquivoRede = new ArquivoRedeViewModel();
            arquivoRede.RedeBayesianaJson = JsonConvert.DeserializeObject<FileBayesianNetwork>(result);
            arquivoRede.Descricao = descricaoRede;
            arquivoRede.NomeRede = nomeRede;

            ArquivoRedeService arquivoRedeService = new ArquivoRedeService(_context);
            ArquivoRedeBayesiana arq = await arquivoRedeService.AddArquivoRedeBayesiana(arquivoRede);


            return RedirectToAction("Details", new { id = arq.ID });
           // return View(arq);
        }

        // GET: ArquivoRede/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(id);
            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }
            return View(arquivoRedeBayesiana);
        }

        // POST: ArquivoRede/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NomeRede,NomeArquivo,Descricao,DataUpload")] ArquivoRedeBayesiana arquivoRedeBayesiana)
        {
            if (id != arquivoRedeBayesiana.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arquivoRedeBayesiana);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArquivoRedeBayesianaExists(arquivoRedeBayesiana.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(arquivoRedeBayesiana);
        }

        // GET: ArquivoRede/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana
                .FirstOrDefaultAsync(m => m.ID == id);
            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }

            return View(arquivoRedeBayesiana);
        }

        // POST: ArquivoRede/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(id);
            _context.ArquivosRedeBayesiana.Remove(arquivoRedeBayesiana);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArquivoRedeBayesianaExists(int id)
        {
            return _context.ArquivosRedeBayesiana.Any(e => e.ID == id);
        }
    }
}
