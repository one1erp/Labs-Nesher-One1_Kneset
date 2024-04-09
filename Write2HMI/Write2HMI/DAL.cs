using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Write2HMI.Screens;
using System.Diagnostics;
using System.Data;

namespace Write2HMI
{
    public class DAL
    {
        private OracleConnection _oracleConnection;
        private plc_current_data plc;
        private plc_speakers_data spk;
        private plc_results_data rst;
        private plc_kmembers_data kmb;
        private plc_message_data msg;
        private plc_personalMessage_data pmsg;
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
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            plc = null;
            try
            {


                string sql = "select * from plc_crnt_data   where dummy_key=1";
                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
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
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        public plc_vote_data GetPlc_vote_data()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            pvd = null;
            try
            {
                string sql = "select * from plc_crnt_data   where dummy_key=1";
                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
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

            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        public plc_speakers_data GetPlc_speakers_data()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            spk = null;
            try
            {
                string SpeakerSeq = "0";
                string sql = " select /*aabbcc*/ * FROM SPEAKERS";
                sql += " WHERE SESSION_ID = '" + plc.Session_id.ToString() + "'";
                sql += " AND SESS_ITEM_NBR ='" + plc.Sess_item_nbr.ToString() + "'";
                sql += " AND IS_CURRENT = 1 ";
                sql += "  ORDER BY SPEECH_SEQ ASC";
                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                var first_speaker = true;
                if (reader.Read())
                {
                    SpeakerSeq = reader["SPEECH_SEQ"].ToString();
                }
                reader.Close();
                cmd.Dispose();

                if (plc != null)
                {
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
                }
                return spk;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return spk;

            }
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }

        }

        public plc_results_data GetPlc_results_data()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            try
            {
                if (!string.IsNullOrEmpty(VoteId))
                {


                    rst = null;


                    //כותרת תוצאות לפי מספר הצבעה
                    string sqlTotalResult = "select * FROM VOTE_RSLTS_HDR  WHERE vote_id = " + VoteId; //+    "' order by vote_nbr_in_sess desc";
                    //knesset_num='" + plc.Kneset_num.ToString() +
                    //  "' and session_id='" + plc.Session_id.ToString() +



                    cmd = new OracleCommand(sqlTotalResult, _oracleConnection);
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        rst = new plc_results_data();
                        rst.voteID = int.Parse(reader["VOTE_ID"].ToString());
                        rst.voteNbrInSess = int.Parse(reader["VOTE_NBR_IN_SESS"].ToString());
                        rst.resultsFor = int.Parse(reader["TOTAL_FOR"].ToString());
                        rst.resultsAgainst = int.Parse(reader["TOTAL_AGAINST"].ToString());
                        rst.resultsAvoid = int.Parse(reader["TOTAL_ABSTAIN"].ToString());
                        rst.sessItemDscr = reader["SESS_ITEM_DSCR"].ToString();

                    }

                    reader.Close();
                    cmd.Dispose();
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
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        public plc_kmembers_data GetPlc_Kmembers_data()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            try
            {
                kmb = null;

                //שליפת הכנסת הנוכחית
                string sqlGetKneset = "select * FROM PLC_CRNT_DATA WHERE DUMMY_KEY =1";

                cmd = new OracleCommand(sqlGetKneset, _oracleConnection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    kmb = new plc_kmembers_data();
                    kmb.knesetNum = int.Parse(reader["KNESSET_NUM"].ToString());
                }

                reader.Close();
                cmd.Dispose();
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
                    var pcode = reader["PRIVATE_CODE"].ToString();
                    if (string.IsNullOrEmpty(pcode))
                    {
                        pcode = "0000";
                    }
                    currKmember.kmember_code = int.Parse(pcode);
                    currKmember.kmember_In_Building = int.Parse(reader["IS_IN_BUILDING"].ToString());
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
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        public plc_message_data GetPlc_Message_data()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            msg = null;
            try
            {
                string sql = "select * FROM MESSAGES WHERE MSG_TYPE =1 ";

                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    msg = new plc_message_data();
                    msg.Msg_txt = reader["MSG_TXT"].ToString();


                }
                reader.Close();
                return msg;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }



        public plc_personalMessage_data GetPlc_PersonalMessage_data(int kmbId)
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            pmsg = null;
            try
            {

                string sql = "select MSG$KMMBR_ID,MSG$TEXT,MSG$SEND_NAME,MSG$TIME_READ,"
                         + "TO_CHAR(MSG$TIME_DONE,'DD/MM/YYYY HH24:MI:SS') MSG$TIME_DONE,"
                         + "ROWIDTOCHAR(Xrowid) Xrowid "
                         + " FROM SANP_VU_MESSAGE "
                         + " WHERE MSG$KMMBR_ID='" + kmbId .ToString()+ "' "
                         + " and MSG$TIME_READ is null "
                         + " ORDER BY MSG$TIME_DONE DESC";


                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                pmsg = new plc_personalMessage_data();
              
           
                if (reader.Read())
                {

                    pmsg.Msg_txt = reader["MSG$TEXT"].ToString();
                    var sent_by = reader["MSG$SEND_NAME"].ToString();
                    if (string.IsNullOrEmpty(sent_by))
                    {
                        sent_by = "בעילום שם";
                    }
                    pmsg.Msg_Sent_by = sent_by;
                    var sent_on = reader["MSG$TIME_DONE"].ToString();
                    if (string.IsNullOrEmpty(sent_on))
                    {
                        sent_on = "00:00:00 00/00/0000";
                    }
                    pmsg.Msg_sent_on = sent_on;
                }
                reader.Close();
                return pmsg;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        internal string CheckMaliaaSignal()
        {
            OracleCommand cmd = null;
            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                cmd = new OracleCommand(sql, _oracleConnection);
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
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }

        internal string CheckResultsSignal()
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            try
            {
                string sql = "select vote_id from plc_crnt_data p, vote_rslts_hdr v " +
                    "where p.dummy_key=1 and p.session_id = v.session_id and p.sess_item_nbr=v.sess_item_nbr and vote_time is not null  order by vote_time desc";
                cmd = new OracleCommand(sql, _oracleConnection);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var value = reader["VOTE_ID"].ToString();
                    this.VoteId = value;
                    return value.ToString();
                }


                //איפוס מספר ההצבעה עבור השליפה
                this.VoteId = null;
                return null;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
            finally
            {

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }
        }

        internal string CheckSpeakersSignal()
        {
            OracleCommand cmd = null;

            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                cmd = new OracleCommand(sql, _oracleConnection);
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
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }

        internal string CheckVoteSignal()
        {
            OracleCommand cmd = null;

            try
            {
                string sql = "select sess_item_rfrsh_nbr from plc_crnt_data   where dummy_key=1";
                cmd = new OracleCommand(sql, _oracleConnection);
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
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }

        internal string CheckMessageSignal()
        {
            OracleCommand cmd = null;

            try
            {
                string sql = "select msg_txt FROM MESSAGES WHERE MSG_TYPE =1";
                cmd = new OracleCommand(sql, _oracleConnection);
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
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }
        internal string CheckPersonalMessageSignal(List<kmember> lst )
        {
            OracleCommand cmd = null;
            OracleDataReader reader = null;
            try
            {
                //שרשור כל הID לצורך שאילתא
                string kmbIds="";
                foreach (var kmb in lst)
                {
                    kmb.hasNewMessage = false;
                    kmbIds += "'" + kmb.kmember_id + "',";
                }
                //להוריד את הפסיק האחרון
                if (kmbIds != "")
                {
                    kmbIds = kmbIds.Substring(0, kmbIds.Length - 1);



                    //מעדכנת לרשימת חכים האם יש להם הודעה חדשה 
                    //שדה HASNEWMESSAGE
                    string sql = "select MSG$KMMBR_ID,MSG$TEXT,MSG$SEND_NAME,MSG$TIME_READ,"
                                 + "TO_CHAR(MSG$TIME_DONE,'DD/MM/YYYY HH24:MI:SS') MSG$TIME_DONE,"
                                 + "ROWIDTOCHAR(Xrowid) Xrowid "
                                 + " FROM SANP_VU_MESSAGE "
                                 + " WHERE MSG$KMMBR_ID in (" + kmbIds + ") "
                                 + " and MSG$TIME_READ is null "
                                 + " ORDER BY MSG$TIME_DONE DESC";

                    cmd = new OracleCommand(sql, _oracleConnection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var tempkmb = lst.FirstOrDefault(x => x.kmember_id.ToString() == reader["MSG$KMMBR_ID"].ToString());
                        if (tempkmb != null) tempkmb.hasNewMessage = true;
                    }
                    reader.Close();
                }
                return lst.Where(x=>x.hasNewMessage).Count().ToString();
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return null;

            }
            finally
            { if(reader !=null) reader.Close();
                if (cmd != null) cmd.Dispose();
            }
        }

        public void updatePlc_PersonalMessage_data(int kmemberID, plc_personalMessage_data pmsg)
        {
 

             OracleCommand cmd = null;
  
  
            try
            {
    //מעדכן תאריך שההודעה נקראה
                string sql = "UPDATE sanp_vu_message "+
                         " SET MSG$TIME_READ = TO_DATE('" + DateTime.Now.ToString() +"','DD/MM/YYYY HH24:MI:SS')" +
                         " WHERE MSG$TEXT='" + pmsg.Msg_txt + "'"+
                         " and MSG$SEND_NAME= '"+ pmsg.Msg_Sent_by+"'";


                cmd = new OracleCommand(sql, _oracleConnection);
                cmd.ExecuteNonQuery();

          
                cmd.Dispose();
                return ;
            }
            catch (Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);
                return ;

            }

            finally
            {

          
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
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
        public string sessItemDscr { get; set; }

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
        public int kmember_code { get; set; }
        public int kmember_In_Building { get; set; }
        public bool hasNewMessage { get; set; }

    }

    public class plc_message_data
    {
        public string Msg_txt { get; set; }

    }



    public class plc_personalMessage_data
    {
        public string Msg_txt { get; set; }
        public string Msg_Sent_by { get; set; }
        public string Msg_sent_on { get; set; }
    }
}

