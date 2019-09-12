using BayesInferCore.Model;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace BayesInferCore.Services
{
    public class BayesInfer
    {
        FileBayesianNetwork _redeBayesiana;
       
        public BayesInfer(FileBayesianNetwork redeBayesiana)
        {
            _redeBayesiana = redeBayesiana;

            //Rand.Restart(12347);
            //_redeBayesiana.Engine = new InferenceEngine();
            //_redeBayesiana.NodoRaiz = Variable.New<int>().Named("NofE");
            //_redeBayesiana.N = new Range(_redeBayesiana.NodoRaiz).Named("N");
            //_redeBayesiana.NodoRaiz.ObservedValue = 1;

            ///Carrega Modelo para inferência
            LoadModel();
        }

        private void LoadModel()
        {
            //// Variáveis aleatórias primárias
            object syncRede = new object();
            long qtdNodosPendentes = 0;
            long teste = 0;
            do
            {
                object sync = new object();
                qtdNodosPendentes = 0;
                Parallel.ForEach(_redeBayesiana.Nodes, () => 0, (node, loop, pendentes) =>
                {
                    ServiceNode serviceNode = new ServiceNode(ref node, ref _redeBayesiana);
                    lock (syncRede)
                    {
                        bool _validParents = serviceNode.ValidParents;
                        if (node.situacaoNodo == 0 && _validParents)
                        {
                            serviceNode.SetState(node.Id, (int)Node.Estado.Carregado);
                            node.NodeParents = serviceNode.NodeParents;
                            serviceNode.LoadNode();
                            serviceNode.SetState(node.Id, (int)Node.Estado.Finalizado);
                        }
                        if (!_validParents)
                        {
                            pendentes++;
                        }
                    }
                    return pendentes;
                }, (finalResult) => Interlocked.Add(ref qtdNodosPendentes, finalResult)
                );
                teste += qtdNodosPendentes;
            } while (qtdNodosPendentes!=0);
        }

        public double InferModel(Dictionary<string, int?> crenca, String nodeInfer)
        {
            foreach (KeyValuePair<string, int?> pairNodeCrenca in crenca)
            {
                Node nodeResult = _redeBayesiana.Nodes.Where(x => x.Id == pairNodeCrenca.Key).SingleOrDefault();
                if (nodeResult != null)
                {
                    if (pairNodeCrenca.Value == null)
                    {
                       // nodeResult.InferPrimary.ClearObservedValue();
                    }
                    else
                    {
                        //nodeResult.InferPrimary.ObservedValue = new int[] { (int)pairNodeCrenca.Value };
                    }
                }
            }
			//Node ndInfer = _redeBayesiana.Nodes.Where(x => x.Id == nodeInfer).SingleOrDefault();
			//ndInfer.InferPrimary.ClearObservedValue();
			//var infer = _redeBayesiana.Engine.Infer<Discrete[]>(ndInfer.InferPrimary);
			//double resultInfer = infer[0].GetProbs()[0];
			//return resultInfer;
			return 0;
        }

        public FileBayesianNetwork InferModel(List<Belief> crenca)
        {
            FileBayesianNetwork _rede;
            foreach (var nodeCrenca in crenca)
            {
                Node nodeResult = _redeBayesiana.Nodes.Where(x => x.Id == nodeCrenca.NodeName).SingleOrDefault();
                if (nodeResult != null)
                {
                    if (nodeCrenca.BeliefValue == null)
                    {
                        //nodeResult.InferPrimary.ClearObservedValue();
                    }
                    else
                    {
                        Double[] val = new Double[] {0,0};
                        if (nodeCrenca.BeliefValue == 1)
                            val[0] = 1;
                        else
                            val[1] = 1;
                        //nodeResult.InferModelResult = new Discrete[] { new Discrete(val) };
                        //nodeResult.InferPrimary.ObservedValue = new int[] { (int)nodeCrenca.BeliefValue };
                    }
                }
            }
            foreach (var ndInfer in _redeBayesiana.Nodes)
            {
                var result = crenca.SingleOrDefault(v => v.NodeName == ndInfer.Id);
                if (result==null)
                {
                    //ndInfer.InferPrimary.ClearObservedValue();
					////Selecionando para inferir apenas quem tem parents
					//if (ndInfer.NodeParents.Count() > 0)
					//{
					//	ndInfer.InferModelResult = _redeBayesiana.Engine.Infer<Discrete[]>(ndInfer.InferPrimary);
					//}
					//Inferindo todo os nodos
					//ndInfer.InferModelResult = _redeBayesiana.Engine.Infer<Discrete[]>(ndInfer.InferPrimary);

				}
                else if(result.BeliefValue == null)
                {
                    //ndInfer.InferModelResult = _redeBayesiana.Engine.Infer<Discrete[]>(ndInfer.InferPrimary);
                }
            }
            _rede = _redeBayesiana;
            return _rede;
        }

    }
}
