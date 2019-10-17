using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticNode
	{
		private Object lockObj;
		public String Description { get; set; }
		public String Name { get; set; }
		public String Label { get; set; }
		public List<ProbabilisticNode> Parents { get; private set; }
		public List<ProbabilisticNode> Children { get; private set; }
		public List<String> States { get; set; }
		public List<ProbabilisticNode> Adjacents { get;  set; }
		public ProbabilisticNodeTable PriorTabelaNode { get;}
		public List<ProbabilisticNodeState> PosteriorTabelaNode { get;}

		public int? BeliefValue { get; set; }

		public ProbabilisticNode()
		{
			lockObj = new object();
			PosteriorTabelaNode = new List<ProbabilisticNodeState>();
			PriorTabelaNode = new ProbabilisticNodeTable();
			Parents = new List<ProbabilisticNode>();
			Children = new List<ProbabilisticNode>();
			Adjacents = new List<ProbabilisticNode>();
		}




		public void AddChild(ProbabilisticNode child)
		{
			lock (lockObj)
			{
				this.Children.Add(child);
			}

		}
		public void AddParent(ProbabilisticNode parent)
		{
			lock (lockObj)
			{
				this.Parents.Add(parent);
			}
		}
		public void AddAdjacent(ProbabilisticNode adjs)
		{
			lock (lockObj)
			{
				Adjacents.Add(adjs);
			}
		}
		public void RemoveChild(ProbabilisticNode child)
		{
			lock (lockObj)
			{
				this.Children.Remove(child);
			}

		}
		public void RemoveParent(ProbabilisticNode parent)
		{
			lock (lockObj)
			{
				this.Parents.Remove(parent);
			}
		}
		public void ClearAdjacents()
		{
			lock (lockObj)
			{
				Adjacents.Clear();
			}
		}



	}
}
