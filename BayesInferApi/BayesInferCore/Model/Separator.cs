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
		public Stage SepStage { get; set; }

		public Separator SeparatorClone { get; set; }

		private Separator()
		{
			this.Nodes = new List<ProbabilisticNode>();
			this.PotentialTable = new List<ProbabilisticTable>();
			this.SepStage = Stage.Empty;
		}

		public bool InitializedSeparator
		{
			get
			{
				return PotentialTable.Where(p => p.Prob.Count > 0).ToList().Count() > 0;
			}
		}

		public Separator(Clique clique1, Clique clique2, bool updateClic)
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
			this.clique1 = clique1;
			this.clique2 = clique2;
			this.SepStage = Stage.Empty;
			if (updateClic)
			{
				this.clique2.Parent = clique1;
				this.clique1.Children.Add(clique2);
			}

		}

		public void ClearTableProb()
		{
			foreach (var linha in this.PotentialTable)
			{
				linha.Prob.Clear();
			}
		}
		public void Clone()
		{
			SeparatorClone = new Separator(clique1, clique2, false);
			SeparatorClone.Nodes = this.Nodes;
			foreach (var linha in this.PotentialTable)
			{
				ProbabilisticTable probabilisticTable = new ProbabilisticTable(linha.Index);
				probabilisticTable.Prob = linha.Prob;
				foreach (var coluna in linha.TableCliqueSeparators)
				{
					var tmp = new TableCliqueSeparator(coluna.StateBase, coluna.NodeBase);
					tmp.StateBaseValue = coluna.StateBaseValue;
					tmp.InitializedValue = coluna.InitializedValue;
					probabilisticTable.TableCliqueSeparators.Add(coluna);
				}
				SeparatorClone.PotentialTable.Add(probabilisticTable);
			}
		}
		public void Divide(Separator separator)
		{
			for (int i = 0; i < this.PotentialTable.Count(); i++)
			{
				if (separator.PotentialTable[i].GetSumProb() != 0)
				{
					float tmp = this.PotentialTable[i].GetSumProb() / separator.PotentialTable[i].GetSumProb();

					this.PotentialTable[i].Prob.Clear();
					this.PotentialTable[i].Prob.Add(tmp);
				}
				else
				{
					this.PotentialTable[i].Prob.Clear();
					this.PotentialTable[i].Prob.Add(0);
				}
			}
		}
	}
}
