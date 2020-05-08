// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArgeMup.HazirKod
{
    public class DurumBildirimi_ : IDisposable
    {
        public const string Sürüm = "V1.5";
        #region Değişkenler
        NotifyIcon Tepsiİkonu = null;
        ToolTip ipucu = null;
        Form Pencere = null;
        System.Timers.Timer Zamanlayıcı = null;
        int Süre_Tepsiİkonu, Süre_ipucu;
        #endregion

        public DurumBildirimi_(Form Pencere_ = null)
        {
            Pencere = Pencere_;
            if (Pencere != null) Pencere.FormClosed += Pencere_FormClosed;
        }

        public void BaloncukluUyarı(string Mesaj, ToolTipIcon İkon = ToolTipIcon.Warning, int ZamanAşımı = 3000, IWin32Window Kutucuk = null)
        {
            try
            {
                if (Zamanlayıcı == null)
                {
                    Zamanlayıcı = new System.Timers.Timer();
                    Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                    Zamanlayıcı.Interval = 1000;
                    Zamanlayıcı.Enabled = true;
                }

                if (Kutucuk == null)
                {
                    if (Tepsiİkonu == null)
                    {
                        Tepsiİkonu = new NotifyIcon();
                        Tepsiİkonu.BalloonTipShown += Tepsiİkonu_BalloonTipShown;
                        Tepsiİkonu.Visible = true;
                        Süre_Tepsiİkonu = int.MaxValue;
                    }

                    if (Pencere == null)
                    {
                        Tepsiİkonu.Icon = SystemIcons.Warning;
                        Tepsiİkonu.Text = "Lütfen dikkate alınız.";
                    }
                    else
                    {
                        Tepsiİkonu.Icon = Pencere.Icon;
                        Tepsiİkonu.Text = Kırp(Pencere.Text, 63);
                    }

                    Tepsiİkonu.BalloonTipTitle = Tepsiİkonu.Text;

                    if (Tepsiİkonu.Tag == null)
                    {
                        Tepsiİkonu.ShowBalloonTip(ZamanAşımı, Tepsiİkonu.Text, Mesaj, İkon);
                        Tepsiİkonu.Tag = Mesaj;
                    }
                    else Tepsiİkonu.Tag = Tepsiİkonu.Tag as string + "\r\r" + Mesaj;

                    Süre_Tepsiİkonu = ZamanAşımı;
                }
                else 
                {
                Devam_ipucu:
                    if (ipucu == null)
                    {
                        ipucu = new ToolTip();
                        ipucu.IsBalloon = true;
                        ipucu.ShowAlways = false;
                        ipucu.AutomaticDelay = 5000;
                    }
                    else { ipucu.Dispose(); ipucu = null; goto Devam_ipucu; }

                    ipucu.ToolTipIcon = İkon;
                    if (Pencere != null) ipucu.ToolTipTitle = Pencere.Text;
                    else ipucu.ToolTipTitle = "Lütfen dikkate alınız.";
                    ipucu.Show(string.Empty, Kutucuk);
                    ipucu.Show(Mesaj, Kutucuk, ZamanAşımı);

                    Süre_ipucu = Environment.TickCount + (ZamanAşımı * 2);
                }

                Zamanlayıcı.Start();
            }
            catch (Exception) { }
        }

        private void Tepsiİkonu_BalloonTipShown(object sender, EventArgs e)
        {
            if (Tepsiİkonu.Tag == null) return;

            Tepsiİkonu.BalloonTipText = (string)Tepsiİkonu.Tag;
            Tepsiİkonu.Tag = null;
            Tepsiİkonu.ShowBalloonTip(Süre_Tepsiİkonu);

            Süre_Tepsiİkonu = Environment.TickCount + Süre_Tepsiİkonu + 6000;
        }
        private void Pencere_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Tepsiİkonu != null) { Tepsiİkonu.Dispose(); Tepsiİkonu = null; }
        }
        private void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Tepsiİkonu != null)
                {
                    if (Tepsiİkonu.Tag != null) { Tepsiİkonu.BalloonTipText = (string)Tepsiİkonu.Tag; Tepsiİkonu.ShowBalloonTip(Süre_Tepsiİkonu); }
                    else if (Environment.TickCount > Süre_Tepsiİkonu) { Tepsiİkonu.Dispose(); Tepsiİkonu = null; }
                }

                if (ipucu != null) if (Environment.TickCount > Süre_ipucu) { ipucu.Dispose(); ipucu = null; }

                if (Tepsiİkonu == null && ipucu == null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
            }
            catch (Exception) { }
        }
        private string Kırp(string Girdi, int Uzunluk)
        {
            if (Girdi.Length > Uzunluk) return Girdi.Insert(Uzunluk - 3, " .....").Substring(0, Uzunluk);
            else return Girdi;
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).                    
					
				if (Tepsiİkonu != null) { Tepsiİkonu.Dispose(); Tepsiİkonu = null; }
                if (ipucu != null) { ipucu.Dispose(); ipucu = null; }
                if (Zamanlayıcı != null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //~DurumBildirimi_()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

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

