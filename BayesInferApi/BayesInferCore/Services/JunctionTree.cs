﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BayesInferCore.Model;

namespace BayesInferCore.Services
{

	public class JunctionTree
	{
		private bool _geraLog;
		FileBayesianNetwork _redeBayesiana;

		private List<Clique> _cliques;

		/**
		 *  List of Associated separatorsMap.
		 */
		private List<Separator> _separators = new List<Separator>();


		ProbabilisticNetwork probabilisticNet;// = TranformModel(_redeBayesiana);
		LogWriter logWriter;
		public JunctionTree(FileBayesianNetwork redeBayesiana, bool geraLog = false)
		{
			_geraLog = geraLog;
			if (_geraLog)
				logWriter = new LogWriter("JunctionTree");

			_cliques = new List<Clique>();
			probabilisticNet = TranformModel(redeBayesiana);
			Moralize(probabilisticNet);
			List<ProbabilisticNode> nodeEliminationOrder = Triangulate(probabilisticNet);
			Cliques(probabilisticNet, nodeEliminationOrder);
			StrongTreeMethod(probabilisticNet, nodeEliminationOrder);
			SortCliqueNodes(probabilisticNet, nodeEliminationOrder);
			AddVariablesToCliqueAndSeparatorTables(probabilisticNet);
		}


		public void GenerateBasicEdges(ProbabilisticNetwork net)
		{
			foreach (var item in net.GetNodes())
			{
				foreach (var parent in item.Parents)
				{
					if ((net.HasEdge(parent, item, net.GetEdges()) < 0))
					{
						Edges moralizationEdge = new Edges(parent, item);

						net.GetEdges().Add(moralizationEdge);
						if (_geraLog)
						{
							logWriter.LogWrite(parent.Name + " - " + item.Name + "\n");
						}
					}
				}
			}
		}

		public void Moralize(ProbabilisticNetwork net)
		{

			// reset adjacency info
			foreach (ProbabilisticNode node in net.GetNodes())
			{
				node.ClearAdjacents();
			}
			GenerateBasicEdges(net);
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
							if ((net.HasEdge(parent1, parent2, net.GetEdges()) < 0))
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
		public List<ProbabilisticNode> Triangulate(ProbabilisticNetwork net)
		{
			List<ProbabilisticNode> auxNodes;
			List<ProbabilisticNode> nodeList = net.GetNodes();

			if (_geraLog)
			{
				logWriter.LogWrite("triangulateLabel");
			}
			auxNodes = nodeList.ToList();
			List<ProbabilisticNode> nodeEliminationOrder = new List<ProbabilisticNode>();
			// fill nodeEliminationOrder until there is no more nodes that can be eliminated in triangulation process
			while (MinimumWeightElimination(auxNodes, net, nodeEliminationOrder)) ;
			MakeAdjacents(net);
			return nodeEliminationOrder;
		}

		private bool MinimumWeightElimination(List<ProbabilisticNode> nodes, ProbabilisticNetwork net, List<ProbabilisticNode> nodeEliminationOrder)
		{
			if (nodeEliminationOrder == null)
			{
				nodeEliminationOrder = new List<ProbabilisticNode>();
			}
			bool hasSome = true;

			while (hasSome)
			{
				hasSome = false;

				for (int i = nodes.Count - 1; i >= 0; i--)
				{
					ProbabilisticNode auxNode = nodes[i];

					if (IsNeedingMoreArc(auxNode, net))
					{
						continue;
					}

					for (int j = auxNode.Adjacents.Count - 1; j >= 0; j--)
					{
						ProbabilisticNode v = auxNode.Adjacents[j];
						v.Adjacents.Remove(auxNode);
					}
					nodes.Remove(auxNode);
					hasSome = true;
					nodeEliminationOrder.Add(auxNode);
					if (_geraLog)
					{
						logWriter.LogWrite("\t" + nodeEliminationOrder.Count + " " + auxNode.Name + "\n");
					}
				}
			}

			if (nodes.Count() > 0)
			{
				ProbabilisticNode auxNo = Weight(nodes); //auxNo: clique with maximum weight.
				nodeEliminationOrder.Add(auxNo);
				if (_geraLog)
				{
					logWriter.LogWrite("\t" + nodeEliminationOrder.Count() + " " + auxNo.Name + "\n");
				}
				AddChordAndEliminateNode(auxNo, nodes, net); //Eliminate node and reduce the scope to be considered.
				return true;
			}

			return false;
		}
		private ProbabilisticNode Weight(List<ProbabilisticNode> nodes)
		{
			ProbabilisticNode v;
			ProbabilisticNode auxNode;
			double p;

			ProbabilisticNode noMin = null;
			double pmin = Double.MaxValue;

			for (int i = nodes.Count() - 1; i >= 0; i--)
			{
				auxNode = nodes[i];
				p = Math.Log(auxNode.States.Count);

				for (int j = auxNode.Adjacents.Count() - 1; j >= 0; j--)
				{
					v = auxNode.Adjacents[j];
					p += Math.Log(v.States.Count());
				}
				if (p < pmin)
				{
					pmin = p;
					noMin = auxNode;
				}
			}

			//		assert noMin != null;
			return noMin;
		}

		/// <summary>
		/// para verificar se precisamos inserir outro arco para eliminar um nó.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="net"></param>
		/// <returns></returns>
		private bool IsNeedingMoreArc(ProbabilisticNode node, ProbabilisticNetwork net)
		{
			if (node.Adjacents.Count() < 2)
			{
				return false;
			}

			for (int i = node.Adjacents.Count - 1; i > 0; i--)
			{
				ProbabilisticNode auxNo1 = node.Adjacents[i];

				for (int j = i - 1; j >= 0; j--)
				{
					ProbabilisticNode auxNo2 = node.Adjacents[j];
					if (!auxNo2.Adjacents.Contains(auxNo1))
					{
						return true;
					}
				}
			}
			return false;
		}
		private void AddChordAndEliminateNode(ProbabilisticNode node, List<ProbabilisticNode> nodes, ProbabilisticNetwork net)
		{
			for (int i = node.Adjacents.Count() - 1; i > 0; i--)
			{
				ProbabilisticNode auxNode1 = node.Adjacents[i];

				for (int j = i - 1; j >= 0; j--)
				{
					ProbabilisticNode auxNode2 = node.Adjacents[j];
					if (!auxNode2.Adjacents.Contains(auxNode1))
					{
						Edges auxArco = new Edges(auxNode1, auxNode2);
						if (_geraLog)
						{
							logWriter.LogWrite(
										auxNode1.Name + " <-> "
									+ auxNode2.Name
									+ "\n");
						}
						net.GetEdges().Add(auxArco);
						auxNode1.Adjacents.Add(auxNode2);
						auxNode2.Adjacents.Add(auxNode1);
					}
				}
			}

			for (int i = node.Adjacents.Count() - 1; i >= 0; i--)
			{
				ProbabilisticNode auxNo1 = node.Adjacents[i];
				auxNo1.Adjacents.Remove(node);
			}
			nodes.Remove(node);
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
		private void Cliques(ProbabilisticNetwork net, List<ProbabilisticNode> nodeEliminationOrder)
		{

			// this will be filled with cliques instantiated so far
			List<Clique> generatedCliques = new List<Clique>();

			foreach (ProbabilisticNode node in net.GetNodes().ToList())
			{
				int eliminationOrderIndex = nodeEliminationOrder.IndexOf(node);
				Clique newClique = new Clique();
				newClique.Nodes.Add(node);

				foreach (ProbabilisticNode adjacentNode in node.Adjacents)
				{
					if (nodeEliminationOrder.IndexOf(adjacentNode) > eliminationOrderIndex)
					{
						newClique.Nodes.Add(adjacentNode);
					}
				}
				generatedCliques.Add(newClique);
			}
			int numCliques = generatedCliques.Count;
			generatedCliques = generatedCliques.OrderBy(x => x.Nodes.Count()).ToList();

			// only consider cliques that are not entirely contained in another clique (e.g. if there are clique {A,B} and {A,B,C}, consider only {A,B,C} and discard {A,B})
			int indexClique = 0;
			for (int i = 0; i < numCliques; i++)
			{
				bool valid = true;
				Clique clique1 = generatedCliques[i];
				for (int j = i + 1; j < numCliques; j++)
				{
					Clique clique2 = generatedCliques[j];
					if (clique2.Nodes.Union(clique1.Nodes).Count() == clique2.Nodes.Count())
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					_cliques.Add(clique1);
				}	
			}
		}
		private void StrongTreeMethod(ProbabilisticNetwork net, List<ProbabilisticNode> nodeEliminationOrder)
		{
			int ndx;
			Clique auxClique;
			Clique auxClique2;
			List<ProbabilisticNode> uni;
			List<ProbabilisticNode> inter;
			List<ProbabilisticNode> auxList;
			List<ProbabilisticNode> listaNos;
			Separator sep;
			List<ProbabilisticNode> alpha = new List<ProbabilisticNode>();

			for (int i = nodeEliminationOrder.Count() - 1; i >= 0; i--)
			{
				alpha.Add(nodeEliminationOrder[i]);
			}

			if (net.GetNodes().Count() > 1)
			{
				int sizeCliques = _cliques.Count();
				for (int i = 0; i < sizeCliques; i++)
				{
					auxClique = (Clique)_cliques[i];
					listaNos = auxClique.Nodes.ToList();
					//calculate index
					while ((ndx = GetCliqueIndexFromAlphaOrder(listaNos, alpha)) <= 0 && listaNos.Count > 1) ;
					if (ndx < 0)
					{
						ndx = 0;
					}
					auxClique.Index = ndx; //= _cliques[ndx];
					listaNos.Clear();
				}
				alpha.Clear();
				_cliques = _cliques.OrderBy(x => x.Index).ToList();
				auxClique = (Clique)_cliques[0];
				uni = auxClique.Nodes.ToList();

				int sizeCliques1 = _cliques.Count();
				for (int i = 1; i < sizeCliques1; i++)
				{
					auxClique = _cliques[i];
					inter = auxClique.Nodes.Intersect(uni).ToList();
					for (int j = 0; j < i; j++)
					{
						auxClique2 = (Clique)_cliques[j];

						if (!(auxClique2.Nodes.Union(inter).Count() == auxClique2.Nodes.Count()))
						{
							continue;
						}

						sep = new Separator(auxClique2, auxClique);
						sep.Nodes=inter;
						_separators.Add(sep);

						auxList = auxClique.Nodes.Union(uni).ToList();
						uni.Clear();
						uni = auxList;
						break;
					}
				}
			}
		}
		private int GetCliqueIndexFromAlphaOrder(List<ProbabilisticNode> nodesInClique, List<ProbabilisticNode> ordering)
		{

			// get the maximum index node
			int maxIndex = -1;
			ProbabilisticNode nodeWithMaxIndex = null;
			foreach (ProbabilisticNode node in nodesInClique)
			{
				int currentIndex = ordering.IndexOf(node);
				if (maxIndex < currentIndex)
				{
					maxIndex = currentIndex;
					nodeWithMaxIndex = node;
				}
			}

			// disconsider the max node
			nodesInClique.Remove(nodeWithMaxIndex);
			if (nodesInClique.Count() == 0)
			{
				return maxIndex;
			}

			// Build list of common neighbors of nodes in clique
			List<ProbabilisticNode> neighbors = nodesInClique[0].Adjacents.ToList();
			int numNodes = nodesInClique.Count();
			for (int i = 1; i < numNodes; i++)
			{
				List<ProbabilisticNode> intersection = neighbors.Intersect(nodesInClique[i].Adjacents).ToList();
				neighbors = intersection;
			}
			neighbors.Remove(nodeWithMaxIndex);

			// return 0 if all neighbors out of this clique has higher index
			int neighborIndex = 0;
			foreach (ProbabilisticNode neighbor in neighbors)
			{
				if (nodesInClique.Contains(neighbor) || (ordering.IndexOf(neighbor) > maxIndex))
				{
					continue;
				}
				neighborIndex = maxIndex;
				break;
			}

			return neighborIndex;
		}

		private void SortCliqueNodes(ProbabilisticNetwork net, List<ProbabilisticNode> nodeEliminationOrder)
		{
			List<Clique> cliqueList = _cliques;

			for (int k = 0; k < cliqueList.Count; k++)
			{
				Clique clique = (Clique)cliqueList[k];
				List<ProbabilisticNode> nodesInClique = clique.Nodes;
				bool hasSwapped = true;
				// In general, cliques are expected to be small (much less than 100 nodes), so bubble sort should be OK for most of cases.
				while (hasSwapped)
				{
					hasSwapped = false;
					for (int i = 0; i < nodesInClique.Count() - 1; i++)
					{
						ProbabilisticNode node1 = nodesInClique[i];
						ProbabilisticNode node2 = nodesInClique[i + 1];
						// Note: cliques and separators must follow same node ordering patterns, so if you change something here, 
						// make sure to make the same changes for the block of code which treats separators.

							// TODO verify why ID and BN needs to use different ordering schemes
							if (nodeEliminationOrder.IndexOf(node1) > nodeEliminationOrder.IndexOf(node2))
							{
								nodesInClique[i + 1]= node1;
								nodesInClique[i]= node2;
								hasSwapped = true;
							}
						
					}
				}
			}

			foreach (Separator separator in _separators)
			{
				List<ProbabilisticNode> nodesInSeparator = separator.Nodes;
				// In general, separators are expected to be small (even smaller than cliques -- less than 100 nodes), so bubble sort should be OK for most of cases.
				bool hasSwapped = true;
				while (hasSwapped)
				{
					hasSwapped = false;
					for (int i = 0; i < nodesInSeparator.Count() - 1; i++)
					{
						ProbabilisticNode node1 = nodesInSeparator[i];
						ProbabilisticNode node2 = nodesInSeparator[i + 1];
						// Note: cliques and separators must follow same node ordering patterns, so if you make any change here, make sure the cliques are also following the same scheme.

							if (nodeEliminationOrder.IndexOf(node1) > nodeEliminationOrder.IndexOf(node2))
							{
								nodesInSeparator[i + 1]= node1;
								nodesInSeparator[i]= node2;
								hasSwapped = true;
							}
						
					}
				}
			}
		}
		private void AddVariablesToCliqueAndSeparatorTables(ProbabilisticNetwork net)
		{
			// TODO clean the code and stop using auxiliary variables reused along the entire method for different purposes.
			ProbabilisticNodeTable auxUtilTab;
			//Clique auxClique;

			for (int i = _cliques.Count() - 1; i >= 0; i--)
			{
				//auxClique = _cliques[i];
				int numNodes = _cliques[i].Nodes.Count();
				int numNodeState = 1;
				foreach (var item in _cliques[i].Nodes)
				{
					numNodeState *= item.States.Count(); 
				}
				_cliques[i].PotentialTable = new List<ProbabilisticTable>();
				for (int l = 0; l < numNodeState; l++)
				{
					_cliques[i].PotentialTable.Add(new ProbabilisticTable(l));
				}
				for (int j = 0; j < _cliques[i].Nodes.Count(); j++)
				{
					int ixState = 0;
					for (int k = 0; k < numNodeState; k++)
					{
						if(k>0 && ((k) % (numNodeState / (_cliques[i].Nodes[j].States.Count() * (j+1))) == 0))
						{
							ixState++;
							if(ixState>= _cliques[i].Nodes[j].States.Count())
							{
								ixState = 0;
							}
						}
						
						string stateString = _cliques[i].Nodes[j].States[ixState];
						_cliques[i].PotentialTable[k].TableCliqueSeparators.Add(new TableCliqueSeparator(stateString, _cliques[i].Nodes[j]));
					}
					
				}
			}




			foreach (Separator auxSep in _separators)
			{
				//auxTable = auxSep.getProbabilityFunction();
				//auxUtilTab = auxSep.getUtilityTable();
				//int numNodes = auxSep.getNodesList().size();
				//for (int c = 0; c < numNodes; c++)
				//{
				//	auxTable.addVariable(auxSep.getNodesList().get(c));
				//	auxUtilTab.addVariable(auxSep.getNodesList().get(c));
				//}
			}
			StringBuilder stringBuilder = new StringBuilder();
			
			foreach (var clique in _cliques)
			{
				stringBuilder.Append("\nIndice do clique=" + clique.Index+ "\n");
				stringBuilder.Append("Nodos:");
				foreach (var item in clique.Nodes)
				{
					stringBuilder.Append(item.Name+" - ");
				}
				stringBuilder.Append("\nTabela Probabilidades:\n");
				foreach (var linha in clique.PotentialTable)
				{
					stringBuilder.Append("\nIndice :"+linha.Index+"- Prob:"+ Math.Round(linha.Prob, 3)+" |");
					foreach (var coluna in linha.TableCliqueSeparators)
					{
						stringBuilder.Append("Nodo :" + coluna.NodeBase.Name+ " - Estado: "+coluna.StateBase + " - Valor: "+coluna.StateBaseValue+" |");
					}
				}
				logWriter.LogWrite(stringBuilder.ToString());
				stringBuilder.Clear();
			}


		}

		public void FowardPropagation(ProbabilisticNetwork net)
		{
			for (int i = _cliques.Count() - 1; i >= 0; i--)
			{

				for (int m = 0; m < _cliques[i].PotentialTable.Count; m++)
				{
					_cliques[i].PotentialTable[m].Prob = 1;
					foreach (var item in _cliques[i].PotentialTable[m].TableCliqueSeparators)
					{
						ProbabilisticNodeTable tabelaNode = item.NodeBase.PriorTabelaNode;
						if (item.NodeBase.Parents.Count == 0)
						{
							var a = item.NodeBase.PriorTabelaNode.TableNodeStates.Where(n => n.StateBase == item.StateBase).FirstOrDefault();
							item.StateBaseValue = a.StateBaseValue;
						}
						else
						{
							List<TableNodeState> tableNodeStates = item.NodeBase.PriorTabelaNode.TableNodeStates.Where(n => n.StateBase == item.StateBase).ToList();


							var qtd = _cliques[i].Nodes.Intersect(item.NodeBase.Parents);
							if (qtd.Count() == 0)
							{
								item.StateBaseValue = 1;
								foreach (var state in tableNodeStates)
								{
									item.StateBaseValue += state.StateBaseValue;
								}
							}
							else
							{
								foreach (var tableNodeState in tableNodeStates)
								{
									bool valueOk = true;
									foreach (var stateParent in tableNodeState.NodeStatesParent)
									{
										var parent = _cliques[i].PotentialTable[m].TableCliqueSeparators.Where(n => n.NodeBase.Name == stateParent.NodeValue.Name && n.StateBase == stateParent.StateString).FirstOrDefault();
										if (parent == null)
										{
											valueOk = false;
											break;
										}
									}
									if (valueOk)
									{
										item.StateBaseValue = tableNodeState.StateBaseValue;
										break;
									}
								}
							}



						}
						_cliques[i].PotentialTable[m].Prob *= item.StateBaseValue;
					}
				}
			}
		}

		//private ProbabilisticTable GetTableProb(ProbabilisticNode node, int indexTable, int indexState, List<ProbabilisticTable> probabilisticTable)
		//{
		//	if (indexNodeProx < clique.Nodes.Count())
		//	{
		//		ProbabilisticNode probabilisticNode = clique.Nodes[indexNodeProx];
		//		string state = clique.Nodes[indexNodeProx].States[indexState];
		//		TableNodeState tableNodeState = probabilisticNode.PriorTabelaNode.TableNodeStates.Find(x => x.StateBase == state);

		//		//TableCliqueSeparator tableCliqueSeparator = new TableCliqueSeparator();

		//	}
		//		return probabilisticTable;
		//}
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
				if (node.Parents != null & node.Parents.Count() > 0)
				{
					foreach (var parent in node.Parents)
					{
						ProbabilisticNode nodeParent = _probabilisticNetwork.GetNodes().Find(x => x.Name == parent);
						nodeParent.AddChild(probabilisticNode);
						probabilisticNode.AddParent(nodeParent);
					}
					for (int i = 0; i < node.States.Count; i++)
					{
						foreach (var item in node.Cpts)
						{
							List<NodeState> nodeStates = new List<NodeState>();
							foreach (KeyValuePair<string, string> pairNodeCrenca in item.WhenDic)
							{
								ProbabilisticNode probNode = probabilisticNode.Parents.Find(x => x.Name == pairNodeCrenca.Key);
								nodeStates.Add(new NodeState(probNode, pairNodeCrenca.Value));
							}
							probabilisticNode.PriorTabelaNode.AddNodeState(node.States[i], float.Parse(item.ThenDic[node.States[i]], CultureInfo.InvariantCulture), nodeStates);
						}
					}
				}
				else
				{

					foreach (KeyValuePair<string, string> pairNodeCrenca in node.Cpts[0].SingleCptDic)
					{
						probabilisticNode.PriorTabelaNode.AddNodeState(pairNodeCrenca.Key, float.Parse(pairNodeCrenca.Value, CultureInfo.InvariantCulture), null);
					}
				}
			});
			return _probabilisticNetwork;

		}



	}
}
