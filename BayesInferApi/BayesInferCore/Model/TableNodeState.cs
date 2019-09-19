using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class TableNodeState
	{

		public string StateBase { get; private set; }
		public float StateBaseValue { get; private set; }
		public List<NodeState> NodeStatesParent { get; private set; }

		public TableNodeState(string stateBase ,float stateBaseValue, List<NodeState> nodeStates=null)
		{
			NodeStatesParent = nodeStates==null?new List<NodeState>():nodeStates;
			StateBaseValue = stateBaseValue;
			StateBase = stateBase;
		}
		public void SetStateValue(string stateBase, float stateBaseValue, List<NodeState> nodeStates=null)
		{
			NodeStatesParent = nodeStates == null ? new List<NodeState>() : nodeStates;
			StateBaseValue = stateBaseValue;
			StateBase = stateBase;
		}
	}
}
