using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticTableOld
	{
		public enum TypeOperator { PRODUCT_OPERATOR = 0, DIVISION_OPERATOR = 1, PLUS_OPERATOR = 2 , MINUS_OPERATOR =3}
		protected List<ProbabilisticNode> variableList;
		/**
		 * The data from the table as a list of floats. The use of this data 
		 * is done by using coordinates and linear coordinates.
		 */
		protected FloatArray dataPT;
		/**
		 * Copy of the data from the data.
		 */
		protected FloatArray dataCopy;

		public int TableSize()
		{
			return dataPT.Size;
		}
		public void CopyData()
		{
			dataCopy.Size = dataPT.Size;
			if (dataCopy.data.Length < dataPT.Size)
			{
				dataCopy.data = new float[dataPT.Size];
			}
			Array.Copy(dataPT.data, 0, dataCopy.data, 0, dataPT.Size);
		}
		public bool DirectOpTab(ProbabilisticTableOld tab, TypeOperator op)
		{

			if (TableSize() != tab.TableSize())
			{
				return false;
			}

			switch (op)
			{
				case TypeOperator.PRODUCT_OPERATOR:
					for (int k = TableSize() - 1; k >= 0; k--)
					{
						dataPT.data[k] *= tab.dataPT.data[k];
					}
					break;

				case TypeOperator.DIVISION_OPERATOR:
					for (int k = TableSize() - 1; k >= 0; k--)
					{
						if (tab.dataPT.data[k] != 0)
						{
							dataPT.data[k] /= tab.dataPT.data[k];
						}
						else
						{
							dataPT.data[k] = 0;
						}
					}
					break;

				case TypeOperator.MINUS_OPERATOR:
					for (int k = TableSize() - 1; k >= 0; k--)
					{
						dataPT.data[k] -= tab.dataPT.data[k];
					}
					break;

				case TypeOperator.PLUS_OPERATOR:
					for (int k = TableSize() - 1; k >= 0; k--)
					{
						dataPT.data[k] += tab.dataPT.data[k];
					}
					break;
				default:
					return false;

			}
			return true;
		}
	}
}
