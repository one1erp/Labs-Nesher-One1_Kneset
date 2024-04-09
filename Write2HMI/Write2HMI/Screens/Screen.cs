using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;
using System.Configuration;

namespace Write2HMI.Screens
{
    public abstract class Screen
    {
        //Dal to comunicate db
        public DAL screenDal { get; set; }
        //"LS3000" קורא תו מהתאריך על המסך. חובה...
        public string sDeviceNameRead { get; set; }
        //"LS6600" לכל מסך יש תו התחלתי ממנו לכתוב
        public string sDeviceNameWrite { get; set; }
        //טריגר לרענון התצוגה
        public int screenTriger { get; set; }
        //אורך שורה מס תוים, 2 תוים בתא של מסך
        public int LineLength { get; set; }
        //מספר שורות במסך
        public int numLines { get; set; }
        //מערך טקסט למסך
        public short[] arrToWrite { get; set; }
        //גודל מערך
        public short arrlength { get; set; }
        public abstract void executeQuery();

        public abstract void generateShortArr();

        public ReorderCls Reorder = new ReorderCls();

    }
}
