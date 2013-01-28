using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ildss
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
            
        }

        private void btnReadEvents_Click(object sender, EventArgs e)
        {
            EventLog myEvents = new EventLog("Security", System.Environment.MachineName);
            Stopwatch logTimer = new Stopwatch();
            logTimer.Start();
            lblTimeProcessed.Text = "";
            System.Diagnostics.EventLogEntryCollection logCollection = myEvents.Entries;

            EventLogEntry[] logCollectionArray = new EventLogEntry[logCollection.Count];    // Possibly compare with old log size?
            logCollection.CopyTo(logCollectionArray, 0);

            var geoff = new SecurityEventQueue();

            foreach (var log in logCollectionArray)
            {
                if (log.InstanceId == 4663)
                {
                    if (Directory.Exists(log.ReplacementStrings[6]) | log.ReplacementStrings[6].Contains("desktop.ini") | log.ReplacementStrings[6].Contains("WinSxS"))
                    {
                        //Console.WriteLine("not interested its a directory!!!!! ****************************************");
                    }
                    else
                    {
                        //Access attempt
                        var eventToLog = new SecurityLogEvent();
                        eventToLog.InstanceId = log.InstanceId;
                        eventToLog.UserName = log.ReplacementStrings[1];
                        eventToLog.DomainName = log.ReplacementStrings[2];
                        eventToLog.ObjectType = log.ReplacementStrings[5];
                        eventToLog.ObjectName = log.ReplacementStrings[6];
                        eventToLog.ProcessName = log.ReplacementStrings[11];
                        eventToLog.ResourceAttributes = log.ReplacementStrings[12];
                        eventToLog.TimeGenerated = log.TimeGenerated;
                        eventToLog.AccessMask = log.ReplacementStrings[9];

                        //eventToLog.PrintEvent();
                        switch (log.ReplacementStrings[9])
                        {
                            case "0x10000":
                                //Console.WriteLine("Delete" + log.TimeGenerated + log.ReplacementStrings[6]);
                                eventToLog.TranslatedAccessMask = "Delete";
                                break;
                            case "0x1":
                                //Console.WriteLine("Read Data (list directory if a directory)" + '\t' + log.TimeGenerated + '\t' + log.ReplacementStrings[6]);
                                eventToLog.TranslatedAccessMask = "Read Data (list directory if a directory)";
                                break;
                            case "0x6":
                                //Console.WriteLine("Write Data (or add file)" + '\t' + log.TimeGenerated + '\t' + log.ReplacementStrings[6]);
                                eventToLog.TranslatedAccessMask = "Write Data (or add file)";
                                break;
                            case "0x4":
                                //Console.WriteLine("Append data (or add subdirectory)" + '\t' + log.TimeGenerated + '\t' + log.ReplacementStrings[6]);
                                eventToLog.TranslatedAccessMask = "Append data (or add subdirectory)";
                                break;
                            default:
                                //Console.WriteLine("############ dunno bro #############");
                                break;
                        }
                        geoff.AddEvent(eventToLog);
                        //eventToLog.PrintEvent();
                        
                    }

                }
            }
            geoff.DeDuplicate();


            geoff.PrintQueue();
            logTimer.Stop();
            lblTimeProcessed.Text = "Processing took " + logTimer.Elapsed.TotalSeconds.ToString() + " seconds.";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
