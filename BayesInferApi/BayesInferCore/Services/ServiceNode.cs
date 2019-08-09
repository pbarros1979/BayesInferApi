using BayesInferCore.Model;
using BayesInferCore;
using Microsoft.ML.Probabilistic.Models;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
namespace BayesInferCore.Services
{
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

    public class ServiceNode
    {
        Node _node;
        FileBayesianNetwork _redeBayesiana;
        System.IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");

        public void SetState(String nodeName, int estado)
        {
            object _lck = new object();
            Parallel.ForEach(_redeBayesiana.Nodes, i =>
            {
                if (i.Id == nodeName)
                {
                    lock (_lck)
                    {
                        i.situacaoNodo = estado;
                    }
                }
            });
        }
        public List<Node> NodeParents { get; set; }
        /// <summary>
        /// Verifica se Parents estão em estado de finalizado para que possa ser carregada a tabela CPT no nodo atual
        /// </summary>
        public bool ValidParents
        {
            get
            {
                if (NodeParents.Count == 0)
                    return true;

                bool nodoFinalizado = true;
                foreach (var item in NodeParents)
                {
                    if (item.situacaoNodo != (int)Node.Estado.Finalizado)
                    {
                        nodoFinalizado = false;
                    }
                }
                return nodoFinalizado;
            }
        }
        public ServiceNode(ref Node node, ref FileBayesianNetwork redeBayesiana)
        {
            _node = node;
            _redeBayesiana = redeBayesiana;
            InitNodeParents();
        }
        /// <summary>
        /// Carrega Node Parents para verificação posterior
        /// </summary>
        private void InitNodeParents()
        {
            NodeParents = new List<Node>();
            //Cria objeto lock exclusivo para esta task parelela, sendo que a adição dos nodos não impolica nos estados, 
            //sem apenas importante se nodo esta no estado 2 finalizado, desprezando estado 0 novo, e carregado 1
            object _lck = new object();
            Parallel.ForEach(_redeBayesiana.Nodes, (i, pls) =>
            {
                if (_node.Parents.Contains(i.Id))
                {
                    lock (_lck)
                    {
                        NodeParents.Add(i);
                    }
                };
                if (NodeParents.Count == _node.Parents.Count)
                    pls.Break();
            });
            _node.NodeParents = NodeParents;
        }
        public bool LoadNode()
        {

            if (ValidParents)
            {
                _node.InferState = new Range(2).Named("State" + _node.Id);

                if (NodeParents.Count == 0)
                {
                    _node.InferProbPrior = Variable.New<Dirichlet>().Named("Prob" + _node.Id + "Prior");
                    _node.InferProb = Variable<Vector>.Random(_node.InferProbPrior).Named("Prob" + _node.Id);
                    _node.InferProb.SetValueRange(_node.InferState);
                    _node.InferPrimary = Variable.Array<int>(_redeBayesiana.N).Named(_node.Id);
                    _node.InferPrimary[_redeBayesiana.N] = Variable.Discrete(_node.InferProb).ForEach(_redeBayesiana.N);

                    Double[] valores = new Double[_node.States.Count];

                    for (int i = 0; i < _node.States.Count; i++)
                    {
                        valores[i] = Double.Parse(_node.Cpts[0].SingleCptDic[_node.States[i]], cultureUS);

                    }
                    Vector prob = Vector.FromArray(valores);
                    _node.InferProbPrior.ObservedValue = Dirichlet.PointMass(prob);
                    //Console.WriteLine("VAlor = "+_node.Id+"-"+ _node.InferProbPrior.ObservedValue.ToString());

                }
                else if (NodeParents.Count == 1)
                {
                    _node.InferProbCPT1Prior = Variable.Array<Dirichlet>(_node.NodeParents[0].InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT1 = Variable.Array<Vector>(_node.NodeParents[0].InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT1[_node.NodeParents[0].InferState] = Variable<Vector>.Random(_node.InferProbCPT1Prior[_node.NodeParents[0].InferState]);
                    _node.InferProbCPT1.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromOneParent(_node.NodeParents[0].InferPrimary, _node.InferProbCPT1).Named(_node.Id);

                    Vector[] probabilidade = new Vector[_node.NodeParents[0].States.Count];

                    for (int i = 0; i < _node.NodeParents[0].States.Count; i++)
                    {

                        Double[] vlrs = new double[_node.States.Count];
                        for (int j = 0; j < _node.States.Count; j++)
                        {
                            vlrs[j] = Double.Parse(_node.Cpts[i].ThenDic.ElementAt(j).Value, cultureUS);
                        }
                        probabilidade[i] = Vector.FromArray(vlrs);

                    }
                    _node.InferProbCPT1Prior.ObservedValue = probabilidade.Select(v => Dirichlet.PointMass(v)).ToArray();
                    //foreach (var item in _node.InferProbCPT1Prior.ObservedValue)
                    //{
                    //    Console.WriteLine("VAlor = " + _node.Id + "-" + item.ToString());
                    //}

                }
                else if (NodeParents.Count == 2)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    _node.InferProbCPT2Prior = Variable.Array(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT2 = Variable.Array(Variable.Array<Vector>(parent0.InferState), parent1.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT2[parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT2Prior[parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT2.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromTwoParents(parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT2).Named(_node.Id);

                    Vector[][] probabilidade = new Vector[_node.NodeParents[1].States.Count()][];
                    int qtd = 0;
                    for (int s1 = 0; s1 < _node.NodeParents[1].States.Count(); s1++)
                    {
                        probabilidade[s1] = new Vector[_node.NodeParents[0].States.Count()];

                        for (int s0 = 0; s0 < _node.NodeParents[0].States.Count(); s0++)
                        {
                            Double[] vlrs = new double[_node.States.Count];
                            for (int j = 0; j < _node.States.Count; j++)
                            {
                                vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);
                            }
                            probabilidade[s1][s0] = Vector.FromArray(vlrs);
                            qtd++;
                        }
                    }
                    _node.InferProbCPT2Prior.ObservedValue = probabilidade.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray();
                    //foreach (var item in _node.InferProbCPT2Prior.ObservedValue)
                    //{
                    //    foreach (var itemb in item)
                    //    {
                    //        Console.WriteLine("VAlor = " + _node.Id + "-" + itemb.ToString());
                    //    }
                    //}
                }
                else if (NodeParents.Count == 3)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];

                    _node.InferProbCPT3Prior = Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT3 = Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT3[parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT3Prior[parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT3.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromThreeParents(parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT3).Named(_node.Id);

                    Vector[][][] probabilidade = new Vector[parent2.States.Count()][][];
                    int qtd = 0;
                    for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                    {
                        probabilidade[s2] = new Vector[parent1.States.Count()][];
                        for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                        {
                            probabilidade[s2][s1] = new Vector[parent0.States.Count()];
                            for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                            {
                                Double[] vlrs = new double[_node.States.Count];
                                for (int j = 0; j < _node.States.Count; j++)
                                {
                                    vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                }
                                probabilidade[s2][s1][s0] = Vector.FromArray(vlrs);
                                qtd++;
                            }
                        }
                    }
                    _node.InferProbCPT3Prior.ObservedValue = probabilidade.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray();
                }
                else if (NodeParents.Count == 4)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];

                    _node.InferProbCPT4Prior = Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT4 = Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT4[parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT4Prior[parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT4.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromFourParents(parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT4).Named(_node.Id);

                    Vector[][][][] probabilidade = new Vector[parent3.States.Count()][][][];
                    int qtd = 0;
                    for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                    {
                        probabilidade[s3] = new Vector[parent2.States.Count()][][];
                        for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                        {
                            probabilidade[s3][s2] = new Vector[parent1.States.Count()][];
                            for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                            {
                                probabilidade[s3][s2][s1] = new Vector[parent0.States.Count()];
                                for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                {
                                    Double[] vlrs = new double[_node.States.Count];
                                    for (int j = 0; j < _node.States.Count; j++)
                                    {
                                        vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                    }
                                    probabilidade[s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                    qtd++;
                                }
                            }
                        }
                    }
                    _node.InferProbCPT4Prior.ObservedValue = probabilidade.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray();
                }
                else if (NodeParents.Count == 5)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];

                    _node.InferProbCPT5Prior = Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT5 = Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT5[parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT5Prior[parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT5.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromFiveParents(parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT5).Named(_node.Id);

                    Vector[][][][][] probabilidade = new Vector[parent4.States.Count()][][][][];
                    int qtd = 0;
                    for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                    {
                        probabilidade[s4] = new Vector[parent3.States.Count()][][][];
                        for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                        {
                            probabilidade[s4][s3] = new Vector[parent2.States.Count()][][];
                            for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                            {
                                probabilidade[s4][s3][s2] = new Vector[parent1.States.Count()][];
                                for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                {
                                    probabilidade[s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                    for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                    {
                                        Double[] vlrs = new double[_node.States.Count];
                                        for (int j = 0; j < _node.States.Count; j++)
                                        {
                                            vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                        }
                                        probabilidade[s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                        qtd++;
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT5Prior.ObservedValue = probabilidade.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();
                }
                else if (NodeParents.Count == 6)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];
                    Node parent5 = _node.NodeParents[5];

                    _node.InferProbCPT6Prior = Variable.Array<VarDirArr5, Dirichlet[][][][][][]>(Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT6 = Variable.Array<VarVectArr5, Vector[][][][][][]>(Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT6[parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT6Prior[parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT6.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromSixParents(parent5.InferPrimary, parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT6).Named(_node.Id);

                    Vector[][][][][][] probabilidade = new Vector[parent5.States.Count()][][][][][];
                    int qtd = 0;
                    for (int s5 = 0; s5 < parent5.States.Count(); s5++)
                    {
                        probabilidade[s5] = new Vector[parent4.States.Count()][][][][];
                        for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                        {
                            probabilidade[s5][s4] = new Vector[parent3.States.Count()][][][];
                            for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                            {
                                probabilidade[s5][s4][s3] = new Vector[parent2.States.Count()][][];
                                for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                                {
                                    probabilidade[s5][s4][s3][s2] = new Vector[parent1.States.Count()][];
                                    for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                    {
                                        probabilidade[s5][s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                        for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                        {
                                            Double[] vlrs = new double[_node.States.Count];
                                            for (int j = 0; j < _node.States.Count; j++)
                                            {
                                                vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                            }
                                            probabilidade[s5][s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                            qtd++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT6Prior.ObservedValue = probabilidade.Select(ve => ve.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();

                }
                else if (NodeParents.Count == 7)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];
                    Node parent5 = _node.NodeParents[5];
                    Node parent6 = _node.NodeParents[6];

                    _node.InferProbCPT7Prior = Variable.Array<VarDirArr6, Dirichlet[][][][][][][]>(Variable.Array<VarDirArr5, Dirichlet[][][][][][]>(Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT7 = Variable.Array<VarVectArr6, Vector[][][][][][][]>(Variable.Array<VarVectArr5, Vector[][][][][][]>(Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT7[parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT7Prior[parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT7.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromSevemParents(parent6.InferPrimary, parent5.InferPrimary, parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT7).Named(_node.Id);

                    Vector[][][][][][][] probabilidade = new Vector[parent6.States.Count()][][][][][][];
                    int qtd = 0;
                    for (int s6 = 0; s6 < parent6.States.Count(); s6++)
                    {
                        probabilidade[s6] = new Vector[parent5.States.Count()][][][][][];
                        for (int s5 = 0; s5 < parent5.States.Count(); s5++)
                        {
                            probabilidade[s6][s5] = new Vector[parent4.States.Count()][][][][];
                            for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                            {
                                probabilidade[s6][s5][s4] = new Vector[parent3.States.Count()][][][];
                                for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                                {
                                    probabilidade[s6][s5][s4][s3] = new Vector[parent2.States.Count()][][];
                                    for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                                    {
                                        probabilidade[s6][s5][s4][s3][s2] = new Vector[parent1.States.Count()][];
                                        for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                        {
                                            probabilidade[s6][s5][s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                            for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                            {
                                                Double[] vlrs = new double[_node.States.Count];
                                                for (int j = 0; j < _node.States.Count; j++)
                                                {
                                                    vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                                }
                                                probabilidade[s6][s5][s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                                qtd++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT7Prior.ObservedValue = probabilidade.Select(vf => vf.Select(ve => ve.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();
                }
                else if (NodeParents.Count == 8)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];
                    Node parent5 = _node.NodeParents[5];
                    Node parent6 = _node.NodeParents[6];
                    Node parent7 = _node.NodeParents[7];

                    _node.InferProbCPT8Prior = Variable.Array<VarDirArr7, Dirichlet[][][][][][][][]>(Variable.Array<VarDirArr6, Dirichlet[][][][][][][]>(Variable.Array<VarDirArr5, Dirichlet[][][][][][]>(Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT8 = Variable.Array<VarVectArr7, Vector[][][][][][][][]>(Variable.Array<VarVectArr6, Vector[][][][][][][]>(Variable.Array<VarVectArr5, Vector[][][][][][]>(Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT8[parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT8Prior[parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT8.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromEightParents(parent7.InferPrimary, parent6.InferPrimary, parent5.InferPrimary, parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT8).Named(_node.Id);

                    Vector[][][][][][][][] probabilidade = new Vector[parent7.States.Count()][][][][][][][];
                    int qtd = 0;
                    for (int s7 = 0; s7 < parent7.States.Count(); s7++)
                    {
                        probabilidade[s7] = new Vector[parent6.States.Count()][][][][][][];
                        for (int s6 = 0; s6 < parent6.States.Count(); s6++)
                        {
                            probabilidade[s7][s6] = new Vector[parent5.States.Count()][][][][][];
                            for (int s5 = 0; s5 < parent5.States.Count(); s5++)
                            {
                                probabilidade[s7][s6][s5] = new Vector[parent4.States.Count()][][][][];
                                for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                                {
                                    probabilidade[s7][s6][s5][s4] = new Vector[parent3.States.Count()][][][];
                                    for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                                    {
                                        probabilidade[s7][s6][s5][s4][s3] = new Vector[parent2.States.Count()][][];
                                        for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                                        {
                                            probabilidade[s7][s6][s5][s4][s3][s2] = new Vector[parent1.States.Count()][];
                                            for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                            {
                                                probabilidade[s7][s6][s5][s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                                for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                                {
                                                    Double[] vlrs = new double[_node.States.Count];
                                                    for (int j = 0; j < _node.States.Count; j++)
                                                    {
                                                        vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                                    }
                                                    probabilidade[s7][s6][s5][s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                                    qtd++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT8Prior.ObservedValue = probabilidade.Select(vg => vg.Select(vf => vf.Select(ve => ve.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();

                }
                else if (NodeParents.Count == 9)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];
                    Node parent5 = _node.NodeParents[5];
                    Node parent6 = _node.NodeParents[6];
                    Node parent7 = _node.NodeParents[7];
                    Node parent8 = _node.NodeParents[8];

                    _node.InferProbCPT9Prior = Variable.Array<VarDirArr8, Dirichlet[][][][][][][][][]>(Variable.Array<VarDirArr7, Dirichlet[][][][][][][][]>(Variable.Array<VarDirArr6, Dirichlet[][][][][][][]>(Variable.Array<VarDirArr5, Dirichlet[][][][][][]>(Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState), parent8.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT9 = Variable.Array<VarVectArr8, Vector[][][][][][][][][]>(Variable.Array<VarVectArr7, Vector[][][][][][][][]>(Variable.Array<VarVectArr6, Vector[][][][][][][]>(Variable.Array<VarVectArr5, Vector[][][][][][]>(Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState), parent8.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT9[parent8.InferState][parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT9Prior[parent8.InferState][parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT9.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromNineParents(parent8.InferPrimary, parent7.InferPrimary, parent6.InferPrimary, parent5.InferPrimary, parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT9).Named(_node.Id);

                    Vector[][][][][][][][][] probabilidade = new Vector[parent8.States.Count()][][][][][][][][];
                    int qtd = 0;
                    for (int s8 = 0; s8 < parent8.States.Count(); s8++)
                    {
                        probabilidade[s8] = new Vector[parent7.States.Count()][][][][][][][];
                        for (int s7 = 0; s7 < parent7.States.Count(); s7++)
                        {
                            probabilidade[s8][s7] = new Vector[parent6.States.Count()][][][][][][];
                            for (int s6 = 0; s6 < parent6.States.Count(); s6++)
                            {
                                probabilidade[s8][s7][s6] = new Vector[parent5.States.Count()][][][][][];
                                for (int s5 = 0; s5 < parent5.States.Count(); s5++)
                                {
                                    probabilidade[s8][s7][s6][s5] = new Vector[parent4.States.Count()][][][][];
                                    for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                                    {
                                        probabilidade[s8][s7][s6][s5][s4] = new Vector[parent3.States.Count()][][][];
                                        for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                                        {
                                            probabilidade[s8][s7][s6][s5][s4][s3] = new Vector[parent2.States.Count()][][];
                                            for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                                            {
                                                probabilidade[s8][s7][s6][s5][s4][s3][s2] = new Vector[parent1.States.Count()][];
                                                for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                                {
                                                    probabilidade[s8][s7][s6][s5][s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                                    for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                                    {
                                                        Double[] vlrs = new double[_node.States.Count];
                                                        for (int j = 0; j < _node.States.Count; j++)
                                                        {
                                                            vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                                        }
                                                        probabilidade[s8][s7][s6][s5][s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                                        qtd++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT9Prior.ObservedValue = probabilidade.Select(vh => vh.Select(vg => vg.Select(vf => vf.Select(ve => ve.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();

                }
                else if (NodeParents.Count == 10)
                {
                    Node parent0 = _node.NodeParents[0];
                    Node parent1 = _node.NodeParents[1];
                    Node parent2 = _node.NodeParents[2];
                    Node parent3 = _node.NodeParents[3];
                    Node parent4 = _node.NodeParents[4];
                    Node parent5 = _node.NodeParents[5];
                    Node parent6 = _node.NodeParents[6];
                    Node parent7 = _node.NodeParents[7];
                    Node parent8 = _node.NodeParents[8];
                    Node parent9 = _node.NodeParents[9];

                    _node.InferProbCPT10Prior = Variable.Array<VarDirArr9, Dirichlet[][][][][][][][][][]>(Variable.Array<VarDirArr8, Dirichlet[][][][][][][][][]>(Variable.Array<VarDirArr7, Dirichlet[][][][][][][][]>(Variable.Array<VarDirArr6, Dirichlet[][][][][][][]>(Variable.Array<VarDirArr5, Dirichlet[][][][][][]>(Variable.Array<VarDirArr4, Dirichlet[][][][][]>(Variable.Array<VarDirArr3, Dirichlet[][][][]>(Variable.Array<VarDirArr2, Dirichlet[][][]>(Variable.Array<VarDirArr, Dirichlet[][]>(Variable.Array<Dirichlet>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState), parent8.InferState), parent9.InferState).Named("CPT" + _node.Id + "Prior");
                    _node.InferProbCPT10 = Variable.Array<VarVectArr9, Vector[][][][][][][][][][]>(Variable.Array<VarVectArr8, Vector[][][][][][][][][]>(Variable.Array<VarVectArr7, Vector[][][][][][][][]>(Variable.Array<VarVectArr6, Vector[][][][][][][]>(Variable.Array<VarVectArr5, Vector[][][][][][]>(Variable.Array<VarVectArr4, Vector[][][][][]>(Variable.Array<VarVectArr3, Vector[][][][]>(Variable.Array<VarVectArr2, Vector[][][]>(Variable.Array<VarVectArr, Vector[][]>(Variable.Array<Vector>(parent0.InferState), parent1.InferState), parent2.InferState), parent3.InferState), parent4.InferState), parent5.InferState), parent6.InferState), parent7.InferState), parent8.InferState), parent9.InferState).Named("CPT" + _node.Id);
                    _node.InferProbCPT10[parent9.InferState][parent8.InferState][parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState] = Variable<Vector>.Random(_node.InferProbCPT10Prior[parent9.InferState][parent8.InferState][parent7.InferState][parent6.InferState][parent5.InferState][parent4.InferState][parent3.InferState][parent2.InferState][parent1.InferState][parent0.InferState]);
                    _node.InferProbCPT10.SetValueRange(_node.InferState);
                    _node.InferPrimary = AddChildFromTenParents(parent9.InferPrimary, parent8.InferPrimary, parent7.InferPrimary, parent6.InferPrimary, parent5.InferPrimary, parent4.InferPrimary, parent3.InferPrimary, parent2.InferPrimary, parent1.InferPrimary, parent0.InferPrimary, _node.InferProbCPT10).Named(_node.Id);

                    Vector[][][][][][][][][][] probabilidade = new Vector[parent9.States.Count()][][][][][][][][][];
                    int qtd = 0;
                    for (int s9 = 0; s9 < parent9.States.Count(); s9++)
                    {
                        probabilidade[s9] = new Vector[parent8.States.Count()][][][][][][][][];
                        for (int s8 = 0; s8 < parent8.States.Count(); s8++)
                        {
                            probabilidade[s9][s8] = new Vector[parent7.States.Count()][][][][][][][];
                            for (int s7 = 0; s7 < parent7.States.Count(); s7++)
                            {
                                probabilidade[s9][s8][s7] = new Vector[parent6.States.Count()][][][][][][];
                                for (int s6 = 0; s6 < parent6.States.Count(); s6++)
                                {
                                    probabilidade[s9][s8][s7][s6] = new Vector[parent5.States.Count()][][][][][];
                                    for (int s5 = 0; s5 < parent5.States.Count(); s5++)
                                    {
                                        probabilidade[s9][s8][s7][s6][s5] = new Vector[parent4.States.Count()][][][][];
                                        for (int s4 = 0; s4 < parent4.States.Count(); s4++)
                                        {
                                            probabilidade[s9][s8][s7][s6][s5][s4] = new Vector[parent3.States.Count()][][][];
                                            for (int s3 = 0; s3 < parent3.States.Count(); s3++)
                                            {
                                                probabilidade[s9][s8][s7][s6][s5][s4][s3] = new Vector[parent2.States.Count()][][];
                                                for (int s2 = 0; s2 < parent2.States.Count(); s2++)
                                                {
                                                    probabilidade[s9][s8][s7][s6][s5][s4][s3][s2] = new Vector[parent1.States.Count()][];
                                                    for (int s1 = 0; s1 < parent1.States.Count(); s1++)
                                                    {
                                                        probabilidade[s9][s8][s7][s6][s5][s4][s3][s2][s1] = new Vector[parent0.States.Count()];
                                                        for (int s0 = 0; s0 < parent0.States.Count(); s0++)
                                                        {
                                                            Double[] vlrs = new double[_node.States.Count];
                                                            for (int j = 0; j < _node.States.Count; j++)
                                                            {
                                                                vlrs[j] = Double.Parse(_node.Cpts[qtd].ThenDic.ElementAt(j).Value, cultureUS);

                                                            }
                                                            probabilidade[s9][s8][s7][s6][s5][s4][s3][s2][s1][s0] = Vector.FromArray(vlrs);
                                                            qtd++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _node.InferProbCPT10Prior.ObservedValue = probabilidade.Select(vi => vi.Select(vh => vh.Select(vg => vg.Select(vf => vf.Select(ve => ve.Select(vd => vd.Select(vc => vc.Select(vb => vb.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray()).ToArray();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper method to add a child from one parent
        /// </summary>
        /// <param name="parent">Parent (a variable array over a range of examples)</param>
        /// <param name="cpt">Conditional probability table</param>
        /// <returns></returns>
        public static VariableArray<int> AddChildFromOneParent(
            VariableArray<int> parent,
            VariableArray<Vector> cpt)
        {
            var n = parent.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent[n]))
            {
                child[n] = Variable.Discrete(cpt[parent[n]]);
            }

            return child;
        }

        /// <summary>
        /// Helper method to add a child from two parents
        /// </summary>
        /// <param name="parent1">First parent (a variable array over a range of examples)</param>
        /// <param name="parent2">Second parent (a variable array over the same range)</param>
        /// <param name="cpt">Conditional probability table</param>
        /// <returns></returns>
        public static VariableArray<int> AddChildFromTwoParents(
            VariableArray<int> parent1,
            VariableArray<int> parent2,
            VariableArray<VariableArray<Vector>, Vector[][]> cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]]);
            }

            return child;
        }
        public static VariableArray<int> AddChildFromThreeParents(
            VariableArray<int> parent1, VariableArray<int> parent2,
            VariableArray<int> parent3,
            VarVectArr3 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]]);
            }

            return child;
        }
        public static VariableArray<int> AddChildFromFourParents(
            VariableArray<int> parent1, VariableArray<int> parent2,
            VariableArray<int> parent3, VariableArray<int> parent4,
            VarVectArr4 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]]);
            }

            return child;
        }
        public static VariableArray<int> AddChildFromFiveParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3,
            VariableArray<int> parent4, VariableArray<int> parent5,
            VarVectArr5 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]]);
            }

            return child;
        }
        public static VariableArray<int> AddChildFromSixParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3, VariableArray<int> parent4,
            VariableArray<int> parent5, VariableArray<int> parent6,
            VarVectArr6 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            using (Variable.Switch(parent6[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]][parent6[n]]);
            }

            return child;
        }
        public static VariableArray<int> AddChildFromSevemParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3, VariableArray<int> parent4,
            VariableArray<int> parent5, VariableArray<int> parent6, VariableArray<int> parent7,
            VarVectArr7 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            using (Variable.Switch(parent6[n]))
            using (Variable.Switch(parent7[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]][parent6[n]][parent7[n]]);
            }
            return child;
        }
        public static VariableArray<int> AddChildFromEightParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3, VariableArray<int> parent4,
            VariableArray<int> parent5, VariableArray<int> parent6, VariableArray<int> parent7, VariableArray<int> parent8,
            VarVectArr8 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            using (Variable.Switch(parent6[n]))
            using (Variable.Switch(parent7[n]))
            using (Variable.Switch(parent8[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]][parent6[n]][parent7[n]][parent8[n]]);
            }
            return child;
        }
        public static VariableArray<int> AddChildFromNineParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3, VariableArray<int> parent4,
            VariableArray<int> parent5, VariableArray<int> parent6, VariableArray<int> parent7, VariableArray<int> parent8,
            VariableArray<int> parent9,
            VarVectArr9 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            using (Variable.Switch(parent6[n]))
            using (Variable.Switch(parent7[n]))
            using (Variable.Switch(parent8[n]))
            using (Variable.Switch(parent9[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]][parent6[n]][parent7[n]][parent8[n]][parent9[n]]);
            }
            return child;
        }
        public static VariableArray<int> AddChildFromTenParents(
            VariableArray<int> parent1, VariableArray<int> parent2, VariableArray<int> parent3, VariableArray<int> parent4,
            VariableArray<int> parent5, VariableArray<int> parent6, VariableArray<int> parent7, VariableArray<int> parent8,
            VariableArray<int> parent9, VariableArray<int> parent10,
            VarVectArr10 cpt)
        {
            var n = parent1.Range;
            var child = Variable.Array<int>(n);
            using (Variable.ForEach(n))
            using (Variable.Switch(parent1[n]))
            using (Variable.Switch(parent2[n]))
            using (Variable.Switch(parent3[n]))
            using (Variable.Switch(parent4[n]))
            using (Variable.Switch(parent5[n]))
            using (Variable.Switch(parent6[n]))
            using (Variable.Switch(parent7[n]))
            using (Variable.Switch(parent8[n]))
            using (Variable.Switch(parent9[n]))
            using (Variable.Switch(parent10[n]))
            {
                child[n] = Variable.Discrete(cpt[parent1[n]][parent2[n]][parent3[n]][parent4[n]][parent5[n]][parent6[n]][parent7[n]][parent8[n]][parent9[n]][parent10[n]]);
            }
            return child;
        }

    }
}
