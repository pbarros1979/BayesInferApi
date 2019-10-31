using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{

	public class PotentialTable
	{
		public enum OPERATOR { PRODUCT=0, DIVISION = 1, PLUS = 2, MINUS = 3 }

		public OPERATOR OperatorTable { get; set; }
		public List<TableLine> Line{ get; set; }


		//public float normalize()
		//{
		//	float n = 0;
		//	float valor;

		//	int dataSize = this.tableSize();
		//	for (int c = 0; c < dataSize; c++)
		//	{
		//		n += this.getValue(c);
		//	}
		//	if (Math.abs(n - 1.0) > 0.00005)
		//	{   // if precision is 4 digits, error margin is half of precision (i.e. 0.00005)
		//		for (int c = 0; c < dataSize; c++)
		//		{
		//			valor = this.getValue(c);
		//			if (valor == 0.0)
		//			{
		//				// zeros will remain always zero.
		//				continue;
		//			}
		//			if (n == 0.0)
		//			{
		//				throw new IllegalStateException(resource.getString("InconsistencyUnderflowException"));
		//			}
		//			valor /= n;
		//			this.setValue(c, valor);
		//		}
		//	}
		//	return n;
		//}

	}
}
