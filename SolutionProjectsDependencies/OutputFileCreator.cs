using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionProjectsDependencies
{
    internal class OutputFileCreator
    {
        private readonly List<ProjectInfo> _projects;

        public OutputFileCreator(IEnumerable<ProjectInfo> projects)
        {
            _projects = projects.ToList();
        }

        internal string CreateWithMermaidContent(string slnFilePath)
        {
            string outFileName = slnFilePath + ".html";

            using (var outFile = new StreamWriter(outFileName))
            {
                WriteHeaderPart(outFile, slnFilePath);
                WrireDependenciesPart(outFile);
                WriteFooterPart(outFile);
            }

            return outFileName;
        }

        private void WriteHeaderPart(StreamWriter outFile, string slnFilePath)
        {
            outFile.WriteLine("<!DOCTYPE html>");
            outFile.WriteLine("<html>");
            outFile.WriteLine("  <body>");
            outFile.WriteLine("    <script src=\"https://unpkg.com/mermaid@8.4.8/dist/mermaid.min.js\"></script>");
            outFile.WriteLine("    <script>mermaid.initialize({startOnLoad:true});</script>");
            outFile.WriteLine($"    <h2>{slnFilePath}</h2>");
            outFile.WriteLine("    <div class=\"mermaid\">");
            outFile.WriteLine("      graph LR");
        }

        private void WrireDependenciesPart(StreamWriter outFile)
        {
            foreach (var p in _projects)
            {
                foreach (var r in p.GetReferences())
                {
                    outFile.WriteLine($"{p.GetMermaidInfo()} --> {r.GetMermaidInfo()}");
                }
            }
        }

        private void WriteFooterPart(StreamWriter outFile)
        {
            outFile.WriteLine("    </div>");
            outFile.WriteLine("    Solution analyzed by <a href=\"https://github.com/tjedrzejczak/SolutionProjectsDependencies\">SolutionProjectsDependencies</a>.");
            outFile.WriteLine("    Diagram displayed by <a href=\"https://github.com/mermaid-js/mermaid\">mermaid-js</a>.");
            outFile.WriteLine($"   <br /> {DateTime.Now}");
            outFile.WriteLine("  </body>");
            outFile.WriteLine("</html>");
        }
    }
}