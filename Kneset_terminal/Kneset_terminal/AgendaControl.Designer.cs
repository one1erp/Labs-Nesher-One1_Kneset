namespace Kneset_terminal
{
    partial class AgendaControl
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
            this.sessionItemsGrid = new System.Windows.Forms.DataGridView();
            this.panelAgenda = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.sessionItemsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // sessionItemsGrid
            // 
            this.sessionItemsGrid.AllowUserToAddRows = false;
            this.sessionItemsGrid.AllowUserToDeleteRows = false;
            this.sessionItemsGrid.AllowUserToResizeColumns = false;
            this.sessionItemsGrid.AllowUserToResizeRows = false;
            this.sessionItemsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionItemsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sessionItemsGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.sessionItemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sessionItemsGrid.Location = new System.Drawing.Point(3, 0);
            this.sessionItemsGrid.MultiSelect = false;
            this.sessionItemsGrid.Name = "sessionItemsGrid";
            this.sessionItemsGrid.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sessionItemsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.sessionItemsGrid.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.sessionItemsGrid.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(7);
            this.sessionItemsGrid.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sessionItemsGrid.RowTemplate.Height = 35;
            this.sessionItemsGrid.RowTemplate.ReadOnly = true;
            this.sessionItemsGrid.Size = new System.Drawing.Size(900, 500);
            this.sessionItemsGrid.TabIndex = 0;
            this.sessionItemsGrid.Visible = false;
            this.sessionItemsGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.sessionItemsGrid_CellClick);
            // 
            // panelAgenda
            // 
            this.panelAgenda.AutoScroll = true;
            this.panelAgenda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAgenda.Location = new System.Drawing.Point(0, 0);
            this.panelAgenda.Name = "panelAgenda";
            this.panelAgenda.Size = new System.Drawing.Size(900, 500);
            this.panelAgenda.TabIndex = 1;
            // 
            // AgendaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelAgenda);
            this.Controls.Add(this.sessionItemsGrid);
            this.Name = "AgendaControl";
            this.Size = new System.Drawing.Size(900, 500);
            ((System.ComponentModel.ISupportInitialize)(this.sessionItemsGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView sessionItemsGrid;
        private System.Windows.Forms.Panel panelAgenda;
    }
}
