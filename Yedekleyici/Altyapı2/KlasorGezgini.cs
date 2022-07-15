// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

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
        public string Sürüm = "V1.2";
        decimal DosyaSayisi, KlasörSayisi, DosyaBoyutu;
        int sayacDeğiştir;
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        bool durdur, çalışıyor;

        public KlasorGezgini()
        {
            InitializeComponent();
        }
        private void KlasorGezgini_Load(object sender, EventArgs e)
        {
            MenuSağ_Hesaplama.SelectedIndex = 0;
            PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, Ortak.Ayarlar, true, "KlasorGezgini", Location.X, Location.Y, Width, Height);
        }
        private void KlasorGezgini_FormClosed(object sender, FormClosedEventArgs e)
        {
            durdur = true;
        }

        private void Etiket_GeçerliYol_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string b = Etiket_GeçerliYol.Text;

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
            else if (Etiket_GeçerliYol.Text.Length == 3)
            {
                Liste.RowCount = 1;
                Liste[0, 0].Value = "";
                Liste[1, 0].Value = "";
                Liste[2, 0].Value = "";
                Liste[3, 0].Value = "";

                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives)
                {
                    Ekle("-", str, 0, 0, 0, str);
                }
                Etiket_GeçerliYol.Text = ".";
            }
        }
        public void Baslangiçİşlemleri(string yol)
        {
            if (string.IsNullOrEmpty(yol)) return;
            if (!yol.EndsWith(Path.DirectorySeparatorChar.ToString())) yol += Path.DirectorySeparatorChar;

            Liste.SuspendLayout();
            Etiket_GeçerliYol.Text = yol;
            Liste.RowCount = 1;
            Liste[0, 0].Value = "";
            Liste[1, 0].Value = "";
            Liste[2, 0].Value = "";
            Liste[3, 0].Value = "";

            if (Liste.SortedColumn != null)
            {
                DataGridViewColumn col = Liste.SortedColumn;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            Liste.ResumeLayout();

            Text = "Klasör Seçici";
            durdur = false;
            çalışıyor = true;
            Etiket_GeçerliYol.Enabled = false;
            Tuş_Durdur.Visible = true;
            Tuş_Yenile.Visible = false;
            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Belirsiz);

            DahaCokKarmasiklastirma_ DaÇoKa = new DahaCokKarmasiklastirma_();
            string Parola = null;

            Depo.Biri birisi = Ortak.ParolaŞablonu.Find(x => x.Adı == MenuSağ_ParolaŞablonu.Text);
            if (birisi.Adı == MenuSağ_ParolaŞablonu.Text && !string.IsNullOrEmpty(birisi.İçeriği))
            {
                Parola = DaÇoKa.Düzelt(birisi.İçeriği, Ortak.Parola);
            }

            string[] fileEntries = Ortak.Listele.Dosya(yol, SearchOption.TopDirectoryOnly);
            foreach (string fileName in fileEntries)
            {
                if (durdur) break;
                
                FileAttributes fileAttributes = File.GetAttributes(fileName);
                decimal gizli = 0;
                if (fileAttributes.HasFlag(FileAttributes.Hidden)) gizli = 1;
                long ll = 0; try { ll = new System.IO.FileInfo(fileName).Length; } catch (Exception) { }

                string çözümlenmişad;
                if (Parola != null && DaÇoKa.Düzelt(fileName, null, Parola, new DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_(), true))
                {
                    if (!string.IsNullOrEmpty(DaÇoKa.Düzelt_ÇıktısınıOku().AsılDosyaAdı) &&
                        Path.GetFileName(fileName) != DaÇoKa.Düzelt_ÇıktısınıOku().AsılDosyaAdı)
                    {
                        çözümlenmişad = Path.GetDirectoryName(fileName) + "\\" + DaÇoKa.Düzelt_ÇıktısınıOku().AsılDosyaAdı;
                    }
                    else çözümlenmişad = fileName;
                }
                else çözümlenmişad = fileName;

                if (!çözümlenmişad.Contains("MupYedekleyiciKlasorAdiDosyasi.mup")) Ekle("", çözümlenmişad, ll, 0, gizli, fileName);
                Application.DoEvents();
            }

            string[] subdirectoryEntries = Ortak.Listele.Klasör(yol, SearchOption.TopDirectoryOnly);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (durdur) break;
                    
                DosyaSayisi = 0;
                KlasörSayisi = 0;
                DosyaBoyutu = 0;

                Application.DoEvents();

                Ekle("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi, subdirectory);
                if (MenuSağ_Hesaplama.SelectedIndex == 0) HedefBilgileriniTopla(subdirectory); //adet, adet, MB
                else { DosyaBoyutu = 0; KlasörSayisi = 0; DosyaSayisi = 0; }

                Değiştir("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi, subdirectory);

                fileEntries = Ortak.Listele.Dosya(subdirectory, SearchOption.TopDirectoryOnly);
                foreach (string fileName in fileEntries)
                {
                    if (!durdur)
                    {
                        if (DaÇoKa.Düzelt(fileName, null, Parola, new DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_(), true))
                        {
                            if (DaÇoKa.Düzelt_ÇıktısınıOku().AsılDosyaAdı == "MupYedekleyiciKlasorAdiDosyasi.mup")
                            {
                                string st = Directory.GetParent(subdirectory).FullName + "\\" + DaÇoKa.Düzelt(D_HexYazı.BaytDizisinden(File.ReadAllBytes(fileName)), Parola);
                                Değiştir("*", st, DosyaBoyutu, KlasörSayisi, DosyaSayisi, subdirectory);
                                break;
                            }
                        }
                    }
                }
            }

            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);

            çalışıyor = false;
            Etiket_GeçerliYol.Enabled = true;
            Tuş_Durdur.Visible = false;
            Tuş_Yenile.Visible = true;
        }
        private void HedefBilgileriniTopla(string hedef)
        {
            try
            {
                string[] fileEntries = Ortak.Listele.Dosya(hedef, SearchOption.TopDirectoryOnly);
                foreach (string fileName in fileEntries)
                {
                    DosyaSayisi++;
                    try { DosyaBoyutu += new System.IO.FileInfo(fileName).Length; } catch (Exception) { }

                    Application.DoEvents();
                    if (durdur)
                    if (MenuSağ_Hesaplama.SelectedIndex == 1) return;

                    if (Environment.TickCount - sayacDeğiştir < 1000) continue;
                    sayacDeğiştir = Environment.TickCount;
                    Değiştir("*", fileName, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                }

                string[] subdirectoryEntries = Ortak.Listele.Klasör(hedef, SearchOption.TopDirectoryOnly);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    HedefBilgileriniTopla(subdirectory);
                    KlasörSayisi++;

                    Application.DoEvents();
                    if (durdur) return;
                    if (MenuSağ_Hesaplama.SelectedIndex == 1) return;

                    if (Environment.TickCount - sayacDeğiştir < 1000) continue;
                    sayacDeğiştir = Environment.TickCount;
                    Değiştir("*", subdirectory, DosyaBoyutu, KlasörSayisi, DosyaSayisi);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Ekle(string bas, string b1, decimal b2, decimal b3, decimal b4, string ipucu)
        {
            if (Liste.RowCount < 1 || Liste.ColumnCount < 4) return;

            if (b1.Length != 3) b1 = b1.Remove(0, Etiket_GeçerliYol.Text.Length);
            if (b1.Substring(0, 1) == "\\") b1 = b1.Remove(0, 1);
            b1 = bas + b1;

            if (Liste.RowCount == 1)
            {
                if (Liste[3, 0].Value.ToString() == "")
                {
                    Liste[3, 0].Value = b1;
                    Liste[3, 0].ToolTipText = ipucu;
                    Liste[2, 0].Value = D_DosyaBoyutu.Yazıya(b2);
                    Liste[2, 0].Tag = b2;
                    Liste[0, 0].Value = b3;
                    Liste[1, 0].Value = b4;
                }
                else
                {
                    Liste.RowCount++;
                    Liste[3, Liste.RowCount - 1].Value = b1;
                    Liste[3, Liste.RowCount - 1].ToolTipText = ipucu;
                    Liste[2, Liste.RowCount - 1].Value = D_DosyaBoyutu.Yazıya(b2);
                    Liste[2, Liste.RowCount - 1].Tag = b2;
                    Liste[0, Liste.RowCount - 1].Value = b3;
                    Liste[1, Liste.RowCount - 1].Value = b4;
                }
            }
            else
            {
                Liste.RowCount++;
                Liste[3, Liste.RowCount - 1].Value = b1;
                Liste[3, Liste.RowCount - 1].ToolTipText = ipucu;
                Liste[2, Liste.RowCount - 1].Value = D_DosyaBoyutu.Yazıya(b2);
                Liste[2, Liste.RowCount - 1].Tag = b2;
                Liste[0, Liste.RowCount - 1].Value = b3;
                Liste[1, Liste.RowCount - 1].Value = b4;
            }
        }
        private void Değiştir(string bas, string b1, decimal b2, decimal b3, decimal b4, string ipucu = "")
        {
            if (Liste.RowCount < 1 || Liste.ColumnCount < 4) return;

            b1 = b1.Remove(0, Etiket_GeçerliYol.Text.Length);
            if (b1.Substring(0, 1) == "\\") b1 = b1.Remove(0, 1);
            b1 = bas + b1;

            Liste[3, Liste.RowCount - 1].Value = b1;
            Liste[3, Liste.RowCount - 1].ToolTipText = ipucu;
            Liste[2, Liste.RowCount - 1].Value = D_DosyaBoyutu.Yazıya(b2);
            Liste[2, Liste.RowCount - 1].Tag = b2;
            Liste[0, Liste.RowCount - 1].Value = b3;
            Liste[1, Liste.RowCount - 1].Value = b4;
        }

        private void Liste_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (çalışıyor || e.RowIndex == -1) return;
            if (string.IsNullOrEmpty(Liste[3, e.RowIndex].ToolTipText)) return;
            string gecici = Convert.ToString(Liste[3, e.RowIndex].Value);
            if (string.IsNullOrEmpty(gecici)) return;

            if (File.Exists(Liste[3, e.RowIndex].ToolTipText))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Liste[3, e.RowIndex].ToolTipText;
                process.Start();
            }
            else if (Directory.Exists(Liste[3, e.RowIndex].ToolTipText))
            {
                Baslangiçİşlemleri(Liste[3, e.RowIndex].ToolTipText);
            }
        }
        private void Liste_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
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
                        a = (decimal)Liste[2, e.RowIndex1].Tag;
                        b = (decimal)Liste[2, e.RowIndex2].Tag;
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

        private void Tuş_Yenile_Click(object sender, EventArgs e)
        {
            Baslangiçİşlemleri(Etiket_GeçerliYol.Text);
        }

        private void MenuSağ_GösterGizle_Click(object sender, EventArgs e)
        {
            if (Liste.SelectedRows.Count == 0) return;
            if (Liste.SelectedRows.Count > 1) return;

            int rowindex_grid = Liste.SelectedRows[0].Index;
            string yol = Liste[3, rowindex_grid].ToolTipText;

            bool gizli = false;
            if (File.Exists(yol))
            {
                FileAttributes fileAttributes = File.GetAttributes(yol);
                if (fileAttributes.HasFlag(FileAttributes.Hidden)) gizli = true;

                try
                {
                    if (gizli) File.SetAttributes(yol, FileAttributes.Normal);
                    else File.SetAttributes(yol, FileAttributes.Hidden | FileAttributes.System);
                }
                catch (Exception) { }
            }
            else if (Directory.Exists(yol))
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(yol);
                    if ((dir.Attributes & FileAttributes.Hidden) > 0) gizli = true;
                    if (gizli) dir.Attributes = FileAttributes.Normal;
                    else dir.Attributes = FileAttributes.Hidden | FileAttributes.System;
                }
                catch (Exception) { }
            }
        }
        private void MenuSağ_YeniSayfadaAç_Click(object sender, EventArgs e)
        {
            if (Liste.SelectedRows.Count == 0) return;
            if (Liste.SelectedRows.Count > 1) return;

            int rowindex_grid = Liste.SelectedRows[0].Index;

            string b = Convert.ToString(Liste[3, rowindex_grid].Value), den = "";
            if (b == "") return;

            if (b.Substring(0, 1) == "*")
            {
                //klasör
                b = b.Remove(0, 1);

                den = Etiket_GeçerliYol.Text.Substring(Etiket_GeçerliYol.Text.Length - 1, 1);
                if (den != "\\") Etiket_GeçerliYol.Text += "\\";

                den = Etiket_GeçerliYol.Text + b;
            }
            else if (b.Substring(0, 1) == "-")
            {
                den = b.Remove(0, 1); 
            }
            else
            {
                //dosya
                den = Etiket_GeçerliYol.Text; 
            }

            KlasorGezgini frm2 = new KlasorGezgini();
            frm2.Show();
            frm2.Baslangiçİşlemleri(den);
        }
        private void MenuSağ_DosyaGezginindeAç_Click(object sender, EventArgs e)
        {
            if (Liste.SelectedRows.Count == 0) return;
            if (Liste.SelectedRows.Count > 1) return;

            int rowindex_grid = Liste.SelectedRows[0].Index;

            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", Liste[3, rowindex_grid].ToolTipText));
        }
        private void Tuş_Durdur_Click(object sender, EventArgs e)
        {
            if (çalışıyor) durdur = true;
        }
    }
}