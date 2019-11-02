using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class TableLineColumn
	{
		public ProbabilisticNode NodeBase { get; set; }
		public string StateBase { get; set; }
		public float Value { get; set; }
	}
	public class TableLine
	{
		public List<TableLineColumn> Column { get; set; }

		public TableLine()
		{
			Column = new List<TableLineColumn>();
		}

		public void AddColumn(float value, ProbabilisticNode node, string state)
		{
			TableLineColumn tableLineColumn = new TableLineColumn() {
				NodeBase = node,
				StateBase=state,
				Value = value
			};
		}

	}
}
