using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionProjectsDependencies
{
    internal class ProjectInfo
    {
        internal string Id { get; }
        internal string Path { get; }
        internal string ShortName { get; }

        private readonly Dictionary<string, ProjectInfo> _references = new Dictionary<string, ProjectInfo>();
        private readonly Dictionary<string, ProjectInfo> _refered = new Dictionary<string, ProjectInfo>();

        public ProjectInfo(string path)
        {
            Id = $"P{_staticId++}";
            Path = path;
            ShortName = new FileInfo(path).Name;
        }

        public void AddReference(ProjectInfo destProj)
        {
            if (!_references.ContainsKey(destProj.Path))
                _references.Add(destProj.Path, destProj);
        }

        public void AddRevReference(ProjectInfo srcProj)
        {
            if (!_refered.ContainsKey(srcProj.Path))
                _refered.Add(srcProj.Path, srcProj);
        }

        public IEnumerable<ProjectInfo> GetReferences()
            => _references.OrderBy(x => x.Key)
                    .Select(x => x.Value);

        public IEnumerable<ProjectInfo> GetRevReferences()
            => _refered.OrderBy(x => x.Key)
                .Select(x => x.Value);

        public string GetMermaidInfo()
            => $"{Id}(\"{ShortName} <br />depends:{GetReferences().Count()} dependent:{GetRevReferences().Count()}\")";

        private static int _staticId = 100;
    }
}