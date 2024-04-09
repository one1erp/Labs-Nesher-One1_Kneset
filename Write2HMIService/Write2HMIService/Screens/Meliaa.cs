using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;

namespace Write2HMIService.Screens
{
    class Meliaa : Screen
    {
        public Meliaa(DAL dal)
        {
            textdesc = "";

            sDeviceNameRead = ConfigurationManager.AppSettings["meliaa_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["meliaa_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["meliaa_screenTriger"]);
            speakersWrite1 = int.Parse(ConfigurationManager.AppSettings["meliaa_speakersWrite1"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["meliaa_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["meliaa_NumLines"]);

            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];
            //השמת dal 
            screenDal = dal;
            //מעדכן ערכים בשדה נושא ודוברים
            executeQuery();
            generateShortArr();
        }
        //נושא במליאה
        public string textdesc { get; set; }
        //3 הדוברים הראשונים
        public List<string> textSpeakers { get; set; }
        //אינדקס דובר1
        public int speakersWrite1 { get; set; }

        public override void executeQuery()
        {
            textSpeakers = new List<string>();
            textdesc = screenDal.GetPlc_current_data().Sess_item_dcsr;
            var speakersData=screenDal.GetPlc_speakers_data();
            if (speakersData != null)
            {


                var tempSpeakers = speakersData.Speakers;
                if (tempSpeakers != null)
                {
                    var countSpeakers = 3;
                    if (tempSpeakers.Count() < 3)
                    {
                        countSpeakers = tempSpeakers.Count();
                    }
                    textSpeakers = screenDal.GetPlc_speakers_data().Speakers.GetRange(0, countSpeakers);
                }
            }
        }

        public override void generateShortArr()
        {
            //טיפול במקרה אנגלית עברית וסוגריים
            textdesc = Reorder.ReorderStr(textdesc);
            //טיפול במקרה שמילה מתחילה בסוף שורה ונגמרת בתחילת שורה הבאה... הוספת רווחים
            if ( textdesc.Length > LineLength)
            {
                textdesc = Reorder.ReorderByLineLength(textdesc, LineLength);
            }

            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(textdesc);
            //מעבר למערך של בייטים דוברים
            byte[] byteArrspeaker;

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

            foreach (var item in textSpeakers)
            {
                //טיפול במקרה אנגלית עברית וסוגריים
                var curritem = Reorder.ReorderStr(item);
                byteArrspeaker = Encoding.Default.GetBytes(curritem);
                index = 0;
                for (int i = speakersWrite1; i < (speakersWrite1 + (byteArrspeaker.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArrspeaker[index] + (byteArrspeaker[index + 1] * 256));
                    index = index + 2;
                }
                speakersWrite1 += (LineLength / 2);
            }
            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;

        }
    }
}
