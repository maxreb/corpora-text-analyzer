using CorporaTextAnalyzer;
using System;
namespace CorporaTextAnalyzerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new Analyzer();
            var freq = x.GetRankAndFrequency("der");
            x.AnalyzeFile("example/2018-10-21-Test-Datei-für-Max.csv");
            Console.WriteLine("Hello World!");

        }
    }
}
