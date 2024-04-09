using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ProEasyDotNet;
using Telerik.WinControls.UI;
using System.Diagnostics;
using Microsoft.Win32;

namespace Kneset_terminal
{
    public partial class Form1 : RadForm
    {


        #region Fields

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        private const int WTS_CURRENT_SESSION = -1;

        private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        private AgendaControl agendaControl;

        private NewsControl newsControl;
        private string _writeDevice;

        #endregion


        public Form1()
        {
            try
            {

                InitializeComponent();

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Adobe\Acrobat Reader\DC\AVGeneral", true);
                if (key != null)
                {
                    key.SetValue("bPinHUD",1);
                    key.Close();
                }

                Logger.WriteEventLog("Program started", EventLogEntryType.Information);
                var clientUserName = Environment.GetEnvironmentVariable("USERNAME");
                InitTimer();

                _writeDevice = ConfigurationManager.AppSettings["WriteDevice"];

                agendaControl = new AgendaControl(panel1);

                agendaControl.Dock = DockStyle.Fill;

                //טעינת סדר יום בפעם הראשונה
                agendaControl.setData();

                //קריאה לכפתור שמרים את הסדר יום
                btn_agenda_Click(null, null);


            }
            catch (Exception ex)
            {


                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        //    this.TopMost = true;
          //  this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
        }


        #region Timer

        private void InitTimer()
        {
            try
            {
                var refreshTimer = new Timer();
                var interval = ConfigurationManager.AppSettings["kneset_Interval"];
                refreshTimer.Interval = int.Parse(interval);
                refreshTimer.Tick += refreshTimer_Tick;
                refreshTimer.Start();
            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                agendaControl.setData();
            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }

        #endregion

        #region Buttons Click

        private void button_exit_Click(object sender, EventArgs e)
        {
            try
            {
                //מרים דגל אצל ה
                Write2Client();

                //TODO:האם להשאיר את המסך היכן שהוא או לעשות שיוצג הסדר יום




                ////יציאה מהטרמינל סרוור
                //if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE,
                //    WTS_CURRENT_SESSION, false))
                //  { }

            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }

        private void btn_agenda_Click(object sender, EventArgs e)
        {
            try
            {

                panel1.Controls.Clear();
                panel1.Controls.Add(agendaControl);
            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }

        private void button_news_Click(object sender, EventArgs e)
        {
            try
            {

                panel1.Controls.Clear();
                NewsControl newsControl = new NewsControl();
                newsControl.Dock = DockStyle.Fill;
                newsControl.SetData();
                panel1.Controls.Add(newsControl);

            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);

            }
        }

        #endregion



        private void Write2Client()
        {
            string sNodeName = "";
            try
            {


                // var clientName = Environment.GetEnvironmentVariable("CLIENTNAME");
                //MessageBox.Show("1");
                //MessageBox.Show(Environment.UserDomainName);
                //MessageBox.Show(Environment.UserName);
                var clientUserName = Environment.UserName.Substring(2);
                //MessageBox.Show("2");


                var startTime = DateTime.Now;
                //MessageBox.Show("3");

                DateTime endTime;

                int j = ProEasy.EasyInit();
                //MessageBox.Show(j.ToString());
                //MessageBox.Show("4");
                string msg1 = "";
                bool isError = false;
                int resultCode = 0;




                //כתיבת המערך למסך
                string DeviceNameWrite = _writeDevice;
                //int IndexInArray = 0;
                //short LengthToSend = 1;
                short[] arr = new short[1];
                //var arrToWrite = new short[1];
                //MessageBox.Show("5");
                //Array.Copy(arrToWrite, 0, arr, 0, LengthToSend);
                //MessageBox.Show("6");
                arr[0] = 1;
                sNodeName = "WGP" + clientUserName + ".#INTERNAL";
                //MessageBox.Show("7");
                resultCode = ProEasy.WriteDevice16(sNodeName, "LS" + DeviceNameWrite, arr, 1);
                    //LengthToSend);
                //MessageBox.Show("8");
                if (resultCode != 0)
                {
                    //MessageBox.Show("9");
                    isError = ProEasy.EasyLoadErrorMessageEx(resultCode, out msg1);
                    if (msg1 != "")
                    {

                        endTime = DateTime.Now;
                        Logger.WriteEventLog(
                            msg1 + "  " + (startTime - endTime).ToString() + sNodeName,
                            EventLogEntryType.Error);
                        return;
                    }
                }
                //MessageBox.Show("10");


            }
            catch (Exception ex)
            {

                Logger.WriteEventLog(ex.Message + sNodeName, EventLogEntryType.Error);

            }
        }





    }
}