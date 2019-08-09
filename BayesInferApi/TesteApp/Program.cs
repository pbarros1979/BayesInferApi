using BayesInferCore.Model;
using BayesInferCore.Services;
using Microsoft.ML.Probabilistic.Algorithms;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace TesteApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            
            string path = GetApplicationRoot();
            ServiceRedeBayesiana serviceRede = new ServiceRedeBayesiana();
            FileBayesianNetwork redeBayesiana = serviceRede.LoadFileRedeBayesiana(path + @"\RedePensamentoCritico.json");
            //RedeBayesiana redeBayesiana = serviceRede.LoadRedeBayesiana(path + @"\WetGrass_Sprinkler_Rain.json");
            //RedeBayesiana redeBayesiana = serviceRede.LoadRedeBayesiana(path + @"\WetGrass_Sprinkler_Rain_Teste.json");
            //RedeBayesiana redeBayesiana = serviceRede.LoadRedeBayesiana(path + @"\rede_bayesianaTeste4.json");
            //RedeBayesiana redeBayesiana = serviceRede.LoadRedeBayesiana(path + @"\rede_bayesianaTeste5.json");
            //RedeBayesiana redeBayesiana = serviceRede.LoadRedeBayesiana(path + @"\rede_bayesiana_3.json"); 
            BayesInfer bayesInfer = new BayesInfer(redeBayesiana);

            Dictionary<string, int?> crenca = new Dictionary<string, int?>();
            Node node;
            node = redeBayesiana.Nodes.Where(n => n.Id == "CasosSimilares_CS").SingleOrDefault();
            crenca.Add(node.Id, 0);
            node = redeBayesiana.Nodes.Where(n => n.Id == "sprinkler").SingleOrDefault();
            crenca.Add(node.Id, 1);
            node = redeBayesiana.Nodes.Where(n => n.Id == "Wet grass").SingleOrDefault();
            crenca.Add(node.Id, 1);
            //var ret=bayesInfer.InferModel(crenca, "Rain");

            //var ret = bayesInfer.InferModel(crenca);
            //Console.WriteLine("Inferencia 1");
            //foreach (var item in ret.Nodes)
            //{
            //    if (item.InferModelResult != null)
            //    {
            //        var NodePosterior = item.InferModelResult[0].GetProbs()[0];
            //        Console.WriteLine("P(" + item.Id + ")= {0:0.0000}", NodePosterior);
            //    }
                
            //}
            //crenca.Clear();
            //Console.WriteLine("Inferencia 2");
            //node = redeBayesiana.Nodes.Where(n => n.Id == "Cloud").SingleOrDefault();
            //crenca.Add(node.Id, null);
            //node = redeBayesiana.Nodes.Where(n => n.Id == "sprinkler").SingleOrDefault();
            //crenca.Add(node.Id, 0);
            //node = redeBayesiana.Nodes.Where(n => n.Id == "Wet grass").SingleOrDefault();
            //crenca.Add(node.Id, 1);

            //ret = bayesInfer.InferModel(crenca);

            //foreach (var item in ret.Nodes)
            //{
            //    if (item.InferModelResult != null)
            //    {
            //        var NodePosterior = item.InferModelResult[0].GetProbs()[0];
            //        Console.WriteLine("P(" + item.Id + ")= {0:0.0000}", NodePosterior);
            //    }

            //}
            //crenca.Clear();

            //Console.WriteLine("Inferencia 3");
            //node = redeBayesiana.Nodes.Where(n => n.Id == "Cloud").SingleOrDefault();
            //crenca.Add(node.Id, 0);
            //node = redeBayesiana.Nodes.Where(n => n.Id == "sprinkler").SingleOrDefault();
            //crenca.Add(node.Id, null);
            //node = redeBayesiana.Nodes.Where(n => n.Id == "Wet grass").SingleOrDefault();
            //crenca.Add(node.Id, null);

            //ret = bayesInfer.InferModel(crenca);

            //foreach (var item in ret.Nodes)
            //{
            //    if (item.InferModelResult != null)
            //    {
            //        var NodePosterior = item.InferModelResult[0].GetProbs()[0];
            //        Console.WriteLine("P(" + item.Id + ")= {0:0.0000}", NodePosterior);
            //    }
            //}
            //var ret = bayesInfer.InferModel(crenca, "sprinkler");
            //Console.WriteLine("VAlor = "+ret.ToString());
            //Console.ReadKey();
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
