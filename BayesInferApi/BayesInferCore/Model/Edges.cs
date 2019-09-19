using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class Edges
	{


		/**
		 *  Guarda o primeiro nó (origem).
		 */
		private ProbabilisticNode node1;

		/**
		 *  Guarda o segundo nó (destino).
		 */
		private ProbabilisticNode node2;

		

		/**
		 *  Status que indica se existe direção no arco.
		 */
		private bool direction;


		/// <summary>
		/// Creates an edge from node1 to node2
		/// </summary>
		/// <param name="no1">node1  starting node</param>
		/// <param name="no2">node2  destination node</param>
		public Edges(ProbabilisticNode no1, ProbabilisticNode no2)
		{
			this.node1 = no1;
			this.node2 = no2;

			direction = true;
		}

		public ProbabilisticNode GetOriginNode()
		{
			return node1;
		}

		public ProbabilisticNode GetDestinationNode()
		{
			return node2;
		}

		/// <summary>
		/// Retorna o status de direção do arco.
		/// </summary>
		/// <returns></returns>
		public bool hasDirection()
		{
			return direction;
		}





		/**
		 * Checks if this edge connects the specified nodes.
		 * @param originNode
		 * @param destinationNode
		 * @param isToIgnoreDirection : if true, the direction of this edge will not be considered when checking for link.
		 * @return true if this edge is connecting the specified nodes. False otherwise.
		 * @see #getOriginNode()
		 * @see #getDestinationNode()
		 */
		public bool IsConnectingNodes(ProbabilisticNode originNode, ProbabilisticNode destinationNode, bool isToIgnoreDirection)
		{
			if (GetOriginNode().Equals(originNode) && GetDestinationNode().Equals(destinationNode))
			{
				// this arc is node1->node2
				return true;
			}
			else if (isToIgnoreDirection && GetOriginNode().Equals(destinationNode) && GetDestinationNode().Equals(originNode))
			{
				// this arc is node2->node1
				return true;
			}
			// this arc either doesn't connect the nodes, or the direction was not the same.
			return false;
		}


		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
		public bool equals(Object obj)
		{

			// initial assertions
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}

			bool ret = false;

			// we may consider 2 edges are the same if they point to the same nodes
			if (obj.GetType()==typeof(Edges)) {
				Edges ed = (Edges)obj;
				if (this.node1 != null && this.node2 != null)
				{
					// no node is null
					ret = this.node1.Equals(ed.node1) && this.node2.Equals(ed.node2);

					// test undirected edge equality as well.
					if (!ret && !ed.hasDirection() && !this.hasDirection())
					{
						ret = this.node1.Equals(ed.node2) && this.node2.Equals(ed.node1);
					}
				}
				else if (this.node1 != null)
				{
					// node2 is null
					ret = this.node1.Equals(ed.node1) && (ed.node2 == null);

					// test undirected edge equality as well.
					if (!ret && !ed.hasDirection() && !this.hasDirection())
					{
						ret = this.node1.Equals(ed.node2) && (ed.node1 == null);
					}
				}
				else if (this.node2 != null)
				{
					//  node 1 is null
					ret = (ed.node1 == null) && this.node2.Equals(ed.node2);

					// test undirected edge equality as well.
					if (!ret && !ed.hasDirection() && !this.hasDirection())
					{
						ret = (ed.node2 == null) && this.node2.Equals(ed.node1);
					}
				}
				else
				{
					// both nodes are null
					ret = (ed.node1 == null) && (ed.node2 == null);
				}

			}

			return ret;
		}




	}
}
