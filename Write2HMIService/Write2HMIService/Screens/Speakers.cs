using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProEasyDotNet;
using System.Configuration;
namespace Write2HMIService.Screens
{
    class Speakers : Screen
    {
        public Speakers(DAL dal)
        {


            sDeviceNameRead = ConfigurationManager.AppSettings["speakers_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["speakers_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["speakers_screenTriger"]);
            speakersWrite1 = int.Parse(ConfigurationManager.AppSettings["speakers_speakersWrite1"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["speakers_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["speakers_NumLines"]);

            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];
            //השמת dal 
            screenDal = dal;
            //מעדכן ערכים בשדה נושא ודוברים
            executeQuery();
            generateShortArr();
        }
        // הדוברים הראשונים
        public List<string> textSpeakers { get; set; }
        //אינדקס דובר1
        public int speakersWrite1 { get; set; }

        public override void executeQuery()
        {
            textSpeakers = new List<string>();
            var plc = screenDal.GetPlc_current_data();
            var speakersData = screenDal.GetPlc_speakers_data();
            if (speakersData!=null)
            {
                  textSpeakers = speakersData.Speakers;
            }
          
        }

        public override void generateShortArr()
        {

            //מעבר למערך של בייטים דוברים
            byte[] byteArrspeaker;
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);
            foreach (var item in textSpeakers)
            {
                //טיפול במקרה אנגלית עברית וסוגריים
                var curritem = Reorder.ReorderStr(item);
                byteArrspeaker = Encoding.Default.GetBytes(curritem);
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                int index = 0;
                for (int i = speakersWrite1; i < (speakersWrite1 + (byteArrspeaker.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArrspeaker[index] + (byteArrspeaker[index + 1] * 256));
                    index = index + 2;
                }
                speakersWrite1 += LineLength / 2;
            }
            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;

        }
    }
}
