using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ildss
{
    public class EventQueue : IEventQueue
    {
        private List<DocEvent> evQueue = new List<DocEvent>();
        private List<DocEvent> evUsed = new List<DocEvent>();

        public void AddEvent(DocEvent de)
        {
            evQueue.Add(de);
            if (evQueue.Count() == 4)
            {
                PrintEvents();
                EventQueueToDb();
            }
        }

        public void PrintEvents()
        {
            foreach (DocEvent ev in evQueue)
            {
                Console.WriteLine(ev.Document.DocumentId + "    " + ev.type);
            }
        }

        public void EventQueueToDb()
        {
            foreach (DocEvent ev in evQueue)
            {
                var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                fic.DocEvents.Add(ev);
                fic.SaveChanges();
                evUsed.Add(ev);
            }

            foreach (DocEvent ev in evUsed)
            {
                evQueue.Remove(ev);
            }
        }

        private void RemoveExtraFiles()
        {

        }


        private bool IsOfficeFile(string name)
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
