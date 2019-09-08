using BayesInferCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BayesInferCore.Services
{
	public class JunctionTree
	{
		FileBayesianNetwork _redeBayesiana;
		public JunctionTree(FileBayesianNetwork redeBayesiana)
		{
			_redeBayesiana = redeBayesiana;

		}

		private void LoadModel()
		{

		}

		public FileBayesianNetwork InferModel(List<Belief> crenca)
		{
			FileBayesianNetwork _rede= new FileBayesianNetwork();


			return _rede;
		}


	}
}
