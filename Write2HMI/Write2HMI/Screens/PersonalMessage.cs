using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Write2HMI.Screens
{
    class PersonalMessage:Screen
    {
          
     
        //רגיסטר לרשום את הקוד האישי של הח"כ על מנת שיוכל לקרוא את ההודעה
        private int screenRegister { get; set; }
        private int _kmember_id { get; set; }
        private int _kmember_personal_code { get; set; }
        private int Sent_by_Max{ get; set; }
        private int Sent_on_lenght { get; set; }
        public plc_personalMessage_data personalmessageData { get; set; }
        public PersonalMessage(DAL dal, int kmember_id, int kmember_personal_code)
        {
            //אורך תוים של תאריך שליחת הודעה 
            Sent_on_lenght = int.Parse(ConfigurationManager.AppSettings["personalMessage_Sent_on_lenght"]);

            sDeviceNameRead = ConfigurationManager.AppSettings["personalMessage_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["personalMessage_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["personalMessage_screenTriger"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["personalMessage_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["personalMessage_NumLines"]);
            screenRegister = int.Parse(ConfigurationManager.AppSettings["personalMessage_screenRegister"]);
            _kmember_id = kmember_id;
            _kmember_personal_code = kmember_personal_code;

            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];

            //5 מספר התווים של פתיח
            //            "מאת: "
            Sent_by_Max = LineLength - 5 - Sent_on_lenght;
            //השמת dal 
            screenDal = dal;
           
            executeQuery();
            generateShortArr();
        }
        #region Overrides of Screen

        public override void executeQuery()
        {
            try
            {

                personalmessageData = screenDal.GetPlc_PersonalMessage_data(_kmember_id);
             
            }
            catch (Exception e)
            {

                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);

            }
        }

        public override void generateShortArr()
        {
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);
           
            //שורה ראשונה כתיבת מאת + שם השולח
            string txt = "מאת: ";
            if(personalmessageData.Msg_Sent_by.Length>Sent_by_Max)
            {
                txt= txt + personalmessageData.Msg_Sent_by.Substring(0, Sent_by_Max);
            }
            else
            {
                txt = txt + personalmessageData.Msg_Sent_by;
            }

            //טיפול במקרה אנגלית עברית וסוגריים
            txt = Reorder.ReorderStr(txt);

            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(txt);

            //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
            int index = 0;
            for (int i = 0; i < (byteArr.Length / 2); i++)
            {
                //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                arrToWrite[i] = (short)(byteArr[index] + (byteArr[index + 1] * 256));
                index = index + 2;
            }
            //----------------------------------------------------------
            //כתיבת התאריך בסוף השורה הראשונה 
            txt = personalmessageData.Msg_sent_on;

            //טיפול במקרה אנגלית עברית וסוגריים
            txt = Reorder.ReorderStr(txt);

            //מעבר למערך של בייטים
            byteArr = Encoding.Default.GetBytes(txt);

            //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
             index = 0;
            int sent_on_position = (LineLength - Sent_on_lenght)/2;
            for (int i = sent_on_position; i < (sent_on_position+(byteArr.Length / 2)); i++)
            {
                //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                arrToWrite[i] = (short)(byteArr[index] + (byteArr[index + 1] * 256));
                index = index + 2;
            }
            
            
            //שורה שניה....כתיבת ההודעה
                string msg; 
                if (personalmessageData.Msg_txt != null)
                {
                    msg = personalmessageData.Msg_txt;
                }
                else
                {
                    msg = "";
                }
                //טיפול במקרה שמילה מתחילה בסוף שורה ונגמרת בתחילת שורה הבאה... הוספת רווחים
                //if ( textdesc.Length > LineLength)
                //{
                msg = Reorder.ReorderByLineLength(msg, LineLength);
                //}
                //טיפול במקרה אנגלית עברית וסוגריים
                msg = Reorder.ReorderStr(msg);
         
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(msg);

                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
               index = 0;
                for (int i =( LineLength/2); i < ((LineLength/2)+(byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[index] + (byteArr[index + 1] * 256));
                    index = index + 2;
                }
          //אבי אמר לא לכתוב כרגע את הקוד האישי של החכ לרגיסטר
            //כתיבת קוד הח"כ לרגיסטר
           // arrToWrite[screenRegister]= (short)_kmember_personal_code;
            //------------------------


            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;
        }
       
          public  void updateQuery()
        {
            try
            {

                screenDal.updatePlc_PersonalMessage_data(_kmember_id, personalmessageData);
             
            }
            catch (Exception e)
            {

                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);

            }
        }
        #endregion
    }
}
