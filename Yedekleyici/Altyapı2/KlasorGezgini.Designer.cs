namespace Yedekleyici
{
    partial class KlasorGezgini
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KlasorGezgini));
            this.Liste = new System.Windows.Forms.DataGridView();
            this.Klasör = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dosya = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Boyut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.İsim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MenuSağ = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuSağ_Hesaplama = new System.Windows.Forms.ToolStripComboBox();
            this.MenuSağ_Ayırıcı1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSağ_DosyaGezginindeAç = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSağ_YeniSayfadaAç = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSağ_Ayırıcı2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSağ_GösterGizle = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSağ_ParolaŞablonu = new System.Windows.Forms.ToolStripComboBox();
            this.Etiket_GeçerliYol = new System.Windows.Forms.LinkLabel();
            this.Tuş_Durdur = new System.Windows.Forms.Button();
            this.Tuş_Yenile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Liste)).BeginInit();
            this.MenuSağ.SuspendLayout();
            this.SuspendLayout();
            // 
            // Liste
            // 
            this.Liste.AllowDrop = true;
            this.Liste.AllowUserToAddRows = false;
            this.Liste.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Liste.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.Liste.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Liste.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Liste.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Liste.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.Liste.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Liste.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Klasör,
            this.Dosya,
            this.Boyut,
            this.İsim});
            this.Liste.ContextMenuStrip = this.MenuSağ;
            this.Liste.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Liste.Location = new System.Drawing.Point(14, 38);
            this.Liste.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Liste.MultiSelect = false;
            this.Liste.Name = "Liste";
            this.Liste.ReadOnly = true;
            this.Liste.RowHeadersWidth = 4;
            this.Liste.RowTemplate.Height = 24;
            this.Liste.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Liste.ShowCellErrors = false;
            this.Liste.ShowEditingIcon = false;
            this.Liste.ShowRowErrors = false;
            this.Liste.Size = new System.Drawing.Size(503, 409);
            this.Liste.TabIndex = 0;
            this.Liste.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Liste_CellDoubleClick);
            this.Liste.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.Liste_SortCompare);
            // 
            // Klasör
            // 
            this.Klasör.HeaderText = "Klasör";
            this.Klasör.MinimumWidth = 6;
            this.Klasör.Name = "Klasör";
            this.Klasör.ReadOnly = true;
            this.Klasör.Width = 89;
            // 
            // Dosya
            // 
            this.Dosya.HeaderText = "Dosya";
            this.Dosya.MinimumWidth = 6;
            this.Dosya.Name = "Dosya";
            this.Dosya.ReadOnly = true;
            this.Dosya.Width = 90;
            // 
            // Boyut
            // 
            this.Boyut.HeaderText = "Boyut";
            this.Boyut.MinimumWidth = 6;
            this.Boyut.Name = "Boyut";
            this.Boyut.ReadOnly = true;
            this.Boyut.Width = 86;
            // 
            // İsim
            // 
            this.İsim.HeaderText = "İsim";
            this.İsim.MinimumWidth = 6;
            this.İsim.Name = "İsim";
            this.İsim.ReadOnly = true;
            this.İsim.Width = 74;
            // 
            // MenuSağ
            // 
            this.MenuSağ.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuSağ.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSağ_Hesaplama,
            this.MenuSağ_Ayırıcı1,
            this.MenuSağ_DosyaGezginindeAç,
            this.MenuSağ_YeniSayfadaAç,
            this.MenuSağ_Ayırıcı2,
            this.MenuSağ_GösterGizle,
            this.MenuSağ_ParolaŞablonu});
            this.MenuSağ.Name = "contextMenuStrip1";
            this.MenuSağ.ShowImageMargin = false;
            this.MenuSağ.Size = new System.Drawing.Size(228, 223);
            // 
            // MenuSağ_Hesaplama
            // 
            this.MenuSağ_Hesaplama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MenuSağ_Hesaplama.DropDownWidth = 150;
            this.MenuSağ_Hesaplama.Items.AddRange(new object[] {
            "Hesaplamalar Açık",
            "Hesaplamalar Kapalı"});
            this.MenuSağ_Hesaplama.Name = "MenuSağ_Hesaplama";
            this.MenuSağ_Hesaplama.Size = new System.Drawing.Size(150, 33);
            // 
            // MenuSağ_Ayırıcı1
            // 
            this.MenuSağ_Ayırıcı1.Name = "MenuSağ_Ayırıcı1";
            this.MenuSağ_Ayırıcı1.Size = new System.Drawing.Size(224, 6);
            // 
            // MenuSağ_DosyaGezginindeAç
            // 
            this.MenuSağ_DosyaGezginindeAç.Name = "MenuSağ_DosyaGezginindeAç";
            this.MenuSağ_DosyaGezginindeAç.Size = new System.Drawing.Size(227, 32);
            this.MenuSağ_DosyaGezginindeAç.Text = "Dosya Gezgininde Aç";
            this.MenuSağ_DosyaGezginindeAç.Click += new System.EventHandler(this.MenuSağ_DosyaGezginindeAç_Click);
            // 
            // MenuSağ_YeniSayfadaAç
            // 
            this.MenuSağ_YeniSayfadaAç.Name = "MenuSağ_YeniSayfadaAç";
            this.MenuSağ_YeniSayfadaAç.Size = new System.Drawing.Size(227, 32);
            this.MenuSağ_YeniSayfadaAç.Text = "Yeni Sayfada Aç";
            this.MenuSağ_YeniSayfadaAç.Click += new System.EventHandler(this.MenuSağ_YeniSayfadaAç_Click);
            // 
            // MenuSağ_Ayırıcı2
            // 
            this.MenuSağ_Ayırıcı2.Name = "MenuSağ_Ayırıcı2";
            this.MenuSağ_Ayırıcı2.Size = new System.Drawing.Size(224, 6);
            // 
            // MenuSağ_GösterGizle
            // 
            this.MenuSağ_GösterGizle.Name = "MenuSağ_GösterGizle";
            this.MenuSağ_GösterGizle.Size = new System.Drawing.Size(227, 32);
            this.MenuSağ_GösterGizle.Text = "Göster / Gizle";
            this.MenuSağ_GösterGizle.Click += new System.EventHandler(this.MenuSağ_GösterGizle_Click);
            // 
            // MenuSağ_ParolaŞablonu
            // 
            this.MenuSağ_ParolaŞablonu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MenuSağ_ParolaŞablonu.DropDownWidth = 150;
            this.MenuSağ_ParolaŞablonu.Name = "MenuSağ_ParolaŞablonu";
            this.MenuSağ_ParolaŞablonu.Size = new System.Drawing.Size(150, 33);
            this.MenuSağ_ParolaŞablonu.Visible = false;
            // 
            // Etiket_GeçerliYol
            // 
            this.Etiket_GeçerliYol.AutoSize = true;
            this.Etiket_GeçerliYol.Location = new System.Drawing.Point(14, 11);
            this.Etiket_GeçerliYol.Name = "Etiket_GeçerliYol";
            this.Etiket_GeçerliYol.Size = new System.Drawing.Size(21, 20);
            this.Etiket_GeçerliYol.TabIndex = 3;
            this.Etiket_GeçerliYol.TabStop = true;
            this.Etiket_GeçerliYol.Text = "...";
            this.Etiket_GeçerliYol.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Etiket_GeçerliYol_LinkClicked);
            // 
            // Tuş_Durdur
            // 
            this.Tuş_Durdur.Location = new System.Drawing.Point(17, 38);
            this.Tuş_Durdur.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Tuş_Durdur.Name = "Tuş_Durdur";
            this.Tuş_Durdur.Size = new System.Drawing.Size(99, 29);
            this.Tuş_Durdur.TabIndex = 4;
            this.Tuş_Durdur.Text = "Durdur";
            this.Tuş_Durdur.UseVisualStyleBackColor = true;
            this.Tuş_Durdur.Visible = false;
            this.Tuş_Durdur.Click += new System.EventHandler(this.Tuş_Durdur_Click);
            // 
            // Tuş_Yenile
            // 
            this.Tuş_Yenile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Tuş_Yenile.Location = new System.Drawing.Point(417, 38);
            this.Tuş_Yenile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Tuş_Yenile.Name = "Tuş_Yenile";
            this.Tuş_Yenile.Size = new System.Drawing.Size(99, 29);
            this.Tuş_Yenile.TabIndex = 5;
            this.Tuş_Yenile.Text = "Yenile";
            this.Tuş_Yenile.UseVisualStyleBackColor = true;
            this.Tuş_Yenile.Click += new System.EventHandler(this.Tuş_Yenile_Click);
            // 
            // KlasorGezgini
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 462);
            this.Controls.Add(this.Tuş_Yenile);
            this.Controls.Add(this.Tuş_Durdur);
            this.Controls.Add(this.Etiket_GeçerliYol);
            this.Controls.Add(this.Liste);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(354, 494);
            this.Name = "KlasorGezgini";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Klasör Seçici";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KlasorGezgini_FormClosed);
            this.Load += new System.EventHandler(this.KlasorGezgini_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Liste)).EndInit();
            this.MenuSağ.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Liste;
        private System.Windows.Forms.LinkLabel Etiket_GeçerliYol;
        private System.Windows.Forms.ContextMenuStrip MenuSağ;
        private System.Windows.Forms.ToolStripComboBox MenuSağ_Hesaplama;
        private System.Windows.Forms.ToolStripSeparator MenuSağ_Ayırıcı1;
        private System.Windows.Forms.ToolStripMenuItem MenuSağ_DosyaGezginindeAç;
        private System.Windows.Forms.ToolStripMenuItem MenuSağ_YeniSayfadaAç;
        private System.Windows.Forms.ToolStripSeparator MenuSağ_Ayırıcı2;
        private System.Windows.Forms.ToolStripMenuItem MenuSağ_GösterGizle;
        private System.Windows.Forms.Button Tuş_Durdur;
        private System.Windows.Forms.DataGridViewTextBoxColumn Klasör;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dosya;
        private System.Windows.Forms.DataGridViewTextBoxColumn Boyut;
        private System.Windows.Forms.DataGridViewTextBoxColumn İsim;
        private System.Windows.Forms.ToolStripComboBox MenuSağ_ParolaŞablonu;
        private System.Windows.Forms.Button Tuş_Yenile;
    }
}