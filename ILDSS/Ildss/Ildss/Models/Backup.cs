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
            this.Documents = new HashSet<Document>();
        }

        [Key]
        public int BackupId { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public long FileCount { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}
