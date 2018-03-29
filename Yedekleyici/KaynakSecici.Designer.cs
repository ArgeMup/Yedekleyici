namespace Yedekleyici
{
    partial class KaynakSecici_
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Liste = new System.Windows.Forms.DataGridView();
            this.Durum = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Filtre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Yol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Arama = new System.Windows.Forms.TextBox();
            this.TümKlasörler = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Liste)).BeginInit();
            this.SuspendLayout();
            // 
            // Liste
            // 
            this.Liste.AllowUserToAddRows = false;
            this.Liste.AllowUserToDeleteRows = false;
            this.Liste.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Liste.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Liste.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Liste.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Liste.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Liste.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Durum,
            this.Filtre,
            this.Yol});
            this.Liste.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Liste.Location = new System.Drawing.Point(0, 23);
            this.Liste.MultiSelect = false;
            this.Liste.Name = "Liste";
            this.Liste.RowHeadersVisible = false;
            this.Liste.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Liste.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Liste.Size = new System.Drawing.Size(335, 128);
            this.Liste.TabIndex = 0;
            this.Liste.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Liste_DataError);
            // 
            // Durum
            // 
            this.Durum.HeaderText = "Durum";
            this.Durum.Items.AddRange(new object[] {
            "Yedekle",
            "Atla",
            "Soyadlarını yedekle",
            "Soyadlarını atla"});
            this.Durum.Name = "Durum";
            this.Durum.Width = 44;
            // 
            // Filtre
            // 
            this.Filtre.HeaderText = "Soyadları";
            this.Filtre.Name = "Filtre";
            this.Filtre.ToolTipText = "(nokta)(soyadı)(boşluk)(nokta)(soyadı) .mup .bin";
            this.Filtre.Width = 75;
            // 
            // Yol
            // 
            this.Yol.HeaderText = "Yol";
            this.Yol.Name = "Yol";
            this.Yol.ReadOnly = true;
            this.Yol.ToolTipText = "Kendi ve alt klasörlerini kapsar";
            this.Yol.Width = 47;
            // 
            // Arama
            // 
            this.Arama.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Arama.BackColor = System.Drawing.Color.White;
            this.Arama.Location = new System.Drawing.Point(0, 1);
            this.Arama.Name = "Arama";
            this.Arama.Size = new System.Drawing.Size(179, 20);
            this.Arama.TabIndex = 1;
            this.Arama.TextChanged += new System.EventHandler(this.Arama_TextChanged);
            // 
            // TümKlasörler
            // 
            this.TümKlasörler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TümKlasörler.AutoSize = true;
            this.TümKlasörler.Location = new System.Drawing.Point(185, 4);
            this.TümKlasörler.Name = "TümKlasörler";
            this.TümKlasörler.Size = new System.Drawing.Size(150, 17);
            this.TümKlasörler.TabIndex = 2;
            this.TümKlasörler.Text = "Alt klasörlerle birlikte listele";
            this.TümKlasörler.UseVisualStyleBackColor = true;
            this.TümKlasörler.CheckedChanged += new System.EventHandler(this.TümKlasörler_CheckedChanged);
            // 
            // KaynakSecici_
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 153);
            this.Controls.Add(this.TümKlasörler);
            this.Controls.Add(this.Arama);
            this.Controls.Add(this.Liste);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KaynakSecici_";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Kaynak Seçici";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KaynakSecici_FormClosing);
            this.Load += new System.EventHandler(this.KaynakSecici_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Liste)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Liste;
        private System.Windows.Forms.DataGridViewComboBoxColumn Durum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filtre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Yol;
        private System.Windows.Forms.TextBox Arama;
        private System.Windows.Forms.CheckBox TümKlasörler;
    }
}