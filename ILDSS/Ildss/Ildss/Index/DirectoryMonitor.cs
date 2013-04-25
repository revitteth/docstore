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

using Ildss.Index;
using Ildss.Models;

namespace Ildss.Index
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class DirectoryMonitor : IMonitor
    {
        private IList<string> _ignoredFiles = Settings.getIgnoredExtensions();
        private string LastChanged { get; set; }
        private string LastCreated { get; set; }
        //private long FilesAffected = 0;

        public void Monitor (string path)
        {
            Logger.write("Directory Monitor Started");

            var fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    var fic = KernelFactory.Instance.Get<IFileIndexContext>();

                    try
                    {
                        if (!_ignoredFiles.Any(pe.OldName.Contains) & !_ignoredFiles.Any(pe.Name.Contains))
                        {
                            //bool isDir = false;
                            if (File.GetAttributes(pe.FullPath) == FileAttributes.Directory)
                            {
                                //isDir = true;
                                //FilesAffected = new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Count();
                                //Logger.write("File affected = " + FilesAffected.ToString());

                                foreach (var directory in fic.DocPaths.Where(i => i.Directory.Contains(pe.OldFullPath)))
                                {
                                    directory.Directory = directory.Directory.Replace(pe.OldFullPath, pe.FullPath); // subdirectories
                                }
                                foreach (var file in fic.DocPaths.Where(i => i.Path.Contains(pe.OldFullPath)))
                                {
                                    file.Path = file.Path.Replace(pe.OldFullPath, pe.FullPath);   // file paths
                                }
                            }
                            else
                            {
                                //FilesAffected = 1;
                                
                                var renamed = fic.DocPaths.First(i => i.Path == pe.OldFullPath);
                                renamed.Path = pe.FullPath;
                                renamed.Directory = pe.FullPath.Replace(pe.Name, "");
                                renamed.Name = pe.Name;
                            }
                            fic.SaveChanges();


                            //var fi = new FileInfo(pe.FullPath);
                            //var fs = new FSEvent() { 
                            //    Type = Settings.EventType.Rename, 
                            //    FileInf = fi, 
                            //    OldPath = pe.OldFullPath, 
                            //    isDirectory = isDir,
                            //    LastWrite = fi.LastWriteTime,
                            //    LastAccess = fi.LastAccessTime
                            //};
                            //// potential thread safety issue here!!!!
                            //KernelFactory.Instance.Get<IEventManager>("Index").AddEvent(fs);

                            //Logger.write("DM Renamed " + pe.Name);
                            //KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                        }
                    }
                    catch (Exception e)
                    {
                        //Logger.write("Rename Exception: Now called:" + pe.Name + " Error msg: " + e.Message);
                        //KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    if (!_ignoredFiles.Any(pattern.EventArgs.Name.Contains) & LastCreated != pattern.EventArgs.FullPath)
                    {
                        KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                        LastCreated = pattern.EventArgs.FullPath;
                        Logger.write("DM Created");
                    }
                }

            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    if (!_ignoredFiles.Any(pattern.EventArgs.Name.Contains))
                    {
                        KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                        Logger.write("DM Deleted");
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    if (!_ignoredFiles.Any(pattern.EventArgs.Name.Contains)) //& FilesAffected == 0)
                    {
                        KernelFactory.Instance.Get<IEventManager>("Index").IndexRequired = true;
                        LastChanged = pattern.EventArgs.FullPath;
                        Logger.write("DM Changed");
                    }
                    else
                    {
                        //FilesAffected--;
                    }
                }

            );

        }
    }
}
