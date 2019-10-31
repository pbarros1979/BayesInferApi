using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class TableLineColumn
	{
		public int IndexColumn { get; set; }
		public float Value { get; set; }
		public List<TableCliqueSeparator> NodeCliqueSeparator { get; set; }
	}
	public class TableLine
	{
		public int IndexLine { get; set; }
		public List<TableLineColumn> Column { get; set; }


		public void AddColumn(float value, List<TableCliqueSeparator> cliqueSeparators)
		{
			int index = this.Column.Count;
			TableLineColumn tableLineColumn = new TableLineColumn() {
				IndexColumn = index,
				NodeCliqueSeparator = cliqueSeparators,
				Value = value
			};
		}

	}
}
