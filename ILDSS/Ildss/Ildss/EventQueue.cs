using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ildss
{
    public static class EventQueue
    {
        private static List<DocEvent> evQueue = new List<DocEvent>();

        public static void AddEvent(DocEvent de)
        {
            evQueue.Add(de);
            EventQueueToDb(de);
        }

        public static void PrintEvents()
        {
            foreach (DocEvent ev in evQueue)
            {
                Console.WriteLine(ev.name + " was " + ev.type);
            }
        }


        public static void DetectOfficeFiles()
        {
            // look for the patterns associated with office file creation/save/rename
            /*
            if (lastRenamedFile.Contains(".tmp"))
            {
                Console.WriteLine("yes bro");
                if (lastRenamedPath == pattern.EventArgs.FullPath)
                {
                    // its a bleeding office file change!!!!
                    Console.WriteLine(pattern.EventArgs.Name + " has been updated");
                }
            }
            else
            {
                Console.WriteLine("nah bro " + lastRenamedFile);
                var dave = ((FileSystemWatcher)pattern.Sender);
            }

            lastRenamedPath = pattern.EventArgs.OldFullPath;
            lastRenamedFile = pattern.EventArgs.Name;
            */
                    }

        public static void EventQueueToDb(DocEvent de)
        {
            try
            {
                //try and put shit in db
                Console.WriteLine("event in db now");
            }
            catch (Exception e)
            {
                // carry on with next one
            }
            finally
            {
                // done
            }
        }


        private static bool IsOfficeFile(string name)
        {
            //check to see if from office
            if (name.Contains(".tmp") | !name.Contains("."))
                return true;
            else
                return false;
        }


        /* GRAVEYARD
        public void RegisterEvent(FileSystemEventArgs e)
        {
            var t = DateTime.Now;
            var time = t.AddTicks(-(t.Ticks % TimeSpan.TicksPerSecond));

            Thread.Sleep(2000);

            using (FileIndexContainer fic = new FileIndexContainer())
            {
                var docPaths = from docpaths in fic.DocPaths
                               where docpaths.path == e.FullPath
                               select docpaths;

                var docPathDefault = docPaths.FirstOrDefault();

                var doc = from documents in fic.Documents
                          where documents.DocumentHash == docPathDefault.DocumentDocumentHash
                          select documents;

                var ev = from events in fic.DocEvents
                         where events.date_time.CompareTo(time) == 0
                         select events;

                if (docPathDefault != null)
                {

                    // Event doesn't exist (events have unique times)
                    if (ev.Count() == 0)
                    {
                        DocEvent de = new DocEvent()
                        {
                            date_time = time,
                            type = e.ChangeType.ToString(),
                            DocumentDocumentHash = docPathDefault.Document.DocumentHash
                        };
                        fic.DocEvents.Add(de);
                        fic.SaveChanges();
                    }
                }
            }

        }*/
    }
}
