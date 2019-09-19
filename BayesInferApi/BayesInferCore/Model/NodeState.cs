using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class NodeState
	{
		public NodeState(ProbabilisticNode node, String state	)
		{
			NodeValue = node;
			StateString = state;
		}
		public ProbabilisticNode NodeValue { get; set; }
		public string StateString { get; set; }
	}
}
