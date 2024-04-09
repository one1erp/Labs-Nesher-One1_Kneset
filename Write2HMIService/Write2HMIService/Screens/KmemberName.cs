using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;
using System.Configuration;
namespace Write2HMIService.Screens
{
    class KmemberName : Screen
    {
 
        string _kmemberName;     
        public KmemberName(string kmemberName)
        {
            sDeviceNameWrite = ConfigurationManager.AppSettings["HMemberName_sDeviceNameWrite"];
            LineLength = int.Parse(ConfigurationManager.AppSettings["HMemberName_length"]);
            arrlength = (short)(LineLength + 1);
            arrToWrite = new short[arrlength];
            _kmemberName = kmemberName;
            generateShortArr();
        }


        public override void executeQuery()
        {
           
        }
      
       
             public override void generateShortArr()
        {
            //אם אורך השם קטן ממספר התוים שהוקצה נוסיף רווחים, אחרת אם גדול נחתוך כמספר השורה
            if (_kmemberName.Length < LineLength)
            {
                var space = new string(' ', LineLength - _kmemberName.Length);
                _kmemberName += space;
            }
            else
            {
                _kmemberName = _kmemberName.Substring(0, LineLength);
            }
            //טיפול במקרה אנגלית עברית וסוגריים
            var textdesc = Reorder.ReorderStr(_kmemberName);
            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(textdesc);

            arrToWrite = new short[arrlength];
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);

            for (int i = 0; i < byteArr.Length; i++)
            {

                arrToWrite[i] = (short)(byteArr[i] + 1000);

            }
        }
    
    }
}
