using BayesInferCore.Model;
using BayesInferCore;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
namespace BayesInferCore.Services
{

    public class ServiceNode
    {
        Node _node;
        FileBayesianNetwork _redeBayesiana;
        System.IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");

        public void SetState(String nodeName, int estado)
        {
            object _lck = new object();
            Parallel.ForEach(_redeBayesiana.Nodes, i =>
            {
                if (i.Id == nodeName)
                {
                    lock (_lck)
                    {
                        i.situacaoNodo = estado;
                    }
                }
            });
        }
        public List<Node> NodeParents { get; set; }
        /// <summary>
        /// Verifica se Parents estão em estado de finalizado para que possa ser carregada a tabela CPT no nodo atual
        /// </summary>
        public bool ValidParents
        {
            get
            {
                if (NodeParents.Count == 0)
                    return true;

                bool nodoFinalizado = true;
                foreach (var item in NodeParents)
                {
                    if (item.situacaoNodo != (int)Node.Estado.Finalizado)
                    {
                        nodoFinalizado = false;
                    }
                }
                return nodoFinalizado;
            }
        }
        public ServiceNode(ref Node node, ref FileBayesianNetwork redeBayesiana)
        {
            _node = node;
            _redeBayesiana = redeBayesiana;
            InitNodeParents();
        }
        /// <summary>
        /// Carrega Node Parents para verificação posterior
        /// </summary>
        private void InitNodeParents()
        {
            NodeParents = new List<Node>();
            //Cria objeto lock exclusivo para esta task parelela, sendo que a adição dos nodos não impolica nos estados, 
            //sem apenas importante se nodo esta no estado 2 finalizado, desprezando estado 0 novo, e carregado 1
            object _lck = new object();
            Parallel.ForEach(_redeBayesiana.Nodes, (i, pls) =>
            {
                if (_node.Parents.Contains(i.Id))
                {
                    lock (_lck)
                    {
                        NodeParents.Add(i);
                    }
                };
                if (NodeParents.Count == _node.Parents.Count)
                    pls.Break();
            });
            _node.NodeParents = NodeParents;
        }
        public bool LoadNode()
        {

            if (ValidParents)
            {

                return true;
            }
            return false;
        }
    }
}
