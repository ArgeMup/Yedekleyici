// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace Yedekleyici
{
    class Ortak
    {
        #region Program Ana Klasörü
        //////////////////////////////////////////////////////////////////////////
        // Program ana klasörü
        //////////////////////////////////////////////////////////////////////////
        public static string pak = Directory.GetCurrentDirectory();
        public static string pak_AnaKlasör = pak + "\\YedekleyiciDosyalari\\"; //programanaklasör
        public static string pak_Banka = pak_AnaKlasör + (Environment.MachineName + "." + Environment.UserName).ToUpper() + "\\";
        public static string pak_Şablon = pak_AnaKlasör + "Sablon\\";
        public static string pak_Geçici = pak_AnaKlasör + "Gecici\\";
        #endregion

        #region Önyüz Malzemeleri
        //////////////////////////////////////////////////////////////////////////
        // Önyüz Malzemeleri
        //////////////////////////////////////////////////////////////////////////
        public static System.Windows.Forms.Form AnaEkran;
        public static System.Windows.Forms.TextBox Günlük_MetinKutusu;
        public static System.Windows.Forms.FlowLayoutPanel Düzlem;
        public static System.Windows.Forms.ToolStripDropDownButton Buton_Günlük;
        public static float KarakterBüyüklüğü = 8;
        public enum PanelListesi { Senaryo, Talepler, KlasörleriListele, KlasörleriKarıştır, Ayarlar, Günlük, Boşta };
        public static PanelListesi EtkinPanel = PanelListesi.Boşta;
        public enum Resimler { Çalışıyor, Hata, Tamam, Tamam_Hayır, Yeni, EnFazla };
        public static void AğaçGörseliniGüncelle(System.Windows.Forms.TreeNode Dal, int Görsel, string İpucu = null)
        {
            if (Dal == null) return;
  
            AnaEkran.Invoke((System.Windows.Forms.MethodInvoker)delegate () 
            {
                if (Dal.ImageIndex != Görsel)
                {
                    Dal.ImageIndex = Görsel;
                    Dal.SelectedImageIndex = Görsel;

                    if (Görsel == (int)Resimler.Çalışıyor)
                    {
                        if (Dal.Parent != null)
                        {
                            if (!Dal.Parent.IsExpanded) Dal.Parent.Expand();
                        }
                    }
                }

                if (İpucu != null) Dal.ToolTipText = İpucu;
            });
        }
        #endregion

        #region HazirKod
        //////////////////////////////////////////////////////////////////////////
        // HazirKod
        //////////////////////////////////////////////////////////////////////////
        public static ArgeMup.HazirKod.Ayarlar_ Ayarlar;
        public static ArgeMup.HazirKod.DurumBildirimi_ DurumBildirimi = new ArgeMup.HazirKod.DurumBildirimi_();
        public static ArgeMup.HazirKod.PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        public static ArgeMup.HazirKod.UygulamaOncedenCalistirildiMi_ UygulamaOncedenCalistirildiMi;
        #endregion

        #region Günlük
        //////////////////////////////////////////////////////////////////////////
        // Günlük
        //////////////////////////////////////////////////////////////////////////
        static Mutex Günlük_Mutex = new Mutex();
        /// <param name="Tür">HATA veya Bilgi veya Komut</param>
        public static void Günlük_Ekle(string Mesaj, string Tür = "HATA")
        {
            Günlük_Mutex.WaitOne();

            try
            {
                string yazı = Mesaj.Trim(' ', '\r', '\n');
                File.AppendAllText(pak_Banka + "Gunluk.csv", Tür + ";" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ";" + yazı.Replace(Environment.NewLine, ";"));

                yazı = Environment.NewLine +
                       Environment.NewLine + Tür + " " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() +
                       Environment.NewLine + yazı;

                AnaEkran.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
                {
                    Günlük_MetinKutusu.Text += yazı;

                    if (Tür == "HATA") Buton_Günlük.Image = Properties.Resources.M_Gunluk_Yeni;
                });
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("İstenmeyen Durum " + ex); }

            Günlük_Mutex.ReleaseMutex();
        }
        #endregion

        #region Diğer
        //////////////////////////////////////////////////////////////////////////
        // Diğer
        //////////////////////////////////////////////////////////////////////////
        public static bool GizliMenüleriGöster = false;
        public static int UygulamayıKapatmaSayacı = 0;
        public static List<ArgeMup.HazirKod.Depo.Biri> ParolaŞablonu;
        public static string Parola = "ArgeMup" + Convert.ToString(090510111938);
        public static string Karıştır(string Girdi, string Parola)
        {
            return new ArgeMup.HazirKod.DahaCokKarmasiklastirma_().Karıştır(Girdi, Parola);
        }
        public static string Düzelt(string Girdi, string Parola)
        {
            return new ArgeMup.HazirKod.DahaCokKarmasiklastirma_().Düzelt(Girdi, Parola);
        }

        static PerformanceCounter CpuYüzdesi_Bilgisayar, HddYüzdesi_Bilgisayar;
        public static int CpuYüzdesi(string Uygulama = "")
        {
            try
            {
                if (string.IsNullOrEmpty(Uygulama))
                {
                    if (CpuYüzdesi_Bilgisayar == null)
                    {
                        CpuYüzdesi_Bilgisayar = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                        CpuYüzdesi_Bilgisayar.NextValue();
                        Thread.Sleep(2000);
                    }

                    return (int)CpuYüzdesi_Bilgisayar.NextValue();
                }
                else
                {
                    Process[] uyglm = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Uygulama));
                    if (uyglm.Length == 0) return -1;

                    TimeSpan[] baslangıç = new TimeSpan[uyglm.Length];
                    TimeSpan[] fark = new TimeSpan[uyglm.Length];

                    for (int i = 0; i < uyglm.Length; i++)
                    {
                        uyglm[i].Refresh();
                        baslangıç[i] = uyglm[i].TotalProcessorTime;
                    }

                    Thread.Sleep(2000);

                    for (int i = 0; i < uyglm.Length; i++)
                    {
                        uyglm[i].Refresh();
                        fark[i] = uyglm[i].TotalProcessorTime - baslangıç[i];
                    }

                    int sonuç = 0;
                    for (int i = 0; i < uyglm.Length; i++)
                    {
                        sonuç += (int)((double)fark[i].TotalMilliseconds / (double)20.0); // fark / 2000 * 100
                    }

                    return sonuç;

                    //PerformanceCounter CpuYüzdesi_Uygulama = new PerformanceCounter("Process", "% Processor Time", Uygulama);
                    //CpuYüzdesi_Uygulama.NextValue();
                    //Thread.Sleep(2000);

                    //return (int)CpuYüzdesi_Uygulama.NextValue();
                }
            }
            catch (Exception) { return -1; }
        }
        public static int HddYüzdesi()
        {
            try
            {
                if (HddYüzdesi_Bilgisayar == null)
                {
                    HddYüzdesi_Bilgisayar = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
                    HddYüzdesi_Bilgisayar.NextValue();
                    Thread.Sleep(1000);
                }

                return (int)HddYüzdesi_Bilgisayar.NextValue();
            }
            catch (Exception) { return -1; }
        }
        #endregion

        #region Dosyalama
        public class Sil
        {
            public static bool Dosya(string DosyaYolu)
            {
                try
                {
                    if (!File.Exists(DosyaYolu)) return true;
                    
                    İzinAl.Dosya(DosyaYolu);
                    File.SetAttributes(DosyaYolu, FileAttributes.Normal);
                    File.Delete(DosyaYolu);

                    return !File.Exists(DosyaYolu);
                }
                catch (Exception) { }

                return false;
            }
            public static bool Klasör(string Yolu)
            {
                try
                {
                    İzinAl.Klasör(Yolu);

                    foreach (string biri in Directory.GetFiles(Yolu)) Dosya(biri);
                    foreach (string biri in Directory.GetDirectories(Yolu)) Klasör(biri);

                    Directory.Delete(Yolu, false);

                    return !Directory.Exists(Yolu);
                }
                catch (Exception) { }
                
                return false;
            }
        }
        public class Listele
        {
            public static string[] Dosya(string Klasör, SearchOption Seçenekler, string Filtre = "*")
            {
                List<string> Liste = new List<string>();
                if (!Klasör.EndsWith(Path.DirectorySeparatorChar.ToString())) Klasör += Path.DirectorySeparatorChar;
                Dsy(ref Liste, Klasör, Filtre, Seçenekler);
                return Liste.ToArray();
            }
            public static string[] Klasör(string Klasör, SearchOption Seçenekler, string Filtre = "*")
            {
                List<string> Liste = new List<string>();
                if (!Klasör.EndsWith(Path.DirectorySeparatorChar.ToString())) Klasör += Path.DirectorySeparatorChar;
                Kls(ref Liste, Klasör, Filtre, Seçenekler);
                return Liste.ToArray();
            }
            static void Dsy(ref List<string> Liste, string Yol, string Filtre, SearchOption Seçenekler)
            {
                string[] dizi = null;
                try
                {
                    dizi = Directory.GetFiles(Yol, Filtre, SearchOption.TopDirectoryOnly);
                }
                catch (Exception) { }

                if (dizi != null) Liste.AddRange(dizi);

                if (Seçenekler == SearchOption.TopDirectoryOnly) return;

                dizi = null;
                try
                {
                    dizi = Directory.GetDirectories(Yol, "*", SearchOption.TopDirectoryOnly);
                }
                catch (Exception) { }

                if (dizi == null) return;

                foreach (string biri in dizi)
                {
                    Dsy(ref Liste, biri, Filtre, SearchOption.AllDirectories);
                }
            }
            static void Kls(ref List<string> Liste, string Yol, string Filtre, SearchOption Seçenekler)
            {
                string[] dizi = null;
                try
                {
                    dizi = Directory.GetDirectories(Yol, Filtre, SearchOption.TopDirectoryOnly);
                }
                catch (Exception) { }

                if (dizi == null) return; 
                    
                Liste.AddRange(dizi);

                if (Seçenekler == SearchOption.TopDirectoryOnly) return;

                foreach (string biri in dizi)
                {
                    Kls(ref Liste, biri, Filtre, SearchOption.AllDirectories);
                }
            }
        }
        public class İzinAl
        {
            public static bool Dosya(string Yol)
            {
                try
                {
                    if (!File.Exists(Yol)) return false;

                    FileSecurity fSecurity = File.GetAccessControl(Yol);
                    string acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //@"DomainName\AccountName"
                    fSecurity.AddAccessRule(new FileSystemAccessRule(@acc, FileSystemRights.FullControl, AccessControlType.Allow));
                    File.SetAccessControl(Yol, fSecurity);

                    return true;
                }
                catch (Exception) { }

                return false;
            }
            public static bool Klasör(string Yol)
            {
                try
                {
                    if (!Directory.Exists(Yol)) return false;

                    DirectorySecurity fSecurity = Directory.GetAccessControl(Yol);
                    string acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //@"DomainName\AccountName"
                    fSecurity.AddAccessRule(new FileSystemAccessRule(@acc, FileSystemRights.FullControl, AccessControlType.Allow));
                    Directory.SetAccessControl(Yol, fSecurity);
                    File.SetAttributes(Yol, FileAttributes.Normal);

                    return true;
                }
                catch (Exception) { }

                return false;
            }
        }
        #endregion

        #region Dosya Sistemi İzleyicisi
        public class Dsi
        {
            public class Bir_DosyaSistemi_İzleyicisi_
            {
                FileSystemWatcher fsw;
                string Hedef;

                public Bir_DosyaSistemi_İzleyicisi_(string Kaynak, string Hedef)
                {
                    fsw = new FileSystemWatcher(Kaynak);
                    fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                    fsw.Filter = "*";
                    fsw.Changed += Fsw_Changed;
                    fsw.Created += Fsw_Changed;
                    fsw.Deleted += Fsw_Changed;
                    fsw.Renamed += Fsw_Changed;
                    fsw.Error += Fsw_Error;
                    fsw.IncludeSubdirectories = true;
                    fsw.EnableRaisingEvents = true;

                    this.Hedef = Hedef;

                    Mutex.WaitOne();
                    Liste.Add(this);
                    Mutex.ReleaseMutex();
                }
                public string Klasör()
                {
                    return fsw.Path + Hedef;
                }
                public void Durdur()
                {
                    fsw.Dispose();
                    fsw = null;

                    Mutex.WaitOne();
                    Liste.Remove(this);
                    Mutex.ReleaseMutex();
                }
                void Fsw_Error(object sender, ErrorEventArgs e)
                {
                    Durdur();
                }
                void Fsw_Changed(object sender, FileSystemEventArgs e)
                {
                    Durdur();
                }
            }
            static Mutex Mutex = new Mutex();
            static List<Bir_DosyaSistemi_İzleyicisi_> Liste = new List<Bir_DosyaSistemi_İzleyicisi_>();
            static Bir_DosyaSistemi_İzleyicisi_ Bul(string Kaynak, string Hedef)
            {
                Mutex.WaitOne();
                Bir_DosyaSistemi_İzleyicisi_ dsi = Liste.Find(x => x.Klasör() == Kaynak + Hedef);
                Mutex.ReleaseMutex();

                return dsi;
            }
            public static void Başlat(string Kaynak, string Hedef)
            {
                if (Kaynak == Hedef) return;
                if (Bul(Kaynak, Hedef) != null) return;

                new Bir_DosyaSistemi_İzleyicisi_(Kaynak, Hedef);
            }
            public static void TümünüDurdur()
            {
                Mutex.WaitOne();
                while (Liste.Count > 0) Liste[0].Durdur();
                Mutex.ReleaseMutex();
            }
            public static bool DeğişiklikVarMı(string Kaynak, string Hedef)
            {
                return Bul(Kaynak, Hedef) == null;
            }
        }
        #endregion
    }
}
