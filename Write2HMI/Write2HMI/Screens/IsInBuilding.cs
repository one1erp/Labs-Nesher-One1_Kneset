using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Write2HMI.Screens
{
    class IsInBuilding : Screen
    {
        private int _isInBuilding;
        public IsInBuilding(int isInBuilding)
        {
            sDeviceNameWrite = ConfigurationManager.AppSettings["isInBuilding_sDeviceNameWrite"];
            LineLength = int.Parse(ConfigurationManager.AppSettings["isInBuilding_length"]);
            arrlength = (short)(LineLength + 1);
            arrToWrite = new short[arrlength];
            _isInBuilding = isInBuilding;
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
            arrToWrite[0] =(short) _isInBuilding;
        }

    }
}
