using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Ildss.Models;
using Ildss.Crypto;

namespace Ildss.Index
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class FrequentChecker : IIndexChecker
    {
        List<DocPath> missingPaths = new List<DocPath>();
        List<DocPath> hashedPaths = new List<DocPath>();
        List<Document> removeDocs = new List<Document>();

        public void RespondToEvent(string path, string type, string oldpath = "")
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var hash = KernelFactory.Instance.Get<IHash>();

            switch (type)
            {
                case "Created":
                    // check db for matching hash
                    Console.WriteLine(path + " was created");
                    var fi = new FileInfo(path);
                    var fileHash = hash.HashFile(path);
                    var docpath = new DocPath() { path = fi.FullName, name = fi.Name, directory = fi.FullName.Replace(fi.Name, "") };

                    if (fic.Documents.Any(i => i.DocumentHash == fileHash))
                    {
                        // hash exists
                        fic.Documents.First(i => i.DocumentHash == fileHash).DocPaths.Add(docpath);
                    }
                    else
                    {
                        // new document + path
                        var doc = new Document() { DocumentHash = fileHash, size = fi.Length };
                        doc.DocPaths.Add(docpath);
                        fic.Documents.Add(doc);
                    }
                    fic.SaveChanges();

                    // Register the event - only if NOT a directory?
                    KernelFactory.Instance.Get<ICollector>().Register(fi.FullName);

                    break;

                case "Renamed":
                    // get old name and new name - figure it out
                    // find the right path by the document hash, add it and delete the old one (probably in same directory with same hash - thats how to tell)
                    // if its a directory - rename all paths which have their directory set as it - and subdirectories somehow...
                    Console.WriteLine(oldpath + " is being renamed to " + path);

                    if (!(File.GetAttributes(path) == FileAttributes.Directory))
                    {
                        // its just a file - rename it in docpaths
                        var finf = new FileInfo(path);
                        var renamed = fic.DocPaths.First(i => i.path == oldpath);
                        renamed.path = path;
                        renamed.directory = finf.FullName.Replace(finf.Name, "");
                        renamed.name = finf.Name;
                        fic.SaveChanges();
                    }
                    else
                    {
                        foreach (var directory in fic.DocPaths.Where(i => i.directory.Contains(oldpath)))
                        {
                            directory.directory = directory.directory.Replace(oldpath, path); // subdirectories
                        }
                        foreach (var file in fic.DocPaths.Where(i => i.path.Contains(oldpath)))
                        {
                            file.path = file.path.Replace(oldpath, path);   // file paths
                        }
                        fic.SaveChanges();
                    }

                    // Register the event - only if NOT a directory?
                    fi = new FileInfo(path);
                    KernelFactory.Instance.Get<ICollector>().Register(fi.FullName);

                    break;

                case "Deleted":
                    // remove db entry for the path then check that document needs deleting
                    Console.WriteLine(path + " was deleted");
                    // see if it matches a directory
                    if (fic.DocPaths.Any(i => i.directory == path))
                    {
                        var deletedPath = fic.DocPaths.First(i => i.path == path);
                        var deletedDoc = fic.Documents.First(i => i.DocumentId == deletedPath.DocumentId);
                        if (deletedDoc.DocPaths.Count() <= 1)
                            fic.Documents.Remove(deletedDoc);
                        else
                            fic.DocPaths.Remove(deletedPath);
                        fic.SaveChanges();
                    }
                    else
                    {
                        foreach (var dp in fic.DocPaths)
                        {
                            if (!File.Exists(dp.path))
                                missingPaths.Add(dp);
                        }
                        foreach (var mp in missingPaths)
                        {
                            fic.DocPaths.Remove(mp);
                        }
                        missingPaths.Clear();

                        foreach (var doc in fic.Documents)
                        {
                            if (!doc.DocPaths.Any())
                                removeDocs.Add(doc);
                        }
                        foreach (var d in removeDocs)
                        {
                            fic.Documents.Remove(d);
                        }
                        removeDocs.Clear();

                        fic.SaveChanges();
                    }
                    break;

                case "Changed":
                    // if single path - update the document and add new path
                    // if many paths - create new document and add path.
                    Console.WriteLine(path + " changed");

                    var finfo = new FileInfo(path);
                    var hashChanged = hash.HashFile(path);
                    var docs = fic.Documents;
                    var paths = fic.DocPaths;

                    if (docs.Any(i => i.DocumentHash == hashChanged))
                    {
                        // unlikely event - new file hash matches an existing document
                        var matchingDoc = docs.First(i => i.DocumentHash == hashChanged);
                        matchingDoc.DocPaths.Add(paths.First(i => i.path == path));
                    }
                    else
                    {
                        if (paths.Any(i => i.path == path))
                        {
                            var relatedDocument = paths.First(i => i.path == path).Document;
                            var thePath = paths.First(i => i.path == path);

                            if (relatedDocument.DocPaths.Count() == 1)
                            {
                                // update the document
                                relatedDocument.DocumentHash = hashChanged;
                                relatedDocument.size = finfo.Length;
                            }
                            else if (relatedDocument.DocPaths.Count() > 1)
                            {
                                // create new document + point the path to it
                                var newDoc = new Document() { size = finfo.Length, DocumentHash = hashChanged };
                                newDoc.DocPaths.Add(thePath);
                                docs.Add(newDoc);
                            }
                            else
                            {
                                // not sure if this can really happen!
                            }
                        }
                        else
                        {
                            // don't think this should happen either
                        }
                    }
    
                    fic.SaveChanges();

                    // Register the event - only if NOT a directory?
                    fi = new FileInfo(path);
                    KernelFactory.Instance.Get<ICollector>().Register(fi.FullName);

                    break;
            }

            MaintainDocuments();

        }

        public void MaintainDocuments()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var hash = KernelFactory.Instance.Get<IHash>();
            List<Document> docsToRemove = new List<Document>();

            // Possibly loop here to check for duplicate documents, copying paths into the docpaths table of one document.
            // also check for null hashes and re-hash one of the paths.
            foreach (var docu in fic.Documents.Distinct())
            {
                if (!docu.DocPaths.Any())
                {
                    docsToRemove.Add(docu);
                }
                else if (docu.DocumentHash == null)
                {
                    Console.WriteLine("Document with null hash found (possibly file was open at time of attempted hash)");
                    docu.DocumentHash = hash.HashFile(docu.DocPaths.FirstOrDefault().path);
                }
            }
            foreach (var docToRemove in docsToRemove)
            {
                fic.Documents.Remove(docToRemove);
            }
            fic.SaveChanges();
            docsToRemove.Clear();
        }
        

    }
}
