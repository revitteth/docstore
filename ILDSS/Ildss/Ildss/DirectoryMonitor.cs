using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Reactive.Linq;
using System.Reactive;

namespace Ildss
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class DirectoryMonitor : IMonitor
    {
        private DateTime LastChange;

        public void Monitor (string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern => {
                    var pe = pattern.EventArgs;

                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                    var id = fic.DocPaths.First(i => i.path == pe.FullPath);
                    var id2 = id.Document.DocumentId;

                    var document = fic.Documents.First(i => i.DocumentId == id2);
                    fic.Documents.Attach(document);
                    fic.Documents.Remove(document);
                    fic.SaveChanges();

                    Console.WriteLine("Delete occurred " + pattern.EventArgs.FullPath);
                }
            );
        }
    }
}
