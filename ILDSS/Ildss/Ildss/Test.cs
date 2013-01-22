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
    int i = 0;
    int nEvents = Convert.ToInt32(txtNumEvents.Text);
 
    EventLog myEvents = new EventLog("Security", "MaxDell");
    Stopwatch logTimer = new Stopwatch();
    logTimer.Start();
    lblTimeProcessed.Text = "";
 
    foreach (System.Diagnostics.EventLogEntry entry in myEvents.Entries)
    {
        if (i < nEvents)
        {
            // Add event fields to the listbox, separated by tabspace (\t).
            if (entry.InstanceId == 4656)
            {
                listBox1.Items.Add(entry.InstanceId
                                    + "\t" + entry.EntryType
                                    + "\t" + entry.TimeGenerated
                                    //+ "\t" + (entry.Data.ToList().ElementAt(0))
                                    );
                foreach (var d in entry.ReplacementStrings)
                {
                    if(File.Exists(d.ToString()) && d.ToString().StartsWith(@"F:\TestDir")){
                    //void* f = &d;
                    //d.ToString();
                    Console.WriteLine("data: " + entry.EntryType + " -- " + entry.TimeGenerated + " -- " + d.ToString());}
                }
                // Increment control for loop, or it will pull the whole log.
            }
            i++;
        }
        else
        {
            // Break out of loop when i = nEvents.
            break;
        }
    }
    logTimer.Stop();
    lblTimeProcessed.Text = "Processing took " + logTimer.Elapsed.TotalSeconds.ToString() + " seconds.";
        }
    }
}
