using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesInferCore.Model
{
	public class Separator
	{
		public List<ProbabilisticTable> PotentialTable { get; set; }

		public List<ProbabilisticNode> Nodes { get; set; }

		public Clique clique1 { get; set; }

		public Clique clique2 { get; set; }

		private Separator()
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
		}

		public Separator(Clique clique1, Clique clique2)
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
			this.clique1 = clique1;
			this.clique2 = clique2;
			this.clique2.Parent = clique1;
			this.clique1.Children.Add(clique2);
		}


		public bool ValidPotential()
		{
			var pot = PotentialTable.Where(p => p.Prob > 0);
			return pot.Count() > 0;
		}
	}
}
