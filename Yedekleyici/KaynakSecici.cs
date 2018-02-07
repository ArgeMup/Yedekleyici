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

        string KaynakAsılHali;
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        private readonly AnaEkran _form1;

        public KaynakSecici_(AnaEkran form1)
        {
            _form1 = form1;
            InitializeComponent();
        }
        private void KaynakSecici_Load(object sender, EventArgs e)
        {
            PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, _form1.Ayarlar, true, "KaynakSecici", Location.X, Location.Y, Width, Height);
        }
        private void KaynakSecici_FormClosing(object sender, FormClosingEventArgs e)
        {
            button1.Focus();//son yapılan değişikliğin kayda alınması için
            Application.DoEvents();

            List<Kaynakça_.BirFiltre_> K1 = new List<Kaynakça_.BirFiltre_>();

            for (int i = 0; i < Liste.RowCount; i++)
            {
                if ((string)Liste[0, i].Value != "Yedekle")
                {
                    Kaynakça_.BirFiltre_ Yeni = new Kaynakça_.BirFiltre_();
                    if ((string)Liste[2, i].Value == "Kök") Yeni.Yol = KaynakAsılHali.ToLower();
                    else Yeni.Yol = (KaynakAsılHali + Liste[2, i].Value).ToLower();

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
            }

            Kaynakça_ Kaynakça = new Kaynakça_();
            Kaynakça.Filreler = K1.ToArray();

            Depo.SınıfDeğişkenleri_Yaz(ref TamListe, Kaynakça);

            PeTeİkKo.Dispose();
        }
        
        public void ÖnYüzüDoldur(string KaynakYolu)
        {
            Cursor = Cursors.WaitCursor;
            Kaynakça_ Kaynakça = Aç(TamListe);

            try
            {
                KaynakYolu = Path.GetFullPath(KaynakYolu);
                KaynakAsılHali = KaynakYolu;
                string[] KlasörListesi = Directory.GetDirectories(KaynakYolu, "*", SearchOption.AllDirectories);
                Text = "Kaynak Seçici " + KlasörListesi.Length.ToString() + " " + KaynakYolu;

                int EkranGüncelleme = Environment.TickCount + 250;
                int AdetKaynak = KaynakYolu.Length;

                Liste.Rows.Clear();
                Liste.RowCount++;
                Liste[0, 0].Value = "Yedekle";
                Liste[1, 0].Value = ".*";
                Liste[2, 0].Value = "Kök";

                foreach (var Biri in Kaynakça.Filreler)
                {
                    if (KaynakAsılHali.ToLower() == Biri.Yol)
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
                        break;
                    }
                }

                for (int i = 1; i < KlasörListesi.Length + 1; i++)
                {
                    Liste.RowCount++;

                    Liste[0, i].Value = "Yedekle";
                    Liste[1, i].Value = ".*";
                    Liste[2, i].Value = KlasörListesi[i - 1].Remove(0, AdetKaynak);

                    foreach (var Biri in Kaynakça.Filreler)
                    {
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
                            break;
                        }
                    }

                    if (Environment.TickCount > EkranGüncelleme) { Application.DoEvents(); EkranGüncelleme = Environment.TickCount + 250; }
                }
            }
            catch (Exception ex)
            {
                Liste.Rows.Clear();
                _form1.PeTeİkKo.MetniBaloncuktaGöster("Hata " + ex.Message, ToolTipIcon.Info, 5000);
            }

            Cursor = Cursors.Default;
        }
        public static Kaynakça_ Aç(string KaydedilmişBilgi)
        {
            Kaynakça_ Kaynakça = new Kaynakça_();
            Depo.SınıfDeğişkenleri_Oku(KaydedilmişBilgi, Kaynakça);
            if (Kaynakça.Filreler == null) Kaynakça.Filreler = new Kaynakça_.BirFiltre_[0];
            return Kaynakça;
        }

        void Liste_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
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
