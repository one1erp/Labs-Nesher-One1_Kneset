using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ProEasyDotNet;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using Write2HMI.Screens;
using Write2HMI.Triggers;
using Screen = Write2HMI.Screens.Screen;
using Timer = System.Windows.Forms.Timer;


namespace Write2HMI
{

    public partial class Form1 : Form
    {





        //משתנים גלובליים לחיי הפורם
        DAL dal;
        plc_kmembers_data kmembers;
        public ReorderCls Reorder = new ReorderCls();
        private List<Trigger> triggers = new List<Trigger>();
        private List<Thread> threadsLst = new List<Thread>();
        private List<Screen> screensToGenerate = new List<Screen>();
        bool updateDateAndName = false;
        bool updatePersonalMessage = false;
        bool refreshScreenTimerIsBusy = false;
        private bool DEBUG = Convert.ToBoolean(ConfigurationManager.AppSettings["kneset_inDebug"]);
        private int threadNumber = int.Parse(ConfigurationManager.AppSettings["kneset_threadNumber"]);
        private Timer refreshScreenTimer = new Timer();
        //כל 5 סבבים של הטיימר נעדכן את רשימת החכים, מי נוכח וכו...
        private int KmemberCounter = 0;

        public Form1()
        {
            try
            {

                InitializeComponent();


                Logger.WriteEventLog("Program Started", EventLogEntryType.Information);
                //Open connection to DB
                dal = new DAL();//TODO: להחליט מתי פותחים וסוגרים CONNECTION
                dal.Open();

                //אם במצב דיבאג מריץ כרגע ל3 מסכים עם חברי כנסת פיקטיביים , אחרת רץ על רישמת החכים מהדטה בייס
                if (DEBUG)
                {
                    kmembers = new plc_kmembers_data();
                    kmembers.knesetNum = 19;
                    kmembers.kmembers = new List<kmember>();
                    //  kmember k1 = new kmember();
                    //  k1.kmember_IP = "253";
                    //  k1.kmember_id = 1;
                    //  k1.kmember_fName = "משה";
                    //  k1.kmember_lName = "זוכמיר";
                    kmember k2 = new kmember();
                    k2.kmember_IP = "250";
                    k2.kmember_id = 2;
                    k2.kmember_fName = "יוסי";
                    k2.kmember_lName = "יוסף";
                    kmember k3 = new kmember();
                    k3.kmember_IP = "251";
                    k3.kmember_id = 3;
                    k3.kmember_fName = "אבי";
                    k3.kmember_lName = "אברהם";
                    kmember k4 = new kmember();
                    k4.kmember_IP = "252";
                    k4.kmember_id = 3;
                    k4.kmember_fName = "יוחנן";
                    k4.kmember_lName = "יוחננוף";


                    //  kmembers.kmembers.Add(k1);
                    kmembers.kmembers.Add(k2);
                    kmembers.kmembers.Add(k3);
                    kmembers.kmembers.Add(k4);

                    InitTriggers();
                    InitTimer();
                }
                else
                {
                    kmembers = dal.GetPlc_Kmembers_data();
                    //אתחול של הטריגרים
                     InitTriggers();
                    //אתחול של הטיימר שמעדכן את מסכים
                     InitTimer();
                }

                //לעבור על רשימת החכים ולתת לכל אחד threadId 
                var kmmbrNum = kmembers.kmembers.Count;
                for (int i = 0; i <= kmmbrNum - 1; i++)
                {
          //          kmembers.kmembers[i].threadId = (i % threadNumber) + 1;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (dal != null)
                {
                    //   dal.Close();
                }
            }

        }



        private void InitTriggers()
        {

            var meliaTrigger = new MeliaaTrigger(dal);
            var voteTrigger = new VoteTrigger(dal);
            var resultsTrigger = new ResultsTrigger(dal);
            var speakersTrigger = new SpeakersTrigger(dal);
            var massageTrigger = new MessageTrigger(dal);
            //טריגר אחראי לרענון שם הח"כ תאריך עברי ,האם נוכח וקוד אישי של הח"כ
            var kmemberNameDate = new KmemberNameDateTrigger(dal);
            var personalMessage = new PersonalMessageTrigger(dal,kmembers.kmembers);
            triggers.Add(meliaTrigger);
            triggers.Add(voteTrigger);
            triggers.Add(resultsTrigger);
            triggers.Add(speakersTrigger);
            triggers.Add(massageTrigger);
            triggers.Add(kmemberNameDate);
            triggers.Add(personalMessage);

        }

        private void InitTimer()
        {


            var interval = ConfigurationManager.AppSettings["kneset_Interval"];
            refreshScreenTimer.Interval = int.Parse(interval);
            refreshScreenTimer.Tick += refreshScreenTimer_Tick;
            refreshScreenTimer.Start();

        }

        void refreshScreenTimer_Tick(object sender, EventArgs e)
        {
            refreshScreenTimer.Stop();
            if (KmemberCounter == 5)
            {
                KmemberCounter = 0;
                kmembers = dal.GetPlc_Kmembers_data();
            }
            else
            {
                KmemberCounter++;
            }



            //Logger.WriteEventLog("timer start " + DateTime.Now, EventLogEntryType.Information );
            while (dal.GetConnectionState() != ConnectionState.Open)
            {
                dal.Open();
                Thread.Sleep(1000);
            }

            bool threadRunning = true;
            int threadRunningNum = 0;
            //while (threadRunning)
            //{
            //    foreach (Thread t in threadsLst)
            //    {
            //        if (t.IsAlive)
            //        {
            //            threadRunningNum++;
            //        }
            //    }
            //    if (threadRunningNum == 0)
            //    {
            //        threadRunning = false;
            //    }
            //    else
            //    {
            //        threadRunningNum = 0;
            //        Thread.Sleep(50);
            //    }
            //}
            //עובר על כל הטריגרים של המסכים ובודק אם נעשה שינוי בדטה בייס
            var refreshTriggers = CheckSignals();

            if (refreshTriggers != null && refreshTriggers.Count > 0)//אם הבקר השתנה
            {
                //אם הבקר השתנה ישלח לפונקציה שם המסך הרלוונטי
                Run(refreshTriggers);

            }
            //Logger.WriteEventLog("timer stop " + DateTime.Now, EventLogEntryType.Information);
            refreshScreenTimer.Start();
        }

        private void Run(List<Trigger> refreshTriggers)
        {
            //שליפת נתונים עדכון ישות מסך וכתיבה למסך ע"פ עמדת מחשב



            screensToGenerate.Clear();

            foreach (var trigger in refreshTriggers)
            {
                switch (trigger.Screen)
                {
                    case "results":
                        screensToGenerate.Add(new Results(dal));
                        break;
                    case "speakers":
                        screensToGenerate.Add(new Speakers(dal));
                        break;
                    case "messages":
                        screensToGenerate.Add(new Messages(dal));
                        break;
                    case "vote":
                        screensToGenerate.Add(new Vote(dal));
                        break;
                    case "maliaa":
                        screensToGenerate.Add(new Meliaa(dal));
                        break;
                    case "personalMessages":
                        updatePersonalMessage = true;
                        break;
                    case "KmemberNameDate":
                        //אם צריך לעדכן גם תאריך ושם חכ אז המשתנה הבוליאני יהיה true
                        //ולפיו בתוך הרצה של כל חבר כנסת נעדכן לפי אותו שם חכ
                        updateDateAndName = true;
                        screensToGenerate.Add(new HebrewDate());

                        break;
                }
            }
            if (screensToGenerate != null)
            {
                //     threadsLst.Clear();
                //todo פה לקרוא בטרדים ל120 מחשבים
                //for (int i = 1; i <= threadNumber; i++)
                //{
                //var tempkmb = kmembers.kmembers.FindAll(k => (k.threadId == i));
                //if (tempkmb.Count > 0)
                //{
                //    var tp = new threadParams();

                //    tp.threadKmembers = tempkmb;

                //    tp.threadScreens = screensToGenerate;

                //    tp.isZombis = false;

                //    var currThread = new Thread(new ParameterizedThreadStart(threadFunc));
                //    threadsLst.Add(currThread);
                //    currThread.Start(tp);

                //}

                string sNodeName;
                //threadParams tp = o as threadParams;
                foreach (var kmb in kmembers.kmembers)
                {
                    sNodeName = kmb.kmember_IP;
                    //  sNodeName = "WGP" + kmb.kmember_IP + ".#INTERNAL";
                    //טריגר אחראי לרענון שם הח"כ תאריך עברי ,האם נוכח וקוד אישי של הח"כ
                    if (updateDateAndName)
                    {
                        List<Screen> screensPerKmember = new List<Screen>();
                        string fullName = kmb.kmember_fName + " " + kmb.kmember_lName;
                        screensPerKmember.Add(new KmemberName(fullName));
                        screensPerKmember.Add(new FrontKmemberName(fullName));
                        screensPerKmember.Add(new IsInBuilding(kmb.kmember_In_Building));
                        screensPerKmember.Add(new PersonalCode(kmb.kmember_code));

                       

                        writeToHmi(screensPerKmember, sNodeName);

                    }
                    // אם יש הודעות חדשות
                    //מייצר מסך הודעות אישיות לכל חכ
                    if (updatePersonalMessage)
                    {
                        //אם הח"כ נוכח נשדר את ההודעה
                        if (kmb.kmember_In_Building == 1 && kmb.hasNewMessage)
                        {
                            List<Screen> screensPersonalMsgKmember = new List<Screen>();
                            PersonalMessage personalmessageScreen = new PersonalMessage(dal, kmb.kmember_id,
                                                                                                kmb.kmember_code);
                            screensPersonalMsgKmember.Add(personalmessageScreen);
                            writeToHmi(screensPersonalMsgKmember, sNodeName);
                            //אחרי שכתבנו את ההודעה בהצלחה נעדכן שההודעה נקראה
                           personalmessageScreen.updateQuery();
                        }
                    }


                    writeToHmi(screensToGenerate, sNodeName);

                }



            }
        }


        private List<Trigger> CheckSignals()
        {
            updateDateAndName = false;
            updatePersonalMessage = false;
            var triggers2Refresh = new List<Trigger>();
            foreach (var trigger in triggers)
            {
                if (dal != null)
                {

                    if (trigger.SignalChanged())
                    {
                        triggers2Refresh.Add(trigger);
                    }
                }
            }
            return triggers2Refresh;
        }

        private void writeToHmi(List<Screen> scr, string sNodeName)
        {

            try
            {


                kmember currentKmember = GetKmemberByIp(sNodeName);


                var startTime = DateTime.Now;

                DateTime endTime;

                int j = ProEasy.EasyInit();
                string msg1 = "";
                bool isError = false;
                int resultCode = 0;

                //קריאה מהמסך
                //======== THIS PART BELONGS TO winGP
                //resultCode = ProEasy.ReadDevice16(sNodeName, scr.sDeviceNameRead, out aa, 500);
                //if (resultCode != 0)
                //{
                //    isError = ProEasy.EasyLoadErrorMessageEx(resultCode, out msg1);
                //}

                foreach (var sc in scr)
                {
                    //כתיבת המערך למסך
                    string DeviceNameWrite = sc.sDeviceNameWrite;
                    int IndexInArray = 0;
                    short LengthToSend = sc.arrlength;
                    short[] arr = new short[LengthToSend];
                    Array.Copy(sc.arrToWrite, 0, arr, 0, LengthToSend);
                    while (LengthToSend > 1000)
                    {
                        //  ProEasy.EasySetWaitType();
                        resultCode = ProEasy.WriteDevice16("WGP" + sNodeName + ".#INTERNAL", "LS" + DeviceNameWrite, arr, 1000);
                        if (resultCode != 0)
                        {
                            isError = ProEasy.EasyLoadErrorMessageEx(resultCode, out msg1);
                            if (msg1 != "")
                            {

                                endTime = DateTime.Now;
                                Logger.WriteEventLog(msg1 + "  " + (startTime - endTime).ToString(), EventLogEntryType.Error);
                                return;
                            }
                        }
                        LengthToSend = (short)(LengthToSend - 1000);
                        IndexInArray += 1000;
                        DeviceNameWrite = (Int32.Parse(DeviceNameWrite) + 1000).ToString();
                        Array.Copy(sc.arrToWrite, IndexInArray, arr, 0, LengthToSend);
                    }
                    if (LengthToSend > 0)
                    {
                        resultCode = ProEasy.WriteDevice16("WGP" + sNodeName + ".#INTERNAL", "LS" + DeviceNameWrite, arr, LengthToSend);
                        if (resultCode != 0)
                        {
                            isError = ProEasy.EasyLoadErrorMessageEx(resultCode, out msg1);
                            if (msg1 != "")
                            {

                                endTime = DateTime.Now;
                                Logger.WriteEventLog(msg1 + "  " + (startTime - endTime).ToString(), EventLogEntryType.Error);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private kmember GetKmemberByIp(string ip)
        {
            var kmbr = kmembers.kmembers.FirstOrDefault(x => x.kmember_IP == ip);
            return kmbr;
        }


    }
    public class threadParams
    {
        public List<kmember> threadKmembers = new List<kmember>();
        public List<Screen> threadScreens = new List<Screen>();
        public bool isZombis { get; set; }
    }
}