using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionProjectsDependencies
{
    internal class DependenciesBuilder
    {
        private readonly Dictionary<string, ProjectInfo> _projects = new Dictionary<string, ProjectInfo>();

        internal IEnumerable<ProjectInfo> GetProjects()
            => _projects.OrderBy(x => x.Key)
                .Select(x => x.Value);


        internal void Process(List<string> absProjects)
        {
            absProjects.ForEach(p =>
            {
                var referredProjects = GetProjectReferences(p);

                string csprojFolder = Path.GetDirectoryName(p);

                referredProjects.ToList().ForEach(x =>
                {
                    string absRefProjPath = Path.GetFullPath(Path.Combine(csprojFolder, x));

                    AddReference(p, absRefProjPath);
                });
            });
        }

        private IEnumerable<string> GetProjectReferences(string absProject)
        {
            var prl = File.ReadAllLines(absProject)
              .Where(x => x.Contains("<ProjectReference"));

            foreach (string line in prl)
            {
                int idx1 = line.IndexOf("Include=\"", StringComparison.OrdinalIgnoreCase);
                if (idx1 > 0)
                {
                    int idx2 = line.IndexOf("\"", idx1 + 10, StringComparison.OrdinalIgnoreCase);
                    if (idx2 > 0)
                    {
                        yield return line.Substring(idx1 + 9, idx2 - idx1 - 9);
                    }
                }
            }
        }

        private void AddReference(string srcPath, string destPath)
        {
            var src = GetProjectInfo(srcPath);
            var desc = GetProjectInfo(destPath);

            src.AddReference(desc);
            desc.AddRevReference(src);
        }

        private ProjectInfo GetProjectInfo(string path)
        {
            if (!_projects.TryGetValue(path, out var pi))
            {
                pi = new ProjectInfo(path);
                _projects.Add(path, pi);
            }

            return pi;
        }
    }
}