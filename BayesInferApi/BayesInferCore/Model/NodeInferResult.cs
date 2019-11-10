using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class NodeInferResult
	{
		public NodeInferResult(string name)
		{
			NodeName = name;
			NodeStates = new List<ProbabilisticNodeState>();
		}
		public NodeInferResult()
		{
			NodeStates = new List<ProbabilisticNodeState>();
		}
		public string NodeName { get; set; }

		public List<ProbabilisticNodeState> NodeStates { get; set; }

	}
}
