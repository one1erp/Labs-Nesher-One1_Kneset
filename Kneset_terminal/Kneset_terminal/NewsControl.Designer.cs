namespace Kneset_terminal
{
    partial class NewsControl
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
            this.panelNews = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelNews
            // 
            this.panelNews.AutoScroll = true;
            this.panelNews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNews.Location = new System.Drawing.Point(0, 0);
            this.panelNews.Name = "panelNews";
            this.panelNews.Size = new System.Drawing.Size(747, 500);
            this.panelNews.TabIndex = 0;
            // 
            // NewsControl
            // 
            this.Controls.Add(this.panelNews);
            this.Name = "NewsControl";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(747, 500);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelNews;



    }
}
