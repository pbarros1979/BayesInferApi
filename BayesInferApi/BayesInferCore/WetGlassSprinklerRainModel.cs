using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Probabilistic.Models;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using System.Linq;

namespace BayesInferCore
{
    public class WetGlassSprinklerRainModel
    {
        // Variáveis aleatórias primárias
        public VariableArray<int> Cloudy;
        public VariableArray<int> Sprinkler;
        public VariableArray<int> Rain;
        public VariableArray<int> WetGrass;
        public Variable<int> NumberOfExamples;


        // Variáveis aleatórias que representam os parâmetros das distribuições das variáveis aleatórias primárias.
        // Para variáveis filho, elas estão na forma de tabelas de probabilidade condicional (CPTs)
        public Variable<Vector> ProbCloudy;
        public VariableArray<Vector> CPTSprinkler;
        public VariableArray<Vector> CPTRain;
        public VariableArray<VariableArray<Vector>, Vector[][]> CPTWetGrass;

        // Distribuições prévias para as variáveis de probabilidade e CPT.
        // As distribuições anteriores são formuladas como variáveis Infer.NET 
        // para que possam ser configurados em tempo de execução sem recompilar o modelo
        public Variable<Dirichlet> ProbCloudyPrior;
        public VariableArray<Dirichlet> CPTSprinklerPrior;
        public VariableArray<Dirichlet> CPTRainPrior;
        public VariableArray<VariableArray<Dirichlet>, Dirichlet[][]> CPTWetGrassPrior;

        // Distribuições posteriores para as variáveis probabilidade e CPT.
        public Dirichlet ProbCloudyPosterior;
        public Dirichlet[] CPTSprinklerPosterior;
        public Dirichlet[] CPTRainPosterior;
        public Dirichlet[][] CPTWetGrassPosterior;

        // Inference engine
        public InferenceEngine Engine = new InferenceEngine();

        /// <summary>
        /// Constructs a new Glass/Sprinkler/Rain model
        /// </summary>
        public WetGlassSprinklerRainModel()
        {
            // Set up the ranges
            NumberOfExamples = Variable.New<int>().Named("NofE");
            Range N = new Range(NumberOfExamples).Named("N");

            // Embora todas as variáveis neste exemplo tenham apenas 2 estados (true/false),
            // o exemplo é formulado de uma forma que mostra como se estender a vários estados
            Range C = new Range(2).Named("C");
            Range S = new Range(2).Named("S");
            Range R = new Range(2).Named("R");
            Range W = new Range(2).Named("W");

            // Definir as prioridades e os parâmetros
            ProbCloudyPrior = Variable.New<Dirichlet>().Named("ProbCloudyPrior");
            ProbCloudy = Variable<Vector>.Random(ProbCloudyPrior).Named("ProbCloudy");
            ProbCloudy.SetValueRange(C);

            // Tabela de probabilidade do sprinkler condicionada estar nublado
            CPTSprinklerPrior = Variable.Array<Dirichlet>(C).Named("CPTSprinklerPrior");
            CPTSprinkler = Variable.Array<Vector>(C).Named("CPTSprinkler");
            CPTSprinkler[C] = Variable<Vector>.Random(CPTSprinklerPrior[C]);
            CPTSprinkler.SetValueRange(S);

            // Tabela de probabilidade de chuva condicionada a estar nublado
            CPTRainPrior = Variable.Array<Dirichlet>(C).Named("CPTRainPrior");
            CPTRain = Variable.Array<Vector>(C).Named("CPTRain");
            CPTRain[C] = Variable<Vector>.Random(CPTRainPrior[C]);
            CPTRain.SetValueRange(R);

            // Tabela de probabilidade de grama molhada, condicionada por aspersão e chuva
            CPTWetGrassPrior = Variable.Array(Variable.Array<Dirichlet>(R), S).Named("CPTWetGrassPrior");
            CPTWetGrass = Variable.Array(Variable.Array<Vector>(R), S).Named("CPTWetGrass");
            CPTWetGrass[S][R] = Variable<Vector>.Random(CPTWetGrassPrior[S][R]);
            CPTWetGrass.SetValueRange(W);

            // Definir as principais variáveis
            Cloudy = Variable.Array<int>(N).Named("Cloudy");
            Cloudy[N] = Variable.Discrete(ProbCloudy).ForEach(N);
            Sprinkler = AddChildFromOneParent(Cloudy, CPTSprinkler).Named("Sprinkler");
            Rain = AddChildFromOneParent(Cloudy, CPTRain).Named("Rain");
            WetGrass = AddChildFromTwoParents(Sprinkler, Rain, CPTWetGrass).Named("WetGrass");
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
            int[] rain,
            int[] wetgrass,
            Dirichlet probCloudyPrior,
            Dirichlet[] cptSprinklerPrior,
            Dirichlet[] cptRainPrior,
            Dirichlet[][] cptWetGrassPrior)
        {
            NumberOfExamples.ObservedValue = cloudy.Length;
            Cloudy.ObservedValue = cloudy;
            Sprinkler.ObservedValue = sprinkler;
            Rain.ObservedValue = rain;
            WetGrass.ObservedValue = wetgrass;
            ProbCloudyPrior.ObservedValue = probCloudyPrior;
            CPTSprinklerPrior.ObservedValue = cptSprinklerPrior;
            CPTRainPrior.ObservedValue = cptRainPrior;
            CPTWetGrassPrior.ObservedValue = cptWetGrassPrior;

           
            // Inference
            ProbCloudyPosterior = Engine.Infer<Dirichlet>(ProbCloudy);
            CPTSprinklerPosterior = Engine.Infer<Dirichlet[]>(CPTSprinkler);
            CPTRainPosterior = Engine.Infer<Dirichlet[]>(CPTRain);
            CPTWetGrassPosterior = Engine.Infer<Dirichlet[][]>(CPTWetGrass);
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
            Dirichlet[][] dirUnifArrayArray = Enumerable.Repeat(dirUnifArray, 2).ToArray();

            LearnParameters(cloudy, sprinkler, rain, wetgrass, probCloudyPrior, dirUnifArray, dirUnifArray, dirUnifArrayArray);
        }

        /// <summary>
        /// Returns the probability of Rain given optional readings on
        /// cloudiness, sprinkler, and wetness of grass, and given prior distributions
        /// over the parameters. Priors may be manually set, or may be the posteriors
        /// from learning the parameters.
        /// </summary>
        /// <param name="cloudy">Optional observation of cloudy or not</param>
        /// <param name="sprinkler">Optional observation of whether sprinkler is on or not</param>
        /// <param name="wet">Optional observation or whether grass is wet or not</param>
        /// <param name="probCloudyPrior">Prior distribution over cloudiness probability vector</param>
        /// <param name="cptSprinklerPrior">Prior distribution over sprinkller conditional probability table</param>
        /// <param name="cptRainPrior">Prior distribution over rain conditional probability table</param>
        /// <param name="cptWetGrassPrior">Prior distribution over wet grass conditional probability table</param>
        /// <returns>Probability that it has rained</returns>
        public double ProbRain(
            int? cloudy,
            int? sprinkler,
            int? wet,
            Dirichlet probCloudyPrior,
            Dirichlet[] cptSprinklerPrior,
            Dirichlet[] cptRainPrior,
            Dirichlet[][] cptWetGrassPrior)
        {
            NumberOfExamples.ObservedValue = 1;
            if (cloudy.HasValue)
            {
                Cloudy.ObservedValue = new int[] { cloudy.Value };
            }
            else
            {
                Cloudy.ClearObservedValue();
            }

            if (sprinkler.HasValue)
            {
                Sprinkler.ObservedValue = new int[] { sprinkler.Value };
            }
            else
            {
                Sprinkler.ClearObservedValue();
            }

            if (wet.HasValue)
            {
                WetGrass.ObservedValue = new int[] { wet.Value };
            }
            else
            {
                WetGrass.ClearObservedValue();
            }

            Rain.ClearObservedValue();

            ProbCloudyPrior.ObservedValue = probCloudyPrior;
            CPTSprinklerPrior.ObservedValue = cptSprinklerPrior;
            CPTRainPrior.ObservedValue = cptRainPrior;
            CPTWetGrassPrior.ObservedValue = cptWetGrassPrior;

            

            // Inference
            var rainPosterior = Engine.Infer<Discrete[]>(Rain);

            // In this example, index 0 is true and index 1 is false
            return rainPosterior[0].GetProbs()[0];
        }

        /// <summary>
        /// Returns the probability of Rain given optional readings on
        /// cloudiness, sprinkler, and wetness of grass, and given known parameters.
        /// </summary>
        /// <param name="cloudy">Optional observation of cloudy or not</param>
        /// <param name="sprinkler">Optional observation of whether sprinkler is on or not</param>
        /// <param name="wet">Optional observation or whether grass is wet or not</param>
        /// <param name="probCloudy">Cloudiness probability vector</param>
        /// <param name="cptSprinkler">Sprinkler conditional probability table</param>
        /// <param name="cptRain">Rain conditional probability table</param>
        /// <param name="cptWetGrass">Wet grass conditional probability table</param>
        /// <returns>Probability that it has rained</returns>
        public double ProbRain(
            int? cloudy,
            int? sprinkler,
            int? wet,
            Vector probCloudy,
            Vector[] cptSprinkler,
            Vector[] cptRain,
            Vector[][] cptWetGrass)
        {
            var probCloudyPrior = Dirichlet.PointMass(probCloudy);
            var cptSprinklerPrior = cptSprinkler.Select(v => Dirichlet.PointMass(v)).ToArray();
            var cptRainPrior = cptRain.Select(v => Dirichlet.PointMass(v)).ToArray();
            var cptWetGrassPrior = cptWetGrass.Select(va => va.Select(v => Dirichlet.PointMass(v)).ToArray()).ToArray();
            return ProbRain(cloudy, sprinkler, wet, probCloudyPrior, cptSprinklerPrior, cptRainPrior, cptWetGrassPrior);
        }

        /// <summary>
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
            Vector[] cptSprinkler,
            Vector[] cptRain,
            Vector[][] cptWetGrass)
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
                int rain = Discrete.Sample(cptRain[cloudy]);
                int wetGrass = Discrete.Sample(cptWetGrass[sprinkler][rain]);
                sample[0][j] = cloudy;
                sample[1][j] = sprinkler;
                sample[2][j] = rain;
                sample[3][j] = wetGrass;
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
