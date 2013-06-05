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
        public void AddVersion(Enums.DocStatus status, List<Tuple<Document, string, DateTime>> documents)
        {
            foreach (var document in documents)
            {
                AddVersion(status, document);
            }
        }

        public void AddVersion(Enums.DocStatus status, List<Tuple<string, string, DateTime>> paths)
        {
            foreach (var path in paths)
            {
                AddVersion(status, path);
            }
        }

        public void AddVersion(Enums.DocStatus status, Tuple<Document, string, DateTime> document)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            Console.WriteLine("SAVING");
            var doc = fic.Documents.First(i => i.DocumentId == document.Item1.DocumentId);
            // update status
            doc.Status = status;
            // add a version
            doc.DocVersions.Add(new DocVersion() { 
                DocumentHash = document.Item2, 
                DocEventTime = document.Item3, 
                VersionKey = document.Item2 + document.Item3.ToString("ddmmyyyymmmmhhss")
            });
            fic.SaveChanges();
            Console.WriteLine("Saved");
        }

        public void AddVersion(Enums.DocStatus status, Tuple<string, string, DateTime> path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var doc = fic.DocPaths.First(i => i.Path == path.Item1).Document;
            // update status
            doc.Status = status;
            // add a version
            doc.DocVersions.Add(new DocVersion()
            {
                DocumentHash = path.Item2,
                DocEventTime = path.Item3,
                VersionKey = path.Item2 + path.Item3.ToString("ddmmyyyymmmmhhss")
            });
            fic.SaveChanges();
        }

        public void UpdateStatus(Enums.DocStatus status, Document doc)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var document = fic.Documents.First(i => i.DocumentId == doc.DocumentId);
            document.Status = Enums.DocStatus.Archived;
            fic.SaveChanges();
        }

        public void UpdateStatus(Enums.DocStatus status, List<Document> docs)
        {
            foreach (var doc in docs)
            {
                UpdateStatus(status, doc);    
            }
        }
    }
}
