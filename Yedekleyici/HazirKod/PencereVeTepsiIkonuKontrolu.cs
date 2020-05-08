// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Windows.Forms;
using ArgeMup.HazirKod.Dönüştürme;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArgeMup.HazirKod
{
    public class PencereVeTepsiIkonuKontrolu_ : IDisposable
    {
        public const string Sürüm = "V1.11";

        #region Değişkenler
        public NotifyIcon Tepsiİkonu = null;

        Form Pencere;
        Ayarlar_ Ayarlar;
        GörevÇubuğundaYüzdeGösterimiDurumu İlerlemeDurumu_ = GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı;
        FormWindowState PencereDurumu;
        bool UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün = true;
        string TakmaAdı = "";
        ulong İlerlemeDeğeri_ = 0, İlerlemeToplamDeğeri_ = 0;
        int şeffaflık;

        struct TepsiİkonuBaloncukluUyarı_
        {
            public string Mesaj;
            public ToolTipIcon İkon;
            public int ZamanAşımı;
            public int Sayac;
        };
        struct TepsiİkonuMetni_
        {
            public int SayacKonum;
            public Color ArkaPlan, Yazı;
            public string Metin, Metin_;
            public int Uzunluğu;
            public Font Fontu;
            public System.Timers.Timer Zamanlayıcı;
        };
        TepsiİkonuMetni_ TeİkMe = new TepsiİkonuMetni_();
        #endregion

        /// <summary>
        /// Form LOAD içerisinde çağırılmalı
        /// </summary>
        /// <param name="ŞeffafBaşlangıç">Kullanılacak ise form un OPACİTY özelliği SIFIR yapılmalı</param>
        public PencereVeTepsiIkonuKontrolu_(Form Pencere_, Ayarlar_ Ayarlar__ = null,  bool UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün_ = false, string TakmaAd = "", int X = 0, int Y = 0, int Genişlik = -1, int Yükseklik = -1, bool ŞeffafBaşlangıç = true)
        {
            Pencere = Pencere_;
            Pencere.Shown += Pencere_Shown;
            Pencere.FormClosed += Pencere_FormClosed;

            if (ŞeffafBaşlangıç)
            {
                if (Pencere.Opacity > 0)
                {
                    MessageBox.Show("PencereVeTepsiIkonuKontrolu_ nun Şeffaf Başlangıcı uygulayabilmesi için " + Pencere.Text + " formunun OPACITY özelliğini 0(sıfır) yapınız");
                    Application.Exit();
                }

                şeffaflık = 0;
            }
            else şeffaflık = 1;

            bool sonuç;
            if (Ayarlar__ == null) Ayarlar = new Ayarlar_(out sonuç, "", "", false, 0, 0);
            else Ayarlar = Ayarlar__;

            if (Genişlik == -1) Genişlik = Screen.PrimaryScreen.WorkingArea.Width;
            if (Yükseklik == -1) Yükseklik = Screen.PrimaryScreen.WorkingArea.Height;

            if (TakmaAd == "") TakmaAdı = Pencere.Name;
            else TakmaAdı = TakmaAd;

            UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün = UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün_;

            Pencere.Location = new Point(Convert.ToInt32(Ayarlar.Oku(TakmaAdı + "_PencereKonumu_X", X.ToString())), Convert.ToInt32(Ayarlar.Oku(TakmaAdı + "_PencereKonumu_Y", Y.ToString())));
            Pencere.Width = Convert.ToInt32(Ayarlar.Oku(TakmaAdı + "_PencereBoyutu_Genişlik", Genişlik.ToString()));
            Pencere.Height = Convert.ToInt32(Ayarlar.Oku(TakmaAdı + "_PencereBoyutu_Yükseklik", Yükseklik.ToString()));

            if (Pencere.WindowState == FormWindowState.Normal && !Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(Pencere.Left, Pencere.Top, Pencere.Width, Pencere.Height))))
            {
                Pencere.Left = X; Pencere.Top = Y; Pencere.Width = Genişlik; Pencere.Height = Yükseklik;
            }
        }

        public void TepsiİkonunuBaşlat(bool İkonTıklandığındaUygulamaBüyüsün_ = true)
        {
            if (!UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün && !İkonTıklandığındaUygulamaBüyüsün_) İkonTıklandığındaUygulamaBüyüsün_ = true;
            if (Tepsiİkonu == null)
            {
                Tepsiİkonu = new NotifyIcon();
                if (İkonTıklandığındaUygulamaBüyüsün_) Tepsiİkonu.MouseClick += Tepsiİkonu_MouseClick;

                Tepsiİkonu.BalloonTipShown += Tepsiİkonu_BalloonTipShown;
                Tepsiİkonu.Icon = Pencere.Icon;
            }
            Tepsiİkonu.BalloonTipTitle = Pencere.Text;
            Tepsiİkonu.Text = Kırp(Pencere.Text, 63);
            Tepsiİkonu.Visible = true; 
        }
        public bool MetniBaloncuktaGöster(string Mesaj, ToolTipIcon İkon = ToolTipIcon.Warning, int ZamanAşımı = 3000)
        {
            if (Tepsiİkonu == null) return false;

            TepsiİkonuBaloncukluUyarı_ Yeni;

            if (Tepsiİkonu.Tag == null)
            {
                Tepsiİkonu.ShowBalloonTip(ZamanAşımı, Tepsiİkonu.Text, Mesaj, İkon);

                Yeni = new TepsiİkonuBaloncukluUyarı_();
                Yeni.ZamanAşımı = ZamanAşımı;
                Yeni.Mesaj = Mesaj;
                Yeni.İkon = İkon;
                Yeni.Sayac = 1;
                Tepsiİkonu.Tag = Yeni;
                return true;
            }

            Yeni = (TepsiİkonuBaloncukluUyarı_)Tepsiİkonu.Tag;
            Yeni.ZamanAşımı = ZamanAşımı;
            Yeni.Mesaj += "\r\r" + Mesaj;
            Yeni.İkon = İkon;
            Yeni.Sayac++;
            Tepsiİkonu.Tag = Yeni;
            return true;
        }
        public bool MetniTepsiİkonundaGöster(string Metin, Color Metin_Rengi = new Color(), Color ArkaPlan_Rengi = new Color())
        {
            if (Tepsiİkonu == null) return false;

            if (Metin == "")
            {
                if (TeİkMe.Zamanlayıcı != null)
                {
                    TeİkMe.Zamanlayıcı.Stop();
                    TeİkMe.Zamanlayıcı.Dispose();
                    TeİkMe.Zamanlayıcı = null;
                }

                Tepsiİkonu.Text = Kırp(Pencere.Text, 63); 
                if (Tepsiİkonu.Icon.Handle != Pencere.Icon.Handle) { D_İkon.Yoket(Tepsiİkonu.Icon); Tepsiİkonu.Icon = Pencere.Icon; }
                return true;
            }

            if (Metin_Rengi.IsEmpty) TeİkMe.Yazı = Color.Black;
            else TeİkMe.Yazı = Metin_Rengi;
            if (ArkaPlan_Rengi.IsEmpty) TeİkMe.ArkaPlan = Color.White;
            else TeİkMe.ArkaPlan = ArkaPlan_Rengi;

            Tepsiİkonu.Text = Kırp(Pencere.Text + " " + Metin, 63);
            if (TeİkMe.Fontu == null) TeİkMe.Fontu = new Font("Microsoft Sans Serif", Tepsiİkonu.Icon.Size.Width-1, FontStyle.Regular, GraphicsUnit.Pixel);

            TeİkMe.Metin = "  " + Metin;
            TeİkMe.Uzunluğu = TextRenderer.MeasureText(TeİkMe.Metin, TeİkMe.Fontu).Width * -1;
            TeİkMe.Metin += "                               ";

            if (TeİkMe.Zamanlayıcı == null)
            {
                TeİkMe.Metin_ = TeİkMe.Metin;
                TeİkMe.Zamanlayıcı = new System.Timers.Timer();
                TeİkMe.Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                TeİkMe.Zamanlayıcı.Interval = 100;
                TeİkMe.Zamanlayıcı.Enabled = true;
                TeİkMe.Zamanlayıcı.AutoReset = false;
                TeİkMe.Zamanlayıcı.Start();
                TeİkMe.SayacKonum = 0;
            }

            return true;
        }
        public bool İlerlemeyiYüzdeOlarakGöster(GörevÇubuğundaYüzdeGösterimiDurumu Durum = GörevÇubuğundaYüzdeGösterimiDurumu.Normal, ulong İlerleme = 0, ulong Toplam = 1, Color Metin_Rengi = new Color(), Color ArkaPlan_Rengi = new Color())
        {
            try
            {
                if (Tepsiİkonu != null)
                {
                    if (Durum == GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı)
                    {
                        if (!MetniTepsiİkonundaGöster("")) return false;
                    }
                    else
                    {
                        if (!MetniTepsiİkonundaGöster("%" + ((float)İlerleme / (float)Toplam * 100).ToString("0.00"), Metin_Rengi, ArkaPlan_Rengi)) return false;
                    }
                }

                if (Environment.OSVersion.Version >= new Version(6, 1))
                {
                    if (TaskbarÖrneği == null) TaskbarÖrneği = (ITaskbarListesi)new SanalTaskbarÖrneği();

                    if (İlerlemeDeğeri_ != İlerleme || İlerlemeToplamDeğeri_ != Toplam)
                    {
                        İlerlemeDeğeri_ = İlerleme;
                        İlerlemeToplamDeğeri_ = Toplam;
                        if ((int)İlerlemeDurumu_ > (int)GörevÇubuğundaYüzdeGösterimiDurumu.Belirsiz) TaskbarÖrneği.SetProgressValue(Pencere.Handle, İlerlemeDeğeri_, İlerlemeToplamDeğeri_);
                    }

                    if (İlerlemeDurumu_ != Durum)
                    {
                        İlerlemeDurumu_ = Durum;
                        TaskbarÖrneği.SetProgressState(Pencere.Handle, İlerlemeDurumu_);
                        if ((int)İlerlemeDurumu_ > (int)GörevÇubuğundaYüzdeGösterimiDurumu.Belirsiz) TaskbarÖrneği.SetProgressValue(Pencere.Handle, İlerlemeDeğeri_, İlerlemeToplamDeğeri_);
                    }

                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        void Pencere_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Pencere.Opacity = şeffaflık;

            Pencere.WindowState = (FormWindowState)Convert.ToInt32(Ayarlar.Oku(TakmaAdı + "_PencereDurumu", "0"));
            Pencere_Resize(null, null);

            Pencere.Resize += Pencere_Resize;

            while (Pencere.Opacity < 1 && Pencere.WindowState != FormWindowState.Minimized)
            {
                Pencere.Opacity += 0.05;
                Application.DoEvents();
                System.Threading.Thread.Sleep(30);
            }
            Pencere.Opacity = 1;
        }
        void Pencere_Resize(object sender, EventArgs e)
        {
            if (Pencere.WindowState == FormWindowState.Minimized)
            {
                if (!UygulamaKüçültüldüğündeGörevÇubuğundaGörünsün && Tepsiİkonu != null) Pencere.Hide();
            }
            else
            {
                PencereDurumu = Pencere.WindowState;
                if (TaskbarÖrneği != null) İlerlemeyiYüzdeOlarakGöster(İlerlemeDurumu_, İlerlemeDeğeri_, İlerlemeToplamDeğeri_, TeİkMe.Yazı, TeİkMe.ArkaPlan);

                if (Pencere.WindowState == FormWindowState.Normal && !Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(Pencere.Left, Pencere.Top, Pencere.Width, Pencere.Height))))
                {
                    Pencere.Left = 0; Pencere.Top = 0; Pencere.Width = Screen.PrimaryScreen.WorkingArea.Width; Pencere.Height = Screen.PrimaryScreen.WorkingArea.Height;
                }
            }
        }
        private void Pencere_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Tepsiİkonu != null) { Tepsiİkonu.Dispose(); Tepsiİkonu = null; }
            Dispose();
        }
        void Tepsiİkonu_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Pencere.Show();

                if (Pencere.WindowState == FormWindowState.Minimized) Pencere.WindowState = PencereDurumu;  

                Pencere.BringToFront();
            }
        }
        private void Tepsiİkonu_BalloonTipShown(object sender, EventArgs e)
        {
            if (Tepsiİkonu.Tag == null) return;

            TepsiİkonuBaloncukluUyarı_ Yeni = (TepsiİkonuBaloncukluUyarı_)Tepsiİkonu.Tag;

            if (Yeni.Sayac > 1)
            {
                Tepsiİkonu.ShowBalloonTip(Yeni.ZamanAşımı, Tepsiİkonu.Text, Yeni.Mesaj, Yeni.İkon);
            }

            Tepsiİkonu.Tag = null;
        }
        void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Tepsiİkonu.Icon.Handle != Pencere.Icon.Handle) D_İkon.Yoket(Tepsiİkonu.Icon);
               
                Tepsiİkonu.Icon = D_İkon.Metinden(TeİkMe.Metin_, Tepsiİkonu.Icon, TeİkMe.Fontu, TeİkMe.Yazı, new Point(TeİkMe.SayacKonum, 0), TeİkMe.ArkaPlan);
                TeİkMe.SayacKonum -= Tepsiİkonu.Icon.Size.Width / 10;
                if (--TeİkMe.SayacKonum < TeİkMe.Uzunluğu) { TeİkMe.SayacKonum = 0; TeİkMe.Metin_ = TeİkMe.Metin; }
            }
            catch (Exception) { }

            if (TeİkMe.Zamanlayıcı != null) TeİkMe.Zamanlayıcı.Enabled = true;
        }

        string Kırp(string Girdi, int Uzunluk)
        {
            if (Girdi.Length > Uzunluk) return Girdi.Insert(Uzunluk - 3, " .....").Substring(0, Uzunluk);
            else return Girdi;
        }

        #region GörevÇubuğuYüzdeGösterimi
        ITaskbarListesi TaskbarÖrneği; 

        public enum GörevÇubuğundaYüzdeGösterimiDurumu
        {
            Kapalı = 0,
            Belirsiz = 0x1,
            Normal = 0x2,
            Hatalı = 0x4,
            Durgun = 0x8
        }
        [ComImport()]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ITaskbarListesi
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, GörevÇubuğundaYüzdeGösterimiDurumu state);
        }

        [ComImport()]
        [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class SanalTaskbarÖrneği
        {
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (Ayarlar != null)
                {
                    Ayarlar.Yaz(TakmaAdı + "_PencereDurumu", ((int)Pencere.WindowState).ToString());
                    if (Pencere.WindowState == FormWindowState.Normal)
                    {
                        Ayarlar.Yaz(TakmaAdı + "_PencereKonumu_X", Pencere.Location.X.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereKonumu_Y", Pencere.Location.Y.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereBoyutu_Genişlik", Pencere.Width.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereBoyutu_Yükseklik", Pencere.Height.ToString());
                    }
                    else
                    {
                        Ayarlar.Yaz(TakmaAdı + "_PencereKonumu_X", Pencere.RestoreBounds.X.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereKonumu_Y", Pencere.RestoreBounds.Y.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereBoyutu_Genişlik", Pencere.RestoreBounds.Width.ToString());
                        Ayarlar.Yaz(TakmaAdı + "_PencereBoyutu_Yükseklik", Pencere.RestoreBounds.Height.ToString());
                    }

                    Ayarlar.DeğişiklikleriKaydet();
                }

                if (Tepsiİkonu != null) { Tepsiİkonu.Dispose(); Tepsiİkonu = null; }

                if (TeİkMe.Fontu != null) { TeİkMe.Fontu.Dispose(); TeİkMe.Fontu = null; }
                if (TeİkMe.Zamanlayıcı != null) { TeİkMe.Zamanlayıcı.Dispose(); TeİkMe.Zamanlayıcı = null; }

                TaskbarÖrneği = null;

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).        
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~PencereVeTepsiIkonuKontrolu_()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}

