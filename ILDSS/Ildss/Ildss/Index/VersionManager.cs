using Ildss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    class VersionManager : IVersionManager
    {
        public void AddVersion(Settings.DocStatus status, List<Tuple<Document, string>> documents)
        {
            foreach (var document in documents)
            {
                AddVersion(status, document);
            }
        }

        public void AddVersion(Settings.DocStatus status, List<Tuple<string, string>> paths)
        {
            foreach (var path in paths)
            {
                AddVersion(status, path);
            }
        }

        public void AddVersion(Settings.DocStatus status, Tuple<Document, string> document)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            Console.WriteLine("SAVING");
            var doc = fic.Documents.First(i => i.DocumentId == document.Item1.DocumentId);
            // update status
            doc.Status = status;
            // add a version
            doc.DocVersions.Add(new DocVersion() { VersionKey = document.Item2 });
            fic.SaveChanges();
            Console.WriteLine("Saved");
        }

        public void AddVersion(Settings.DocStatus status, Tuple<string, string> path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var document = fic.DocPaths.First(i => i.Path == path.Item1).Document;
            // update status
            document.Status = status;
            // add a version
            document.DocVersions.Add(new DocVersion() { VersionKey = path.Item2});
            fic.SaveChanges();
        }
    }
}
