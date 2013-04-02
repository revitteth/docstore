using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Models
{
    public class Backup
    {
        public Backup()
        {
            this.Documents = new HashSet<DocPath>();
        }

        [Key]
        public int BackupId { get; set; }
        public string Name { get; set; }




        public virtual ICollection<DocPath> Documents { get; set; }
    }
}
