using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
    public abstract class InferModel
    {
        public InferenceEngine Engine;
        public Variable<int> NodoRaiz;
        public Range N;
    }
}
