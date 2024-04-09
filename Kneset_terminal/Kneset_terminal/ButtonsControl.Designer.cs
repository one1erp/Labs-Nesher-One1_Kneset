using Telerik.WinControls.UI;

namespace Kneset_terminal
{
    partial class ButtonsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAgenda = new Telerik.WinControls.UI.RadButton();
            this.button2 = new Telerik.WinControls.UI.RadButton();
            this.btnExitRdp = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnAgenda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.button2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExitRdp)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAgenda
            // 
            this.btnAgenda.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgenda.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnAgenda.Location = new System.Drawing.Point(650, 137);
            this.btnAgenda.Name = "btnAgenda";
            this.btnAgenda.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // 
            // 
            this.btnAgenda.RootElement.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnAgenda.RootElement.Padding = new System.Windows.Forms.Padding(15);
            this.btnAgenda.Size = new System.Drawing.Size(200, 70);
            this.btnAgenda.TabIndex = 0;
            this.btnAgenda.Text = "סדר יום";
            this.btnAgenda.TextWrap = true;
            this.btnAgenda.Click += new System.EventHandler(this.btn_agenda_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.button2.Location = new System.Drawing.Point(383, 136);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 70);
            this.button2.TabIndex = 1;
            this.button2.Text = "חדשות";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnExitRdp
            // 
            this.btnExitRdp.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnExitRdp.Location = new System.Drawing.Point(116, 136);
            this.btnExitRdp.Name = "btnExitRdp";
            this.btnExitRdp.Size = new System.Drawing.Size(200, 70);
            this.btnExitRdp.TabIndex = 2;
            this.btnExitRdp.Text = "חזרה";
            this.btnExitRdp.Click += new System.EventHandler(this.button3_Click);
            // 
            // ButtonsControl
            // 
            this.Controls.Add(this.btnExitRdp);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnAgenda);
            this.Name = "ButtonsControl";
            this.Size = new System.Drawing.Size(1066, 392);
            ((System.ComponentModel.ISupportInitialize)(this.btnAgenda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.button2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExitRdp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RadButton btnAgenda;
        private RadButton button2;
        private RadButton btnExitRdp;
    }
}
