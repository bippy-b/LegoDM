using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using LEGO.DM.LEGO_BATCH;
using System.Timers;

namespace LEGO.DM
{
    class batch
    {
        //The EventLog needs to be customized for each application
        private EventLog eventLog1 = new EventLog("Application", ".", "ICON.LEGO.Batch");

        //Get SQL String to use for all functions
        private string sMSSQLconn = ConfigurationManager.AppSettings["SQLConn"].ToString();

        //Determine if we are to output debugging information
        private bool bDebugInfo = System.Convert.ToBoolean(ConfigurationManager.AppSettings["OutputDebugInfo"].ToString());

        /// <summary>
        /// Function to lock a batch schedule so it isn't executed by another batch process or executed twice.
        /// </summary>
        public void lock_schedules()
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch PlaceLocksOnDM function reached");
            }

            //Execute the stored procedure that is already available to us
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);

            //Database needs to be specified in connectioin string
            SqlCommand cmd = new SqlCommand(@"dbo.usp_batch_lock_schedules", conn); 
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@computername", System.Environment.MachineName.ToString()));

            try
            {
                conn.Open();
                if (bDebugInfo == true)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("cmd.CommandText:" + cmd.CommandText.ToString() + Environment.NewLine);
                    sb.AppendLine("cmd.Connection:" + cmd.Connection.ConnectionString.ToString() + Environment.NewLine);
                    sb.AppendLine("cmd.CommandType:" + cmd.CommandType.ToString() + Environment.NewLine);
                    eventLog1.WriteEntry("ICON.LEGO.Batch debug before execute:" + sb.ToString());
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                string error_message = "ICON.LEGO.Batch Service has encountered an error while trying to execute dbo.usp_batch_lock_schedules:";
                eventLog1.WriteEntry(error_message + ee.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(error_message + ee.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred on: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }//end finally/try
        }

        /// <summary>
        /// Function which loops through the list of batches ready to run.
        /// </summary>
        public void process_batch_list()
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.LEGO.Batch process_batch_list function reached");
            }
            
            //Begin batch execution loop here
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);

            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                conn.Open();

                // Pass the connection to a command object
                SqlCommand cmd = new SqlCommand("dbo.usp_batch_get_locked_list", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@computername", System.Environment.MachineName.ToString()));

                // get query results
                rdr = cmd.ExecuteReader();

                if (bDebugInfo == true)
                {
                    eventLog1.WriteEntry("ICON.LEGO.Batch rows are present:" + rdr.HasRows.ToString());
                }

                // Read through the list of batch schedules that need to be executed
                while (rdr.Read())
                {
                    //If this is a web service schedule, call the web service.
                    if (rdr[4].ToString() == "WS")
                    {
                        //0 = schedule_id / 1 = projcode / 2 = database_name / 3 = config_id / 4 = batch_type
                        call_webservice(System.Convert.ToInt32(rdr[3].ToString()), rdr[1].ToString(), rdr[0].ToString(), rdr[2].ToString());
 
                    }

                    //if this is a sproc schedule, call the stored procedure
                    if (rdr[4].ToString() == "SP")
                    {
                        //0 = schedule_id / 1 = projcode / 2 = database_name / 3 = config_id / 4 = batch_type
                        call_stored_procedure(System.Convert.ToInt32(rdr[3].ToString()), rdr[1].ToString(), rdr[0].ToString(), rdr[2].ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                string error_message = "ICON.LEGO.Batch Service has encountered an error while trying to execute dbo.usp_batch_get_locked_list:";
                eventLog1.WriteEntry(error_message + ex.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(error_message + ex.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred on: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Function to remove the lock placed on a scheduled batch.
        /// </summary>
        /// <param name="schedule_id">This is the schedule_id we wish to remove from the lock table.</param>
        /// <param name="database_name">This is the database name for the schedule we are looking for.</param>
        private void remove_batch_lock(string schedule_id, string database_name)
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.Batch remove_batch_lock function reached");
            }

            //Execute the stored procedure that is already available to us
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);
            SqlCommand cmd = new SqlCommand(@"dbo.usp_batch_remove_lock", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@schedule_id", schedule_id));
            cmd.Parameters.Add(new SqlParameter("@databasename", database_name));
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                if (bDebugInfo == true)
                {
                    eventLog1.WriteEntry("ICON.Batch remove_batch_lock stored procedure executed!");
                }
            }
            catch (Exception ee)
            {                
                string error_message = "ICON.LEGO.Batch Service has encountered an error while trying to execute dbo.usp_batch_remove_lock:";
                eventLog1.WriteEntry(error_message + ee.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(error_message + ee.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred on: " + System.Environment.MachineName);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        /// <summary>
        /// Function to update the scheduled batch with a run date.
        /// </summary>
        /// <param name="schedule_id">This is the schedule_id we wish to update with the current date.</param>
        /// <param name="database_name">This is the database where the table resides.</param>
        private void update_batch_schedule(string schedule_id, string database_name, string status)
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.Batch update_batch_rundate function reached");
            }

            //Execute the stored procedure that is already available to us
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);
            SqlCommand cmd = new SqlCommand(@"dbo.usp_batch_update_schedule", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@schedule_id", schedule_id));
            cmd.Parameters.Add(new SqlParameter("@databasename", database_name));
            cmd.Parameters.Add(new SqlParameter("@status", status));
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                if (bDebugInfo == true)
                {
                    eventLog1.WriteEntry("ICON.Batch update_batch_rundate stored procedure completed.");
                }
            }
            catch (Exception ee)
            {
                if (bDebugInfo == true)
                {
                    eventLog1.WriteEntry("ICON.Batch update_batch_rundate stored procedure error has occurred.");
                }
                //Build our error message
                StringBuilder sb = new StringBuilder();
                sb.Append("Database:" + database_name);
                sb.Append(Environment.NewLine);
                sb.Append("Schedule_id:" + schedule_id);
                sb.Append(Environment.NewLine); 
                sb.Append("ICON.LEGO.Batch Service has encountered an error while trying to execute dbo.usp_batch_update_schedule:");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                eventLog1.WriteEntry(sb.ToString() + ee.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(sb.ToString() + ee.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        /// <summary>
        /// Function to call the Web Logic web service.
        /// </summary>
        /// <param name="config_id">This is the config_id we wish to run.</param>
        /// <param name="project_code">This is the project .</param>
        /// <param name="schedule_id">This is the schedule id which we are running.</param>
        /// <param name="database_name">This is the database name which holds the tables.</param>
        private void call_webservice(int config_id, string project_code, string schedule_id, string database_name)
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.Batch call_webservice function reached");
            }

            //string to capture the response
            string WSResponse = "";
            string WSError = "";

            try
            {
                //Create an instance of the web service proxy
                ScheduledTaskRunnerClient myWS = new ScheduledTaskRunnerClient();

                WSResponse = myWS.runDM(project_code, config_id);
            }
            catch (Exception ee)
            {
                WSError = ee.Message;
                //Build our error message
                StringBuilder sb = new StringBuilder();
                sb.Append("Database:" + database_name);
                sb.Append(Environment.NewLine);
                sb.Append("Schedule_id:" + schedule_id);
                sb.Append(Environment.NewLine);
                sb.Append("config_id:" + config_id.ToString());
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("ICON.LEGO.Batch Service has encountered an error while trying to call the web service:");
                sb.Append(Environment.NewLine);

                eventLog1.WriteEntry(sb.ToString() + ee.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(sb.ToString() + ee.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (WSResponse == "")
                {
                    update_batch_schedule(schedule_id, database_name, "Error when calling web service: " + WSError);
                }
                else
                {
                    update_batch_schedule(schedule_id, database_name, WSResponse);
                }
                remove_batch_lock(schedule_id, database_name);
                
            }

        }

        /// <summary>
        /// Function to call a stored procedure on a schedule.
        /// </summary>
        /// <param name="config_id">This is the config_id we wish to run.</param>
        /// <param name="project_code">This is the project .</param>
        /// <param name="schedule_id">This is the schedule id which we are running.</param>
        /// <param name="database_name">This is the database name which holds the tables.</param>
        private void call_stored_procedure(int config_id, string project_code, string schedule_id, string database_name)
        {
            //Used for live debugging since there is no GUI
            if (bDebugInfo == true)
            {
                eventLog1.WriteEntry("ICON.Batch call_stored_procedure function reached");
            }

            //begin sproc lookup here
            string strSQLConnString;
            strSQLConnString = sMSSQLconn;
            SqlConnection conn = new SqlConnection(strSQLConnString);


            //Declare a variable to hold the SQL to execute the sproc
            string strSQLSproc = "EXECUTE ";
            SqlDataReader rdr = null;
            Boolean bError = false;
            string error_text = "";

            try
            {
                //Used for live debugging since there is no GUI
                if (bDebugInfo == true)
                {
                    eventLog1.WriteEntry("ICON.Batch call_stored_procedure function first TRY reached.");
                }
                // Open the connection
                conn.Open();

                // Pass the connection to a command object
                SqlCommand cmd = new SqlCommand("SELECT stored_procedure FROM " + database_name + ".dbo.tbl_batch_sproc WHERE ID =" + config_id +" ORDER BY ID", conn);
                cmd.CommandType = CommandType.Text;
                
                // get query results
                rdr = cmd.ExecuteReader();

                //Should only be one record here
                while (rdr.Read())
                {

                    strSQLSproc = strSQLSproc + rdr[0].ToString();
                    //Used for live debugging since there is no GUI
                    if (bDebugInfo == true)
                    {
                        eventLog1.WriteEntry("Sproc schedule found.  SQL to EXECUTE is:" + strSQLSproc);
                    }

                }

                //2nd connection string for the sproc execution
                string sSQLExectutionConn = "";
                sSQLExectutionConn = sMSSQLconn;

                //2nd connection for the sproc execution
                SqlConnection SPconn = new SqlConnection(sSQLExectutionConn);
                
                //2nd cmd for sproc execution
                SqlCommand SPcmd = new SqlCommand(strSQLSproc, SPconn);
                SPcmd.CommandType = CommandType.Text;

                SqlDataReader SPrdr = null;

                try
                {
                    //Used for live debugging since there is no GUI
                    if (bDebugInfo == true)
                    {
                        eventLog1.WriteEntry("ICON.Batch call_stored_procedure function second TRY reached.");
                    }
                    // Open the connection
                    SPconn.Open();
                    //execute the sproc
                    SPrdr = SPcmd.ExecuteReader();
                }
                catch (Exception spex)
                {
                    bError = true;

                    //Build our error message
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Database:" + database_name);
                    sb.Append(Environment.NewLine);
                    sb.Append("Schedule_id:" + schedule_id);
                    sb.Append(Environment.NewLine);
                    sb.Append("config_id:" + config_id);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("ICON.LEGO.Batch Service has encountered an error while trying to execute a scheduled stored procedure:");
                    sb.Append(Environment.NewLine);

                    eventLog1.WriteEntry(sb.ToString() + spex.Message, EventLogEntryType.Error);
                    eMail myMail = new eMail();
                    myMail.SendEmail(sb.ToString() + spex.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred: " + System.Environment.MachineName);
                    
                }
                finally
                {
                    //Update the schdule to reflect the results
                    if (bError == true)
                    {
                        update_batch_schedule(schedule_id, database_name, "Error when calling stored procedure:" + error_text);
                    }
                    else
                    {
                        update_batch_schedule(schedule_id, database_name, "Success");
                    }

                    //Remove the lock on the schedule
                    remove_batch_lock(schedule_id, database_name);

                    //If connection has not been closed, close it
                    if (SPconn != null)
                    {
                        SPconn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                //Build our error message
                StringBuilder sb = new StringBuilder();
                sb.Append("Database:" + database_name);
                sb.Append(Environment.NewLine);
                sb.Append("Schedule_id:" + schedule_id);
                sb.Append(Environment.NewLine);
                sb.Append("config_id:" + config_id);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("ICON.LEGO.Batch Service has encountered an error while trying to select from dbo.tbl_batch_sproc:");
                sb.Append(Environment.NewLine);

                eventLog1.WriteEntry(sb.ToString() + ex.Message, EventLogEntryType.Error);
                eMail myMail = new eMail();
                myMail.SendEmail(sb.ToString() + ex.Message, "ICON.LEGO.Batch - Batch Service - An error has occurred on: " + System.Environment.MachineName);
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }

        }
    }
}
