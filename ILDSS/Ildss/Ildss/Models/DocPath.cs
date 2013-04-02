using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Models
{
    public class DocPath
    {
        public int DocPathId { get; set; }
        public int DocumentId { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }
        public string Name { get; set; }

        public virtual Document Document { get; set; }
    }
}
