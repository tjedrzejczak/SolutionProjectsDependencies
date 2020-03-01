using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionProjectsDependencies
{
    internal class SolutionFileAnalyzer
    {
        private readonly string _slnFilePath;

        public SolutionFileAnalyzer(string slnFilePath)
        {
            _slnFilePath = slnFilePath;
        }

        internal static bool Analyze(string slnFilePath)
        {
            Console.WriteLine($"Analyzing {slnFilePath} ...");

            return (new SolutionFileAnalyzer(slnFilePath)).Process();
        }

        private bool Process()
        {
            var lines = File.ReadAllLines(_slnFilePath);

            if (!IsSolutionFile(lines))
            {
                Console.WriteLine("This is not valid .sln file.");
                return false;
            }

            var relProjects = GetAllCSProjs(lines);
            if (!relProjects.Any())
            {
                Console.WriteLine("Found 0 projects.");
                return false;
            }

            string slnFolder = Path.GetDirectoryName(_slnFilePath);
            var absProjects = relProjects
                .Select(p => GetAbsoluteProjectPath(slnFolder, p))
                .ToList();

            Console.WriteLine($"Found projects ({absProjects.Count}):");
            foreach (string ap in absProjects)
            {
                Console.WriteLine($"\t{ap}");
            }

            var builder = new DependenciesBuilder();
            builder.Process(absProjects);

            var outCreator = new OutputFileCreator(builder.GetProjects());
            var outFileName = outCreator.CreateWithMermaidContent(_slnFilePath);

            Console.WriteLine();
            Console.WriteLine($"Genereted file: {outFileName}");

            System.Diagnostics.Process.Start(outFileName);

            return true;
        }

        private bool IsSolutionFile(string[] lines)
            => lines.Any(x => x.Contains("Microsoft Visual Studio Solution File"));

        private IEnumerable<string> GetAllCSProjs(string[] lines)
        {
            foreach (string line in lines.Where(l => l.StartsWith("Project(")))
            {
                int idx1 = line.IndexOf(".csproj", StringComparison.OrdinalIgnoreCase);
                if (idx1 > 0)
                {
                    int idx2 = line.Substring(0, idx1).LastIndexOf("\"", StringComparison.OrdinalIgnoreCase);
                    yield return line.Substring(idx2 + 1, idx1 - idx2 + 6);
                }
            }
        }

        private string GetAbsoluteProjectPath(string slnFolder, string relProjectPath)
            => Path.GetFullPath(Path.Combine(slnFolder, relProjectPath));
    }
}