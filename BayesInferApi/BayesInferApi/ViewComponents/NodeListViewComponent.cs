using BayesInferApi.Data;
using BayesInferApi.Models;
using BayesInferApi.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BayesInferApi.ViewComponents
{
    [ViewComponent(Name = "NodeList")]
    public class NodeListViewComponent : ViewComponent
    {

        private readonly BayesInferContext _context;
        private INodeBeliefService _nodeBeliefService;

        public NodeListViewComponent(BayesInferContext context, INodeBeliefService nodeBeliefService)
        {
            _context = context;
            _nodeBeliefService = nodeBeliefService;
        }



        private async Task<List<NodeBelief>> GetItemsAsync()
        {
            return _nodeBeliefService.NodesBelief;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string MyView = "Default";
            var items = await GetItemsAsync();
            return View(MyView, items);
        }
    }
}
