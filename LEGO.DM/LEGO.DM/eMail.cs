using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace LEGO.DM
{
    class eMail
    {
        //The EventLog needs to be customized for each application
        private EventLog eventLog1 = new EventLog("Application", ".", "ICON.LEGO.Batch");

        //Get SQL String to use for all functions
        private string sMSSQLconn = ConfigurationManager.AppSettings["SQLConn"].ToString();

        //Determine if we are to output debugging information
        private bool bDebugInfo = System.Convert.ToBoolean(ConfigurationManager.AppSettings["OutputDebugInfo"].ToString());


        /// <summary>
        /// Overloaded function to send an email to someone or if no to/from defined, send to email defined in config file.
        /// </summary>
        /// <param name="sMessage">This parameter is the message that will be sent to the PM.</param> 
        /// <param name="sSubject">This parameter is the subject of the email .</param>
        /// <param name="sMailTo">This parameter will be added in the TO field of the email .</param>
        /// <param name="sMailFrom">This parameter will be added in the FROM field of the email .</param>
        /// <param name="bHTMLEmail">This parameter will define if the message in the email is HTML.</param>
        /// <param name="sCC">This parameter will be added in the CC field of the email .</param>
        /// <param name="sBCC">This parameter will be added in the BCC field of the email .</param>
        /// <param name="sServerEnv">This parameter is used to determine if the subject should be tagged with the environment.</param>
        public void SendEmail(string sMessage, string sSubject, string sMailTo, string sMailFrom, bool bHTMLEmail, string sCC, string sBCC, string sServerEnv)
        {
            //System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(sMailFrom, sMailTo, sSubject, strMessage);
            //We can not create a message with a blank "to" field, so create one with a fake address and clear it out afterwards
            MailAddress mAddressFrom = new MailAddress(sMailFrom, ConfigurationManager.AppSettings["MailDisplayName"].ToString());
            MailAddress mAddressTo = new MailAddress("homer@doh.net", ConfigurationManager.AppSettings["MailDisplayName"].ToString());
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(mAddressFrom, mAddressTo);
            message.To.Clear();

            SmtpClient emailClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPHost"].ToString());
            message.IsBodyHtml = bHTMLEmail;

            //loop through the string array and add addresses
            //The emails are ";" delimited so we use the split function and add them
            string[] sToAddresses = sMailTo.Split(';');
            foreach (string address in sToAddresses)
            {
                try
                {
                    if (address.Length > 0)
                    {
                        message.To.Add(address);
                    }
                }
                catch
                {
                    continue;
                }
            }

            //if the CC string contains data, add the addresses
            if (sCC.Length > 0)
            {
                //The emails are ";" delimited so we use the split function and add them
                string[] sCCAddresses = sCC.Split(';');
                foreach (string address in sCCAddresses)
                {
                    try
                    {
                        message.CC.Add(address);
                    }
                    catch
                    {
                        continue;
                    }

                }
            }

            //if the BCC string contains data, add the addresses
            if (sBCC.Length > 0)
            {
                //The emails are ";" delimited so we use the split function and add them
                string[] sBCCAddresses = sBCC.Split(';');
                foreach (string address in sBCCAddresses)
                {
                    try
                    {
                        message.Bcc.Add(address);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            //Add subject
            if (sServerEnv == ConfigurationManager.AppSettings["ProdIdentifier"].ToString())
            {
                message.Subject = sSubject;
            }
            else
            {
                message.Subject = @"[" + sServerEnv + "] - " + sSubject;
            }

            //Add message
            message.Body = sMessage;

            //Try sending the 
            try
            {
                emailClient.Send(message);
            }
            catch (Exception ee)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch Service has encountered an error while trying to send an email:" + ee.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Overloaded function to send email to Maint directly via SMTP
        /// </summary>
        /// <param name="sMessage">This parameter is the message that will be sent to the PM.</param> 
        /// <param name="sSubject">This parameter is the subject of the email .</param>
        public void SendEmail(string strMessage, string strSubject)
        {
            config cfg = new config();

            //Check environment we are in
            string environment = cfg.GetTableConfigSetting("SERVERTYPE");

            //If environment matches prodidentifier
            try
            {
                if (environment != cfg.GetAppConfigSettings("ProdIdentifier").ToString())
                {
                    strSubject = "[" + environment + "]" + strSubject;
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch Service has encountered an error while trying to get an appsetting value:" + ex.Message, EventLogEntryType.Error);
            }

            //No to address or from address specified.  Send to Maint Email.
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(ConfigurationManager.AppSettings["emailFrom"].ToString(),
                ConfigurationManager.AppSettings["emailTo"].ToString(), strSubject, strMessage);
            SmtpClient emailClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPHost"].ToString());
            try
            {
                emailClient.Send(message);
            }
            catch (Exception ee)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch Service has encountered an error while trying to send an email:" + ee.Message, EventLogEntryType.Error);
            }
        }
        public void alert_project_manager(string message, string subject, string project_code)
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch alert_project_manager function reached");
            }

            //Check environment we are in
            config cfg = new config();
            string environment = cfg.GetTableConfigSetting("SERVERTYPE");

            //If environment matches prodidentifier
            if (environment != cfg.GetAppConfigSettings("ProdIdentifier").ToString())
            {
                subject = "[" + environment + "]" + subject;
            }

            //Execute the stored procedure that is already available to us
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);

            //Database needs to be specified in connectioin string
            SqlCommand cmd = new SqlCommand(@"dbo.usp_alert_project_manager", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@strSubject", subject));
            cmd.Parameters.Add(new SqlParameter("@strMessage", message));
            cmd.Parameters.Add(new SqlParameter("@strProjectCode", project_code));

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                string error_message = "ICON.LEGO.Batch Service has encountered an error while trying to execute dbo.usp_alert_project_manager:";
                eMail myMail = new eMail();
                myMail.SendEmail(error_message + ee.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred on: " + System.Environment.MachineName);
                eventLog1.WriteEntry(error_message + ee.Message, EventLogEntryType.Error);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }//end finally/try

        }
    }
}
