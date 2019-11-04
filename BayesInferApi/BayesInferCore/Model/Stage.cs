using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public enum Stage
	{
		Empty = 0,
		CollectEvidence = 1,
		DistributeEvidences = 2
	}
}
