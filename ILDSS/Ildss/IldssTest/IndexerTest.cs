using Ildss;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace IldssTest
{
    [TestClass]
    public class IndexerTest
    {
        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            foreach (FileInfo fi in dir.GetFiles())
                fi.Delete();

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }


        [TestMethod]
        public void IndexerUpdatingFiles()
        {
            var indexer =  Ildss.KernelFactory.Instance.Get<IIndexer>();
            //var fic = Ildss.KernelFactory.Instance.Get<IFileIndexContext>();

            // Set up and clear the directory & db
            DirectoryInfo testDir = new DirectoryInfo(@"C:\TestDirectory");
            testDir.Create();
            //clearFolder(testDir.FullName);

            // Populate with the test directories + files
            testDir.CreateSubdirectory(@"directory1");
            File.WriteAllText(Path.Combine(testDir.FullName, @"file1.txt"), "some text");
            File.WriteAllText(Path.Combine(testDir.FullName, @"directory1\file1.txt"), "some text");

            // Run test sequence, compare output from db
            indexer.IndexFiles(testDir.FullName);

            // modify files
            File.AppendAllText(@"C:\TestDirectory\directory1\file1.txt", "test");

            // Run indexer again
            indexer.IndexFiles(testDir.FullName);

            //var documents = fic.Documents.ToList();
            //var paths = fic.DocPaths.ToList();
            //var events = fic.DocEvents.ToList();

            //documents            

        }

        [TestMethod]
        public void IndexerRenamingFiles()
        {

        }
    }
}
