using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public class DocPath
    {
        public int DocPathId { get; set; }
        public int DocumentId { get; set; }
        public string path { get; set; }
        public string name { get; set; }

        public virtual Document Document { get; set; }
    }
}
