using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace LEGO.DM
{
    class audit
    {
        //The EventLog needs to be customized for each application
        private EventLog eventLog1 = new EventLog("Application", ".", "ICON.LEGO.Batch");

        /// <summary>
        /// Function to write to the audittrail
        /// </summary>
        /// <param name="sMessage">This parameter contains the message to write to the audittrail.</param> 
        /// <param name="sProjcode">This parameter contains the projcode to tag the message with.</param> 
        public void WriteToAuditTrail(string sMessage, string sProjcode)
        {
            //Execute the stored procedure that is already available to us
            //string strSQLConnString;
            //strSQLConnString = ConfigurationManager.AppSettings["SQLConn"].ToString();
            //SqlConnection conn = new SqlConnection(strSQLConnString);
            //SqlCommand cmd = new SqlCommand(@"etmtech.dbo.sp_WriteAuditTrail", conn);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add(new SqlParameter("@computer", System.Environment.MachineName.ToString()));
            //cmd.Parameters.Add(new SqlParameter("@projcode", sProjcode));
            //cmd.Parameters.Add(new SqlParameter("@siteid", "0"));
            //cmd.Parameters.Add(new SqlParameter("@userid", "0"));
            //cmd.Parameters.Add(new SqlParameter("@phoneline", "0"));
            //cmd.Parameters.Add(new SqlParameter("@description", sMessage));
            //try
            //{
            //    conn.Open();
            //    cmd.ExecuteNonQuery();
            //}
            //catch (Exception ee)
            //{
            //    eMail myMail = new eMail();
            //    myMail.SendEmail(ee.Message, "ICON.LEGO.Batch Service - An error has occurred trying to execute sp_WriteAudittrail from: " + System.Environment.MachineName);
            //    eventLog1.WriteEntry("ICON.LEGO.Batch Service has encountered an error while trying to execute sp_WriteAudittrail:" + ee.Message, EventLogEntryType.Error);
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}
        }
    }
}
