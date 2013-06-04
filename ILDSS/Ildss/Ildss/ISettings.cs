using System;
namespace Ildss
{
    interface ISettings
    {
        string BucketName { get; set; }
        bool FirstRun { get; set; }
        System.Collections.Generic.IList<string> IgnoredExtensions { get; set; }
        int IncrementalBackupInterval { get; set; }
        int IndexInterval { get; set; }
        string StorageDir { get; set; }
        long TargetDiskUtilisation { get; set; }
        TimeSpan TargetDocumentMaxAge { get; set; }
        string WorkingDir { get; set; }
    }
}
