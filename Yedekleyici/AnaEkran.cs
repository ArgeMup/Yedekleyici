// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Dönüştürme;

namespace Yedekleyici
{
    public partial class AnaEkran : Form
    {
        bool Ayarlar_DallarıKendiliğinden_AçKapat = true;

        public AnaEkran()
        {
            InitializeComponent();
        }
        void AnaEkran_Load(object sender, EventArgs e)
        {
            Ortak.AnaEkran = this;
            Ortak.Günlük_MetinKutusu = Günlük_MetinKutusu;
            Ortak.Düzlem = Düzlem_Talepler;
            Ortak.Buton_Günlük = Buton_Günlük;

            DateTime date_terhis = new DateTime(2011, 05, 05, 09, 20, 10);
            DateTime dates_simdi = DateTime.Now;
            TimeSpan fark = dates_simdi.Subtract(date_terhis);

            string ProgAdVer = "Mup Yedekleyici V" + Application.ProductVersion + " " + fark.Days;
            if (Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower() != "yedekleyici") ProgAdVer += " (" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ")";
            this.Text = ProgAdVer;

            if (File.Exists(Ortak.pak_Banka + "GizliMenuleriGoster.Ayarlar")) Ortak.GizliMenüleriGöster = true;

            bool sonuç;
        YenidenDene_Ayarlar:
            Ortak.Ayarlar = new Ayarlar_(out sonuç, "", Ortak.pak_Banka + "\\.Yedekleyici.Ayarlar");
            if (!sonuç)
            {
                File.Delete(Ortak.pak_Banka + "\\.Yedekleyici.Ayarlar");
                goto YenidenDene_Ayarlar;
            }

            Ayarlar_KarakterBüyüklüğü.Value = Convert.ToDecimal(Ortak.Ayarlar.Oku("Ayarlar_KarakterBüyüklüğü", "8"));
            Ortak.PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, Ortak.Ayarlar);
            Ortak.PeTeİkKo.TepsiİkonunuBaşlat();
            Ortak.PeTeİkKo.Tepsiİkonu.ContextMenuStrip = MenuSağ_Uyg;

            Ortak.UygulamaOncedenCalistirildiMi = new UygulamaOncedenCalistirildiMi_();
            if (Ortak.UygulamaOncedenCalistirildiMi.KontrolEt(Application.ExecutablePath))
            {
                Ortak.UygulamaOncedenCalistirildiMi.DiğerUygulamayıÖneGetir();
                Ortak.PeTeİkKo.MetniBaloncuktaGöster("Aynı klasörden birden fazla çalıştırılmamalıdır, kapatılacaktır.\r\r" + Application.ExecutablePath, ToolTipIcon.Info, 10000);
                Hide();
                System.Threading.Thread.Sleep(5000);
                Application.Exit();
                return;
            }

            Directory.CreateDirectory(Ortak.pak_Banka);
            Directory.CreateDirectory(Ortak.pak_Şablon);
            File.WriteAllBytes(Ortak.pak_Şablon + ".Yedekleyici_Senaryo", Properties.Resources.Senaryo);
            File.WriteAllBytes(Ortak.pak_Şablon + ".Yedekleyici_Talep", Properties.Resources.Talep);
            File.WriteAllBytes(Ortak.pak_Şablon + ".Yedekleyici_DosyaAtlamaListesi", Properties.Resources.DosyaAtlamaListesi);
            File.WriteAllBytes(Ortak.pak_Şablon + "GizliMenuleriGoster.Ayarlar", Properties.Resources.GizliMenuleriGoster);

            File.Delete(Ortak.pak_Banka + "Gunluk.csv");
        }
        void AnaEkran_Shown(object sender, EventArgs e)
        {
            while (Panel_AnaEkran.Controls.Count > 0) Panel_AnaEkran.Controls.RemoveAt(0);
            Panel_AnaEkran.Controls.Add(Panel_Senaryo);
            Panel_AnaEkran.Controls.Add(Panel_Talepler);
            Panel_AnaEkran.Controls.Add(Panel_KlasörleriListele);
            Panel_AnaEkran.Controls.Add(Panel_KlasörleriKarıştır);
            Panel_AnaEkran.Controls.Add(Panel_Ayarlar);
            Panel_AnaEkran.Controls.Add(Panel_Günlük);
            Panel_Aç(Ortak.PanelListesi.Senaryo);

            #region Kalıp Kontrolü
            bool sonuç = true;
            List<string> Kalıp_Dosyaları = new List<string>();
            Kalıp_Dosyaları.AddRange(Ortak.Listele.Dosya(Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Talep"));
            Kalıp_Dosyaları.AddRange(Ortak.Listele.Dosya(Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Senaryo"));
            string kalıplar = Ortak.Ayarlar.Oku("Kalıplar");
            foreach (var dosya in Kalıp_Dosyaları)
            {
                string kalıp = Ortak.Ayarlar.Oku_AltDal(kalıplar, dosya);
                if (string.IsNullOrEmpty(kalıp)) { sonuç = false; break; }

                byte[] dosyaiçeriği_dizi = File.ReadAllBytes(dosya);
                dosyaiçeriği_dizi = D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(dosyaiçeriği_dizi);
                string dosyaiçeriği_metin = D_HexMetin.BaytDizisinden(dosyaiçeriği_dizi);
                if (kalıp != dosyaiçeriği_metin) { sonuç = false; break; }
            }
            if (!sonuç)
            {
                if (Ortak.GizliMenüleriGöster) Buton_KalıplarıGüncelle_Click(null, null);
                else
                {
                    Buton_KalıplarıGüncelle.Visible = true;
                    Buton_KalıplarıGüncelle.Enabled = true;
                    Buton_KalıplarıGüncelle.Dock = DockStyle.Fill;
                    Panel_Aç(Ortak.PanelListesi.Ayarlar);

                    Günlük_Zamanlayıcı.Enabled = true;
                    return;
                }
            }
            #endregion

            Senaryo.Ortak.BilinenleriAç(Ağaç_Senaryo);
            Talep.Ortak.Listele();

            Buton_Talepler.DropDownItems.Clear();
            (MenuSağ_Uyg.Items[1] as ToolStripMenuItem).DropDownItems.Clear();
            foreach (var t in Talep.Ortak.Liste)
            {
                #region Önyüz
                ToolStripMenuItem ts = new ToolStripMenuItem(t.Key);
                ts.DisplayStyle = ToolStripItemDisplayStyle.Text;
                Buton_Talepler.DropDownItems.Add(ts);

                ToolStripMenuItem ts_1 = new ToolStripMenuItem("Çalıştır");
                ts_1.Click += Buton_Talepler_AltButonlar_Click;
                ts_1.ToolTipText = t.Key;
                ts_1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ts.DropDownItems.Add(ts_1);

                ToolStripMenuItem ts_2 = new ToolStripMenuItem("Talep Dosyasi");
                ts_2.Click += Buton_Talepler_AltButonlar_Click;
                ts_2.ToolTipText = t.Value[0];
                ts_2.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ts.DropDownItems.Add(ts_2);

                ToolStripMenuItem ts_3 = new ToolStripMenuItem("Kaynak Klasör");
                ts_3.Click += Buton_Talepler_AltButonlar_Click;
                ts_3.ToolTipText = t.Value[1];
                ts_3.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ts.DropDownItems.Add(ts_3);

                ToolStripMenuItem ts_4 = new ToolStripMenuItem("Hedef Klasör");
                ts_4.Click += Buton_Talepler_AltButonlar_Click;
                ts_4.ToolTipText = t.Value[2];
                ts_4.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ts.DropDownItems.Add(ts_4);
                #endregion

                #region MenuSağ
                ToolStripMenuItem ts_5 = new ToolStripMenuItem(t.Key);
                ts_5.Click += Buton_Talepler_AltButonlar_Click;
                ts_5.ToolTipText = t.Key;
                ts_5.DisplayStyle = ToolStripItemDisplayStyle.Text;
                (MenuSağ_Uyg.Items[1] as ToolStripMenuItem).DropDownItems.Add(ts_5);
                #endregion
            }

            while ((MenuSağ_Uyg.Items[0] as ToolStripMenuItem).DropDownItems.Count > 1) (MenuSağ_Uyg.Items[0] as ToolStripMenuItem).DropDownItems.RemoveAt(1);
            foreach (Senaryo.Bir_Senaryo_ sny in Senaryo.Ortak.Senaryo_Listesi)
            {
                ToolStripMenuItem ts_sny = new ToolStripMenuItem(sny.Tanim);
                ts_sny.Click += MenuSağ_Uyg_Senaryo_TümünüDurdur_Click;
                (MenuSağ_Uyg.Items[0] as ToolStripMenuItem).DropDownItems.Add(ts_sny);
            }

            ParŞab_Liste.Items.Clear();
            Ortak.ParolaŞablonu = Ortak.Ayarlar.Listele_AltDal(Ortak.Ayarlar.Oku("ParolaŞablonu"));
            foreach (var ş in Ortak.ParolaŞablonu) ParŞab_Liste.Items.Add(ş.Adı);

            if (Ortak.GizliMenüleriGöster)
            {
                //Günlük_Zamanlayıcı.Interval = 100;
                Buton_Deneme.Visible = true;
            }

            if (Directory.Exists(Ortak.pak_Geçici))
            {
                string tlp_adı = "GeciciDosyalariSil";
                string[] tlp_içeriği = { "Tanim;" + tlp_adı + Environment.NewLine,
                                         "Kaynak;" + Ortak.pak_Geçici + Environment.NewLine,
                                         "Hedef;" + Ortak.pak_Geçici + Environment.NewLine,
                                         "Dosyalari;Sil" };
                File.WriteAllLines(Ortak.pak_Geçici + tlp_adı + ".Yedekleyici_Talep", tlp_içeriği);
                Talep.Ortak.Liste.Add(tlp_adı, new string[] { Ortak.pak_Geçici + tlp_adı + ".Yedekleyici_Talep", "", "" });

                string[] sny_içeriği = { "Tanim;Geçici Klasörü Sil" + Environment.NewLine,
                                         ">;E;Komut;Talep;" + tlp_adı };
                Senaryo.Bir_Senaryo_ sny_kendi = new Senaryo.Bir_Senaryo_(sny_içeriği);
                Senaryo.Ortak.Senaryo_Başlat(sny_kendi);
            }

            Günlük_Zamanlayıcı.Enabled = true;
        }
        void AnaEkran_FormClosed(object sender, FormClosedEventArgs e)
        {
            Anaİzin(false);
        }

        void Buton_Deneme_Click(object sender, EventArgs e)
        {
            AnaEkran_FormClosed(null, null);
            AnaEkran_Shown(null, null);
            //Senaryo.Ortak.Senaryo_Başlat(Senaryo.Ortak.Senaryo_Listesi[0].Tanim);
        }
        void Buton_Senaryo_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.Senaryo);
        }
        void Buton_Talepler_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.Talepler);
        }
        void Buton_Talepler_AltButonlar_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem; string girdi = tsmi.ToolTipText;
            try { girdi = Senaryo.Değişken.Oku_MetinVeyaDeğişken(tsmi.ToolTipText); } catch (Exception) { }

            if (File.Exists(girdi)) System.Diagnostics.Process.Start("notepad.exe", girdi);
            else if (Directory.Exists(girdi)) System.Diagnostics.Process.Start(girdi);
            else
            {
                Ortak.Dsi.TümünüDurdur();

                string sny_adı = D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), 6);
                string[] sny_içeriği = { "Tanim;" + sny_adı + Environment.NewLine,
                                         ">;E;Komut;Talep;" + girdi };

                Senaryo.Bir_Senaryo_ sny_kendi = new Senaryo.Bir_Senaryo_(sny_içeriği);
                Senaryo.Ortak.Senaryo_Başlat(sny_kendi);
            }
        }
        void Buton_KlasörleriListele_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.KlasörleriListele);
        }
        void Buton_KlasörleriKarıştır_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.KlasörleriKarıştır);
        }
        void Buton_Günlük_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.Günlük);
            Buton_Günlük.Image = Properties.Resources.M_Gunluk;
        }
        void Buton_Ayarlar_Click(object sender, EventArgs e)
        {
            Panel_Aç(Ortak.PanelListesi.Ayarlar); 
        }
        void Buton_KalıplarıGüncelle_Click(object sender, EventArgs e)
        {
            Buton_KalıplarıGüncelle.Text = "Uygulamayı Tekrar Başlatın";
            Application.DoEvents();

            List<string> Kalıp_Dosyaları = new List<string>();
            Kalıp_Dosyaları.AddRange(Ortak.Listele.Dosya(Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Talep"));
            Kalıp_Dosyaları.AddRange(Ortak.Listele.Dosya(Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Senaryo"));
            string kalıplar = "";
            foreach (var dosya in Kalıp_Dosyaları)
            {
                byte[] dosyaiçeriği_dizi = File.ReadAllBytes(dosya);
                dosyaiçeriği_dizi = D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(dosyaiçeriği_dizi);
                string dosyaiçeriği_metin = D_HexMetin.BaytDizisinden(dosyaiçeriği_dizi);

                Ortak.Ayarlar.Yaz_AltDal(ref kalıplar, dosya, dosyaiçeriği_metin);
            }
            Ortak.Ayarlar.Yaz("Kalıplar", kalıplar);

            kalıplar = Ortak.pak_AnaKlasör + "Yedek\\Kalip_" + (Environment.MachineName + "." + Environment.UserName).ToUpper() + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".zip";
            Directory.CreateDirectory(Path.GetDirectoryName(kalıplar));
            System.IO.Compression.ZipFile.CreateFromDirectory(Ortak.pak_Banka, kalıplar, System.IO.Compression.CompressionLevel.Optimal, true);

            Application.DoEvents();
            System.Threading.Thread.Sleep(500);

            if (Ortak.GizliMenüleriGöster) { Buton_KalıplarıGüncelle.Enabled = false; Buton_KalıplarıGüncelle.Visible = false; }
            else Close();
        }
        void Buton_Çalışıyor_Click(object sender, EventArgs e)
        {
            Anaİzin(!(Buton_Çalışıyor.Tag == null));
        }

        void ParŞab_Par1_TextChanged(object sender, EventArgs e)
        {
            ParŞab_Ekle.Enabled = ParŞab_Par1.Text == ParŞab_Par2.Text;
        }
        void ParŞab_Ekle_Click(object sender, EventArgs e)
        {
            if (ParŞab_Tanım.Text == "" || ParŞab_Par1.Text == "")
            {
                Ortak.DurumBildirimi.BaloncukluUyarı("Boş olamaz", ToolTipIcon.Error, 3500, ParŞab_Tanım);
                return;
            }

            if (ParŞab_Liste.Items.Contains(ParŞab_Tanım.Text))
            {
                Ortak.DurumBildirimi.BaloncukluUyarı("Benzersiz bir tanım seçilmeli", ToolTipIcon.Error, 3500, ParŞab_Liste);
                return;
            }

            ParŞab_Sil.Enabled = false;
            ParŞab_Liste.Items.Add(ParŞab_Tanım.Text);

            string g = ""; 
            Ortak.ParolaŞablonu.Add(new Depo.Biri(ParŞab_Tanım.Text, Ortak.Karıştır(D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(ParŞab_Par1.Text), 128)), Ortak.Parola)));
            Ortak.Ayarlar.ListeyiEkle_AltDal(ref g, Ortak.ParolaŞablonu);
            Ortak.Ayarlar.Yaz("ParolaŞablonu", g);
        }
        void ParŞab_Liste_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParŞab_Sil.Enabled = true;
        }
        void ParŞab_Sil_Click(object sender, EventArgs e)
        {
            string g = "";
            Ortak.Ayarlar.ListeyiEkle_AltDal(ref g, Ortak.ParolaŞablonu);
            Ortak.Ayarlar.Sil_AltDal(ref g, ParŞab_Liste.SelectedItem.ToString());
            Ortak.Ayarlar.Yaz("ParolaŞablonu", g);

            ParŞab_Liste.Items.Remove(ParŞab_Liste.SelectedItem);

            ParŞab_Sil.Enabled = false;
        }

        void Ayarlar_KarakterBüyüklüğü_ValueChanged(object sender, EventArgs e)
        {
            PuntoDeğişti((float)Ayarlar_KarakterBüyüklüğü.Value);
            Ortak.Ayarlar.Yaz("Ayarlar_KarakterBüyüklüğü", Ayarlar_KarakterBüyüklüğü.Value.ToString());
        }
        void Günlük_Zamanlayıcı_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Ortak.UygulamayıKapatmaSayacı > 0)
                {
                    Ortak.UygulamayıKapatmaSayacı -= Günlük_Zamanlayıcı.Interval;
                    if (Ortak.UygulamayıKapatmaSayacı <= 0) Close();
                }

                if (Buton_KalıplarıGüncelle.Enabled == true)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    Show();
                    BringToFront();

                    Ortak.PeTeİkKo.MetniBaloncuktaGöster("Kalıpları Güncelle", ToolTipIcon.Error);
                }

                if (Buton_Çalışıyor.Tag == null)
                {
                    //Talep sayfasındaki eski görsellerin silinmesi
                    foreach (Talep.Görsel_ biri in Ortak.Düzlem.Controls)
                    {
                        if (DateTime.Now - biri.SonGüncellenme > TimeSpan.FromHours(5))
                        {
                            biri.Dispose();
                            Ortak.Düzlem.Controls.Remove(biri);
                        }
                    }

                    //Dalları kırpma
                    if (WindowState != FormWindowState.Minimized && Ortak.EtkinPanel == Ortak.PanelListesi.Senaryo)
                    {
                        foreach (TreeNode biri in Ağaç_Senaryo.Nodes)
                        {
                            if (biri.ToolTipText.Contains("hatalı")) continue;

                            Ortak.AğaçGörseliniGüncelle(biri, Ağaç_AltDallarınSimgesiniKkontrolet(biri));
                        }
                    }

                    //Senaryoları çalıştır
                    foreach (Senaryo.Bir_Senaryo_ biri in Senaryo.Ortak.Senaryo_Listesi)
                    {
                        if (biri.TekrarÇalıştırmaZamanAralığı_msn > 0)
                        {
                            if (biri.TekrarÇalıştırmaZamanAralığı_msn_sayac < Environment.TickCount)
                            {
                                Senaryo.Ortak.Senaryo_Başlat(biri);
                                biri.TekrarÇalıştırmaZamanAralığı_msn_sayac = Environment.TickCount + biri.TekrarÇalıştırmaZamanAralığı_msn;
                            }
                        }
                    }
                } 
                else if ((DateTime.Now - (DateTime)Buton_Çalışıyor.Tag).TotalSeconds > 30)
                {
                    Buton_Çalışıyor.Image = Properties.Resources.D_Yeni;
                    Refresh();
                    System.Media.SystemSounds.Asterisk.Play();
                    System.Threading.Thread.Sleep(500);
                    Buton_Çalışıyor.Image = Properties.Resources.D_Hata;

                    Buton_Çalışıyor.Tag = DateTime.Now;
                }
            }
            catch (Exception ex) { Ortak.Günlük_Ekle(ex.ToString()); }
        }
        int Ağaç_AltDallarınSimgesiniKkontrolet(TreeNode Dal)
        {
            int[] AltDallar = new int[Dal.Nodes.Count+1];
            for (int i = 0; i < Dal.Nodes.Count; i++) AltDallar[i] = Ağaç_AltDallarınSimgesiniKkontrolet(Dal.Nodes[i]);
            
            if (Dal.Parent == null) AltDallar[Dal.Nodes.Count] = (int)Ortak.Resimler.EnFazla;
            else AltDallar[Dal.Nodes.Count] = Dal.ImageIndex;

            int EnDüşük = AltDallar.Min();

            if (Ayarlar_DallarıKendiliğinden_AçKapat)
            {
                if (EnDüşük >= (int)Ortak.Resimler.Tamam) { if (Dal.IsExpanded) Dal.Collapse(true); }
                else { if (!Dal.IsExpanded) Dal.Expand(); }
            }

            return EnDüşük;
        }

        void MenuSağ_Senaryo_Başlat_Click(object sender, EventArgs e)
        {
            if (Ağaç_Senaryo.SelectedNode.Parent == null)
            {
                Senaryo.Ortak.Senaryo_Başlat(Ağaç_Senaryo.SelectedNode.Text);
            }
            else
            {
                Senaryo.Bir_Adım_ Adım = (Senaryo.Bir_Adım_)Ağaç_Senaryo.SelectedNode.Tag;
                if (Adım.Seviye > 0)
                {
                    Adım.Senaryo.Durdurmaİsteği = false;
                    Senaryo.Ortak.Adım_Başlat(Adım);
                }
            }
        }
        void MenuSağ_Senaryo_Durdur_Click(object sender, EventArgs e)
        {
            TreeNode tn = Ağaç_Senaryo.SelectedNode;
            while (tn.Parent != null) tn = tn.Parent;

            Senaryo.Ortak.Senaryo_Durdur(tn.Text);
        }
        void MenuSağ_Senaryo_Dosya_Click(object sender, EventArgs e)
        {
            if (Ağaç_Senaryo.SelectedNode.Parent == null)
            {
                Senaryo.Bir_Senaryo_ sny = Senaryo.Ortak.Senaryo_Bul(Ağaç_Senaryo.SelectedNode.Text);
                if (sny != null)
                {
                    System.Diagnostics.Process.Start("notepad.exe", sny.DosyaYolu);
                }
            }
        }
        void MenuSağ_Senaryo_Genişlet_Click(object sender, EventArgs e)
        {
            Ağaç_Senaryo.SelectedNode.ExpandAll();
        }
        void MenuSağ_Senaryo_Daralt_Click(object sender, EventArgs e)
        {
            Ağaç_Senaryo.SelectedNode.Collapse(false);
        }

        void Ağaç_Senaryo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Ağaç_Senaryo.SelectedNode = e.Node;
            Ayarlar_DallarıKendiliğinden_AçKapat = false;
        }
        void Ağaç_Senaryo_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        void MenuSağ_Uyg_Çıkış_Click(object sender, EventArgs e)
        {
            Close();
        }
        void MenuSağ_Uyg_Senaryo_TümünüDurdur_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi.Text == "Tümünü Durdur") AnaEkran_FormClosed(null, null);
            else Senaryo.Ortak.Senaryo_Başlat(tsmi.Text);
        }

        void PuntoDeğişti(float Boyut)
        {
            Ortak.KarakterBüyüklüğü = Boyut;

            statusStrip1.ImageScalingSize = new Size((int)Boyut * 2, (int)Boyut * 2);
            statusStrip1.Font = new Font(statusStrip1.Font.FontFamily, Boyut);
            this.Font = new Font(this.Font.FontFamily, Boyut);

            ResimListesi.ImageSize = new Size((int)(Boyut * 1.75), (int)(Boyut * 1.75));
            ResimListesi.Images.Clear();
            ResimListesi.Images.Add(Properties.Resources.D_Calisiyor);
            ResimListesi.Images.Add(Properties.Resources.D_Hata);
            ResimListesi.Images.Add(Properties.Resources.D_Tamam);
            ResimListesi.Images.Add(Properties.Resources.D_Tamam_Hayir);
            ResimListesi.Images.Add(Properties.Resources.D_Yeni);
            ResimListesi.Images.Add(Properties.Resources.M_Senaryo);
            ResimListesi.Images.Add(Properties.Resources.M_Talepler);
            ResimListesi.Images.Add(Properties.Resources.M_KlasorleriListele);
            ResimListesi.Images.Add(Properties.Resources.M_KlasorleriKaristirDuzelt);
            ResimListesi.Images.Add(Properties.Resources.M_Ayarlar);
        }
        void Panel_Aç(Ortak.PanelListesi Adı)
        {
            Ayarlar_DallarıKendiliğinden_AçKapat = true;

            if (Ortak.EtkinPanel == Adı) return;

            for (int i = 0; i < Panel_AnaEkran.Controls.Count; i++) Panel_AnaEkran.Controls[i].Visible = false;
            Panel_AnaEkran.Controls[(int)Adı].Visible = true;
            Ortak.EtkinPanel = Adı;
        }
        void Anaİzin(bool Aç)
        {
            if (Aç)
            {
                Buton_Çalışıyor.Image = Properties.Resources.D_Tamam;
                Buton_Çalışıyor.Tag = null;

                foreach (var s in Senaryo.Ortak.Senaryo_Listesi) s.TekrarÇalıştırmaZamanAralığı_msn_sayac = 0;
            }
            else
            {
                Buton_Çalışıyor.Image = Properties.Resources.D_Hata;
                Buton_Çalışıyor.Tag = DateTime.Now;

                foreach (var s in Senaryo.Ortak.Senaryo_Listesi)
                {
                    s.Durdurmaİsteği = true;
                    while (s.ÇalışanGörevAdedi > 0) { System.Threading.Thread.Sleep(100); Application.DoEvents(); }
                }

                Ortak.Dsi.TümünüDurdur();
                Senaryo.Değişken.TümünüSil();
            }
        }
		void Panel_KlasörleriListele_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        void Panel_KlasörleriListele_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string alinan;
            foreach (string file in files)
            {
                if (File.Exists(file)) alinan = Path.GetDirectoryName(file);
                else if (Directory.Exists(file)) alinan = file;
                else alinan = "";

                if (alinan != "")
                {
                    KlasorGezgini frm2 = new KlasorGezgini();
                    frm2.Show();
                    frm2.Baslangiçİşlemleri(alinan);
                }
            }
        }

        void GeDönülemezKa_Girdi_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string[] dizi = GeDönülemezKa_Girdi.Text.Split('|');
                GeDönülemezKa_Çıktı.Text = D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(dizi[1], Convert.ToInt32(dizi[0]));
            }
            catch (Exception) { GeDönülemezKa_Girdi.Text = "32|" + GeDönülemezKa_Girdi.Text; }
        }
        void GeDönülebilirKa_Parola_TextChanged(object sender, EventArgs e)
        {
            string sonuç = Ortak.Düzelt(GeDönülebilirKa_Girdi.Text, GeDönülebilirKa_Parola.Text);
            if (sonuç == "") sonuç = Ortak.Karıştır(GeDönülebilirKa_Girdi.Text, GeDönülebilirKa_Parola.Text);
            GeDönülebilirKa_Çıktı.Text = sonuç;
        }
    }
}
