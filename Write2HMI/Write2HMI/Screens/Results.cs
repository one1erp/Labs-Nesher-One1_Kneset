using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEasyDotNet;
using System.Configuration;
namespace Write2HMI.Screens
{
    class Results : Screen
    {
        public Results(DAL dal)
        {

            sDeviceNameRead = ConfigurationManager.AppSettings["results_sDeviceNameRead"];
            sDeviceNameWrite = ConfigurationManager.AppSettings["results_sDeviceNameWrite"];
            screenTriger = int.Parse(ConfigurationManager.AppSettings["results_screenTriger"]);
            LineLength = int.Parse(ConfigurationManager.AppSettings["results_LineLength"]);
            numLines = int.Parse(ConfigurationManager.AppSettings["results_NumLines"]);
            spaceMembers = int.Parse(ConfigurationManager.AppSettings["results_spaceNum"]);
            //בכתיבת 3 חכים בשורה, נחסר פעמיים רווח ונחלק ב3 חכים, זה המספר תוים מקסימלי לשם חכ
            maxMemberName = (LineLength - (spaceMembers * 2)) / 3;
            //אורך המערך לפי מיקום הטריגר להצגתו
            arrlength = (short)(screenTriger + 1);
            arrToWrite = new short[arrlength];
            //השמת dal 
            screenDal = dal;
            //מעדכן ערכים בשדה נושא ודוברים
            executeQuery();
            generateShortArr();
        }

        //רוחים בהדפסת 3 חכים בשורה
        public int spaceMembers { get; set; }
        //מספר תוים לשם חכ כך שיכנסו 3 בשורה
        public int maxMemberName { get; set; }
        //תוצאות הצבעה
        public plc_results_data results { get; set; }

        public override void executeQuery()
        {
            //var pcd = screenDal.GetPlc_current_data();
            try
            {


                results = screenDal.GetPlc_results_data();
            }
            catch (Exception e)
            {

                Logger.WriteEventLog(e.Message + " " + e.StackTrace, EventLogEntryType.Error);

            }
        }

        public override void generateShortArr()
        {


            string tempTxt = "";
            int tempIndex = 0;
            int saveIndex = 0;
            //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
            int dualIndex = 0;
            byte[] byteArr;

            //ניקוי מערך לכתיבה למסך
            Array.Clear(arrToWrite, 0, arrlength);

            if (results != null)
            {
                int voteNbr = 0;
                int resultsFor = 0;
                int resultsAgainst = 0;
                int resultsAvoid = 0;
                string sessItem = "";

                voteNbr = results.voteNbrInSess;
                resultsFor = results.resultsFor;
                resultsAgainst = results.resultsAgainst;
                resultsAvoid = results.resultsAvoid;
                sessItem = results.sessItemDscr;

                //כתיבת שורה ראשונה תוצאות הצבעה
                tempTxt = "תוצאות הצבעה מספר: " + voteNbr;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = 0; i < (byteArr.Length / 2); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //-----------------------------------------------
                //כתיבת שורה שניה נושא לדיון
                tempTxt = "נושא לדיון: " + sessItem;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //קידום האינדקס ב2 שורות, 2 תוים בתא
                tempIndex += LineLength;
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //--------------------------------------------------------
                //כתיבת שורה הבאה בעד נגד נמנע
                tempTxt = "בעד: " + resultsFor;
                tempTxt += "    ";
                tempTxt += "נגד: " + resultsAgainst;
                tempTxt += "    ";
                tempTxt += "נמנע: " + resultsAvoid;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //קידום האינדקס ב3 שורות, 2 תוים בתא
                tempIndex += LineLength;
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //-----------------------------------------------
                //כתיבת שורת בעד ואחכ רשימת חכ בעד...
                tempTxt = "בעד: " + resultsFor;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //קידום האינדקס ב2 שורות,2 תוים בתא
                tempIndex += LineLength;
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //כתיבת שמות ח"כ בעד
                //,קידום האינדקס בשורה 2 תוים בתא
                tempIndex += LineLength / 2;



                for (int j = 0; j < results.MembersFor.Count(); j = j + 3)
                {
                    saveIndex = tempIndex;
                    //ח"כ ראשון בשורה
                    //טיפול במקרה אנגלית עברית וסוגריים
                    var curritem1 = Reorder.ReorderStr(results.MembersFor[j]);
                    //לחתוך את שם החכ כך שיכנסו עוד בשורה
                    if (curritem1.Length > maxMemberName)
                    {
                        curritem1 = curritem1.Substring(0, maxMemberName);
                    }
                    byteArr = Encoding.Default.GetBytes(curritem1);
                    dualIndex = 0;
                    //כתיבת החכ הנוכחי
                    for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                    {
                        //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                        arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                        dualIndex = dualIndex + 2;
                    }
                    if ((j + 1) < results.MembersFor.Count())
                    {
                        //ח"כ שני בשורה
                        //טיפול במקרה אנגלית עברית וסוגריים
                        var curritem2 = Reorder.ReorderStr(results.MembersFor[j + 1]);
                        //לחתוך את שם החכ כך שיכנסו עוד בשורה
                        if (curritem2.Length > maxMemberName)
                        {
                            curritem2 = curritem2.Substring(0, maxMemberName);
                        }
                        byteArr = Encoding.Default.GetBytes(curritem2);
                        dualIndex = 0;
                        //כתיבת החכ הנוכחי
                        tempIndex += spaceMembers + (maxMemberName / 2);
                        for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                        {
                            //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                            arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                            dualIndex = dualIndex + 2;
                        }
                        if ((j + 2) < results.MembersFor.Count())
                        {
                            //ח"כ שלישי בשורה
                            //טיפול במקרה אנגלית עברית וסוגריים
                            var curritem3 = Reorder.ReorderStr(results.MembersFor[j + 2]);
                            //לחתוך את שם החכ כך שיכנסו עוד בשורה
                            if (curritem3.Length > maxMemberName)
                            {
                                curritem3 = curritem3.Substring(0, maxMemberName);
                            }
                            byteArr = Encoding.Default.GetBytes(curritem3);
                            dualIndex = 0;
                            //כתיבת החכ הראשון בשורה
                            tempIndex += spaceMembers + (maxMemberName / 2);
                            for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                            {
                                //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                                arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                                dualIndex = dualIndex + 2;
                            }
                        }
                    }
                    //,קידום האינדקס בשורה 2 תוים בתא
                    tempIndex = saveIndex + (LineLength / 2);
                }


                //-----------------------------------------------
                //כתיבת שורת נגד ואחכ רשימת חכ נגד...
                tempTxt = "נגד: " + resultsAgainst;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //קידום האינדקס ב2 שורות,2 תוים בתא
                tempIndex += LineLength;
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //כתיבת שמות ח"כ נגד
                //,קידום האינדקס בשורה 2 תוים בתא
                tempIndex += LineLength / 2;

                for (int j = 0; j < results.MembersAgainst.Count(); j = j + 3)
                {
                    saveIndex = tempIndex;
                    //ח"כ ראשון בשורה
                    //טיפול במקרה אנגלית עברית וסוגריים
                    var curritem1 = Reorder.ReorderStr(results.MembersAgainst[j]);
                    //לחתוך את שם החכ כך שיכנסו עוד בשורה
                    if (curritem1.Length > maxMemberName)
                    {
                        curritem1 = curritem1.Substring(0, maxMemberName);
                    }
                    byteArr = Encoding.Default.GetBytes(curritem1);
                    dualIndex = 0;
                    //כתיבת החכ הראשון בשורה
                    for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                    {
                        //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                        arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                        dualIndex = dualIndex + 2;
                    }
                    if ((j + 1) < results.MembersAgainst.Count())
                    {
                        //ח"כ שני בשורה
                        //טיפול במקרה אנגלית עברית וסוגריים
                        var curritem2 = Reorder.ReorderStr(results.MembersAgainst[j + 1]);
                        //לחתוך את שם החכ כך שיכנסו עוד בשורה
                        if (curritem2.Length > maxMemberName)
                        {
                            curritem2 = curritem2.Substring(0, maxMemberName);
                        }
                        byteArr = Encoding.Default.GetBytes(curritem2);
                        dualIndex = 0;
                        //כתיבת החכ הנוכחי
                        tempIndex += spaceMembers + (maxMemberName / 2);
                        for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                        {
                            //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                            arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                            dualIndex = dualIndex + 2;
                        }
                        if ((j + 2) < results.MembersAgainst.Count())
                        {
                            //ח"כ שלישי בשורה
                            //טיפול במקרה אנגלית עברית וסוגריים
                            var curritem3 = Reorder.ReorderStr(results.MembersAgainst[j + 2]);
                            //לחתוך את שם החכ כך שיכנסו עוד בשורה
                            if (curritem3.Length > maxMemberName)
                            {
                                curritem3 = curritem3.Substring(0, maxMemberName);
                            }
                            byteArr = Encoding.Default.GetBytes(curritem3);
                            dualIndex = 0;
                            //כתיבת החכ הנוכחי
                            tempIndex += spaceMembers + (maxMemberName / 2);
                            for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                            {
                                //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                                arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                                dualIndex = dualIndex + 2;
                            }
                        }
                    }
                    //,קידום האינדקס בשורה 2 תוים בתא
                    tempIndex = saveIndex + (LineLength / 2);
                }

                //-----------------------------------------------
                //כתיבת שורת נמנע ואחכ רשימת חכ נמנע...
                tempTxt = "נמנע: " + resultsAvoid;
                //טיפול במקרה אנגלית עברית וסוגריים
                tempTxt = Reorder.ReorderStr(tempTxt);
                //מעבר למערך של בייטים
                byteArr = Encoding.Default.GetBytes(tempTxt);
                //קידום האינדקס ב2 שורות,2 תוים בתא
                tempIndex += LineLength;
                //אינדקס נוסף לכתיבה למערך למסך- מכיון ומכניסים 2 תוים בתא
                dualIndex = 0;
                for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                {
                    //כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                    arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                    dualIndex = dualIndex + 2;
                    saveIndex = i;
                }
                //כתיבת שמות ח"כ נגד
                //,קידום האינדקס בשורה 2 תוים בתא
                tempIndex += LineLength / 2;

                for (int j = 0; j < results.MembersAvoid.Count(); j = j + 3)
                {
                    saveIndex = tempIndex;
                    //ח"כ ראשון בשורה
                    //טיפול במקרה אנגלית עברית וסוגריים
                    var curritem1 = Reorder.ReorderStr(results.MembersAvoid[j]);
                    //לחתוך את שם החכ כך שיכנסו עוד בשורה
                    if (curritem1.Length > maxMemberName)
                    {
                        curritem1 = curritem1.Substring(0, maxMemberName);
                    }
                    byteArr = Encoding.Default.GetBytes(curritem1);
                    dualIndex = 0;
                    //כתיבת החכ הראשון בשורה
                    for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                    {
                        //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                        arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                        dualIndex = dualIndex + 2;
                    }
                    if ((j + 1) < results.MembersAvoid.Count())
                    {
                        //ח"כ שני בשורה
                        //טיפול במקרה אנגלית עברית וסוגריים
                        var curritem2 = Reorder.ReorderStr(results.MembersAvoid[j + 1]);
                        //לחתוך את שם החכ כך שיכנסו עוד בשורה
                        if (curritem2.Length > maxMemberName)
                        {
                            curritem2 = curritem2.Substring(0, maxMemberName);
                        }
                        byteArr = Encoding.Default.GetBytes(curritem2);
                        dualIndex = 0;
                        //כתיבת החכ הנוכחי
                        tempIndex += spaceMembers + (maxMemberName / 2);
                        for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                        {
                            //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                            arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                            dualIndex = dualIndex + 2;
                        }
                        if ((j + 2) < results.MembersAvoid.Count())
                        {
                            //ח"כ שלישי בשורה
                            //טיפול במקרה אנגלית עברית וסוגריים
                            var curritem3 = Reorder.ReorderStr(results.MembersAvoid[j + 2]);
                            //לחתוך את שם החכ כך שיכנסו עוד בשורה
                            if (curritem3.Length > maxMemberName)
                            {
                                curritem3 = curritem3.Substring(0, maxMemberName);
                            }
                            byteArr = Encoding.Default.GetBytes(curritem3);
                            dualIndex = 0;
                            //כתיבת החכ הנוכחי
                            tempIndex += spaceMembers + (maxMemberName / 2);
                            for (int i = tempIndex; i < (tempIndex + (byteArr.Length / 2)); i++)
                            {
                                //  כל 2 תוים מהמערך של הביטים נכתבים לתא אחד במערך למסך
                                arrToWrite[i] = (short)(byteArr[dualIndex] + (byteArr[dualIndex + 1] * 256));
                                dualIndex = dualIndex + 2;
                            }
                        }
                    }
                    //,קידום האינדקס בשורה 2 תוים בתא
                    tempIndex = saveIndex + (LineLength / 2);
                }

            }
            //הפעלת טריגר לרענון התצוגה
            arrToWrite[screenTriger] = 1;

        }
    }
}
