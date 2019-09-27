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

		//    private int internalIdentificator = Integer.MIN_VALUE;

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

		/**
		 *  Lista de Nos Clusterizados
		 */
		public List<ProbabilisticNode> Nodes { get; set; }

		/**
		 * List of probabilistic nodes related to Clique.
		 */
		private List<ProbabilisticNode> associatedNodes;



		/**
		 * Creates a new clique. Initializes array of children, array of cluster
		 * nodes, and associated nodes. The association status is set to false.
		 */
		public Clique()
		{
			Children = new List<Clique>();
			Nodes = new List<ProbabilisticNode>();
			associatedNodes = new List<ProbabilisticNode>();
			//PotentialTable = new ProbabilisticTable();
		}

		/**
		 * Constructor initializing fields.
		 * @param cliqueProb : potential table representing clique potentials (probability).
		 * Specify this parameter if you want to use special instance of clique potential for this clique.
		 * @see #Clique(PotentialTable, PotentialTable)
		 */
		public Clique(ProbabilisticNode cliqueProb)
		{
			Children = new List<Clique>();
			Nodes = new List<ProbabilisticNode>();
			associatedNodes = new List<ProbabilisticNode>();
			//PotentialTable = new ProbabilisticTable();
		}

		/**
		 * Constructor initializing fields.
		 * @param cliqueProb : potential table representing clique potentials (probability).
		 * Specify this parameter if you want to use special instance of clique potential for this clique.
		 * @param cliqueUtility : potential table representing clique utility values.
		 * Specify this parameter if you want to use special instance of utility table for this clique.
		 */
		public Clique(ProbabilisticNode cliqueProbability, ProbabilisticNode cliqueUtility)
		{
			Children = new List<Clique>();
			Nodes = new List<ProbabilisticNode>();
			associatedNodes = new List<ProbabilisticNode>();
			//PotentialTable = new ProbabilisticTable();
			//potentialTable = cliqueProbability;
			//if (potentialTable == null)
			//{
			//	potentialTable = new ProbabilisticTable();
			//}
			//utilityTable = cliqueUtility;
			//if (utilityTable == null)
			//{
			//	utilityTable = new UtilityTable();
			//}
		}
	}
}
