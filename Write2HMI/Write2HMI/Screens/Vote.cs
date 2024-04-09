using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Write2HMI.Screens
{
    class Vote : Screen
    {

        public Vote(DAL dal)
        {


            sDeviceNameRead = ConfigurationManager.AppSettings["vote_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["vote_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["vote_screenTriger"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["vote_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["vote_NumLines"]);
            voteWrite = int.Parse(ConfigurationManager.AppSettings["vote_voteWrite1"]);
            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];
            //השמת dal 
            screenDal = dal;
            //מעדכן ערכים בשדה נושא ודוברים
            executeQuery();
            generateShortArr();
        }
        //נושא דיון
        public string textdesc { get; set; }
        //נושא הצבעה
        public string votedesc { get; set; }
        //נושא הצבעה
        public int voteWrite { get; set; }

        #region Overrides of Screen

        public override void executeQuery()
        {
            try
            {


                textdesc = "נושא דיון: ";
                votedesc = "נושא הצבעה: ";
                if (screenDal != null)
                {
                    var vote_data = screenDal.GetPlc_vote_data();
                    if (vote_data != null)
                    {
                        textdesc += vote_data.Sess_item_dcsr;
                        votedesc += vote_data.Vote_item_dcsr;
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);

            }
        }

        public override void generateShortArr()
        {
            //טיפול במקרה אנגלית עברית וסוגריים
            textdesc = Reorder.ReorderStr(textdesc);
            //טיפול במקרה שמילה מתחילה בסוף שורה ונגמרת בתחילת שורה הבאה... הוספת רווחים
            //if (textdesc.Length > LineLength)
            //{
            textdesc = Reorder.ReorderByLineLength(textdesc, LineLength);
            //}

            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(textdesc);

            //מעבר למערך של בייטים נושא דיון
            byte[] byteArrVote;
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);

            //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
            int index = 0;
            for (int i = 0; i < (byteArr.Length / 2); i++)
            {
                //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                arrToWrite[i] = (short)(byteArr[index] + (byteArr[index + 1] * 256));
                index = index + 2;
            }
            //כתיבת נושא הצבעה
            //טיפול במקרה אנגלית עברית וסוגריים
            votedesc = Reorder.ReorderStr(votedesc);
            //טיפול במקרה שמילה מתחילה בסוף שורה ונגמרת בתחילת שורה הבאה... הוספת רווחים
            //if (votedesc.Length > LineLength)
            //{
            votedesc = Reorder.ReorderByLineLength(votedesc, LineLength);
            //}
            byteArrVote = Encoding.Default.GetBytes(votedesc);
            index = 0;
            for (int i = voteWrite; i < (voteWrite + (byteArrVote.Length / 2)); i++)
            {
                //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                arrToWrite[i] = (short)(byteArrVote[index] + (byteArrVote[index + 1] * 256));
                index = index + 2;
            }

            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;

        }

        #endregion

    }
}
