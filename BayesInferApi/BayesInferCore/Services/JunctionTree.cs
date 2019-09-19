using BayesInferCore.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BayesInferCore.Services
{
	public class JunctionTree
	{
		private bool _geraLog;
		FileBayesianNetwork _redeBayesiana;

		ProbabilisticNetwork probabilisticNet;// = TranformModel(_redeBayesiana);
		LogWriter logWriter;
		public JunctionTree(FileBayesianNetwork redeBayesiana, bool geraLog = false)
		{
			_geraLog = geraLog;
			if(_geraLog)
				logWriter = new LogWriter("JunctionTree");
		
			probabilisticNet = TranformModel(redeBayesiana);
			Moralize(probabilisticNet);
			

		}




		public void Moralize(ProbabilisticNetwork net)
		{

			// reset adjacency info
			foreach (ProbabilisticNode node in net.GetNodes())
			{
				node.ClearAdjacents();
			}
			int sizeNos = net.GetNodes().Count();
			for (int n = 0; n < sizeNos; n++)
			{
				ProbabilisticNode currentNode = net.GetNodes()[n];
				if (currentNode.Parents.Count() > 1)
				{
					int sizePais = currentNode.Parents.Count();
					for (int j = 0; j < sizePais - 1; j++)
					{
						ProbabilisticNode parent1 = currentNode.Parents[j];
						for (int k = j + 1; k < sizePais; k++)
						{
							ProbabilisticNode parent2 = currentNode.Parents[k];
							if ((net.hasEdge(parent1, parent2, net.GetEdges()) < 0))
							{
								Edges moralizationEdge = new Edges(parent1, parent2);

								net.GetEdges().Add(moralizationEdge);
								if (_geraLog)
								{
									logWriter.LogWrite(parent1.Name + " - " + parent2.Name + "\n");
								}
							}
						}
					}
				}
			}

			MakeAdjacents(net);

		}

		public void MakeAdjacents(ProbabilisticNetwork net)
		{
			// resets the adjacency information
			foreach (ProbabilisticNode node in net.GetNodes())
			{
				node.ClearAdjacents();
			}
			for (int z = net.GetEdges().Count - 1; z >= 0; z--)
			{
				Edges arc = net.GetEdges()[z];
				arc.GetOriginNode().Adjacents.Add(arc.GetDestinationNode());
				arc.GetDestinationNode().Adjacents.Add(arc.GetOriginNode());
			}
		}


		public FileBayesianNetwork InferModel(List<Belief> crenca)
		{
			




			return TranformModel(probabilisticNet);
		}

		private FileBayesianNetwork TranformModel(ProbabilisticNetwork probabilisticNet)
		{
			FileBayesianNetwork _network = new FileBayesianNetwork();

			return _network;

		}

		private ProbabilisticNetwork TranformModel(FileBayesianNetwork net)
		{
			ProbabilisticNetwork _probabilisticNetwork = new ProbabilisticNetwork();

			//// Variáveis aleatórias primárias
			object syncNetwork = new object();
			long qtdNodosPendentes = 0;

			foreach (var node in net.Nodes)
			{
				ProbabilisticNode pNode = new ProbabilisticNode();
				pNode.Name = node.Id;
				pNode.States = node.States;
				pNode.Description = node.Id;
				_probabilisticNetwork.AddNode(pNode);
			}
			Parallel.ForEach(net.Nodes, (node) =>
			{
				ProbabilisticNode probabilisticNode = _probabilisticNetwork.getNode(node.Id);
				if (node.Parents != null & node.Parents.Count()>0)
				{
					foreach (var parent in node.Parents)
					{
						ProbabilisticNode nodeParent = _probabilisticNetwork.GetNodes().Find(x => x.Name == parent);
						nodeParent.AddChild(probabilisticNode);
						probabilisticNode.AddParent(nodeParent);
					}
					for (int i = 0; i <node.States.Count ; i++)
					{
						foreach (var item in node.Cpts)
						{
							List<NodeState> nodeStates = new List<NodeState>();
							foreach(KeyValuePair<string, string> pairNodeCrenca in item.WhenDic)
							{
								ProbabilisticNode probNode = probabilisticNode.Parents.Find(x => x.Name == pairNodeCrenca.Key);
								nodeStates.Add(new NodeState(probNode, pairNodeCrenca.Value));
							}
							probabilisticNode.PriorTabelaNode.AddNodeState(node.States[i], float.Parse(item.ThenDic[node.States[i]], CultureInfo.InvariantCulture),nodeStates);
						}
					}
				}
				else
				{
					
					foreach (KeyValuePair<string, string> pairNodeCrenca in node.Cpts[0].SingleCptDic)
					{
						probabilisticNode.PriorTabelaNode.AddNodeState(pairNodeCrenca.Key, float.Parse(pairNodeCrenca.Value, CultureInfo.InvariantCulture),null);
					}
				}
			});
			return _probabilisticNetwork;

		}


		private void LoadModel()
		{
			//// Variáveis aleatórias primárias
			object syncNetwork = new object();
			long qtdNodosPendentes = 0;
			long teste = 0;
			do
			{
				object sync = new object();
				qtdNodosPendentes = 0;
				Parallel.ForEach(_redeBayesiana.Nodes, () => 0, (node, loop, pendentes) =>
				{
					ServiceNode serviceNode = new ServiceNode(ref node, ref _redeBayesiana);
					lock (syncNetwork)
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
			} while (qtdNodosPendentes != 0);
		}


	}
}
