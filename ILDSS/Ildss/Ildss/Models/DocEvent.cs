using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public class DocEvent
    {
        public int DocEventId { get; set; }
        public string type { get; set; }
        public string DocumentDocumentHash { get; set; }
        public string path { get; set; }
        public string old_path { get; set; }
        public string name { get; set; }
        public string old_name { get; set; }
        public System.DateTime last_access { get; set; }
        public System.DateTime last_write { get; set; }

        public virtual Document Document { get; set; }

    }
}
