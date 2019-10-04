using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class Separator
	{
		public List<ProbabilisticTable> PotentialTable { get; set; }

		public List<ProbabilisticNode> Nodes { get; set; }

		public Clique Origem { get; set; }

		public Clique Destino { get; set; }

		private Separator()
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
		}

		public Separator(Clique origem, Clique destino)
		{
			Nodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
			this.Origem = origem;
			this.Destino = destino;
		}
	}
}
