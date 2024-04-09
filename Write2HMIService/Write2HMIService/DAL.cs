using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Write2HMIService.Screens;
using System.Diagnostics;
using System.Data;

namespace Write2HMIService
{
    public class DAL
    {
        private OracleConnection _oracleConnection;
        private plc_current_data plc;
        private plc_speakers_data spk;
        private plc_results_data rst;
        private plc_kmembers_data kmb;
        private plc_message_data msg;
        private plc_vote_data pvd;
        public string VoteId { get; set; }

        public void Open()
        {

            try
            {



                if (_oracleConnection != null)
                {
                    if (_oracleConnection.State != System.Data.ConnectionState.Open)
                    {
                        var connectionString =
                            ConfigurationManager.ConnectionStrings["kneset_connection_String"].ConnectionString;
                        _oracleConnection = new OracleConnection(connectionString);
                        _oracleConnection.Open();
                    }
                }
                else
                {
                    var connectionString =
                        ConfigurationManager.ConnectionStrings["kneset_connection_String"].ConnectionString;
                    _oracleConnection = new OracleConnection(connectionString);
                    _oracleConnection.Open();
                }
                Logger.WriteEventLog("Connected to DB ", EventLogEntryType.Information);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        public ConnectionState GetConnectionState()
        {
            if (_oracleConnection != null)
            {
                return _oracleConnection.State;
            }
            return ConnectionState.Broken;
        }

        public void Close()
        {
            _oracleConnection.Close();
        }

        public plc_current_data GetPlc_current_data()
        {
            plc = null;
            try
            {


                string sql = "select * from plc_crnt_data   where dummy_key=1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    plc = new plc_current_data();
                    plc.Sess_item_dcsr = reader["SESS_ITEM_DSCR"].ToString();
                    plc.Kneset_num = (reader["KNESSET_NUM"].ToString());
                    plc.Session_id = (reader["SESSION_ID"].ToString());
                    plc.Sess_item_nbr = (reader["SESS_ITEM_NBR"].ToString());
                    //plc.VoteItemId = (reader["VOTE_ITEM_ID"].ToString());
                }
                reader.Close();
                cmd.Dispose();
                return plc;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return plc;


            }
        }

        public plc_vote_data GetPlc_vote_data()
        {
            pvd = null;
            try
            {
                string sql = "select * from plc_crnt_data   where dummy_key=1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pvd = new plc_vote_data();
                    pvd.Sess_item_dcsr = reader["SESS_ITEM_DSCR"].ToString();
                    pvd.Vote_item_dcsr = reader["VOTE_ITEM_DSCR"].ToString();
                }
                reader.Close();
                cmd.Dispose();
                return pvd;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return pvd;

            }
        }

        public plc_speakers_data GetPlc_speakers_data()
        {
            spk = null;
            try
            {
                string SpeakerSeq = "0";
                string sql = " select * FROM SPEAKERS";
                sql += " WHERE SESSION_ID = '" + plc.Session_id.ToString() + "'";
                sql += " AND SESS_ITEM_NBR ='" + plc.Sess_item_nbr.ToString() + "'";
                sql += " AND IS_CURRENT = 1 ";
                sql += "  ORDER BY SPEECH_SEQ ASC";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                var first_speaker = true;
                if (reader.Read())
                {
                    SpeakerSeq = reader["SPEECH_SEQ"].ToString();
                }
                reader.Close();
                sql = " select * FROM SPEAKERS";
                sql += " WHERE SESSION_ID = '" + plc.Session_id.ToString() + "'";
                sql += " AND SESS_ITEM_NBR ='" + plc.Sess_item_nbr.ToString() + "'";
                sql += " AND SPEECH_SEQ >=  " + SpeakerSeq;
                sql += "  ORDER BY SPEECH_SEQ ASC";
                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (first_speaker)
                    {
                        spk = new plc_speakers_data();
                        spk.Speakers = new List<string>();
                        first_speaker = false;
                    }
                    spk.Speakers.Add(reader["SPEAKER_NAME"].ToString());
                }
                reader.Close();
                cmd.Dispose();
                return spk;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return spk;

            }
        }

        public plc_results_data GetPlc_results_data()
        {
            try
            {
                if (!string.IsNullOrEmpty(VoteId))
                {


                    rst = null;


                    //כותרת תוצאות לפי מספר הצבעה
                    string sqlTotalResult = "select * FROM VOTE_RSLTS_HDR  WHERE vote_id = " + VoteId; //+    "' order by vote_nbr_in_sess desc";
                    //knesset_num='" + plc.Kneset_num.ToString() +
                    //  "' and session_id='" + plc.Session_id.ToString() +



                    var cmd = new OracleCommand(sqlTotalResult, _oracleConnection);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        rst = new plc_results_data();
                        rst.voteID = int.Parse(reader["VOTE_ID"].ToString());
                        rst.voteNbrInSess = int.Parse(reader["VOTE_NBR_IN_SESS"].ToString());
                        rst.resultsFor = int.Parse(reader["TOTAL_FOR"].ToString());
                        rst.resultsAgainst = int.Parse(reader["TOTAL_AGAINST"].ToString());
                        rst.resultsAvoid = int.Parse(reader["TOTAL_ABSTAIN"].ToString());

                    }

                    reader.Close();
                    //תוצאות הצבעת החכים לפי מספר הצבעה
                    string sqlKmmbersResult = " select * FROM VOTE_RSLTS_KMMBR WHERE VOTE_ID = '" + VoteId +
                                              "' ORDER BY KMMBR_NAME";

                    cmd.CommandText = sqlKmmbersResult;
                    reader = cmd.ExecuteReader();
                    rst.MembersFor = new List<string>();
                    rst.MembersAgainst = new List<string>();
                    rst.MembersAvoid = new List<string>();
                    while (reader.Read())
                    {
                        switch (reader["VOTE_RESULT"].ToString())
                        {
                            case "1":
                                //first for
                                rst.MembersFor.Add(reader["KMMBR_NAME"].ToString());
                                break;
                            case "2":
                                //first against
                                rst.MembersAgainst.Add(reader["KMMBR_NAME"].ToString());
                                break;
                            case "3":
                                //first avoid
                                rst.MembersAvoid.Add(reader["KMMBR_NAME"].ToString());
                                break;
                        }
                    }

                    reader.Close();
                    cmd.Dispose();
                    return rst;
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        public plc_kmembers_data GetPlc_Kmembers_data()
        {
            try
            {
                kmb = null;

                //שליפת הכנסת הנוכחית
                string sqlGetKneset = "select * FROM PLC_CRNT_DATA WHERE DUMMY_KEY =1";

                var cmd = new OracleCommand(sqlGetKneset, _oracleConnection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    kmb = new plc_kmembers_data();
                    kmb.knesetNum = int.Parse(reader["KNESSET_NUM"].ToString());
                }

                reader.Close();
                //תוצאות הצבעת החכים לפי מספר הצבעה
                string sqlKmmbersData = "select * FROM KNESSET_MEMBERS WHERE KNESSET_NUM = '" + kmb.knesetNum.ToString() +
                                        "' AND IS_ACTIVE = 1";

                cmd.CommandText = sqlKmmbersData;
                reader = cmd.ExecuteReader();
                kmb.kmembers = new List<kmember>();
                kmember currKmember;
                while (reader.Read())
                {
                    currKmember = new kmember();
                    currKmember.kmember_id = int.Parse(reader["KMMBR_ID"].ToString());
                    currKmember.kmember_fName = reader["FIRST_NAME"].ToString();
                    currKmember.kmember_lName = reader["LAST_NAME"].ToString();
                    currKmember.kmember_IP = reader["IP_ADDRESS"].ToString();
                    kmb.kmembers.Add(currKmember);

                }

                reader.Close();
                cmd.Dispose();
                return kmb;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        public plc_message_data GetPlc_Message_data()
        {
            msg = null;
            try
            {
                string sql = "select * FROM MESSAGES WHERE MSG_TYPE =1 ";

                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    msg = new plc_message_data();
                    msg.Msg_txt = reader["MSG_TXT"].ToString();


                }
                reader.Close();
                cmd.Dispose();
                return msg;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }


        internal string CheckMaliaaSignal()
        {
            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var value = cmd.ExecuteScalar();
                if (value != null)
                {
                    return value.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        internal string CheckResultsSignal()
        {
            try
            {
                string sql = "select vote_id from plc_crnt_data p, vote_rslts_hdr v " +
                    "where p.dummy_key=1 and p.session_id = v.session_id and vote_time is not null order by vote_time desc";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var value = reader["VOTE_ID"].ToString();
                    this.VoteId = value;
                    return value.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        internal string CheckSpeakersSignal()
        {
            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var value = cmd.ExecuteScalar();
                if (value != null)
                {
                    return value.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        internal string CheckVoteSignal()
        {
            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var value = cmd.ExecuteScalar();
                if (value != null)
                {
                    return value.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

        internal string CheckMessageSignal()
        {
            try
            {
                string sql = "select msg_txt FROM MESSAGES WHERE MSG_TYPE =1";
                var cmd = new OracleCommand(sql, _oracleConnection);
                var value = cmd.ExecuteScalar();
                if (value != null)
                {
                    return value.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
        }

    }

    public class plc_current_data
    {
        public string Sess_item_dcsr { get; set; }
        public string Kneset_num { get; set; }
        public string Session_id { get; set; }
        public string Sess_item_nbr { get; set; }

        //public string VoteItemId { get; set; }
    }

    public class plc_vote_data
    {
        public string Sess_item_dcsr { get; set; }
        public string Vote_item_dcsr { get; set; }

    }

    public class plc_speakers_data
    {
        public List<string> Speakers { get; set; }

    }

    public class plc_results_data
    {
        public int voteNbrInSess { get; set; }
        public int voteID { get; set; }
        public int resultsFor { get; set; }
        public int resultsAgainst { get; set; }
        public int resultsAvoid { get; set; }
        public List<string> MembersFor { get; set; }
        public List<string> MembersAgainst { get; set; }
        public List<string> MembersAvoid { get; set; }

    }

    public class plc_kmembers_data
    {
        public int knesetNum { get; set; }
        public List<kmember> kmembers { get; set; }
    }

    public class kmember
    {
        public int kmember_id { get; set; }
        public string kmember_fName { get; set; }
        public string kmember_lName { get; set; }
        public string kmember_IP { get; set; }
        public int threadId { get; set; }




    }

    public class plc_message_data
    {
        public string Msg_txt { get; set; }
    }
}

