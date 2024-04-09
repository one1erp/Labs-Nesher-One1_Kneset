using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kneset_terminal;
using Oracle.DataAccess.Client;
using System.Diagnostics;

namespace Kneset_Terminal
{
    class DAL
    {
        private OracleConnection _oracleConnection;
        public string RefreshSignal { get; set; }

        public void Open()
        {

            try
            {


                if (_oracleConnection != null)
                {
                    if (_oracleConnection.State != System.Data.ConnectionState.Open)
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings["kneset_connection_String"].ConnectionString;
                        _oracleConnection = new OracleConnection(connectionString);
                        _oracleConnection.Open();
                    }
                }
                else
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["kneset_connection_String"].ConnectionString;
                    _oracleConnection = new OracleConnection(connectionString);
                    _oracleConnection.Open();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }
        public void Close()
        {
            if (_oracleConnection != null) _oracleConnection.Close();
        }

        public List<SessionItem> GetSessionItems()
        {
            try
            {
                SessionItem firstsessionItem = null;
                List<SessionItem> sessionItemslList = new List<SessionItem>();


                string sql = "select plc.session_id, si.seq_in_session ,si.SESS_ITEM_DSCR ,si.SESS_ITEM_ID from plc_crnt_data plc,session_items si where plc.session_id= si.session_id and sess_item_stat=2";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    firstsessionItem = new SessionItem();
                    firstsessionItem.SESS_ITEM_DSCR = reader["SESS_ITEM_DSCR"].ToString();
                    firstsessionItem.SESSION_ID = reader["SESSION_ID"].ToString();
                    firstsessionItem.SESS_ITEM_ID = reader["SESS_ITEM_ID"].ToString();
                    firstsessionItem.SEQ_IN_SESSION = reader["SEQ_IN_SESSION"].ToString();
                    sessionItemslList.Add(firstsessionItem);
                }
                reader.Close();
                cmd.Dispose();
                sql = "select * from session_items WHERE session_id=" + firstsessionItem.SESSION_ID +
                     " and seq_in_session > " + firstsessionItem.SEQ_IN_SESSION + " and sess_item_stat=1 order by seq_in_session";


                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SessionItem sessionItem = new SessionItem();
                    sessionItem.SESS_ITEM_DSCR = reader["SESS_ITEM_DSCR"].ToString();
                    sessionItem.SESSION_ID = reader["SESSION_ID"].ToString();
                    sessionItem.SESS_ITEM_ID = reader["SESS_ITEM_ID"].ToString();
                    sessionItem.SMRY_DATA = reader["SMRY_DATA"].ToString();

                    sessionItemslList.Add(sessionItem);
                }
                reader.Close();
                cmd.Dispose();
                return sessionItemslList;
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        public bool CheckSignal()
        {
            OracleCommand cmd = null;
            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data";
                cmd = new OracleCommand(sql, _oracleConnection);
                var val = cmd.ExecuteScalar();
                string newValue = val.ToString();
                if (newValue != RefreshSignal)
                {
                    RefreshSignal = newValue;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
                return false;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }
    }

    public class SessionItem
    {
        [DisplayName("סדר יום")]
        public string SESS_ITEM_DSCR { get; set; }

        //    public string SMRY_DATA { get; set; }

        [Browsable(false)]
        public string SESSION_ID { get; set; }

        [Browsable(false)]
        public string SESS_ITEM_ID { get; set; }

        [Browsable(false)]
        public string SEQ_IN_SESSION { get; set; }

        //[DisplayName("מגיש ההצעה")]
        //public string MyProperty { get; set; }
        //[DisplayName("כותרת")]
        //public string MyProperty1 { get; set; }
        //[DisplayName("תאריך")]
        //public string MyProperty11 { get; set; }



        public string SMRY_DATA { get; set; }
    }


}
