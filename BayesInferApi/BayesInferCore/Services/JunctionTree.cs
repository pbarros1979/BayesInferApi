using System;
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
		//FileBayesianNetwork _redeBayesiana;
		private List<Clique> _cliques;
		/**
		 *  List of Associated separatorsMap.
		 */
		private List<Separator> _separators = new List<Separator>();
		ProbabilisticNetwork probabilisticNet;
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
			SortCliqueNodes(nodeEliminationOrder);
			AddVariablesToCliqueAndSeparatorTables();
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
			CliqueNodesComparer cliqueComparer = new CliqueNodesComparer();
			generatedCliques.Sort(cliqueComparer);

			// considere apenas cliques que não estão totalmente contidos em outra clique (por exemplo, se houver clique {A, B} e {A, B, C}, considere apenas {A, B, C} e descarte {A, B})
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
				//_cliques = _cliques.OrderBy(x => x.Index).ToList();

				CliqueComparer clcomp = new CliqueComparer();
				_cliques.Sort(clcomp);
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

						sep = new Separator(auxClique2, auxClique, true);
						sep.Nodes = inter;
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
		private void SortCliqueNodes(List<ProbabilisticNode> nodeEliminationOrder)
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
							nodesInClique[i + 1] = node1;
							nodesInClique[i] = node2;
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
							nodesInSeparator[i + 1] = node1;
							nodesInSeparator[i] = node2;
							hasSwapped = true;
						}

					}
				}
			}
		}
		private void AddVariablesToCliqueAndSeparatorTables()
		{
			Clique auxClique;
			Separator auxSeparator;

			for (int i = _cliques.Count() - 1; i >= 0; i--)
			{
				auxClique = _cliques[i];
				int numNodes = auxClique.Nodes.Count();
				int numNodeState = 1;
				foreach (var item in auxClique.Nodes)
				{
					numNodeState *= item.States.Count();
				}
				auxClique.PotentialTable = new List<ProbabilisticTable>();
				//auxClique.Potential = new PotentialTable();
				for (int l = 0; l < numNodeState; l++)
				{
					auxClique.PotentialTable.Add(new ProbabilisticTable(l));
				}
				for (int j = 0; j < auxClique.Nodes.Count(); j++)
				{
					int ixState = 0;
					for (int k = 0; k < numNodeState; k++)
					{
						if (k > 0 && ((k) % (numNodeState / (auxClique.Nodes[j].States.Count() * (j + 1))) == 0))
						{
							ixState++;
							if (ixState >= auxClique.Nodes[j].States.Count())
							{
								ixState = 0;
							}
						}
						string stateString = auxClique.Nodes[j].States[ixState];
						auxClique.PotentialTable[k].TableCliqueSeparators.Add(new TableCliqueSeparator(stateString, _cliques[i].Nodes[j]));
					}
				}
			}

			for (int i = 0; i < _separators.Count; i++)
			{
				auxSeparator = _separators[i];
				int numNodes = auxSeparator.Nodes.Count();
				int numNodeState = 1;
				foreach (var item in auxSeparator.Nodes)
				{
					numNodeState *= item.States.Count();
				}
				auxSeparator.PotentialTable = new List<ProbabilisticTable>();
				for (int m = 0; m < numNodeState; m++)
				{
					auxSeparator.PotentialTable.Add(new ProbabilisticTable(m));

				}
				for (int j = 0; j < auxSeparator.Nodes.Count(); j++)
				{
					int ixState = 0;
					for (int k = 0; k < numNodeState; k++)
					{
						if (k > 0 && ((k) % (numNodeState / (auxSeparator.Nodes[j].States.Count() * (j + 1))) == 0))
						{
							ixState++;
							if (ixState >= auxSeparator.Nodes[j].States.Count())
							{
								ixState = 0;
							}
						}

						string stateString = auxSeparator.Nodes[j].States[ixState];
						auxSeparator.PotentialTable[k].TableCliqueSeparators.Add(new TableCliqueSeparator(stateString, _separators[i].Nodes[j]));
					}

				}
			}
		}
		private void UpdateCliques()
		{
			foreach (var auxClique in _cliques)
			{
				//Percorre tabela potential para adicionar valor padrão para o estado do nodo
				foreach (var linhaCliq in auxClique.PotentialTable)
				{
					foreach (var columnCliq in linhaCliq.TableCliqueSeparators)
					{
						//Verifica se existe crença para o nodo
						if (columnCliq.NodeBase.BeliefValue != null)
						{
							if(columnCliq.NodeBase.States[(int)columnCliq.NodeBase.BeliefValue] == columnCliq.StateBase)
							{
								columnCliq.StateBaseValue = 1F;
								columnCliq.InitializedValue = true;
							}
							else
							{
								columnCliq.StateBaseValue = 0F;
								columnCliq.InitializedValue = true;
							}

							
						}
						else if (columnCliq.NodeBase.Parents.Count() > 0)
						{
							//recupera nodos do clique diferente do nodo atual 
							var otherNodes = linhaCliq.TableCliqueSeparators.Where(c => c.NodeBase != columnCliq.NodeBase).Select(n => n.NodeBase);
							//recupera lista de parents que estão no clique em questão diferente do atual
							var parentsInCliqNode = columnCliq.NodeBase.Parents.Intersect(otherNodes);
							//percorre lista de parents que estão no clique em questão diferente do nodo atual
							if (parentsInCliqNode.Count() > 0)
							{
								columnCliq.StateBaseValue = GetValuePriorTableNode(columnCliq.NodeBase, columnCliq.StateBase, linhaCliq);
								columnCliq.InitializedValue = true;

							}
							else //se clique não possui parents não é possivel calcular valor pois sera atualizado via separador, sendo assim valor=0
							{
								columnCliq.StateBaseValue = 0;
								//não é possove calcular sem propagar 
								columnCliq.InitializedValue = false;
							}
						}
						else
						{
							//pega valor do nodo para o estado em questão
							columnCliq.StateBaseValue = columnCliq.NodeBase.PriorTabelaNode.TableNodeStates.Where(n => n.StateBase.Equals(columnCliq.StateBase)).Select(n => n.StateBaseValue).FirstOrDefault();
							columnCliq.InitializedValue = true;
						}
					}
				}
				UpdateInitializedClique(auxClique);
			}
		}
		private void UpdateInitializedClique(Clique clique)
		{
			foreach (var linha in clique.PotentialTable)
			{
				linha.Prob.Clear();
				foreach (var prob in linha.TableCliqueSeparators.Where(i => i.InitializedValue).Select(p => p.StateBaseValue))
				{
					linha.Prob.Add(prob);
				}
			}
		}
		private bool ContainsExact(List<ProbabilisticNode> collection, ProbabilisticNode o)
		{
			foreach (var obj in collection)
			{
				if (obj == o)
				{
					return true;
				}
			}
			return false;
		}
		private bool ContainsAllExact(List<ProbabilisticNode> container, List<ProbabilisticNode> contents)
		{
			bool hasObject;
			foreach (var content in contents)
			{
				hasObject = false;
				foreach (var obj in container)
				{
					if (obj == content)
					{
						hasObject = true;
						break;
					}
				}
				if (!hasObject)
				{
					return false;
				}
			}
			return true;
		}
		private void CollectEvidence(Clique clique)
		{
			foreach (var auxClique in clique.Children)
			{
				CollectEvidence(auxClique);
				Absorb(clique, auxClique);
			}
		}
		private void DistributeEvidences(Clique clique)
		{
			foreach (Clique auxClique in clique.Children)
			{
				Absorb(auxClique, clique);
				DistributeEvidences(auxClique);
			}
		}
		private void Absorb(Clique clique1, Clique clique2)
		{

			//Recupera separador entre clique 1 e 2 
			Separator separator = GetSeparatorNoDirection(clique1, clique2);

			if (separator.InitializedSeparator)
			{
				separator.PotentialClone();
				separator.ClearTableProb();
			}
			for (int m = 0; m < clique2.PotentialTable.Count; m++)
			{
				//atualiza valores para tabela do separador em função do clic2
				List<ProbabilisticNode> intersectNodes = clique2.Nodes.Intersect(separator.Nodes).ToList();
				List<TableCliqueSeparator> tmpProbTables = new List<TableCliqueSeparator>();
				foreach (var node in intersectNodes)
				{
					TableCliqueSeparator table = clique2.PotentialTable[m].TableCliqueSeparators.Where(t => t.NodeBase.Equals(node)).FirstOrDefault();
					tmpProbTables.Add(table);
				}
				SetTableSeparator(tmpProbTables, separator, clique2.PotentialTable[m].GetProductProb());
			}
			if (separator.PotentialTableClone != null && separator.PotentialTableClone.Count() > 0)
			{
				separator.DividePotentials();
			}
			List<ProbabilisticNode> exceptNodesC1 = clique1.Nodes.Except(separator.Nodes).ToList();
			foreach (var linha in separator.PotentialTable)
			{
				SetProbClique(linha, exceptNodesC1, clique1);
			}




			//Verifica se é ultimo clic
			//if (clique1.Parent == null)
			//{
			//	//Verifica se ja foram calculados todos os cliques e separadores filhos do clique1
			//	var sepPreviousClique1 = _separators.Where(s => s.clique1.Equals(clique1));
			//	if (sepPreviousClique1 != null)
			//	{
			//		if (sepPreviousClique1.Where(s => s.SepStage == Stage.CollectEvidence).Count() != sepPreviousClique1.Count())
			//		{
			//			return;
			//		}
			//	}
			//}
		}
		public void InitBelief()
		{
			Clique auxClique;
			for (int i = 0; i < _cliques.Count(); i++)
			{
				auxClique = _cliques[i];
				var nodes = auxClique.Nodes.Where(p => p.PosteriorTabelaNode.Count() == 0);
				foreach (var node in nodes)
				{
					foreach (var linha in auxClique.PotentialTable)
					{
						foreach (var col in linha.TableCliqueSeparators)
						{
							if (col.NodeBase.Equals(node))
							{
								var prob = node.PosteriorTabelaNode.Find(p => p.StateBase == col.StateBase);
								if (prob != null)
								{
									prob.StateBaseValue += linha.GetProductProb();
								}
								else
								{
									node.PosteriorTabelaNode.Add(new ProbabilisticNodeState()
									{
										StateBase = col.StateBase,
										StateBaseValue = linha.GetProductProb()
									});
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Percorre tabela de propbabilidade a priori do node para recuperar valores presentes na linha da tebala do clique
		/// </summary>
		/// <param name="node">Node para recuperara valor a priori para estados de parent valido no clique</param>
		/// <param name="state">Estado desejado para busca</param>
		/// <param name="lineCliqueSeparator">Linha da tabela do clique para recuperar estados valido</param>
		/// <returns></returns>
		public float GetValuePriorTableNode(ProbabilisticNode node, string state, ProbabilisticTable lineCliqueSeparator)
		{
			float returnValue = 0;
			//pega valores da tabela a priori do nodo para o estado da tabela do clique
			var linhaTableNode = node.PriorTabelaNode.TableNodeStates.Where(t => t.StateBase.Equals(state));
			bool colLineValid = false;
			foreach (var col in linhaTableNode)
			{
				//inicializa boleana com true e verifica se parentes do estado atual da linha a priori tem mesmo valor do estado da tabela do clique
				colLineValid = true;
				foreach (var item in col.NodeStatesParent)
				{
					var tmp = lineCliqueSeparator.TableCliqueSeparators.FirstOrDefault(n => n.NodeBase.Equals(item.NodeValue));
					if (item.StateString != tmp.StateBase)
					{
						colLineValid = false;
					}
				}
				if (colLineValid)
				{
					returnValue = col.StateBaseValue;
					break;
				}
			}
			return returnValue;
		}

		private void SetTableSeparator(List<TableCliqueSeparator> tableCliqueSeparator, Separator separator, float prob)
		{
			foreach (var linha in separator.PotentialTable)
			{
				bool valid = true;
				foreach (var col in linha.TableCliqueSeparators)
				{
					var orig = tableCliqueSeparator.Find(n => n.NodeBase.Name == col.NodeBase.Name);
					if (orig.StateBase != col.StateBase)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					linha.Prob.Add(prob);
				}
			}
		}
		private void SetProbClique(ProbabilisticTable linhaSeparator, List<ProbabilisticNode> exceptNodes, Clique clique)
		{
			foreach (var linha in clique.PotentialTable)
			{
				bool valid = true;
				foreach (var col in linha.TableCliqueSeparators)
				{
					var orig = linhaSeparator.TableCliqueSeparators.FirstOrDefault(n => n.NodeBase.Equals(col.NodeBase));
					if (orig != null && orig.StateBase != col.StateBase)
					{
						valid = false;
						break;
					}
				}
				//Encontrou a linha valida do clique
				if (valid)
				{
					linha.Prob.Add(linhaSeparator.GetSumProb());
				}

			}
		}
		public Separator GetSeparator(Clique clique1, Clique clique2)
		{

			return _separators.FirstOrDefault(s => s.clique2.Index == clique2.Index && s.clique1.Index == clique1.Index);
		}
		public IEnumerable<Separator> GetAllSeparatorNoDirection(Clique clique1, Clique clique2)
		{
			var sep = _separators.Where(s => s.clique2.Index == clique2.Index && s.clique1.Index == clique1.Index);
			if (sep != null && sep.Count() > 0)
				return sep;
			return _separators.Where(s => s.clique2.Index == clique1.Index && s.clique1.Index == clique2.Index);
		}
		public Separator GetSeparatorNoDirection(Clique clique1, Clique clique2)
		{
			var sep = _separators.FirstOrDefault(s => s.clique2.Index == clique2.Index && s.clique1.Index == clique1.Index);
			if (sep != null)
				return sep;
			return _separators.FirstOrDefault(s => s.clique2.Index == clique1.Index && s.clique1.Index == clique2.Index);
		}

		public List<NodeInferResult> InferModel(List<Belief> lstBelief)
		{
			Parallel.ForEach(lstBelief, (belief) =>
			{
				//busca no com mesmo nome para adicionar crença
				probabilisticNet.GetNodes().Find(n => n.Name == belief.NodeName).BeliefValue = belief.BeliefValue;
			});
			UpdateCliques();
			VerifyRoot(_cliques[0]);
			CollectEvidence(_cliques[0]);
			DistributeEvidences(_cliques[0]);
			InitBelief();
			return TranformModel(probabilisticNet);
		}
		private List<NodeInferResult> TranformModel(ProbabilisticNetwork probabilisticNet)
		{
			List<NodeInferResult> result = new List<NodeInferResult>();

			foreach (var node in probabilisticNet.GetNodes())
			{
				NodeInferResult nodeInferResult = new NodeInferResult(node.Name);
				foreach (var linhaPos in node.PosteriorTabelaNode)
				{
					nodeInferResult.NodeStates.Add(new ProbabilisticNodeState(linhaPos));

				}
				result.Add(nodeInferResult);
			}
			return result;
		}
		private ProbabilisticNetwork TranformModel(FileBayesianNetwork net)
		{
			ProbabilisticNetwork _probabilisticNetwork = new ProbabilisticNetwork();
			//// Variáveis aleatórias primárias
			object syncNetwork = new object();
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
		private void VerifyRoot(Clique cliqueRoot)
		{
			bool validRoot = true;
			foreach (var item in _cliques[0].Nodes)
			{
				if (item.Parents.Count == 0)
				{
					validRoot = false;
				}
			}
			if (!validRoot)
			{
				InvertSeparator(cliqueRoot);

				int i = 0;
				foreach (var clique in _cliques.OrderByDescending(c => c.Index))
				{
					clique.Index = i;
					var sepChildren = _separators.Where(s => s.clique1.Equals(clique));
					clique.Children.Clear();
					foreach (var item in sepChildren)
					{
						clique.Children.Add(item.clique2);
					}
					var p = _separators.FirstOrDefault(s => s.clique2.Equals(clique));
					if (p != null)
					{
						clique.Parent = p.clique1;
					}
					else
					{
						clique.Parent = null;
					}
					i++;
				}

				_cliques = _cliques.OrderBy(c => c.Index).ToList();


			}
		}
		private void InvertSeparator(Clique cliqueRoot)
		{
			foreach (var auxClique in cliqueRoot.Children)
			{
				InvertSeparator(auxClique);
				var sep = GetSeparator(auxClique.Parent, auxClique);
				if (sep != null)
				{
					Clique aux = sep.clique2;
					sep.clique2 = sep.clique1;
					sep.clique1 = aux;
				}
			}


		}
	}

	public class CliqueNodesComparer : IComparer<Clique>
	{
		//Fonte https://docs.microsoft.com/pt-br/dotnet/api/system.collections.generic.list-1.sort?view=netframework-4.8
		public int Compare(Clique x, Clique y)
		{
			if (x == null)
			{
				if (y == null)
				{
					// If x is null and y is null, they're
					// equal. 
					return 0;
				}
				else
				{
					// If x is null and y is not null, y
					// is greater. 
					return -1;
				}
			}
			else
			{
				// If x is not null...
				//
				if (y == null)
				// ...and y is null, x is greater.
				{
					return 1;
				}
				else
				{
					// ...and y is not null, compare the 
					// lengths of the two strings.
					//
					int retval = x.Nodes.Count() - y.Nodes.Count();

					if (retval != 0)
					{
						// If the strings are not of equal length,
						// the longer string is greater.
						//
						return retval;
					}
					else
					{
						// If the strings are of equal length,
						// sort them with ordinary string comparison.
						//
						return x.Nodes.Count() - y.Nodes.Count();
					}
				}
			}
		}
	}
	public class CliqueComparer : IComparer<Clique>
	{
		//Fonte https://docs.microsoft.com/pt-br/dotnet/api/system.collections.generic.list-1.sort?view=netframework-4.8
		public int Compare(Clique x, Clique y)
		{
			Clique c1 = x;
			Clique c2 = y;
			if (c1.Index > c2.Index)
			{
				return 1;
			}
			if (c1.Index < c2.Index)
			{
				return -1;
			}
			return 0;
		}
	}

}

