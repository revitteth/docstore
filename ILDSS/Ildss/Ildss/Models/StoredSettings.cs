using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Models
{
    public class StoredSettings
    {
        [Key]
        public int StoredSettingId { get; set;}

        public string IgnoredExtensions { get; set; }
        public string WorkingDir { get; set; } //@"E:\Documents\GitHub\docstore\TestDir"; //@"C:\Users\Max\Documents\GitHub\docstore\TestDir";
        public string StorageDir { get; set; } //@"E:\Documents\GitHub\docstore\StorageDir"; //@"C:\Users\Max\Documents\GitHub\docstore\StorageDir";
        public bool FirstRun { get; set; }
        public int IndexInterval { get; set; }
        public string BucketName { get; set; }
    }
}
