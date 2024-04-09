using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Kneset_Terminal;
using Telerik.WinControls.UI;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;


namespace Kneset_terminal
{
    public partial class AgendaControl : UserControl
    {
        #region fields
        private RadPanel containerPanel;
        private string SessionFilesPath;
        private DAL dal;
        #endregion

        public AgendaControl(RadPanel p)
        {
            try
            {


                InitializeComponent();



                containerPanel = p;
                SessionFilesPath = ConfigurationManager.AppSettings["SessionFilesPath"];
                dal = new DAL();
                dal.Open();
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            }
        }

        public void setData()
        {

            try
            {
                if (dal != null && dal.CheckSignal())
                {

                    var list = dal.GetSessionItems();
                    if (list != null)
                    {

                        int y = 0;
                        int x = 0;
                        foreach (var item in list)
                        {



                            var component = new AgendaComponent(containerPanel);
                            if (component.Init(item))
                            {

                                component.Location = new Point(x, y);
                                panelAgenda.Controls.Add(component);
                                y = y + component.Size.Height;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            }
        }

        public void setDataOld()
        {

            try
            {
                if (dal != null && dal.CheckSignal())
                {

                    var list = dal.GetSessionItems();
                    if (list != null)
                    {
                        //sessionItemsGrid.DataSource = null;
                        sessionItemsGrid.DataSource = list;
                        foreach (Control ctrl in sessionItemsGrid.Controls)
                            if (ctrl.GetType() == typeof(VScrollBar))
                                ctrl.Width = 2000;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            }
        }
        private void sessionItemsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{

            //    SessionItem currentObject = (SessionItem)sessionItemsGrid.Rows[e.RowIndex].DataBoundItem;
            //    if (currentObject == null) return;

            //    var path = GetpdfPathFromXml(currentObject);
            //    if (!string.IsNullOrEmpty(path))
            //    {
            //        containerPanel.Controls.Clear();
            //        var pdfControl = new PdfControl(path);
            //        pdfControl.Dock = DockStyle.Fill;
            //        pdfControl.SetData();
            //        containerPanel.Controls.Add(pdfControl);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            //}
        }
        //private string GetpdfPathFromXml(SessionItem currentObject)
        //{

        //    try
        //    {
        //        //פתיחת מסמך XML
        //        XElement root = XElement.Load(SessionFilesPath);

        //        //שליפה של הסשן הנוכחי
        //        IEnumerable<XElement> currentSession =
        //            from el in root.Elements("Session")
        //            where (string)el.Attribute("ItemID") == currentObject.SESSION_ID
        //            select el;

        //        //שליפה של הסשן איטם הנוכחי
        //        IEnumerable<XElement> currenSessionItem =
        //        from el in currentSession.Elements("SessionItem")
        //        where (string)el.Attribute("ItemID") == currentObject.SESS_ITEM_ID
        //        select el;


        //        var temp = currenSessionItem.Elements("File").FirstOrDefault();
        //        string path = "";

        //        if (temp != null)
        //        {
        //            path = temp.Attribute("FileName").Value;
        //        }

        //        return path;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);


        //        return "";
        //    }
        //}


    }
}
