using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;
using System.Configuration;
namespace Write2HMIService.Screens
{
    class FrontKmemberName : Screen
    {

        string _kmemberName;
        public FrontKmemberName(string kmemberName)
        {
            sDeviceNameWrite = ConfigurationManager.AppSettings["HfrontMemberName_sDeviceNameWrite"];
            LineLength = int.Parse(ConfigurationManager.AppSettings["HfrontMemberName_length"]);
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
            string name = _kmemberName.PadRight(16, ' ').Substring(0, 16);
            
            //while (name.Length < 16)
            //{
            //    name += " ";
            //}
            var charArr = name.Reverse();

            name = string.Concat(charArr);
            name = string.Format("{0}{1}{2}", @"[01\ST000000\V01<    ", name, @">]  [01\SR][01\SL00002359]");



            //טיפול במקרה אנגלית עברית וסוגריים
            //var textdesc = Reorder.ReorderStr(_kmemberName);
            //מעבר למערך של בייטים
            byte[] byteArr = Encoding.Default.GetBytes(name);

            arrToWrite = new short[arrlength];
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);
            int i;
            for ( i = 0; i < byteArr.Length; i++)
            {

                arrToWrite[i] = (short)(byteArr[i]);

            }
            arrToWrite[i] = 1;
        }
    }
}
