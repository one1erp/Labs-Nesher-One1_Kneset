using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Kneset_terminal
{
    public partial class NewsComponent : UserControl
    {
        public NewsComponent()
        {
            InitializeComponent();

            txtDate.BackColor = Color.White;
            txtTitle.BackColor = Color.White;
            txtBody.BackColor = Color.White;


        }

        public bool Init(string NewsString)
        {

            NewsString = NewsString.Replace("\"\"", "\"");
            try
            {
                if (NewsString.Length < 2)//Last row
                {
                    return false;
                }
                var item = NewsString;
                if (item[0].Equals('\"'))//first row
                {
                    item = item.Substring(1, item.Length - 1);

                }


                var firstIndex = item.IndexOf('|');
                if (firstIndex < 0)
                {
                    firstIndex = 0;
                }
                var secondIndex = item.IndexOf('|', firstIndex + 1);
                if (secondIndex < 0)
                {
                    secondIndex = 0;
                }
                var date = item.Substring(0, firstIndex);

                //המרה של התאריך שמגיע מהקובץ
                var convertedDate = UnixTimeStampToDateTime(date);

                var title = item.Substring(firstIndex + 1, secondIndex - (firstIndex + 1));

                var newsText = item.Substring(secondIndex + 1);

                txtDate.Text = convertedDate;
                txtTitle.Text = title;
                txtBody.Text = newsText;


                return true;//Success

            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
                return false;//Failed

            }
        }
        string
            UnixTimeStampToDateTime(string date)
        {

            double unixTimeStamp = 0;
            if (double.TryParse(date, out unixTimeStamp))
            {



                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime.ToString("hh:mm dd/MM/yy");
            }
            return "0";
        }

    }
}
