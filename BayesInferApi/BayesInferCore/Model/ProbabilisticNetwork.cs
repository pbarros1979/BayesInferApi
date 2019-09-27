using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticNetwork
	{
		protected String id;
		protected String name;
		protected List<ProbabilisticNode> NodeList;
		protected List<Edges> EdgeList;


		public ProbabilisticNetwork()
		{
			NodeList = new List<ProbabilisticNode>();
			EdgeList = new List<Edges>();
		}
		public ProbabilisticNetwork(String name)
		{
			this.id = this.name = name;
			NodeList = new List<ProbabilisticNode>();
			EdgeList = new List<Edges>();
		}

		/**
		 *@return    the list of all edges. Caution: a modification in this list
		 * WILL affect the original network.
		 */
		public List<Edges> GetEdges()
		{
			return this.EdgeList;
		}

		/**
		 *@return  all nodes in the graph.
		 */
		public List<ProbabilisticNode> GetNodes()
		{
			return this.NodeList;
		}

		/**
		 * This is equivalent to calling {@link #getNodes()}
		 * and then {@link ArrayList#size()}
		 *@return    how many nodes this network contains.
		 */
		public int getNodeCount()
		{
			return NodeList == null ? 0 : NodeList.Count;
		}

		/**
		 * Returns a node in this network given an index.
		 * @param  index : the index of the node to retrieve
		 * @return	equivalent to calling {@link #getNodes()}
		 * and then {@link ArrayList#get(int)}.
		 */
		public ProbabilisticNode getNodeAt(int index)
		{
			return NodeList[index];
		}

		/**
		 *  Returns the node that has the given name.
		 *
		 *@param  name  node name.
		 *@return the node that has the given name.
		 */
		public ProbabilisticNode getNode(String name)
		{
			var node = NodeList.Find(x => x.Name == name);
			return node;
		}


		public void removeEdge(Edges edge)
		{
			edge.GetOriginNode().RemoveChild(edge.GetDestinationNode());
			edge.GetDestinationNode().RemoveParent(edge.GetOriginNode());
			//removeArc(edge);
		}

		/**
		 *  Add the node.
		 *
		 *@param  node  node to be added.
		 */
		public void AddNode(ProbabilisticNode node)
		{
			NodeList.Add(node);
		}

		/**
		 *  Adds an edge into the net.
		 *  
		 *  - The table of the destination node will be updated with the new Variable
		 *
		 * @param  edge  An edge to be inserted.
		 * @throws InvalidParentException 
		 */
		public void AddEdge(Edges edge)
		{
			edge.GetOriginNode().Children.Add(edge.GetDestinationNode());
			edge.GetDestinationNode().Parents.Add(edge.GetOriginNode());
			EdgeList.Add(edge);
		}



		///**
		// *  Verifies existence of an edge.
		// *
		// *@param  node1  : origin node
		// *@param  node2  : destination node
		// *@return      index of the edge in {@link #getEdges()}, or -1 if it does not exist.
		// */
		public int HasEdge(ProbabilisticNode node1, ProbabilisticNode node2)
		{
			// TODO use a more efficient structure (like hash map) instead of linear search on edgeList
			return HasEdge(node1, node2, EdgeList);
		}

		public int HasEdge(ProbabilisticNode node1, ProbabilisticNode node2, List<Edges> vetArcos)
		{
			if (node1 == node2)
			{
				return 1;
			}

			int sizeArcos = vetArcos.Count;
			Edges auxA;
			// TODO use Edge#equals
			for (int i = 0; i < sizeArcos; i++)
			{
				auxA = (Edges)vetArcos[i];
				if ((auxA.GetOriginNode() == node1)
					&& (auxA.GetDestinationNode() == node2)
					|| (auxA.GetOriginNode() == node2)
					&& (auxA.GetDestinationNode() == node1))
				{
					return i;
				}
			}
			return -1;
		}

		///**
		// * Retorna o arco entre dois nﾃｳs caso ele exista
		// * 
		// * @param no1 Nﾃｳ origem
		// * @param no2 Nﾃｳ destino
		// * @return o arco entre no1 e no 2 caso ele exista ou null cc. 
		// */
		//public Edge getEdge(Node no1, Node no2)
		//{

		//	List<Edge> vetArcos = edgeList;

		//	if (no1 == no2)
		//	{
		//		return null;
		//	}

		//	int sizeArcos = vetArcos.size();
		//	Edge auxA;
		//	for (int i = 0; i < sizeArcos; i++)
		//	{
		//		auxA = (Edge)vetArcos.get(i);
		//		if ((auxA.getOriginNode() == no1)
		//			&& (auxA.getDestinationNode() == no2)
		//			|| (auxA.getOriginNode() == no2)
		//			&& (auxA.getDestinationNode() == no1))
		//		{
		//			return (Edge)vetArcos.get(i);
		//		}
		//	}
		//	return null;
		//}


	}
}
