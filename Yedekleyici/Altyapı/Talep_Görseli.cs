// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Talep
{
    public partial class Görsel_ : UserControl
    {
        public DateTime SonGüncellenme = DateTime.Now;
        bool iptal = false;

        public Görsel_()
        {
            InitializeComponent();
        }
        public void Başlat(string Senaryo, string Talep, float Boyut, int Genişlik, string Kaynak, string Hedef)
        {
            Yedekleyici.Ortak.AnaEkran.Invoke((MethodInvoker)delegate ()
            {
                Senaryo_Tanımı.Text = Senaryo;
                Talep_Tanımı.Text = Talep;

                İpUcu.SetToolTip(Senaryo_Tanımı, Kaynak);
                İpUcu.SetToolTip(Talep_Tanımı, Hedef);

                int tanım_genişliği = Senaryo_Tanımı.Size.Width > Talep_Tanımı.Size.Width ?
                                    Senaryo_Tanımı.Size.Width :
                                    Talep_Tanımı.Size.Width;

                int boşluk = 15;
                tanım_genişliği += boşluk;

                Width = Genişlik;

                İlerlemeÇubuğu_üst.Location = new Point(Senaryo_Tanımı.Location.X + tanım_genişliği, İlerlemeÇubuğu_üst.Location.Y);
                İlerlemeÇubuğu_üst.Size = new Size(Genişlik - boşluk - tanım_genişliği - boşluk, İlerlemeÇubuğu_üst.Height);

                İlerlemeÇubuğu_alt.Location = new Point(İlerlemeÇubuğu_üst.Location.X, İlerlemeÇubuğu_alt.Location.Y);
                İlerlemeÇubuğu_alt.Size = İlerlemeÇubuğu_üst.Size;

                İptal.Location = new Point(İlerlemeÇubuğu_üst.Location.X + boşluk, İptal.Location.Y);

                Detay.Location = new Point(İptal.Location.X + İptal.Width + 3, Detay.Location.Y);
                Detay.Size = new Size(İlerlemeÇubuğu_üst.Width - boşluk - boşluk - İptal.Width - 3, Detay.Size.Height);

                Font = new Font(this.Font.FontFamily, Boyut);
            });
        }
        public bool Güncelle(int İlerleme_üst, int ilerleme_alt, string DosyaAdı, string Açıklama)
        {
            Yedekleyici.Ortak.AnaEkran.Invoke((MethodInvoker)delegate ()
            {
                if (İlerleme_üst < 0) İlerlemeÇubuğu_üst.Style = ProgressBarStyle.Marquee;
                else
                {
                    İlerlemeÇubuğu_üst.Style = ProgressBarStyle.Blocks;
                    İlerlemeÇubuğu_üst.Value = İlerleme_üst;
                }

                if (ilerleme_alt < 0) İlerlemeÇubuğu_alt.Style = ProgressBarStyle.Marquee;
                else
                {
                    İlerlemeÇubuğu_alt.Style = ProgressBarStyle.Blocks;
                    İlerlemeÇubuğu_alt.Value = ilerleme_alt;
                }

                Detay.Text = Açıklama;
                İşelmGörenDosya.Text = DosyaAdı;

                İpUcu.SetToolTip(Detay, Açıklama);
                İpUcu.SetToolTip(İşelmGörenDosya, DosyaAdı);
            });

            SonGüncellenme = DateTime.Now;

            return iptal;
        }
        private void İptal_Click(object sender, EventArgs e)
        {
            iptal = true;
            İptal.Enabled = false;
        }
    }
}
