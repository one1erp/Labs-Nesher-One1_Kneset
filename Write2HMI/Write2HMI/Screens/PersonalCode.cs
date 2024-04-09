using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Write2HMI.Screens
{
 
    class PersonalCode : Screen
    {
        private int _personalCode;
        public PersonalCode(int personalCode)
        {
            sDeviceNameWrite = ConfigurationManager.AppSettings["personalCode_sDeviceNameWrite"];
            LineLength = int.Parse(ConfigurationManager.AppSettings["personalCode_length"]);
            arrlength = (short)(LineLength + 1);
            arrToWrite = new short[arrlength];
            _personalCode = personalCode;
            generateShortArr();
        }


        public override void executeQuery()
        {

        }


        public override void generateShortArr()
        {
            arrToWrite = new short[arrlength];
            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);
            arrToWrite[0] = (short)_personalCode;
        }

    }
}
