using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Write2HMI.Screens
{
    class Messages : Screen
    {
        private string msg;


        public Messages(DAL dal)
        {


            sDeviceNameRead = ConfigurationManager.AppSettings["messages_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["messages_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["messages_screenTriger"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["messages_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["messages_NumLines"]);

            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];
            //השמת dal 
            screenDal = dal;
            //מעדכן ערכים בשדה נושא ודוברים
            executeQuery();
            generateShortArr();
        }
        #region Overrides of Screen

        public override void executeQuery()
        {
            try
            {


                var md = screenDal.GetPlc_Message_data();
                if (md != null)
                {
                    msg = md.Msg_txt;
                }
                else
                {
                    msg = "";
                }
            }
            catch (Exception e)
            {

                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);

            }


        }

        public override void generateShortArr()
        {

            msg = Reorder.ReorderByLineLength(msg, LineLength);
            //טיפול במקרה אנגלית עברית וסוגריים
            msg = Reorder.ReorderStr(msg);
            //טיפול במקרה שמילה מתחילה בסוף שורה ונגמרת בתחילת שורה הבאה... הוספת רווחים
            //if (msg.Length > LineLength)
            //{

            //}
            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(msg);



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

            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;

        }

        #endregion
    }
}
