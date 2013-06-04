using System;

namespace Ildss.Models
{
    public interface IFileIndexContext
    {
        int SaveChanges();
        System.Data.Entity.DbSet<Document> Documents { get; set; }
        System.Data.Entity.DbSet<DocEvent> DocEvents { get; set; }
        System.Data.Entity.DbSet<DocPath> DocPaths { get; set; }
        System.Data.Entity.DbSet<StoredSettings> StoredSettings { get; set; }
    }
}
