using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Kneset_Terminal;
using System.Xml.Linq;
using System.Configuration;
using Telerik.WinControls.UI;

namespace Kneset_terminal
{
    public partial class AgendaComponent : UserControl
    {
        private string SessionFilesPath;
        private RadPanel containerPanel;

        public SessionItem CurrentSessionItem { get; set; }
        public AgendaComponent(RadPanel p)
        {
            InitializeComponent();
            txtSubject.BackColor = Color.White;
            txtBrief.BackColor = Color.White;
            txtBody.BackColor = Color.White;
          
            SessionFilesPath = ConfigurationManager.AppSettings["SessionFilesPath"];
            this.containerPanel = p;
          
            //Events for open pdf
            this.Click+= AgendaComponent_Click;
            txtBody.Click += AgendaComponent_Click;
            txtBrief.Click += AgendaComponent_Click;
            txtSubject.Click += AgendaComponent_Click;

        }


        public bool Init(SessionItem sessionItem)
        {
            this.CurrentSessionItem = sessionItem;
            if (sessionItem != null)
            {
                txtSubject.Text = "אני לא מוצא את השדה נושא";
                txtBody.Text = sessionItem.SESS_ITEM_DSCR;
                txtBrief.Text = sessionItem.SMRY_DATA;
                return true;
            }
            return false;
        }


        private void AgendaComponent_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentSessionItem == null) return;

                var path = GetpdfPathFromXml(CurrentSessionItem);
                if (!string.IsNullOrEmpty(path))
                {
                    containerPanel.Controls.Clear();
                    var pdfControl = new PdfControl(path);
                    pdfControl.Dock = DockStyle.Fill;
                    pdfControl.SetData();
                    containerPanel.Controls.Add(pdfControl);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);
            }
        }
        private string GetpdfPathFromXml(SessionItem currentObject)
        {

            try
            {
                //פתיחת מסמך XML
                XElement root = XElement.Load(SessionFilesPath);

                //שליפה של הסשן הנוכחי
                IEnumerable<XElement> currentSession =
                    from el in root.Elements("Session")
                    where (string)el.Attribute("ItemID") == currentObject.SESSION_ID
                    select el;

                //שליפה של הסשן איטם הנוכחי
                IEnumerable<XElement> currenSessionItem =
                from el in currentSession.Elements("SessionItem")
                where (string)el.Attribute("ItemID") == currentObject.SESS_ITEM_ID
                select el;


                var temp = currenSessionItem.Elements("File").FirstOrDefault();
                string path = "";

                if (temp != null)
                {
                    path = temp.Attribute("FileName").Value;
                }

                return path;
            }
            catch (Exception ex)
            {
                Logger.WriteEventLog(ex.Message, EventLogEntryType.Error);


                return "";
            }
        }







    }
}
