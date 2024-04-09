using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;
using System.Configuration;
using System.Globalization;

namespace Write2HMI.Screens
{
    class HebrewDate : Screen
    {


        string hebrewDate;


        public HebrewDate()
        {
            sDeviceNameWrite = ConfigurationManager.AppSettings["HDate_sDeviceNameWrite"];
            LineLength = int.Parse(ConfigurationManager.AppSettings["HDate_length"]);
            arrlength = (short)(LineLength + 1);
            arrToWrite = new short[arrlength];
            //לטעון את התאריך
            executeQuery();
            generateShortArr();

        }

        public override void generateShortArr()
        {
            //אם אורך התאריך קטן ממספר התוים שהוקצה נוסיף רווחים בהתחלה לישור לימין, אחרת אם גדול נחתוך כמספר השורה
            if (hebrewDate.Length < LineLength)
            {
                var space = new string(' ', LineLength - hebrewDate.Length);
                hebrewDate = space + hebrewDate;
            }
            else
            {
                hebrewDate = hebrewDate.Substring(0, LineLength);
            }
            //טיפול במקרה אנגלית עברית וסוגריים
            var textdesc = Reorder.ReorderStr(hebrewDate);
            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(textdesc);

            arrToWrite = new short[arrlength];
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);

            for (int i = 0; i < byteArr.Length; i++)
            {
                //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                arrToWrite[i] = (short)(byteArr[i]);

            }



        }

   
        //Get hebrew date
        public override void executeQuery()
        {
            var ci = CultureInfo.CreateSpecificCulture("he-IL");
            ci.DateTimeFormat.Calendar = new HebrewCalendar();

            string f = ConfigurationManager.AppSettings["HDate_dateFormat"];
            string retval = DateTime.Now.ToString(f, ci);

            //debug
          //  retval = new DateTime(2015, 4, 5).ToString(f, ci);


            int hebrewYear = ci.DateTimeFormat.Calendar.GetYear(DateTime.Now);
            int monthsInYear = ci.DateTimeFormat.Calendar.GetMonthsInYear(hebrewYear);
            int currMonth = ci.DateTimeFormat.Calendar.GetMonth(DateTime.Now);
            //אם שנה מעוברת נעדכן את אדר...
            if ((monthsInYear == 13) && (currMonth == 6))
            {
                retval = retval.Replace("אדר", "אדר א");
            }
            
            hebrewDate = retval;
        }









    }
}
