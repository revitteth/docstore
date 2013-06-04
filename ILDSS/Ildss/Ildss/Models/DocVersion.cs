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
        [Required]
        public string DocumentHash { get; set; }
        [Required]
        public DateTime DocEventTime { get; set; }
        public string VersionKey { get; set; }
        
        // need to also store hash and datetime - makes it easier to go through history

        public virtual Document Document { get; set; }
    }
}
