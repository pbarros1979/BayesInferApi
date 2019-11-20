using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BayesInferApi.Data;
using BayesInferApi.Models;
using BayesInferApi.Service;
using BayesInferApi.Util;
using BayesInferApi.ViewModels;
using BayesInferCore.Model;
using BayesInferCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BayesInferApi.Controllers
{
    public class NodeBeliefController : Controller
    {
        private readonly BayesInferContext _context;
        private INodeBeliefService _nodeBeliefService;
		private readonly ILogger _logger;




		public NodeBeliefController(BayesInferContext context, INodeBeliefService nodeBeliefService, ILogger<NodeBeliefController> logger)
        {
            _context = context;
            _nodeBeliefService = nodeBeliefService;
			_logger = logger;
		}
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Index()
        {
			return View(await _context.ArquivosRedeBayesiana.ToListAsync());
        }
        [HttpPost]
        public ActionResult Create(NodeBelief nodeBelief)
        {

            if (nodeBelief.BeliefType == (int)TypeBelief.Presente){
                nodeBelief.BeliefTypeName = "Presente";
            }               
            else if(nodeBelief.BeliefType == (int)TypeBelief.Ausente){
                nodeBelief.BeliefTypeName = "Ausente";
            }
            else{
                nodeBelief.BeliefTypeName = "Desault";
            }
            _nodeBeliefService.NodesBelief.Add(nodeBelief);

            return RedirectToAction("CREATE");
        }
        // GET: NetworkNodeBelief/Create
        public ActionResult Create(int id)
        {

            _nodeBeliefService.IdRede= id;
            ArquivoRedeBayesiana arquivoRedeBayesiana = _context.ArquivosRedeBayesiana.Find(id);

            if (arquivoRedeBayesiana == null)
            {
                return NotFound();
            }
            ServiceRedeBayesiana serviceRede = new ServiceRedeBayesiana();
            FileBayesianNetwork redeBayesiana = JsonConvert.DeserializeObject<FileBayesianNetwork>(arquivoRedeBayesiana.ArquivoJson);

            List<Node> lstNode = redeBayesiana.Nodes.Where(n => n.Parents.Count==0).ToList();

            foreach (var item in _nodeBeliefService.NodesBelief)
            {
                Node _node = lstNode.Where(n => n.Id == item.Id).SingleOrDefault();
                if (_node != null)
                    lstNode.Remove(_node);
            }
            ViewData["Id"] = new SelectList(lstNode, "Id", "Id");
            ViewData["BeliefType"] = new[]
            {
                new SelectListItem(){ Value = ((int)TypeBelief.Presente).ToString(), Text = "Presente"},
                new SelectListItem(){ Value = ((int)TypeBelief.Ausente).ToString(), Text = "Ausente"},
                new SelectListItem(){ Value = ((int)TypeBelief.Default).ToString(), Text = "Default"},
            };
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> InferBelief()
        {
            var arquivoRedeBayesiana = await _context.ArquivosRedeBayesiana.FindAsync(_nodeBeliefService.IdRede);

 
            ServiceRedeBayesiana serviceRede = new ServiceRedeBayesiana();
            FileBayesianNetwork redeBayesiana = JsonConvert.DeserializeObject<FileBayesianNetwork>(arquivoRedeBayesiana.ArquivoJson);
            redeBayesiana = serviceRede.LoadRedeBayesiana(redeBayesiana);


			List<Belief> beliefs = new List<Belief>();
			foreach (var item in _nodeBeliefService.NodesBelief)
			{
				beliefs.Add(new Belief
				{

					NodeName = item.Id,
					BeliefValue = item.BeliefType == 2 ? null : (int?)item.BeliefType

				});
			}
			JunctionTree junctionTree = new JunctionTree(redeBayesiana, true);
			List<NodeInferResult> lstNodeBeliefResult = junctionTree.InferModel(beliefs);
			List<NodeBeliefResult> nd = new List<NodeBeliefResult>();
			foreach (var item in lstNodeBeliefResult)
			{
				NodeBeliefResult rs = new NodeBeliefResult();
				rs.NodeName = item.NodeName;
				rs.ResultPresente = item.NodeStates[0].StateBaseValue;
				rs.ResultAusente = item.NodeStates[1].StateBaseValue;
				nd.Add(rs);
			}

			//return Json(lstNodeBeliefResult);
			return Json(nd);

        }

    }
}