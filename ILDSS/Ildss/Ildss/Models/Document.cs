﻿using System;
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
            this.DocVersions = new HashSet<DocVersion>();
        }

        [Key]
        public int DocumentId { get; set; }
        public string DocumentHash { get; set; }
        public long Size { get; set; }
        public Enums.DocStatus? Status { get; set; }

        public virtual ICollection<DocPath> DocPaths { get; set; }
        public virtual ICollection<DocEvent> DocEvents { get; set; }
        public virtual ICollection<DocVersion> DocVersions { get; set; }

    }
}
