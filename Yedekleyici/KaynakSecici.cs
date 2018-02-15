using System.Windows.Forms;
using System.IO;
using System;
using System.Collections.Generic;
using ArgeMup.HazirKod;

namespace Yedekleyici
{
    public partial class KaynakSecici_ : Form
    {
        public string TamListe = "";

        string KaynakYoluAsılKopyası;
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        private readonly AnaEkran _form1;

        public KaynakSecici_(AnaEkran form1)
        {
            _form1 = form1;
            InitializeComponent();
        }
        void KaynakSecici_Load(object sender, EventArgs e)
        {
            PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, _form1.Ayarlar, true, "KaynakSecici", Location.X, Location.Y, Width, Height);
            Arama.Tag = 0;
        }
        void KaynakSecici_FormClosing(object sender, FormClosingEventArgs e)
        {
            Arama.Text = "";
            Arama.Focus();//son yapılan değişikliğin kayda alınması için
            Application.DoEvents();
            int Süre = Environment.TickCount + 500;
            Arama.Tag = 0;

            List<Kaynakça_.BirFiltre_> K1 = new List<Kaynakça_.BirFiltre_>();

            for (int i = 0; i < Liste.RowCount; i++)
            {
                if ((string)Liste[0, i].Value != "Yedekle")
                {
                    Kaynakça_.BirFiltre_ Yeni = new Kaynakça_.BirFiltre_();
                    if ((string)Liste[2, i].Value == "Kök") Yeni.Yol = KaynakYoluAsılKopyası.ToLower();
                    else Yeni.Yol = (KaynakYoluAsılKopyası + Liste[2, i].Value).ToLower();

                    if (((string)Liste[0, i].Value).Contains("Soyadlarını"))
                    {
                        if ((string)Liste[0, i].Value == "Soyadlarını atla") Yeni.Tip = Kaynakça_.Tip_.SoyadlarınıAtla;
                        else Yeni.Tip = Kaynakça_.Tip_.SoyadlarınıYedekle;

                        if (string.IsNullOrEmpty((string)Liste[1, i].Value))
                        {
                            Liste[0, i].Selected = true;
                            e.Cancel = true;
                            return;
                        }

                        Yeni.Soyadları = ((string)Liste[1, i].Value).Split(' ');

                        for (int iii = 0; iii < Yeni.Soyadları.Length; iii++)
                        {
                            Yeni.Soyadları[iii] = Yeni.Soyadları[iii].Trim();
                            if (Yeni.Soyadları[iii][0] != '.')
                            {
                                Liste.BackgroundColor = System.Drawing.Color.Red;
                                Liste[0, i].Selected = true;
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        Yeni.Tip = Kaynakça_.Tip_.Atla;
                        Yeni.Soyadları = new string[] { ".*" };
                    }

                    K1.Add(Yeni);
                }

                if (Süre < Environment.TickCount)
                {
                    Süre = Environment.TickCount + 500;
                    Application.DoEvents();
                }
            }

            Kaynakça_ Kaynakça = new Kaynakça_();
            Kaynakça.Filreler = K1.ToArray();

            Depo.SınıfDeğişkenleri_Yaz(ref TamListe, Kaynakça);

            PeTeİkKo.Dispose();
        }

        public void ÖnYüzüDoldur(string KaynakYolu, bool AltKlasörlerleBirlikte = false)
        {
            Kaynakça_ Kaynakça = Aç(TamListe);
            Arama.BackColor = System.Drawing.Color.GreenYellow;

            TekrarDene:
            try
            {
                KaynakYolu = KaynakYolu.TrimEnd('\\');
                KaynakYolu = Path.GetFullPath(KaynakYolu);
                KaynakYoluAsılKopyası = KaynakYolu;
                string[] KlasörListesi = Directory.GetDirectories(KaynakYolu, "*", AltKlasörlerleBirlikte ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                Text = "Kaynak Seçici " + KlasörListesi.Length.ToString() + " " + KaynakYolu;

                int EkranGüncelleme = Environment.TickCount + 250;
                int AdetKaynak = KaynakYolu.Length;
                KaynakYolu = KaynakYolu.ToLower();

                Liste.Rows.Clear();
                Liste.RowCount++;
                if (Liste.SortedColumn != null)
                {
                    DataGridViewColumn col = Liste.SortedColumn;
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.SortMode = DataGridViewColumnSortMode.Automatic;
                }

                Liste[0, 0].Value = "Yedekle";
                Liste[1, 0].Value = ".*";
                Liste[2, 0].Value = "Kök";

                bool[] Bulunanlar = new bool[Kaynakça.Filreler.Length];

                for (int i = 0; i < Kaynakça.Filreler.Length; i++)
                {
                    Kaynakça_.BirFiltre_ Biri = Kaynakça.Filreler[i];

                    if (KaynakYolu == Biri.Yol)
                    {
                        if (Biri.Tip == Kaynakça_.Tip_.Atla) Liste[0, 0].Value = "Atla";
                        else if (Biri.Tip == Kaynakça_.Tip_.SoyadlarınıYedekle) Liste[0, 0].Value = "Soyadlarını yedekle";
                        else Liste[0, 0].Value = "Soyadlarını atla";

                        string Soyadları = "";
                        foreach (var soyadı in Biri.Soyadları)
                        {
                            Soyadları += soyadı + " ";
                        }
                        Liste[1, 0].Value = Soyadları.Trim();

                        Bulunanlar[i] = true;
                        break;
                    }
                }

                for (int i = 1; i < KlasörListesi.Length + 1; i++)
                {
                    Liste.RowCount++;

                    Liste[0, i].Value = "Yedekle";
                    Liste[1, i].Value = ".*";
                    Liste[2, i].Value = KlasörListesi[i - 1].Remove(0, AdetKaynak);

                    for (int ii = 0; ii < Kaynakça.Filreler.Length; ii++)
                    {
                        Kaynakça_.BirFiltre_ Biri = Kaynakça.Filreler[ii];
                 
                        if (KlasörListesi[i - 1].ToLower() == Biri.Yol)
                        {
                            if (Biri.Tip == Kaynakça_.Tip_.Atla) Liste[0, i].Value = "Atla";
                            else if (Biri.Tip == Kaynakça_.Tip_.SoyadlarınıYedekle) Liste[0, i].Value = "Soyadlarını yedekle";
                            else Liste[0, i].Value = "Soyadlarını atla";

                            string Soyadları = "";
                            foreach (var soyadı in Biri.Soyadları)
                            {
                                Soyadları += soyadı + " ";
                            }
                            Liste[1, i].Value = Soyadları.Trim();

                            Bulunanlar[ii] = true;
                            break;
                        }
                    }

                    if (Environment.TickCount > EkranGüncelleme)
                    {
                        Application.DoEvents();
                        EkranGüncelleme = Environment.TickCount + 250;
                    }
                }

                if (!AltKlasörlerleBirlikte)
                {
                    for (int i = 0; i < Bulunanlar.Length; i++)
                    {
                        if (!Bulunanlar[i])
                        {
                            Liste.RowCount++;

                            Kaynakça_.BirFiltre_ Biri = Kaynakça.Filreler[i];

                            if (Biri.Tip == Kaynakça_.Tip_.Atla) Liste[0, Liste.RowCount - 1].Value = "Atla";
                            else if (Biri.Tip == Kaynakça_.Tip_.SoyadlarınıYedekle) Liste[0, Liste.RowCount - 1].Value = "Soyadlarını yedekle";
                            else Liste[0, Liste.RowCount - 1].Value = "Soyadlarını atla";

                            string Soyadları = "";
                            foreach (var soyadı in Biri.Soyadları)
                            {
                                Soyadları += soyadı + " ";
                            }
                            Liste[1, Liste.RowCount - 1].Value = Soyadları.Trim();
                            Liste[2, Liste.RowCount - 1].Value = Biri.Yol.Remove(0, AdetKaynak);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Liste.Rows.Clear();
                if (AltKlasörlerleBirlikte)
                {
                    AltKlasörlerleBirlikte = false;

                    TümKlasörler.CheckedChanged -= TümKlasörler_CheckedChanged;
                    TümKlasörler.Checked = false;
                    TümKlasörler.CheckedChanged += TümKlasörler_CheckedChanged;

                    goto TekrarDene;
                }
                _form1.PeTeİkKo.MetniBaloncuktaGöster("Hata " + ex.Message, ToolTipIcon.Info, 5000);
            }

            Arama.BackColor = System.Drawing.Color.White;
        }
        public static Kaynakça_ Aç(string KaydedilmişBilgi)
        {
            Kaynakça_ Kaynakça = new Kaynakça_();
            Depo.SınıfDeğişkenleri_Oku(KaydedilmişBilgi, Kaynakça);
            if (Kaynakça.Filreler == null) Kaynakça.Filreler = new Kaynakça_.BirFiltre_[0];
            return Kaynakça;
        }

        void Liste_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
        void Arama_TextChanged(object sender, EventArgs e)
        {
            int aşım = 250;
            int Süre = Environment.TickCount + aşım;
            Arama.BackColor = System.Drawing.Color.GreenYellow;
            string aranan = Arama.Text.ToLower();

            int kendi = (int)Arama.Tag + 1;
            Arama.Tag = kendi;

            for (int y = 0; y < Liste.RowCount; y++)
            {
                if ((Liste[0, y].Value as string).ToLower().Contains(aranan) ||
                    (Liste[1, y].Value as string).ToLower().Contains(aranan) ||
                    (Liste[2, y].Value as string).ToLower().Contains(aranan))
                {
                     Liste.Rows[y].Visible = true;
                }
                else Liste.Rows[y].Visible = false;

                if (Süre < Environment.TickCount)
                {
                    Süre = Environment.TickCount + aşım;
                    Application.DoEvents();
                    if ((int)Arama.Tag != kendi) return;
                }
            }

            Arama.BackColor = System.Drawing.Color.White;
        }
        void TümKlasörler_CheckedChanged(object sender, EventArgs e)
        {
            Arama.Text = "";
            Arama.Tag = 0;

            ÖnYüzüDoldur(KaynakYoluAsılKopyası, TümKlasörler.Checked);
        }
    }

    public class Kaynakça_
    {
        [Serializable]
        public struct BirFiltre_
        {
            public Tip_ Tip;
            public string[] Soyadları;
            public string Yol;
        }

        [Serializable]
        public enum Tip_
        {
            Yedekle,
            Atla,
            SoyadlarınıYedekle,
            SoyadlarınıAtla
        };

        public BirFiltre_[] Filreler;
    }
}
