using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class Separator
	{
		private ProbabilisticTable TabelaPot { get; set; }

		private int internalIdentificator = int.MinValue;

		public List<ProbabilisticNode> Nodes { get; set; }

		private Clique clique1;

		private Clique clique2;

		private Separator()
		{
			Nodes = new List<ProbabilisticNode>();
			//TabelaPot = new ProbabilisticTable();

		}

		/**
		 * Constructor initializing fields
		 * @param clique1
		 * @param clique2
		 * @param probTable : probability table
		 * @param utilTable : utility table
		 * @param updateCliques
		 */
		public Separator(Clique clique1, Clique clique2)
		{
			this.clique1 = clique1;
			this.clique2 = clique2;
			
		}
	}
}
