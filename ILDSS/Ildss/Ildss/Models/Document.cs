using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Models
{
    public class Document
    {
        public Document()
        {
            this.DocPaths = new HashSet<DocPath>();
            this.DocEvents = new HashSet<DocEvent>();
            this.Backups = new HashSet<Backup>();
        }

        public int DocumentId { get; set; }
        public string DocumentHash { get; set; }
        public long Size { get; set; }
        public Settings.DocStatus? Status { get; set; }

        public virtual ICollection<DocPath> DocPaths { get; set; }
        public virtual ICollection<DocEvent> DocEvents { get; set; }
        public virtual ICollection<Backup> Backups { get; set; }

    }
}
