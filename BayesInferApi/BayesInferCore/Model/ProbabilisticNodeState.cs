using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticNodeState
	{
		public ProbabilisticNodeState()
		{

		}
		public ProbabilisticNodeState(ProbabilisticNodeState arg)
		{
			StateBase = arg.StateBase;
			StateBaseValue = arg.StateBaseValue;
		}

		public string StateBase { get; set; }
		public float StateBaseValue { get; set; }
	}
}
