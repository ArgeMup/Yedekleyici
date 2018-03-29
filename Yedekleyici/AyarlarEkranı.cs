using System;
using System.Windows.Forms;
using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Dönüştürme;

namespace Yedekleyici
{
    public partial class AyarlarEkranı_ : Form
    {
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        private readonly AnaEkran _form1;

        public AyarlarEkranı_(AnaEkran form1)
        {
            _form1 = form1;
            InitializeComponent();
        }
        private void AyarlarEkranı__Load(object sender, EventArgs e)
        {
            PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, _form1.Ayarlar, true, "AyarlarEkranı", Location.X, Location.Y, Width, Height);
        }
        private void AyarlarEkranı__Shown(object sender, EventArgs e)
        {
            Süre_Arka_ValueChanged(null, null);
            SadeceKopyala_CheckedChanged(null, null);
        }
        private void AyarlarEkranı__FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Karıştır.Checked || Düzelt.Checked)
            {
                if (string.IsNullOrEmpty(Parola.Text) || Parola.Text == "TEKRAR YAZINIZ")
                {
                    MessageBox.Show("Parola Girilmeli");
                    Parola.Focus();
                    e.Cancel = true;
                    return;
                }
            }

            if (Başlangıç.Value >= Bitiş.Value)
            {
                MessageBox.Show("Soldaki bölme sağdaki bölmeden büyük değerde olmalıdır");
                Başlangıç.Focus();
                e.Cancel = true;
                return;
            }
        }

        private void Süre_Arka_KeyUp(object sender, KeyEventArgs e)
        {
            Süre_Arka_ValueChanged(null, null);
        }
        private void Süre_Arka_ValueChanged(object sender, EventArgs e)
        {
            Süre_Ön.Text = D_Süre.Metne.Saniyeden(Convert.ToUInt64(Süre_Arka.Value) * 60);
        }
        private void FazlaDosyalarıSil_CheckedChanged(object sender, EventArgs e)
        {
            FarklıKlasörSayısı.Enabled = !FazlaDosyalarıSil.Checked;
            label12.Enabled = FarklıKlasörSayısı.Enabled;
        }
        private void SadeceKopyala_CheckedChanged(object sender, EventArgs e)
        {
            Parola.Enabled = !SadeceKopyala.Checked;
            label1.Enabled = Parola.Enabled;
        }
        private void FarklıKlasörSayısı_ValueChanged(object sender, EventArgs e)
        {
            FazlaDosyalarıSil.Enabled = FarklıKlasörSayısı.Value > 1 ? false : true;
            label6.Enabled = FazlaDosyalarıSil.Enabled;
        }
    }
}
