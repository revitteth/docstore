using Ildss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    public class FSEvent
    {
        public Enums.EventType Type { get; set; }
        public FileInfo FileInf { get; set; }
        public string OldPath { get; set; }
        public bool isDirectory { get; set; }
        public int DocumentId { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime LastWrite { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
