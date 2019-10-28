using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticTable
	{
		public int Index { get; set; }
		public float Prob { get; set; }
		public List<TableCliqueSeparator> TableCliqueSeparators { get; set; }

		public ProbabilisticTable(int ix)
		{
			Index = ix;
			TableCliqueSeparators = new List<TableCliqueSeparator>();
		}


		

	}
}
