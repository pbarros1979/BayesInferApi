using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class Clique
	{
		
		/**
		 *  It identifies the clique uniquely if the network is connected. If disconnected, then the uniqueness is not guaranteed.
		 */
		public  int Index { get; set; }

		
		/**
		 *  Referencia para o clique pai.
		 */
		public Clique Parent { get; set; }

		/**
		 *  Lista de nodes filhos.
		 */
		public List<Clique> Children { get; set; }

		/**
		 *  Tabela de Potencial Associada ao Clique.
		 */
		public List<ProbabilisticTable> PotentialTable { get; set; }


		public PotentialTable Potential { get; set; }

		/**
		 *  Lista de Nos Clusterizados
		 */
		public List<ProbabilisticNode> Nodes { get; set; }
		public List<ProbabilisticNode> AssociatedNodes { get; set; }

		public Stage CliqueStage { get; set; }
		/**
		 * Creates a new clique. Initializes array of children, array of cluster
		 * nodes, and associated nodes. The association status is set to false.
		 */
		public Clique()
		{
			Children = new List<Clique>();
			Nodes = new List<ProbabilisticNode>();
			AssociatedNodes = new List<ProbabilisticNode>();
			PotentialTable = new List<ProbabilisticTable>();
			CliqueStage = Stage.Empty;
		}


		public void Normalize()
		{
			float n = 0;
			float valor;
			foreach (var item in PotentialTable)
			{
				n += item.Prob;
			}
			if (Math.Abs(n - 1.0) > 0.00005)
			{
				foreach (var item in PotentialTable)
				{
					valor = item.Prob;
					if (valor == 0.0)
					{
						// zeros will remain always zero.
						continue;
					}

					valor /= n;
					item.Prob = valor;
				}
			}
		}
	}
}
