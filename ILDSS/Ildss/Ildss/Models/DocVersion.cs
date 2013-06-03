using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Models
{
    public class DocVersion
    {
        [Key]
        public int DocEventId { get; set; }
        [Required]
        public int DocumentId { get; set; }
        public string VersionKey { get; set; }

        public virtual Document Document { get; set; }
    }
}
