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
	}
}
