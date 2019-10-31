using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesInferCore.Model
{
	public class Separator
	{
		public List<ProbabilisticTable> PotentialTable { get; set; }

		public PotentialTable Potential { get; set; }

		public List<ProbabilisticNode> Nodes { get; set; }

		public Clique clique1 { get; set; }

		public Clique clique2 { get; set; }

		private Separator()
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
		}

		public Separator(Clique clique1, Clique clique2, bool updateClic)
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
			this.clique1 = clique1;
			this.clique2 = clique2;
			if (updateClic)
			{
				this.clique2.Parent = clique1;
				this.clique1.Children.Add(clique2);
			}

		}

		public Separator Clone()
		{
			Separator sep = new Separator(clique1, clique2, false);
			sep.Nodes = this.Nodes;
			foreach (var pot in this.PotentialTable)
			{
				ProbabilisticTable probabilisticTable = new ProbabilisticTable(pot.Index);
				foreach (var cliqueSeparator in pot.TableCliqueSeparators)
				{
					probabilisticTable.TableCliqueSeparators.Add(cliqueSeparator);
					
				}
				probabilisticTable.Prob = 0;
				sep.PotentialTable.Add(probabilisticTable);
			}
			return sep;
		}
		public void Divide(Separator separator)
		{
			for (int i = 0; i < this.PotentialTable.Count(); i++)
			{
				if (separator.PotentialTable[i].Prob != 0)
				{
					this.PotentialTable[i].Prob /= separator.PotentialTable[i].Prob;
				}
				else
				{
					this.PotentialTable[i].Prob = 0;
				}
			}
		}

		public bool ValidPotential()
		{
			var pot = PotentialTable.Where(p => p.Prob > 0);
			return pot.Count() > 0;
		}
	}
}
