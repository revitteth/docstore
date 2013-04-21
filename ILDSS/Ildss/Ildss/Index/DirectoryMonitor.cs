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

namespace Ildss.Index
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class DirectoryMonitor : IMonitor
    {
        private IList<string> _ignoredFiles = Settings.IgnoredExtensions;

        public void Monitor (string path)
        {
            Logger.write("Directory Monitor Started");

            var fsw = new FileSystemWatcher(path);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;
            //fsw.InternalBufferSize = 32768;

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;

                    try
                    {
                        if (!_ignoredFiles.Any(pe.OldFullPath.Contains))
                        {
                            bool isDir = false;
                            if (File.GetAttributes(pe.OldFullPath) == FileAttributes.Directory | File.GetAttributes(pe.FullPath) == FileAttributes.Directory)
                            {
                                isDir = true;
                            }
                            var fs = new FSEvent() { Type = Settings.EventType.Rename, FileInf = new FileInfo(pe.FullPath), OldPath = pe.OldFullPath, isDirectory = isDir };
                            KernelFactory.Instance.Get<IEventManager>().AddEvent(fs);

                            Logger.write("FSW Event - Renamed");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.write("TempFile Atomic Update. Error: " + e.Message);
                    }
                    
                    //var fi = new FileInfo(pe.FullPath);

                    //Logger.write(pe.OldFullPath + " " + pe.FullPath);


                    // RE_EVALUATE LOGIC HERE
                    // if its a folder ... update all the paths
                    // if its throwing an error its probably an excel file or something
                    // if its a file... just update its path


                    //if (_ignoredFiles.Any(pe.FullPath.Contains))
                    //{
                    //    Logger.write("Renamed to an ignored file " + pe.Name);
                    //    _changedOffice = pe.FullPath;
                    //}
                    //else if (_ignoredFiles.Any(pe.OldFullPath.Contains) | !pe.OldFullPath.Contains("."))
                    //{
                    //    Logger.write("Changed file (atomic temp) " + pe.Name);
                    //    freqChecker.RespondToEvent(pe.FullPath, "Changed");
                    //}
                    //else if (!_ignoredFiles.Any(pe.Name.Contains)) // beware the extensionless excel temp - same as a directory!! :(
                    //{
                    //    freqChecker.RespondToEvent(pe.FullPath, "Renamed", pe.OldFullPath);
                    //}
                }
            );

        }
    }
}
