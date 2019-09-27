using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticNodeTable
	{
		public List<TableNodeState> TableNodeStates { get; private set; }

		public ProbabilisticNodeTable()
		{
			TableNodeStates = new List<TableNodeState>();
		}

		public void AddNodeState(string stateBase, float statesBaseValue, List<NodeState> nodeStates)
		{
			TableNodeStates.Add( new TableNodeState(stateBase, statesBaseValue, nodeStates));
		}

		public float Normalize()
		{
			float n = 0;
			float valor;

			foreach (var item in TableNodeStates)
			{
				n += item.StateBaseValue;
			}
			
			if (Math.Abs(n - 1.0) > 0.00005)
			{   // if precision is 4 digits, error margin is half of precision (i.e. 0.00005)
				foreach (var item in TableNodeStates)
				{
					valor = item.StateBaseValue;
					if (valor == 0.0)
					{
						// zeros will remain always zero.
						continue;
					}
					
					valor /= n;
					item.StateBaseValue=valor;
				}
			}
			return n;
		}
	}
}
