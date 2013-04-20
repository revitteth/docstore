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
        private string _changedOffice = "";
        private IList<string> _ignoredFiles = Settings.IgnoredExtensions;

        public void Monitor (string path)
        {
            Logger.write("Directory Monitor Started");

            var fsw = new FileSystemWatcher(path);
            var freqChecker = KernelFactory.Instance.Get<IIndexChecker>();
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;

            IObservable<EventPattern<FileSystemEventArgs>> fswCreated = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Created");
            fswCreated.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Created");

                    try
                    {
                        if (!_ignoredFiles.Any(pe.Name.Contains) & pe.Name.Contains("."))
                        {
                            var fi = new FileInfo(pe.FullPath);
                            if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                            {
                                if (!_ignoredFiles.Any(pe.Name.Contains) && fi.Extension != "")
                                {
                                    freqChecker.RespondToEvent(pe.FullPath, "Created");
                                }
                                else
                                {
                                    Logger.write("Ignored file created" + path);
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException e)
                    {
                        Logger.write("FileNotFoundException " + e.Message);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswDeleted = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Deleted");
            fswDeleted.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Deleted");
                    if (!_ignoredFiles.Any(pe.Name.Contains) & pe.Name.Contains("."))
                    {
                        freqChecker.RespondToEvent(pe.FullPath, "Deleted");
                    }
                    else
                    {
                        Logger.write("Ignored file deleted " + path);
                        if(_ignoredFiles.Any(pe.Name.Contains))
                        {
                            freqChecker.MaintainDocuments();
                        }
                    }
                }
            );

            IObservable<EventPattern<RenamedEventArgs>> fswRenamed = Observable.FromEventPattern<RenamedEventArgs>(fsw, "Renamed");
            fswRenamed.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Renamed");
                    var fi = new FileInfo(pe.FullPath);

                    Logger.write(pe.OldFullPath + "   " + pe.FullPath);

                    if (_ignoredFiles.Any(pe.FullPath.Contains))
                    {
                        Logger.write("Renamed to an ignored file " + pe.Name);
                        _changedOffice = pe.FullPath;
                    }
                    else if(_ignoredFiles.Any(pe.OldFullPath.Contains) | !pe.OldFullPath.Contains("."))
                    {
                        Logger.write("Changed file (atomic temp) " + pe.Name);
                        freqChecker.RespondToEvent(pe.FullPath, "Changed");    
                    }
                    else if (!_ignoredFiles.Any(pe.Name.Contains))  // beware the extensionless excel temp - same as a directory!! :(
                    {
                        freqChecker.RespondToEvent(pe.FullPath, "Renamed", pe.OldFullPath);
                    }
                }
            );

            IObservable<EventPattern<FileSystemEventArgs>> fswChanged = Observable.FromEventPattern<FileSystemEventArgs>(fsw, "Changed");
            fswChanged.Subscribe(
                pattern =>
                {
                    var pe = pattern.EventArgs;
                    Logger.write("FSW Event - Changed");
                    var fi = new FileInfo(pe.FullPath);
                    if (!_ignoredFiles.Any(pe.Name.Contains) & fi.Extension != "")
                    {
                        if (!(File.GetAttributes(pe.FullPath) == FileAttributes.Directory))
                        {
                            freqChecker.RespondToEvent(pe.FullPath, "Changed");
                        }
                        else
                        {
                            Logger.write("Ignored file changed " + path);
                        }
                    }
                }
            );

        }
    }
}
