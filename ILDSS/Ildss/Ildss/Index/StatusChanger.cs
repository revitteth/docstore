using Ildss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    class StatusChanger : IStatusChanger
    {
        public void UpdateStatus(Settings.DocStatus status, Document document)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            Console.WriteLine("SAVING");
            fic.Documents.First(i => i.DocumentId == document.DocumentId).Status = status;
            fic.SaveChanges();
            Console.WriteLine("Saved");
        }

        public void UpdateStatus(Settings.DocStatus status, string path)
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var document = fic.DocPaths.First(i => i.Path == path).Document;
            document.Status = status;
            fic.SaveChanges();
        }


        public void UpdateStatus(Settings.DocStatus status, List<Document> documents)
        {
            foreach (var document in documents)
            {
                UpdateStatus(status, document);
            }
        }

        public void UpdateStatus(Settings.DocStatus status, List<string> paths)
        {
            foreach (var path in paths)
            {
                UpdateStatus(status, path);
            }
        }
    }
}
