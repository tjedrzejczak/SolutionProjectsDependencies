using System;

namespace SolutionProjectsDependencies
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("    spd.exe  SolutionFileName.sln");
            }

            if (!SolutionFileAnalyzer.Analyze(args[0]))
                Environment.Exit(1);
        }
    }
}