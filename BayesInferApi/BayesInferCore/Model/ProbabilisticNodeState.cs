﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class ProbabilisticNodeState
	{

		private Object lockObj;
		public string StateBase { get; set; }
		public float StateBaseValue { get; set; }
	}
}
