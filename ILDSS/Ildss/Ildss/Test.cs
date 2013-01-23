﻿using System;
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

            foreach (var log in logCollectionArray)
            {
                if (log.InstanceId == 4663)
                {
                    // Access attempt
                    var eventToLog = new SecurityLogEvent();
                    eventToLog.InstanceId = log.InstanceId;
                    eventToLog.UserName = log.ReplacementStrings[1];
                    eventToLog.DomainName = log.ReplacementStrings[2];
                    eventToLog.ObjectType = log.ReplacementStrings[5];
                    eventToLog.ObjectName = log.ReplacementStrings[6];
                    eventToLog.ProcessName = log.ReplacementStrings[11];
                    eventToLog.ResourceAttributes = log.ReplacementStrings[12];
                    eventToLog.TimeGenerated = log.TimeGenerated;

                    eventToLog.PrintEvent();
                }
            }

            logTimer.Stop();
            lblTimeProcessed.Text = "Processing took " + logTimer.Elapsed.TotalSeconds.ToString() + " seconds.";
        }
    }
}
