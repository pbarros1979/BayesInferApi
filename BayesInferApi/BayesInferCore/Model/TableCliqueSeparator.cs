using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class TableCliqueSeparator
	{
		public ProbabilisticNode NodeBase { get;  set; }
		public string StateBase { get;  set; }
		public float StateBaseValue { get; set; }
		
		public TableCliqueSeparator(string stateBase , ProbabilisticNode nodeBase )
		{
			NodeBase = nodeBase;
			StateBase = stateBase;
		}

	}
}
