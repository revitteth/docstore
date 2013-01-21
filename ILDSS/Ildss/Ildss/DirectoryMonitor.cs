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
    public class DirectoryMonitor : IDirectoryMonitor
    {
        private DateTime LastChange;

        public void MonitorFileSystem (string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path, "*.*");

            fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern => {
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                    var pe = pattern.EventArgs;
                    var fi = new FileInfo(pe.FullPath);
                    
                    //Ensure not a directory
                    if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                    {
                        var fileHash = KernelFactory.Instance.Get<IHash>().HashFile(pe.FullPath);
                        var d = new Document();

                        // create document + path + event - CHECK IT DOESN'T EXIST ALREADY!!!!! AS A HASH PAL
                        if(fic.Documents.Any(i => i.DocumentHash == fileHash))
                        {
                            // document hash exists - give it a new path
                            d = fic.Documents.First(i => i.DocumentHash == fileHash);
                            d.DocPaths.Add(new DocPath() { path = pe.FullPath });
                        }
                        else 
                        {
                            // Document is new 
                            d = new Document() { DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(pe.FullPath), size = fi.Length };
                            var p = new DocPath() { path = fi.FullName, Document = d };
                            d.DocPaths.Add(p);
                        }

                        
                        var e = new DocEvent()
                        {
                            Document = d,
                            type = "Created",
                            last_write = fi.LastWriteTime,
                            last_access = fi.LastWriteTime,
                            //event_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond))
                        };

                        fic.DocEvents.Add(e);
                        fic.SaveChanges();
                        Console.WriteLine("Creation occurred " + pattern.EventArgs.FullPath);
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                    var fi = new FileInfo(pe.FullPath);
                    
                    //Ensure not a directory
                    if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                    {
                        // Find renamed document -- EXCEPTION HERE ON NEW FILE
                        var ddd = fic.Documents.First(i => i.DocPaths.Any(j => j.path == pe.OldFullPath) == true);

                        // Update path
                        var pathToUpdate = ddd.DocPaths.First(i => i.path == pe.OldFullPath);
                        pathToUpdate.path = fi.FullName;
                        fic.SaveChanges();

                        // Create event
                        var de = new DocEvent()
                        {
                            type = WatcherChangeTypes.Renamed.ToString(),
                            last_access = fi.LastAccessTime,
                            last_write = fi.LastWriteTime,
                            Document = ddd,
                            //event_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond))
                        };

                        //KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                        fic.DocEvents.Add(de);
                        fic.SaveChanges();
                        Console.WriteLine("Rename occurred " + pe.FullPath);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern => {
                    var pe = pattern.EventArgs;
                    
                    //Ensure not a directory
                    //if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                   // {
                        var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                        var id = fic.DocPaths.First(i => i.path == pe.FullPath);
                        var id2 = id.Document.DocumentId;

                        var document = fic.Documents.First(i => i.DocumentId == id2);
                        fic.Documents.Attach(document);
                        fic.Documents.Remove(document);
                        fic.SaveChanges();

                        Console.WriteLine("Delete occurred " + pattern.EventArgs.FullPath);
                    //}
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern => {
                    var pe = pattern.EventArgs;
                    
                    //Ensure not a directory
                    if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory) && LastChange != (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond)))
                    {

                        var fi = new FileInfo(pe.FullPath);
                        var fic = KernelFactory.Instance.Get<IFileIndexContext>();

                        var updatedDocument = new Document();

                        if (fic.Documents.Any(i => i.DocPaths.Any(j => j.path == pe.FullPath) == true))
                        {
                            updatedDocument = fic.Documents.First(i => i.DocPaths.Any(j => j.path == pe.FullPath) == true);
                            updatedDocument.DocumentHash = KernelFactory.Instance.Get<IHash>().HashFile(pe.FullPath);
                            updatedDocument.size = fi.Length;
                            fic.SaveChanges();
                        }
                        else
                        {
                            //document doesn't exist?! - INVESTIGATE THIS FURTHER!!!
                            // EDIDING A FILE THAT HASN'T BEEN INDEXED
                            Console.WriteLine(fi.FullName + " APPARENTLY DOESN'T EXIST");
                        }

                        var de = new DocEvent()
                        {
                            type = WatcherChangeTypes.Changed.ToString(),
                            Document = updatedDocument,
                            last_access = fi.LastAccessTime,
                            last_write = fi.LastWriteTime,
                            //event_time = (DateTime.Now).AddTicks(-((DateTime.Now).Ticks % TimeSpan.TicksPerSecond))
                        };

                        fic.DocEvents.Add(de);
                        fic.SaveChanges();

                        //KernelFactory.Instance.Get<IEventQueue>().AddEvent(de);
                        //LastChange = de.event_time;
                        Console.WriteLine("Change occurred " + fi.FullName);
                    }
                }
            );
        }
    }
}
