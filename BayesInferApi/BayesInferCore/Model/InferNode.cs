using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using System;
using System.Collections.Generic;
using System.Text;




namespace BayesInferCore.Model
{
    using DirArr = DistributionRefArray<Dirichlet, Vector>;
    using DirArr2 = DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>;
    using DirArr3 = DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>;
    using DirArr4 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>;
    using DirArr5 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>;
    using DirArr6 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>;
    using DirArr7 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>;
    using DirArr8 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>;
    using DirArr9 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>, Vector[][][][][][][][]>;
    using DirArr10 = DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<DistributionRefArray<Dirichlet, Vector>, Vector[]>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>, Vector[][][][][][][][]>, Vector[][][][][][][][][]>;

    using VarVectArr = VariableArray<Vector>;
    using VarVectArr2 = VariableArray<VariableArray<Vector>, Vector[][]>;
    using VarVectArr3 = VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>;
    using VarVectArr4 = VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>;
    using VarVectArr5 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>;
    using VarVectArr6 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>;
    using VarVectArr7 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>;
    using VarVectArr8 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>, Vector[][][][][][][][]>;
    using VarVectArr9 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>, Vector[][][][][][][][]>, Vector[][][][][][][][][]>;
    using VarVectArr10 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Vector>, Vector[][]>, Vector[][][]>, Vector[][][][]>, Vector[][][][][]>, Vector[][][][][][]>, Vector[][][][][][][]>, Vector[][][][][][][][]>, Vector[][][][][][][][][]>, Vector[][][][][][][][][][]>;


    using VarDirArr = VariableArray<Dirichlet>;
    using VarDirArr2 = VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>;
    using VarDirArr3 = VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>;
    using VarDirArr4 = VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>;
    using VarDirArr5 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>;
    using VarDirArr6 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>, Dirichlet[][][][][][]>;
    using VarDirArr7 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>, Dirichlet[][][][][][]>, Dirichlet[][][][][][][]>;
    using VarDirArr8 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>, Dirichlet[][][][][][]>, Dirichlet[][][][][][][]>, Dirichlet[][][][][][][][]>;
    using VarDirArr9 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>, Dirichlet[][][][][][]>, Dirichlet[][][][][][][]>, Dirichlet[][][][][][][][]>, Dirichlet[][][][][][][][][]>;
    using VarDirArr10 = VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<VariableArray<Dirichlet>, Dirichlet[][]>, Dirichlet[][][]>, Dirichlet[][][][]>, Dirichlet[][][][][]>, Dirichlet[][][][][][]>, Dirichlet[][][][][][][]>, Dirichlet[][][][][][][][]>, Dirichlet[][][][][][][][][]>, Dirichlet[][][][][][][][][][]>;


    public abstract class InferNode
    {

        //Model Infer.NET
        public int situacaoNodo { get; set; }
        public VariableArray<int> InferPrimary { get; set; }
        public Discrete[] InferModelResult { get; set; }

        // Variáveis aleatórias que representam os parâmetros das distribuições, variáveis filho estão em forma de CPTs
        public Variable<Vector> InferProb;
        public VariableArray<Vector> InferProbCPT1;
        public VariableArray<VariableArray<Vector>, Vector[][]> InferProbCPT2;
        public VarVectArr3 InferProbCPT3;
        public VarVectArr4 InferProbCPT4;
        public VarVectArr5 InferProbCPT5;
        public VarVectArr6 InferProbCPT6;
        public VarVectArr7 InferProbCPT7;
        public VarVectArr8 InferProbCPT8;
        public VarVectArr9 InferProbCPT9;
        public VarVectArr10 InferProbCPT10;

        // Distribuições prévias para as variáveis de probabilidade e CPT.
        public Variable<Dirichlet> InferProbPrior;
        public VariableArray<Dirichlet> InferProbCPT1Prior;
        public VariableArray<VariableArray<Dirichlet>, Dirichlet[][]> InferProbCPT2Prior;
        public VarDirArr3 InferProbCPT3Prior;
        public VarDirArr4 InferProbCPT4Prior;
        public VarDirArr5 InferProbCPT5Prior;
        public VarDirArr6 InferProbCPT6Prior;
        public VarDirArr7 InferProbCPT7Prior;
        public VarDirArr8 InferProbCPT8Prior;
        public VarDirArr9 InferProbCPT9Prior;
        public VarDirArr10 InferProbCPT10Prior;
        


        // Distribuições posteriores para as variáveis probabilidade e CPT.
        public Dirichlet InferProbPosterior;
        public Dirichlet[] InferCPT1Posterior;
        public Dirichlet[][] InferCPT2Posterior;
        public DirArr3 InferCPT3Posterior;
        public DirArr4 InferCPT4Posterior;
        public DirArr5 InferCPT5Posterior;
        public DirArr6 InferCPT6Posterior;
        public DirArr7 InferCPT7Posterior;
        public DirArr8 InferCPT8Posterior;
        public DirArr9 InferCPT9Posterior;
        public DirArr10 InferCPT10Posterior;

        public Range InferState { get; set; }

        public List<InferNode> NodeParents { get; set; }
    }
}
