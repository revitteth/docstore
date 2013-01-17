using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ildss
{
    class FileIndexContext : DbContext, IFileIndexContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocPath> DocPaths { get; set; }
        public DbSet<DocEvent> DocEvents { get; set; }
    }
}
