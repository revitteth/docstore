﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ildss.Models
{
    public class FileIndexContext : DbContext, IFileIndexContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocPath> DocPaths { get; set; }
        public DbSet<DocEvent> DocEvents { get; set; }
        public DbSet<DocVersion> DocVersions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // For speedup
            //Configuration.AutoDetectChangesEnabled = false;
            //Configuration.ValidateOnSaveEnabled = false;

            modelBuilder.Entity<DocPath>()
                        .HasRequired(dp => dp.Document)
                        .WithMany(d => d.DocPaths)
                        .HasForeignKey(dp => dp.DocumentId)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<DocEvent>()
                        .HasRequired(de => de.Document)
                        .WithMany(d => d.DocEvents)
                        .HasForeignKey(de => de.DocumentId)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<DocVersion>()
                        .HasRequired(dv => dv.Document)
                        .WithMany(d => d.DocVersions)
                        .HasForeignKey(dv => dv.DocumentId)
                        .WillCascadeOnDelete();
        }
    }
}
