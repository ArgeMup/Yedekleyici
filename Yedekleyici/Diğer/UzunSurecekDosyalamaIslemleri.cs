using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;

namespace Yedekleyici
{
    class UzunSurecekDosyalamaIslemleri
    {
        List<Tekil_> Nesneler = new List<Tekil_>();
        Mutex ElSıkışma = new Mutex();
        System.Timers.Timer Zamanlayıcı; 
        public string Durum, DetaylıDurum;           

        public class Tekil_
        {
            Thread İşlemci;
            string KopyalamaİşlemiHatası;
            ArgeMup.HazirKod.DahaCokKarmasiklastirma_ Aş;
            ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_ YığınKarıştırGirdi;
            ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_ YığınDüzeltGirdi;

            public bool DurDurÇık;
            public int Kip;
            public string DosyaBoyutu;
            public string Şifre;
            public string Kaynak;
            public string GeciciKonumu;
            public string Hedef;
            public string Durum = "Bekliyor";
            public void Başlat()
            {
                DurDurÇık = false;
                Durum = "Başladı";

                if (İşlemci == null ||
                    (İşlemci.ThreadState != ThreadState.Running && İşlemci.ThreadState != ThreadState.WaitSleepJoin)
                   )
                {
                    İşlemci = new Thread(Th_ArkaPlanİşlemi);
                    İşlemci.Start();
                }
            }

            public Tekil_(int Kip)
            {
                if (Kip != 0)
                {
                    Aş = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_();
                    if (Kip == 1) YığınKarıştırGirdi = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_("", false, YüzdeDeğeriniDeğerlendir);
                    else YığınDüzeltGirdi = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_(YüzdeDeğeriniDeğerlendir);
                }
            }
            int YüzdeDeğeriniDeğerlendir(string Adı, long İşlenenBoyut, long ToplamBoyut)
            {
                if (Kip != 0 && DurDurÇık) Aş.AcilDur();
                Durum = "%" + ((double)(İşlenenBoyut * 100) / (double)ToplamBoyut).ToString("0.#####");
                return 1000;
            }
            bool Argemup_Dosyalama_Kopyala(string Kaynak, string Hedef)
            {
                System.IO.FileStream Kaynak_FileStream = null, Hedef_FileStream = null;

                try
                {
                    Kaynak_FileStream = new System.IO.FileStream(Kaynak, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    if (!Directory.Exists(Path.GetDirectoryName(Hedef))) Directory.CreateDirectory(Path.GetDirectoryName(Hedef));
                    Hedef_FileStream = new System.IO.FileStream(Hedef, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                    long KaynakOkunmuşAdet = 0;
                    int KaynakOkunacakDosyaBoyutu;
                    Byte[] ŞifrelemeTampon = new Byte[1024];

                    int Tick = Environment.TickCount + 1000;
                    while (KaynakOkunmuşAdet < Kaynak_FileStream.Length && !DurDurÇık)
                    {
                        if (Kaynak_FileStream.Length - KaynakOkunmuşAdet > ŞifrelemeTampon.Length) KaynakOkunacakDosyaBoyutu = ŞifrelemeTampon.Length;
                        else KaynakOkunacakDosyaBoyutu = (int)Kaynak_FileStream.Length - (int)KaynakOkunmuşAdet;

                        Kaynak_FileStream.Read(ŞifrelemeTampon, 0, KaynakOkunacakDosyaBoyutu);
                        Hedef_FileStream.Write(ŞifrelemeTampon, 0, KaynakOkunacakDosyaBoyutu);

                        KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;

                        if (Environment.TickCount > Tick)
                        {
                            Hedef_FileStream.Flush(true);
                            YüzdeDeğeriniDeğerlendir(Hedef, KaynakOkunmuşAdet, Kaynak_FileStream.Length);
                            Tick = Environment.TickCount + 1000;
                        }
                    }

                    Kaynak_FileStream.Close();
                    Hedef_FileStream.Close();

                    File.SetCreationTime(Hedef, File.GetCreationTime(Kaynak));
                    File.SetLastAccessTime(Hedef, File.GetLastAccessTime(Kaynak));
                    File.SetLastWriteTime(Hedef, File.GetLastWriteTime(Kaynak));
                    File.SetAccessControl(Hedef, File.GetAccessControl(Kaynak));
                    File.SetAttributes(Hedef, File.GetAttributes(Kaynak));

                    if (DurDurÇık) { KopyalamaİşlemiHatası = "Durdurma Sinyali " + Hedef; goto Cik; }

                    return true;
                }
                catch (Exception ex) { KopyalamaİşlemiHatası = ex.Message; }

            Cik:
                try
                {
                    Kaynak_FileStream.Close();
                }
                catch (Exception) { }

                try
                {
                    Hedef_FileStream.Close();
                }
                catch (Exception) { }

                try
                {
                    AnaEkran.SilDosya(Hedef);
                }
                catch (Exception) { }

                return false;
            }
            void Th_ArkaPlanİşlemi()
            {
                try
                {
                    bool sonuc = false;

                    try
                    {
                        if (File.Exists(Hedef + "_mup_") && File.Exists(Hedef)) File.Delete(Hedef + "_mup_");
                        if (File.Exists(Hedef)) File.Move(Hedef, Hedef + "_mup_");

                        switch (Kip)
                        {
                            case (0)://şifreleme yok  
                                sonuc = Argemup_Dosyalama_Kopyala(GeciciKonumu, Hedef);
                                break;

                            case (1)://şifrele
                                sonuc = Aş.Karıştır(GeciciKonumu, Hedef, Şifre, YığınKarıştırGirdi);
                                break;

                            case (2)://çöz
                                sonuc = Aş.Düzelt(GeciciKonumu, Hedef, Şifre, YığınDüzeltGirdi);
                                break;
                        }

                        if (sonuc)
                        {
                            Durum = "Bitti";
                            AnaEkran.YazLog("Bilgi", "Uzun surecek dosyalama islemi tamamlandı. " + Hedef);

                            if (File.Exists(Hedef + "_mup_")) File.Delete(Hedef + "_mup_");
                        }
                        else
                        {
                            string ek = "";
                            if (Kip != 0) ek = Aş.SonHatayıOku();
                            else ek = KopyalamaİşlemiHatası;
                            AnaEkran.YazLog("HATA", "UzDoİş " + ek + " " + Hedef);
                            Durum = "Hata";

                            if (File.Exists(Hedef)) File.Delete(Hedef);
                            if (File.Exists(Hedef + "_mup_")) File.Move(Hedef + "_mup_", Hedef);
                        }                
                    }
                    catch (Exception ex) { Durum = "Hata"; AnaEkran.YazLog("HATA", "UzDoİş " + ex.Message + " " + Hedef); }
                }
                catch (Exception ex) { Durum = "Hata"; AnaEkran.YazLog("HATA", "UzDoİş " + ex.Message + " " + Hedef); }
            }
        }

        public void ListeyiTemizle()
        {
            foreach (var nesne in Nesneler) nesne.DurDurÇık = true;
            Nesneler.Clear();
            if (Zamanlayıcı != null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
        }
        public string DurumuOku()
        {
            return Durum;
        }
        public bool VarMı(string Kaynak, string Hedef)
        {
            Kaynak = Path.GetFullPath(Kaynak);
            Hedef = Path.GetFullPath(Hedef);

            foreach (var Mevcut in Nesneler) if (Mevcut.Kaynak == Kaynak && Mevcut.Hedef == Hedef) return true;

            return false;
        }
        public string KaynakDosyaGeciciKlasöreKopyalandıMı(string Kaynak)
        {
            ElSıkışma.WaitOne();

            Kaynak = Path.GetFullPath(Kaynak);          
            foreach (var Mevcut in Nesneler) { if (Mevcut.Kaynak == Kaynak) return Mevcut.GeciciKonumu; }

            ElSıkışma.ReleaseMutex();
            return "";
        }
        public string DosyaAdıOluştur(string Kaynak)
        {
            if (Nesneler.Count == 0 && Directory.Exists(Kaynak))
            {
                string[] fileEntries = Directory.GetFiles(Kaynak);
                foreach (string fileName in fileEntries) AnaEkran.SilDosya(fileName);
            }

            Kaynak = Path.GetFullPath(Kaynak) + "\\";

            string çıktı;
            do { çıktı = Kaynak + Path.GetRandomFileName(); } while (File.Exists(çıktı));
            return çıktı;
        }
        public void Ekle(string Kaynak, string Hedef, string GeciciKonumu, string Kip)
        {
            int Gecici = Convert.ToInt32(Kip.Substring(0, 1));
            Tekil_ Yeni = new Tekil_(Gecici);

            Yeni.Kip = Gecici;
            Yeni.Şifre = Kip.Remove(0, 1);
            Yeni.Kaynak = Path.GetFullPath(Kaynak);
            Yeni.Hedef = Path.GetFullPath(Hedef);
            Yeni.GeciciKonumu = Path.GetFullPath(GeciciKonumu);
            Yeni.DosyaBoyutu = ArgeMup.HazirKod.Dönüştürme.D_DosyaBoyutu.Metne(new FileInfo(Kaynak).Length);

            Nesneler.Add(Yeni);

            try { ElSıkışma.ReleaseMutex(); } catch (System.Exception) { }

            if (Zamanlayıcı == null)
            {
                Zamanlayıcı = new System.Timers.Timer();
                Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                Zamanlayıcı.Interval = 5000;
                Zamanlayıcı.AutoReset = false;
                Zamanlayıcı.Enabled = true;
            }
        }
        private void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Nesneler.Count;)
            {
                if (Nesneler[i].Durum == "Bitti")
                {
                    ElSıkışma.WaitOne();

                    int bulunan = 0;
                    foreach (var Mevcut in Nesneler) { if (Mevcut.Kaynak == Nesneler[i].Kaynak) bulunan++; }
                    if (bulunan < 2) AnaEkran.SilDosya(Nesneler[i].GeciciKonumu);
                    Nesneler.RemoveAt(i);

                    ElSıkışma.ReleaseMutex();
                }
                else i++;
            }

            Durum = ""; DetaylıDurum = "";
            int GösterimSınırı = 0;
            List<int> çalışıyor = new List<int>();
            List<int> bekliyor = new List<int>();
            List<int> hata = new List<int>();

            for (int i = 0; i < Nesneler.Count; i++)
            {
                if (GösterimSınırı < 25)
                {
                    DetaylıDurum += Nesneler[i].Durum + " " + Nesneler[i].DosyaBoyutu + " " + Nesneler[i].Hedef + Environment.NewLine;
                    Durum += Nesneler[i].Durum + " ";
                    GösterimSınırı++;
                }

                if (Nesneler[i].Durum == "Bekliyor") bekliyor.Add(i);
                else if (Nesneler[i].Durum == "Hata") hata.Add(i);
                else çalışıyor.Add(i);
            }

            if (Nesneler.Count == 0) Durum = "";
            else
            {
                if (Nesneler.Count - GösterimSınırı > 0)
                {
                    DetaylıDurum += "ve " + (Nesneler.Count - GösterimSınırı).ToString() + " adet daha";
                }
                Durum = Nesneler.Count.ToString() + " : " + Durum;
            }

            while (bekliyor.Count + hata.Count > 0 && çalışıyor.Count < 5)
            {
                if (bekliyor.Count > 0)
                {
                    bool var = false;
                    foreach (var çalışan in çalışıyor) if (Path.GetPathRoot(Nesneler[çalışan].Hedef).ToUpper() == Path.GetPathRoot(Nesneler[bekliyor[0]].Hedef).ToUpper()) { var = true; break; }
                    if (!var) { çalışıyor.Add(bekliyor[0]); Nesneler[bekliyor[0]].Başlat(); }
                    bekliyor.RemoveAt(0);
                }
                else
                {
                    bool var = false;
                    foreach (var çalışan in çalışıyor) if (Path.GetPathRoot(Nesneler[çalışan].Hedef).ToUpper() == Path.GetPathRoot(Nesneler[hata[0]].Hedef).ToUpper()) { var = true; break; }
                    if (!var) { çalışıyor.Add(hata[0]); Nesneler[hata[0]].Başlat(); }
                    hata.RemoveAt(0);
                }
            }

            Zamanlayıcı.Enabled = true;
        }
    }
}