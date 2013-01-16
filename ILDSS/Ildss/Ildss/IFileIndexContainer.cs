using System;
namespace Ildss
{
    interface IFileIndexContainer
    {
        int SaveChanges();
        System.Data.Entity.DbSet<DocEvent> DocEvents { get; set; }
        System.Data.Entity.DbSet<DocPath> DocPaths { get; set; }
        System.Data.Entity.DbSet<Document> Documents { get; set; }
        System.Data.Entity.DbSet<EventQueueBackup> EventQueueBackups { get; set; }
    }
}
