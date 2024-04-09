using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Kneset_terminal
{
    public partial class NewsControl : UserControl
    {
        private RadPanel containerPanel;
        private string newsPath;


        public NewsControl()
        {

            InitializeComponent();
            newsPath = ConfigurationManager.AppSettings["NewsFilePath"];
        }

        internal void SetData()
        {


            //קריאה של הקובץ
            var textarr = File.ReadAllLines(newsPath, Encoding.GetEncoding("Windows-1255"));

            int y = 0;
            int x = 0;
            //add component per row
            foreach (var line in textarr)
            {
                var item = line;

                var component = new NewsComponent();
                if (component.Init(item))
                {

                    component.Location = new Point(x, y);
                    panelNews.Controls.Add(component);
                    y = y + component.Size.Height;
                }

            }
        }
    }
}
