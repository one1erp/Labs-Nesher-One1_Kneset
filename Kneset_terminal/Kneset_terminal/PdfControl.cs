using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Kneset_Terminal;
using System.Xml;
using System.Configuration;
using System.Xml.Linq;

namespace Kneset_terminal
{
    public partial class PdfControl : UserControl
    {
        private SessionItem currentObject;

      
        private string pdfPath;


        public PdfControl(string path)
        {
            InitializeComponent();
            pdfPath = path +"#toolbar=0&navpanes=0";


        }
        public void SetData()
        {
        
            webBrowser1.Navigate(pdfPath);
            
        }

  
     
    }
}
