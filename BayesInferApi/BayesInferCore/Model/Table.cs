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
		//public enum OPERATOR { PRODUCT = 0, DIVISION = 1, PLUS = 2, MINUS = 3 }
		public List<TableLineColumn> Column { get; set; }

		/// <summary>
		/// Used for only separator
		/// </summary>
		public float SeparatorValue { get; set; }

		public TableLine()
		{
			Column = new List<TableLineColumn>();
		}
		public float GetColumnProduct()
		{
			float val = 0;
			foreach (var item in Column)
			{
				val *= item.Value;
			}
			return val;
		}
		public void AddColumn(float value, ProbabilisticNode node, string state)
		{
			Column.Add(new TableLineColumn()
			{
				NodeBase = node,
				StateBase = state,
				Value = value
			});

		}

	}
}
