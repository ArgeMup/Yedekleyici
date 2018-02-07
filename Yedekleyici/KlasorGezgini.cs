// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/Yedekleyici>

using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Dönüştürme;

namespace Yedekleyici
{
    public partial class KlasorGezgini : Form
    {
        public string Sürüm = "V1.1";
        decimal DosyaSayisi, KlasörSayisi, DosyaBoyutu;
        int sayacDeğiştir, rowindex_grid;
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        bool durdur, çalışıyor;
        private readonly AnaEkran _form1;

        public KlasorGezgini(AnaEkran form1, bool KaynakHedefGörünsün = false)
        {
            _form1 = form1;
            InitializeComponent();

            if(KaynakHedefGörünsün)
            {
                toolStripSeparator3.Visible = true;
                toolStripMenuItem4.Visible = true;
                toolStripMenuItem5.Visible = true;
            }
        }
        private void KlasorGezgini_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
            PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, _form1.Ayarlar, true, "KlasorGezgini", Location.X, Location.Y, Width, Height);
        }
        private void KlasorGezgini_FormClosed(object sender, FormClosedEventArgs e)
        {
            PeTeİkKo.Dispose();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            rowindex_grid = 0;
            string b = linkLabel1.Text;

            if (b.Length > 3)
            {
                //bir klasör yukarı
                if (b.Substring(b.Length - 1, 1) == "\\") b = b.Remove(b.Length - 1, 1);
                while (!string.IsNullOrEmpty(b) && b.Substring(b.Length - 1, 1) != "\\")
                {
                    b = b.Remove(b.Length - 1, 1);
                }

                Baslangiçİşlemleri(b);
            }
            else if (linkLabel1.Text.Length == 3)
            {
                dataGridView1.RowCount = 1;
                dataGridView1[0, 0].Value = "";
                dataGridView1[1, 0].Value = "";
                dataGridView1[2, 0].Value = "";
                dataGridView1[3, 0].Value = "";

                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives)
                {
                    Ekle("-", str, 0, 0, 0);
                }
                linkLabel1.Text = ".";
            }
        }
        public void Baslangiçİşlemleri(string yol)
        {
            if (yol == "") return;
            if (yol.Substring(yol.Length - 1, 1) != "\\") yol += "\\";
 
            linkLabel1.Text = yol;
            dataGridView1.RowCount = 1;
            dataGridView1[0, 0].Value = "";
            dataGridView1[1, 0].Value = "";
            dataGridView1[2, 0].Value = "";
            dataGridView1[3, 0].Value = "";

            if (dataGridView1.SortedColumn != null)
            {
                DataGridViewColumn col = dataGridView1.SortedColumn;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            durdur = false;
            çalışıyor = true;
            linkLabel1.Enabled = false;
            toolStripMenuItem4.Enabled = false;
            toolStripMenuItem5.Enabled = false;
            button1.Visible = true;
            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Belirsiz);

            try
            {
                string[] fileEntries = Directory.GetFiles(yol);
                foreach (string fileName in fileEntries)
                {
                    if (!durdur)
                    {
                        FileAttributes fileAttributes = File.GetAttributes(fileName);
                        decimal gizli = 0;
                        if (fileAttributes.HasFlag(FileAttributes.Hidden)) gizli = 1;
                        long ll = 0; try { ll = new System.IO.FileInfo(fileName).Length; } catch (Exception) { }
                        Ekle("", fileName, ll, 0, gizli);
                        Application.DoEvents();
                    }
                }

                string[] subdirectoryEntries = Directory.GetDirectories(yol);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    if (!durdur)
                    {
                        DosyaSayisi = 0;
                        KlasörSayisi = 0;
                        DosyaBoyutu = 0;

                        Application.DoEvents();

                        Ekle("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                        if (toolStripComboBox1.SelectedIndex == 0) HedefBilgileriniTopla(subdirectory); //adet, adet, MB
                        else { DosyaBoyutu = 0; KlasörSayisi = 0; DosyaSayisi = 0; }
                        Değiştir("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                    }
                }

                PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);
            }
            catch (Exception) { }

            çalışıyor = false;
            linkLabel1.Enabled = true;
            toolStripMenuItem4.Enabled = true;
            toolStripMenuItem5.Enabled = true;
            button1.Visible = false;
        }
        private void HedefBilgileriniTopla(string hedef)
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(hedef);
                foreach (string fileName in fileEntries)
                {
                    DosyaSayisi++;
                    try { DosyaBoyutu += new System.IO.FileInfo(fileName).Length; } catch (Exception) { }

                    Application.DoEvents();
                    if (durdur)
                    if (toolStripComboBox1.SelectedIndex == 1) return;

                    if (Environment.TickCount - sayacDeğiştir < 1000) continue;
                    sayacDeğiştir = Environment.TickCount;
                    Değiştir("*", fileName, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                }

                string[] subdirectoryEntries = Directory.GetDirectories(hedef);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    HedefBilgileriniTopla(subdirectory);
                    KlasörSayisi++;

                    Application.DoEvents();
                    if (durdur) return;
                    if (toolStripComboBox1.SelectedIndex == 1) return;

                    if (Environment.TickCount - sayacDeğiştir < 1000) continue;
                    sayacDeğiştir = Environment.TickCount;
                    Değiştir("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                }
            }
            catch (Exception)
            {
            }
            
        }

        private void Ekle(string bas, string b1, decimal b2, decimal b3, decimal b4)
        {
            if (b1.Length != 3) b1 = b1.Remove(0, linkLabel1.Text.Length);
            if (b1.Substring(0, 1) == "\\") b1 = b1.Remove(0, 1);
            b1 = bas + b1;
            if (dataGridView1.RowCount == 1)
            {
                if (dataGridView1[3, 0].Value.ToString() == "")
                {
                    dataGridView1[3, 0].Value = b1;
                    dataGridView1[2, 0].Value = D_DosyaBoyutu.Metne(b2);
                    dataGridView1[2, 0].Tag = b2;
                    dataGridView1[0, 0].Value = b3;
                    dataGridView1[1, 0].Value = b4;
                }
                else
                {
                    dataGridView1.RowCount++;
                    dataGridView1[3, dataGridView1.RowCount - 1].Value = b1;
                    dataGridView1[2, dataGridView1.RowCount - 1].Value = D_DosyaBoyutu.Metne(b2);
                    dataGridView1[2, dataGridView1.RowCount - 1].Tag = b2;
                    dataGridView1[0, dataGridView1.RowCount - 1].Value = b3;
                    dataGridView1[1, dataGridView1.RowCount - 1].Value = b4;
                }
            }
            else
            {
                dataGridView1.RowCount++;
                dataGridView1[3, dataGridView1.RowCount - 1].Value = b1;
                dataGridView1[2, dataGridView1.RowCount - 1].Value = D_DosyaBoyutu.Metne(b2);
                dataGridView1[2, dataGridView1.RowCount - 1].Tag = b2;
                dataGridView1[0, dataGridView1.RowCount - 1].Value = b3;
                dataGridView1[1, dataGridView1.RowCount - 1].Value = b4;
            }
        }
        private void Değiştir(string bas, string b1, decimal b2, decimal b3, decimal b4)
        {
            b1 = b1.Remove(0, linkLabel1.Text.Length);
            if (b1.Substring(0, 1) == "\\") b1 = b1.Remove(0, 1);
            b1 = bas + b1;

            dataGridView1[3, dataGridView1.RowCount - 1].Value = b1;
            dataGridView1[2, dataGridView1.RowCount - 1].Value = D_DosyaBoyutu.Metne(b2);
            dataGridView1[2, dataGridView1.RowCount - 1].Tag = b2;
            dataGridView1[0, dataGridView1.RowCount - 1].Value = b3;
            dataGridView1[1, dataGridView1.RowCount - 1].Value = b4;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            rowindex_grid = 0;

            if (çalışıyor || e.RowIndex == -1) return;

            string b = Convert.ToString(dataGridView1[3, e.RowIndex].Value);
            if (b == "") return;

            try
            {
                if (b.Substring(0, 1) == "*")
                {
                    //klasör
                    b = b.Remove(0, 1);

                    string den = linkLabel1.Text.Substring(linkLabel1.Text.Length - 1, 1);
                    if (den != "\\") linkLabel1.Text += "\\";

                    den = linkLabel1.Text + b;
                    Baslangiçİşlemleri(den);
                }
                else if (b.Substring(0, 1) == "-")
                {
                    b = b.Remove(0, 1);
                    Baslangiçİşlemleri(b);
                }
                else
                {
                    //dosya
                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = linkLabel1.Text + b;
                    process.Start();
                }
            }
            catch (Exception) {}
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowindex_grid = e.RowIndex;
        }
        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            decimal a = 0, b = 0;
            try
            {
                switch (e.Column.Name)
                {
                    case ("Klasör"):
                    case ("Dosya"):
                        a = (decimal)e.CellValue1;
                        b = (decimal)e.CellValue2;
                        break;

                    case ("Boyut"):
                        a = (decimal)dataGridView1[2, e.RowIndex1].Tag;
                        b = (decimal)dataGridView1[2, e.RowIndex2].Tag;
                        break;

                    default:
                        a = string.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
                        b = 0;            
                        break;
                }

                if (a > b) e.SortResult = 1;
                else if (a < b) e.SortResult = -1;
                else e.SortResult = 0;

                e.Handled = true;
            }
            catch (Exception) { e.Handled = false; }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (rowindex_grid < 0) return;
            string b = Convert.ToString(dataGridView1[3, rowindex_grid].Value);
            bool gizli = false;
            if (b.Substring(0, 1) == "*")
            {
                //klasör
                b = b.Remove(0, 1);
                b = linkLabel1.Text + b;

                try
                {
                    DirectoryInfo dir = new DirectoryInfo(b);
                    if ((dir.Attributes & FileAttributes.Hidden) > 0) gizli = true;
                    if (gizli) dir.Attributes = FileAttributes.Normal;
                    else dir.Attributes = FileAttributes.Hidden | FileAttributes.System;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                //dosya
                b = linkLabel1.Text + b;

                FileAttributes fileAttributes = File.GetAttributes(b);
                if (fileAttributes.HasFlag(FileAttributes.Hidden)) gizli = true;

                try
                {
                    if (gizli) File.SetAttributes(b, FileAttributes.Normal);
                    else File.SetAttributes(b, FileAttributes.Hidden | FileAttributes.System);
                }
                catch (Exception)
                {
                }
            }
        }
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (linkLabel1.Text == ".") return;

            if (rowindex_grid >= 0)
            {
                //klasör seçili
                string b = Convert.ToString(dataGridView1[3, rowindex_grid].Value);
                b = b.Remove(0, 1);

                if (linkLabel1.Text.Substring(linkLabel1.Text.Length - 1, 1) != "\\") linkLabel1.Text += "\\";
                b = linkLabel1.Text + b;
                if (b.Substring(b.Length - 1, 1) != "\\") b += "\\";

                _form1.Form2ToForm1(1, b);
            }
            else _form1.Form2ToForm1(1, linkLabel1.Text);
        }
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (linkLabel1.Text == ".") return;
            if (rowindex_grid >= 0)
            {
                //klasör seçili
                string b = Convert.ToString(dataGridView1[3, rowindex_grid].Value);
                b = b.Remove(0, 1);

                if (linkLabel1.Text.Substring(linkLabel1.Text.Length - 1, 1) != "\\") linkLabel1.Text += "\\";
                b = linkLabel1.Text + b;
                if (b.Substring(b.Length - 1, 1) != "\\") b += "\\";

                _form1.Form2ToForm1(2, b);
            }
            else _form1.Form2ToForm1(2, linkLabel1.Text); 
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            if (dataGridView1.SelectedRows.Count > 1) return;

            rowindex_grid = dataGridView1.SelectedRows[0].Index;

            string b = Convert.ToString(dataGridView1[3, rowindex_grid].Value), den = "";
            if (b == "") return;

            if (b.Substring(0, 1) == "*")
            {
                //klasör
                b = b.Remove(0, 1);

                den = linkLabel1.Text.Substring(linkLabel1.Text.Length - 1, 1);
                if (den != "\\") linkLabel1.Text += "\\";

                den = linkLabel1.Text + b;
            }
            else if (b.Substring(0, 1) == "-")
            {
                den = b.Remove(0, 1); 
            }
            else
            {
                //dosya
                den = linkLabel1.Text; 
            }

            KlasorGezgini frm2 = new KlasorGezgini(_form1);
            frm2.Show();
            frm2.Baslangiçİşlemleri(den);
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            if (dataGridView1.SelectedRows.Count > 1) return;

            rowindex_grid = dataGridView1.SelectedRows[0].Index;

            string b = Convert.ToString(dataGridView1[3, rowindex_grid].Value), den = "";
            if (b == "") return;

            if (b.Substring(0, 1) == "*")
            {
                //klasör
                b = b.Remove(0, 1);

                den = linkLabel1.Text.Substring(linkLabel1.Text.Length - 1, 1);
                if (den != "\\") linkLabel1.Text += "\\";

                den = linkLabel1.Text + b;
            }
            else if (b.Substring(0, 1) == "-")
            {
                den = b.Remove(0, 1);
            }
            else
            {
                //dosya
                den = linkLabel1.Text;
            }

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = den;
            process.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (çalışıyor) durdur = true;
        }
    }
}