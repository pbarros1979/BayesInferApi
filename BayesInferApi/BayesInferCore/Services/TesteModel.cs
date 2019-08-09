using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesInferCore.Services
{
    public class TesteModel
    {
        
        // Variáveis aleatórias primárias
        public VariableArray<int> Cloudy;
        public VariableArray<int> Sprinkler;
       
        public Variable<int> NumberOfExamples;


        // Variáveis aleatórias que representam os parâmetros das distribuições das variáveis aleatórias primárias.
        // Para variáveis filho, elas estão na forma de tabelas de probabilidade condicional (CPTs)
        public Variable<Vector> ProbCloudy;
        public VariableArray<Vector> CPTSprinkler;
        

        // Distribuições prévias para as variáveis de probabilidade e CPT.
        // As distribuições anteriores são formuladas como variáveis Infer.NET 
        // para que possam ser configurados em tempo de execução sem recompilar o modelo
        public Variable<Dirichlet> ProbCloudyPrior;
        public VariableArray<Dirichlet> CPTSprinklerPrior;
        

        // Distribuições posteriores para as variáveis probabilidade e CPT.
        public Dirichlet ProbCloudyPosterior;
        public Dirichlet[] CPTSprinklerPosterior;
        

        // Inference engine
        public InferenceEngine Engine = new InferenceEngine();

        /// <summary>
        /// Constructs a new Glass/Sprinkler/Rain model
        /// </summary>
        public TesteModel()
        {
            // Set up the ranges
            NumberOfExamples = Variable.New<int>().Named("NofE");
            Range N = new Range(NumberOfExamples).Named("N");

            // Embora todas as variáveis neste exemplo tenham apenas 2 estados (true/false),
            // o exemplo é formulado de uma forma que mostra como se estender a vários estados
            Range C = new Range(2).Named("C");
            Range S = new Range(2).Named("S");
            

            // Definir as prioridades e os parâmetros
            ProbCloudyPrior = Variable.New<Dirichlet>().Named("ProbCloudyPrior");
            ProbCloudy = Variable<Vector>.Random(ProbCloudyPrior).Named("ProbCloudy");
            ProbCloudy.SetValueRange(C);

            // Tabela de probabilidade do sprinkler condicionada estar nublado
            CPTSprinklerPrior = Variable.Array<Dirichlet>(C).Named("CPTSprinklerPrior");
            CPTSprinkler = Variable.Array<Vector>(C).Named("CPTSprinkler");
            CPTSprinkler[C] = Variable<Vector>.Random(CPTSprinklerPrior[C]);
            CPTSprinkler.SetValueRange(S);

            

            // Definir as principais variáveis
            Cloudy = Variable.Array<int>(N).Named("Cloudy");
            Cloudy[N] = Variable.Discrete(ProbCloudy).ForEach(N);
            Sprinkler = AddChildFromOneParent(Cloudy, CPTSprinkler).Named("Sprinkler");
   
        }

        /// <summary>
        /// Learns the parameters of the cloud/sprinkler/rain example
        /// </summary>
        /// <param name="cloudy">Cloudiness data</param>
        /// <param name="sprinkler">Sprinkler data</param>
        /// <param name="rain">Rain data</param>
        /// <param name="wetgrass">Wet grass data</param>
        /// <param name="probCloudyPrior">Prior for cloudiness probability vector</param>
        /// <param name="cptSprinklerPrior">Prior for sprinkler conditional probability table</param>
        /// <param name="cptRainPrior">Prior for rain conditional probability table</param>
        /// <param name="cptWetGrassPrior">Prior for wet grass conditional probability table</param>
        public void LearnParameters(
            int[] cloudy,
            int[] sprinkler,
            Dirichlet probCloudyPrior,
            Dirichlet[] cptSprinklerPrior
            )
        {
            NumberOfExamples.ObservedValue = cloudy.Length;
            Cloudy.ObservedValue = cloudy;
            Sprinkler.ObservedValue = sprinkler;
            ProbCloudyPrior.ObservedValue = probCloudyPrior;
            CPTSprinklerPrior.ObservedValue = cptSprinklerPrior;
            // Inference
            ProbCloudyPosterior = Engine.Infer<Dirichlet>(ProbCloudy);
            CPTSprinklerPosterior = Engine.Infer<Dirichlet[]>(CPTSprinkler);
            
        }

        /// <summary>
        /// Learns the parameters of the cloud/sprinkler/rain example assuming uniform priors
        /// </summary>
        /// <param name="cloudy">Cloudiness data</param>
        /// <param name="sprinkler">Sprinkler data</param>
        /// <param name="rain">Rain data</param>
        /// <param name="wetgrass">Wet grass data</param>
        public void LearnParameters(
            int[] cloudy,
            int[] sprinkler,
            int[] rain,
            int[] wetgrass)
        {
            // Set all priors to uniform
            Dirichlet probCloudyPrior = Dirichlet.Uniform(2);
            Dirichlet[] dirUnifArray = Enumerable.Repeat(Dirichlet.Uniform(2), 2).ToArray();
            //Dirichlet[][] dirUnifArrayArray = Enumerable.Repeat(dirUnifArray, 2).ToArray();

            LearnParameters(cloudy, sprinkler, probCloudyPrior, dirUnifArray);
        }


        /// Sample the model
        /// </summary>
        /// <param name="numData">Number of data in sample</param>
        /// <param name="probCloudy">Cloudiness probability vector</param>
        /// <param name="cptSprinkler">Sprinkler conditional probability table</param>
        /// <param name="cptRain">Rain conditional probability table</param>
        /// <param name="cptWetGrass">Wet grass conditional probability table</param>
        /// <returns></returns>
        public static int[][] Sample(
            int numData,
            Vector probCloudy,
            Vector[] cptSprinkler
            )
        {
            int[][] sample = new int[4][];
            for (int i = 0; i < 4; i++)
            {
                sample[i] = new int[numData];
            }

            for (int j = 0; j < numData; j++)
            {
                int cloudy = Discrete.Sample(probCloudy);
                int sprinkler = Discrete.Sample(cptSprinkler[cloudy]);
                sample[0][j] = cloudy;
                sample[1][j] = sprinkler;
                ;
            }

            return sample;
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
    }

}
