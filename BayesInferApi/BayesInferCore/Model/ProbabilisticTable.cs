using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BayesInferCore.Model
{
	public class ProbabilisticTable
	{
		public int Index { get; set; }
		public List<float> Prob { get; set; }
		public List<TableCliqueSeparator> TableCliqueSeparators { get; set; }


		public float GetProductProb()
		{
			float val = 1;
			foreach (var item in Prob)
			{
				val *= item;
			}
			return val;
		}
		public float GetSumProb()
		{
			float val = 0;
			foreach (var item in Prob)
			{
				val += item;
			}
			return val;
		}

		public ProbabilisticTable(int ix)
		{
			Index = ix;
			TableCliqueSeparators = new List<TableCliqueSeparator>();
			Prob = new List<float>();
		}


		

	}
}
