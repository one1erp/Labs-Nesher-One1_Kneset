using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Kneset_terminal
{
    public partial class ButtonsControl :UserControl
    {

        private RadPanel containerPanel;
        public ButtonsControl(RadPanel p)
        {
            InitializeComponent();
            containerPanel = p;

        }
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        private const int WTS_CURRENT_SESSION = -1;
        private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;



        private void button3_Click(object sender, EventArgs e)
        {
            if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE,
                          WTS_CURRENT_SESSION, false))
                throw new Win32Exception();
        }

        private void btn_agenda_Click(object sender, EventArgs e)
        {
            containerPanel.Controls.Clear();
            AgendaControl agendaControl = new AgendaControl(containerPanel);
            agendaControl.Dock=DockStyle.Fill;
            agendaControl.setData();
            containerPanel.Controls.Add(agendaControl);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            containerPanel.Controls.Clear();
            NewsControl newsControl = new NewsControl();
            newsControl.Dock = DockStyle.Fill;     
            newsControl.SetData();
            containerPanel.Controls.Add(newsControl);
        }
    }
}
