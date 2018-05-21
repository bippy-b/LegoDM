using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Timers;
//using LEGO.DM;
using System.Collections;
using System.Collections.Specialized;


namespace LEGO.DM
{
    public partial class Service : ServiceBase
    {
        private System.Timers.Timer tmrMyTimer = new Timer();

        //Setup global variables to hold constants
        //private string sWebAddress;

        //private string sExportDir;
        public string sServerType;
        private bool bDebugInfo = System.Convert.ToBoolean(ConfigurationSettings.AppSettings["OutputDebugInfo"].ToString());
        private string sMSSQLconn = System.Configuration.ConfigurationSettings.AppSettings["SQLConn"].ToString();  
   
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("ICON.LEGO.Batch has been started.");
            // Get the AppSettings section.
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            //Get the check interval.
            this.tmrMyTimer.Interval = System.Convert.ToInt32(appSettings["DMInterval"].ToString());

            //Hookup the event
            this.tmrMyTimer.Elapsed +=new ElapsedEventHandler(tmrMyTimer_Elapsed);

            //Enable the timer.  Set Autoreset=true and START
            this.tmrMyTimer.Enabled = true;
            this.tmrMyTimer.AutoReset = true;
            this.tmrMyTimer.Start();

            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("tmrMyTimer.Enabled:" + this.tmrMyTimer.Enabled.ToString());
                eventLog1.WriteEntry("tmrMyTimer.Interval:" + this.tmrMyTimer.Interval.ToString());
            }

            config constants = new config();

            //Get the constant values and assign to variables
            sServerType = constants.GetTableConfigSetting("SERVERTYPE");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("ICON.LEGO.Batch has been stopped.");

            //If we stop the service we want to stop the timer
            this.tmrTimer.Stop();
        }
        protected override void  OnPause()
        {
            eventLog1.WriteEntry("ICON.LEGO.Batch has been paused.");

            //If we pause the service we need to stop the timer
            this.tmrTimer.Stop();
        }
        protected override void OnContinue()
        {
            eventLog1.WriteEntry("ICON.LEGO.Batch has continued operation.");

            //If we continue the service, we need to start the timer again
            this.tmrTimer.Start();
        }
        private void tmrMyTimer_Elapsed(object sender, EventArgs e)
        {
            //Disable the timer so that if the process takes too long there isn't double execution
            tmrTimer.Enabled = false;
            //tmrMyTimer.Stop();

            //This is used during debugging because a Windows Service does not have GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch tmrMyTimer_Elapsed event has fired.");
            }

            //Lock batch schedules so they won't be run by other instances of the batch service
            batch my_batch = new batch();
            my_batch.lock_schedules();
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch lock_schedules complete.");
            }

            //Loop through list of ready schedules
            my_batch.process_batch_list();
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch process_batch_list complete.");
            }

            //Enable the timer to prepare for the next round.
            tmrTimer.Enabled = true;
            //tmrMyTimer.Start();

            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("tmrMyTimer.Enabled:" + this.tmrMyTimer.Enabled.ToString());
                eventLog1.WriteEntry("tmrMyTimer.Interval:" + this.tmrMyTimer.Interval.ToString());
            }

        }
    }
}
