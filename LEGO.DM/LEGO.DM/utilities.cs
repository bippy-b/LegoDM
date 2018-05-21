using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration.Assemblies;

namespace LEGO.DM
{
    class config
    {
        //The EventLog needs to be customized for each application
        private EventLog eventLog1 = new EventLog("Application", ".", "ICON.LEGO.Batch");

        /// <summary>
        /// Get a config value from the table.
        /// </summary>
        /// <param name="strConfig">This parameter used to look up a constant value.</param> 
        public string GetTableConfigSetting(string strConfig)
        {
            string strSetting = "";
            //Execute the stored procedure that is already available to us
            string strSQLConnString;
            strSQLConnString = ConfigurationManager.AppSettings["SQLConn"].ToString();
            SqlConnection conn = new SqlConnection(strSQLConnString);
            SqlCommand cmd = new SqlCommand(@"dbo.usp_get_setting_value", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SettingName", strConfig));
            SqlParameter RetVal = cmd.Parameters.Add("@SettingValue", SqlDbType.VarChar, 1000);
            RetVal.Direction = ParameterDirection.Output;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                string error_message = "ICON.LEGO.Batch Service has encountered an error while trying to execute usp_GetSettingValue:";
                eventLog1.WriteEntry(error_message + ee.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(error_message + ee.Message, "ICON.LEGO.Batch Service - An error has occurred trying to execute usp_GetSettingValue from: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            strSetting = RetVal.Value.ToString();
            if (System.Convert.ToBoolean(ConfigurationManager.AppSettings["OutputDebugInfo"].ToString()) == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch setting value for " + strConfig + " = " + strSetting);
            }

            return strSetting;
        }

        /// <summary>
        /// Get a config value from the app.config file.
        /// </summary>
        /// <param name="SettingName">This parameter used to look up a setting value.</param> 
        public string GetAppConfigSettings(string SettingName)
        {
            string SettingValue = "";

            // Get the AppSettings section.
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            //Get the value
            SettingValue = appSettings[SettingName].ToString();

            return SettingValue;
        }
    }
}
