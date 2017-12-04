// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/Yedekleyici>

using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;
using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Dönüştürme;

namespace Yedekleyici
{
    public partial class AnaEkran : Form
    {
        #region Tanımlamalar
        static DahaCokKarmasiklastirma_ Aş = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_();
        static UzunSurecekDosyalamaIslemleri UzSüDoİş = new UzunSurecekDosyalamaIslemleri();
        static string pak = Directory.GetCurrentDirectory() + "\\YedekleyiciDosyalari\\"; //programanaklasör
        PencereVeTepsiIkonuKontrolu_ PeTeİkKo;
        public Ayarlar_ Ayarlar;
        List<ArgeMup.HazirKod.Ayarlar_.BirParametre_> TaleplerDosyası;

        public DateTime SürekliYdeklemeZamanlama = new DateTime();
        bool checkedListBox1_iptalEt = true;
        static public bool Durdur;
        public void Form2ToForm1(int tür, string bilgi)
        {
            switch (tür)
            { 
                case (1):
                    textBox1.Text = bilgi;
                    break;

                case (2):
                    textBox2.Text = bilgi;
                    break;
            }
        }

        string ProgAdVer;
        static string IslemGorenDosya = "";

        enum Yapilacak_İşler { Bekleme = 0, Hazirlik, TalepArttır, TalepBul, Sayma, tarihsel_karşilaştir, crc_karşilaştir, yedekle, fazla_dosyaları_sil, DahaBekle};
        static Yapilacak_İşler Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;

        struct _Talep{
            public int ToplamDosyaSayısı;
            public int ToplamKlasörSayısı;
            public int SonTalepNo;
            public int YedeklenenDosyaSayısı;
            public int şifrelemeYontemi;

            public string Tanım;
            public string Hedef;
            public string OrjinalHedef;
            public string Kaynak;
            public bool Sil;
            public int Süre;
            public int HedefKlasörSayisi;
            public string SonYedekTarihi;
            public string SilinecekKlasör;
            public string Şifre;
            public bool UzunSürecekDosyalamaİşlemlerindeArkaplandaÇalıştır;

            public int SayacOtoYedek;

            public Decimal ToplamBoyut, IslemormusBoyut;
        };
        static _Talep Talep = new _Talep();

        struct _DoSaBu
        {
            public bool Calistir;
        };
        static _DoSaBu DoSaBu = new _DoSaBu();
        static DosyaSayisiniBulan Object_DosyaSayisiniBulan = new DosyaSayisiniBulan(); 
        static Thread Thread_DosyaSayisiniBulan;

        struct _TaKa
        {
            public bool Calistir;
            public bool checkbox3;
            public int biten, hatalı, GenelHatalı;
        };
        static _TaKa TaKa = new _TaKa();
        struct _SüBıİş
        {
            public List<string> SürükleBırakListesi;
            public string şifre;
            public string konum_sifreleme, konum_sifrecozme;
            public ŞifrelemeYöntem yöntem_kararverilen, yöntem_seçilen;
            public bool FareTuşunaBasılıyor;
            public int FareTuşunaBasılıyorX, FareTuşunaBasılıyorY;
            public int DosyaAdet_Toplam, DosyaAdet_İşlenen;
            public bool İsimlerideŞifrele;
            public string ipucu;
        };
        static _SüBıİş SüBıİş = new _SüBıİş();

        enum ŞifrelemeYöntem { KararVer = 0, Şifrele, Çöz };
        static TarihselKarşilaştiran Object_TarihselKarşilaştiran = new TarihselKarşilaştiran();
        static Thread Thread_TarihselKarşilaştiran;
        static Thread Thread_TaKa_SürükleBırakListesi;

        struct _FaDo
        {
            public bool Calistir;
            public int biten;
            public int Toplam;
        };
        static _FaDo FaDo = new _FaDo();
        static FazlaDosyalarıSilen Object_FazlaDosyalarıSilen = new FazlaDosyalarıSilen();
        static Thread Thread_FazlaDosyalarıSilen;

        struct _TaFaA
        {
            public bool Calistir;
            public bool KlasörlerAynı;
            public string A, B;
        };
        static _TaFaA TaFaA = new _TaFaA();
        static TarihselFarkArayan Object_TarihselFarkArayan = new TarihselFarkArayan();
        static Thread Thread_TarihselFarkArayan;

        static public List<string> GeUyKa_Uygulama = new List<string>();
        static GereksizUygulamaKapatan Object_GereksizUygulamaKapatan = new GereksizUygulamaKapatan();
        static Thread Thread_GereksizUygulamaKapatan;

        static List<string> Liste_Hatalar = new List<string>();
        #endregion

        #region Görevler
        public class DosyaSayisiniBulan
        {
            public void DosyaSayisiniBul()
            {
                Durdur = false;
                Talep.ToplamDosyaSayısı = 0;
                Talep.ToplamKlasörSayısı = 0;
                Talep.IslemormusBoyut = 0;
                Talep.ToplamBoyut = 0;

                if (Directory.Exists(Talep.Kaynak)) ProcessDirectory(Talep.Kaynak);
                DoSaBu.Calistir = false;
            }
            public static void ProcessDirectory(string targetDirectory)
            {
                string[] fileEntries;
                try
                {
                    fileEntries = Directory.GetFiles(targetDirectory);
                    foreach (string fileName in fileEntries)
                    {
                        Talep.ToplamDosyaSayısı++;
                        try { Talep.ToplamBoyut += new System.IO.FileInfo(fileName).Length; }
                        catch (Exception) { }
                        if (Durdur) return;
                    }

                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        Talep.ToplamKlasörSayısı++;
                        ProcessDirectory(subdirectory);
                        if (Durdur) return;
                    }
                }
                catch (Exception ex)
                {
                    Liste_Hatalar.Add("HATA");
                    Liste_Hatalar.Add(ex.Message + " " + targetDirectory);
                }
            }
        }
        public class TarihselKarşilaştiran
        {
            static long OncekiIslenenBoyut;
            static DateTime OncekiIslemAnı;
            static int Konum_DosyaİşlemiUzunSürecek; //-1:iptal 0:uzun sürecek ise işlemi durdur 1:durduruldu uzun sürecek işlemler klasörüne kopyala 
            static List<double> ortalama = new List<double>();
            static string Hata;

            static public DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_ YığınKarıştırGirdi = new DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_("", false, YüzdeDeğeriniDeğerlendir_Sifrele);
            static public DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_ YığınDüzeltGirdi = new DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_(YüzdeDeğeriniDeğerlendir_SifresiniCoz);
            static public int YüzdeDeğeriniDeğerlendir_Sifrele(string Adı, long İşlenenBoyut, long ToplamBoyut)
            {
                if (Durdur) Aş.AcilDur();
                IslemGorenDosya = "Karıştırma %" + ((double)(İşlenenBoyut * 100) / (double)ToplamBoyut).ToString("0.##");

                double Sndekiİlerleme_Bayt = İşlenenBoyut - OncekiIslenenBoyut;
                Talep.IslemormusBoyut += (decimal)Sndekiİlerleme_Bayt;
                TimeSpan difference = DateTime.Now - OncekiIslemAnı;
                Sndekiİlerleme_Bayt /= difference.TotalSeconds;

                Sndekiİlerleme_Bayt = (ToplamBoyut - İşlenenBoyut) / Sndekiİlerleme_Bayt; //sn sürecek
                ortalama.Add(Sndekiİlerleme_Bayt); 
                IslemGorenDosya += " " + D_Süre.Saniye.Metne(ortalama.Sum(item => item) / ortalama.Count);
                if (ortalama.Count > 25) ortalama.RemoveAt(0);
                Sndekiİlerleme_Bayt /= 60; //dk sürecek

                if (Konum_DosyaİşlemiUzunSürecek == 0)
                {
                    if (Sndekiİlerleme_Bayt > 1 && Talep.UzunSürecekDosyalamaİşlemlerindeArkaplandaÇalıştır)
                    {
                        Konum_DosyaİşlemiUzunSürecek = 1;
                        Aş.AcilDur();
                        Durdur = true;
                    }
                }
                else if (Konum_DosyaİşlemiUzunSürecek > 0) { IslemGorenDosya += " *" + Konum_DosyaİşlemiUzunSürecek.ToString(); }

                IslemGorenDosya += ">" + Adı;
                OncekiIslenenBoyut = İşlenenBoyut;
                OncekiIslemAnı = DateTime.Now;
                return 1000;
            }
            static public int YüzdeDeğeriniDeğerlendir_SifresiniCoz(string Adı, long İşlenenBoyut, long ToplamBoyut)
            {
                int sonuc = YüzdeDeğeriniDeğerlendir_Sifrele(Adı, İşlenenBoyut, ToplamBoyut);
                IslemGorenDosya = IslemGorenDosya.Remove(0, "Karıştırma".Length);
                IslemGorenDosya = "Düzeltme" + IslemGorenDosya;
                return sonuc;
            }

            public void TarihselKarşilaştir()
            {
                while (TaKa.Calistir)
                {
                    TaKa.GenelHatalı += TaKa.hatalı;
                    TaKa.biten = 0;
                    TaKa.hatalı = 0;
                    Talep.YedeklenenDosyaSayısı = 0;
                    ortalama.Clear();

                    try
                    {
                        if (!Directory.Exists(Talep.Kaynak)) throw new DirectoryNotFoundException();
                        if (!Directory.Exists(Talep.Hedef)) Directory.CreateDirectory(Talep.Hedef);

                        if (Directory.Exists(Talep.Kaynak)) ProcessDirectory(Talep.Kaynak);
                        else Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;
                    }
                    catch (Exception ex)
                    {
                        //olmayan hedef atlanıyor
                        if (Convert.ToString(ex).Contains("System.IO.DirectoryNotFoundException") && TaKa.checkbox3) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                    }

                    TaKa.Calistir = false;
                }
            }
            public static void ProcessDirectory(string targetDirectory)
            {
                string[] fileEntries;
                try
                {
                    fileEntries = Directory.GetFiles(targetDirectory);
                    for (int i = 0; i < fileEntries.Count(); i++)
                    {
                        string fileName = fileEntries[i];

                        if (Durdur) return;

                        string Fazlalık = fileName.Remove(0, Talep.Kaynak.Length);
                        IslemGorenDosya = Talep.Hedef + "\\" + Fazlalık;

                        bool sonuc = false;
                        decimal Onceki_IslemormusBoyut = Talep.IslemormusBoyut;
                        Konum_DosyaİşlemiUzunSürecek = 0;
                        if (Path.GetPathRoot(Talep.Kaynak).ToUpper() == Path.GetPathRoot(Talep.Hedef).ToUpper() || Path.GetPathRoot(Talep.Hedef).ToUpper() == Path.GetPathRoot(pak).ToUpper()) Konum_DosyaİşlemiUzunSürecek = -1; //aynı disk te olduğundan uzun sürecek işlemlere eklemeye gerek yok 

                        if (!File.Exists(Talep.Hedef + "\\" + Fazlalık))
                        {
                            if (fileName.StartsWith(pak + "Banka\\_")) goto while_devam;

                            try
                            {
                                OncekiIslenenBoyut = 0;
                                OncekiIslemAnı = DateTime.Now;
                                //ortalama.Clear();
                                switch (Talep.şifrelemeYontemi)
                                {
                                    case (0)://şifreleme yok  
                                        sonuc = Argemup_Dosyalama_Kopyala(fileName, Talep.Hedef + "\\" + Fazlalık);
                                        break;

                                    case (1)://şifrele
                                        sonuc = Aş.Karıştır(fileName, Talep.Hedef + "\\" + Fazlalık, Talep.Şifre, YığınKarıştırGirdi);
                                        break;

                                    case (2)://çöz
                                        sonuc = Aş.Düzelt(fileName, Talep.Hedef + "\\" + Fazlalık, Talep.Şifre, YığınDüzeltGirdi);
                                        break;
                                }

                                Talep.IslemormusBoyut = Onceki_IslemormusBoyut;

                                if (Konum_DosyaİşlemiUzunSürecek == 1)
                                {
                                    OncekiIslenenBoyut = 0;
                                    OncekiIslemAnı = DateTime.Now;
                                    //ortalama.Clear();
                                    Durdur = false;

                                    if (!Path.GetFullPath(Path.GetDirectoryName(fileName)).StartsWith(pak + "Banka\\_"))
                                    {
                                        if (!UzSüDoİş.VarMı(fileName, Talep.Hedef + "\\" + Fazlalık))
                                        {
                                            string temsilidosya = UzSüDoİş.KaynakDosyaGeciciKlasöreKopyalandıMı(fileName);
                                            if (temsilidosya == "")
                                            {
                                                temsilidosya = UzSüDoİş.DosyaAdıOluştur(pak + "Banka\\_");
                                                if (!Argemup_Dosyalama_Kopyala(fileName, temsilidosya)) goto devam_hata_1;
                                            }
                                            UzSüDoİş.Ekle(fileName, Talep.Hedef + "\\" + Fazlalık, temsilidosya, Talep.şifrelemeYontemi.ToString() + Talep.Şifre);
                                            YazLog("Bilgi", "Uzun surecek dosyalama islemi listeye eklendi. " + Talep.Hedef + "\\" + Fazlalık);
                                        }
                                        else YazLog("Bilgi", "Uzun surecek dosyalama islemi listede olduğu için atlandı. " + Talep.Hedef + "\\" + Fazlalık);
                                    }
                                    sonuc = true;
                                }

                                devam_hata_1:
                                if (!sonuc)
                                {
                                    TaKa.hatalı++;
                                    string ek = "";
                                    if (Talep.şifrelemeYontemi != 0) ek = Aş.SonHatayıOku();
                                    else ek = Hata;
                                    YazLog("HATA", ek + " " + fileName + " ----->>>>>> " + Talep.Hedef + "\\" + Fazlalık);
                                }
                                else
                                {
                                    Talep.IslemormusBoyut += new FileInfo(fileName).Length;
                                    Talep.YedeklenenDosyaSayısı++;
                                }
                            }
                            catch (Exception ex)
                            {
                                TaKa.hatalı++;
                                YazLog("HATA", ex.Message + " " + fileName + " ----->>>>> " + Talep.Hedef + "\\" + Fazlalık + " ----->>>>> ");
                            }
                        }
                        else
                        {
                            DateTime ftime = File.GetLastWriteTime(fileName);
                            DateTime ftime2 = File.GetLastWriteTime(Talep.Hedef + "\\" + Fazlalık);
                            TimeSpan ftimefark = ftime - ftime2;

                            long boyut_k = 0;
                            long boyut_h = 0;
                            if (Talep.şifrelemeYontemi == 0)
                            {
                                boyut_k = new FileInfo(fileName).Length;
                                boyut_h = new FileInfo(Talep.Hedef + "\\" + Fazlalık).Length;
                            }

                            if (System.Math.Abs(ftimefark.TotalSeconds) > 2 ||  //iki dosya arası fark 2 sn yi geçmiş ise 
                                boyut_k != boyut_h)                             //iki dosya boyutu farklı ise 
                            {
                                try
                                {
                                    if (File.Exists(Talep.Hedef + "\\" + Fazlalık + "_mup_")) SilDosya(Talep.Hedef + "\\" + Fazlalık + "_mup_");
                                    File.Move(Talep.Hedef + "\\" + Fazlalık, Talep.Hedef + "\\" + Fazlalık + "_mup_");

                                    OncekiIslenenBoyut = 0;
                                    OncekiIslemAnı = DateTime.Now;
                                    //ortalama.Clear();
                                    switch (Talep.şifrelemeYontemi)
                                    {
                                        case (0)://şifreleme yok
                                            sonuc = Argemup_Dosyalama_Kopyala(fileName, Talep.Hedef + "\\" + Fazlalık);
                                            break;

                                        case (1)://şifrele
                                            sonuc = Aş.Karıştır(fileName, Talep.Hedef + "\\" + Fazlalık, Talep.Şifre, YığınKarıştırGirdi);
                                            break;

                                        case (2)://çöz
                                            sonuc = Aş.Düzelt(fileName, Talep.Hedef + "\\" + Fazlalık, Talep.Şifre, YığınDüzeltGirdi);
                                            break;
                                    }

                                    Talep.IslemormusBoyut = Onceki_IslemormusBoyut;

                                    if (Konum_DosyaİşlemiUzunSürecek == 1)
                                    {
                                        OncekiIslenenBoyut = 0;
                                        OncekiIslemAnı = DateTime.Now;
                                        //ortalama.Clear();
                                        Durdur = false;

                                        if (!Path.GetFullPath(Path.GetDirectoryName(fileName)).StartsWith(pak + "Banka\\_"))
                                        {
                                            if (!UzSüDoİş.VarMı(fileName, Talep.Hedef + "\\" + Fazlalık))
                                            {
                                                string temsilidosya = UzSüDoİş.KaynakDosyaGeciciKlasöreKopyalandıMı(fileName);
                                                if (temsilidosya == "")
                                                {
                                                    temsilidosya = UzSüDoİş.DosyaAdıOluştur(pak + "Banka\\_");
                                                    if (!Argemup_Dosyalama_Kopyala(fileName, temsilidosya)) goto devam_hata_2;
                                                }
                                                UzSüDoİş.Ekle(fileName, Talep.Hedef + "\\" + Fazlalık, temsilidosya, Talep.şifrelemeYontemi.ToString() + Talep.Şifre);
                                                YazLog("Bilgi", "Uzun surecek dosyalama islemi listeye eklendi. " + Talep.Hedef + "\\" + Fazlalık);
                                            }
                                            else YazLog("Bilgi", "Uzun surecek dosyalama islemi listede olduğu için atlandı. " + Talep.Hedef + "\\" + Fazlalık);
                                        }
                                        sonuc = true;
                                    }

                                    devam_hata_2:
                                    if (!sonuc)
                                    {
                                        TaKa.hatalı++;
                                        string ek = "";
                                        if (Talep.şifrelemeYontemi != 0) ek = Aş.SonHatayıOku();
                                        else ek = Hata;
                                        YazLog("HATA", ek + " " + fileName + " ----->>>>>> " + Talep.Hedef + "\\" + Fazlalık);

                                        if (File.Exists(Talep.Hedef + "\\" + Fazlalık)) SilDosya(Talep.Hedef + "\\" + Fazlalık);
                                        File.Move(Talep.Hedef + "\\" + Fazlalık + "_mup_", Talep.Hedef + "\\" + Fazlalık);
                                    }
                                    else
                                    {
                                        Talep.IslemormusBoyut += new FileInfo(fileName).Length;
                                        Talep.YedeklenenDosyaSayısı++;
                                        if (Konum_DosyaİşlemiUzunSürecek == 0) SilDosya(Talep.Hedef + "\\" + Fazlalık + "_mup_");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    TaKa.hatalı++;
                                    YazLog("HATA", ex.Message + " " + fileName + " ----->>>>> " + Talep.Hedef + "\\" + Fazlalık + " ----->>>>> ");
                                }
                            }
                            else
                            {
                                try { Talep.IslemormusBoyut += new System.IO.FileInfo(fileName).Length; }
                                catch (Exception) { }
                            }
                        }

                        while_devam:
                        TaKa.biten++;
                    }

                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (Durdur) return;

                        string Fazlalık = subdirectory.Remove(0, Talep.Kaynak.Length);
                        if (!Directory.Exists(Talep.Hedef + "\\" + Fazlalık)) Directory.CreateDirectory(Talep.Hedef + "\\" + Fazlalık);

                        ProcessDirectory(subdirectory);
                    }
                }
                catch (Exception ex)
                {
                    Liste_Hatalar.Add("HATA");
                    Liste_Hatalar.Add(ex.Message + " " + targetDirectory);
                }
            }
            public static bool Argemup_Dosyalama_Kopyala(string Kaynak, string Hedef)
            {
                System.IO.FileStream Kaynak_FileStream = null, Hedef_FileStream = null;

                try
                {
                    Kaynak_FileStream = new System.IO.FileStream(Kaynak, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    if (!Directory.Exists(Path.GetDirectoryName(Hedef))) Directory.CreateDirectory(Path.GetDirectoryName(Hedef));
                    Hedef_FileStream = new System.IO.FileStream(Hedef, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                    long KaynakOkunmuşAdet = 0;
                    int KaynakOkunacakDosyaBoyutu;
                    byte[] Tampon = new byte[4 * 1024];

                    int Tick = Environment.TickCount + 1000;
                    while (KaynakOkunmuşAdet < Kaynak_FileStream.Length && !Durdur)
                    {
                        if (Kaynak_FileStream.Length - KaynakOkunmuşAdet > Tampon.Length) KaynakOkunacakDosyaBoyutu = Tampon.Length;
                        else KaynakOkunacakDosyaBoyutu = (int)Kaynak_FileStream.Length - (int)KaynakOkunmuşAdet;

                        Kaynak_FileStream.Read(Tampon, 0, KaynakOkunacakDosyaBoyutu);
                        Hedef_FileStream.Write(Tampon, 0, KaynakOkunacakDosyaBoyutu);

                        KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;

                        if (Environment.TickCount > Tick)
                        {
                            Hedef_FileStream.Flush(true);

                            YüzdeDeğeriniDeğerlendir_Sifrele(Hedef, KaynakOkunmuşAdet, Kaynak_FileStream.Length);
                            IslemGorenDosya = IslemGorenDosya.Remove(0, "Karıştırma".Length);
                            IslemGorenDosya = "Kopyalama" + IslemGorenDosya;

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

                    if (Durdur) { Hata = "Durdurma Sinyali"; goto Cik; }

                    return true;
                }
                catch (Exception ex) { Hata = ex.Message; }

                Cik:
                try { Kaynak_FileStream.Close(); } catch (Exception) { }
                try { Hedef_FileStream.Close(); } catch (Exception) { }
                try { SilDosya(Hedef); } catch (Exception) { }

                return false;
            }
            public void SürükleBırakKlasörSifreleme()
            {
                ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_ YığınKarıştırGirdi_2 = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_(SüBıİş.ipucu, SüBıİş.İsimlerideŞifrele);

                while (SüBıİş.SürükleBırakListesi.Count > 0)
                {
                    try
                    {
                        SüBıİş.yöntem_kararverilen = SüBıİş.yöntem_seçilen;
                        string Fazlalık, HedefKlasör, HedefKlasör_, DosyaAdı, konum_sifrecozme, konum_sifreleme;
                        string[] fileEntries;
                        if (string.IsNullOrEmpty(SüBıİş.konum_sifrecozme)) SüBıİş.konum_sifrecozme = "";
                        if (string.IsNullOrEmpty(SüBıİş.konum_sifreleme)) SüBıİş.konum_sifreleme = "";

                        konum_sifrecozme = SüBıİş.konum_sifrecozme;
                        Dene_Oluştur_düzeltme:
                        if (!Directory.Exists(konum_sifrecozme))
                        {
                            try
                            {
                                Directory.CreateDirectory(konum_sifrecozme);
                                Directory.Delete(konum_sifrecozme);
                            }
                            catch (Exception)
                            {
                                konum_sifrecozme = Path.GetDirectoryName(SüBıİş.SürükleBırakListesi[0]) + "\\Duzeltme\\";
                                goto Dene_Oluştur_düzeltme;
                            }
                        }
                        konum_sifrecozme += "\\";

                        konum_sifreleme = SüBıİş.konum_sifreleme;
                        Dene_Oluştur_karıştırma:
                        if (!Directory.Exists(konum_sifreleme))
                        {
                            try
                            {
                                Directory.CreateDirectory(konum_sifreleme);
                                Directory.Delete(konum_sifreleme);
                            }
                            catch (Exception)
                            {
                                konum_sifreleme = Path.GetDirectoryName(SüBıİş.SürükleBırakListesi[0]) + "\\Karistirma\\";
                                goto Dene_Oluştur_karıştırma;
                            }
                        }
                        konum_sifreleme += "\\";

                        bool Sadece1dosya = false;
                        string SüBıİş_SürükleBırakListesi_0_;
                        if (File.Exists(SüBıİş.SürükleBırakListesi[0]))
                        {
                            fileEntries = SüBıİş.SürükleBırakListesi[0].Split('>');
                            HedefKlasör_ = "";
                            SüBıİş_SürükleBırakListesi_0_ = Path.GetDirectoryName(fileEntries[0]);
                            Sadece1dosya = true;
                        }
                        else if (Directory.Exists(SüBıİş.SürükleBırakListesi[0]))
                        {
                            fileEntries = Directory.GetFiles(SüBıİş.SürükleBırakListesi[0], "*", System.IO.SearchOption.AllDirectories);
                            HedefKlasör_ = SüBıİş.SürükleBırakListesi[0].Substring(SüBıİş.SürükleBırakListesi[0].LastIndexOf('\\'));
                            SüBıİş_SürükleBırakListesi_0_ = SüBıİş.SürükleBırakListesi[0];
                        }
                        else throw new System.InvalidOperationException("HATALI BİLGİ KULLANILAMIYOR");

                        SürükleBırakKlasörSifreleme_YenidenDene:
                        SüBıİş.DosyaAdet_Toplam = fileEntries.Length;
                        SüBıİş.DosyaAdet_İşlenen = 0;

                        for (int i = 0; i < fileEntries.Count(); i++)
                        {
                            string fileName = fileEntries[i];

                            try
                            {
                                SüBıİş.DosyaAdet_İşlenen++;

                                DosyaAdı = Path.GetFileName(fileName);
                                Fazlalık = Path.GetDirectoryName(fileName);
                                Fazlalık = Fazlalık.Remove(0, SüBıİş_SürükleBırakListesi_0_.Length);
                                while (Fazlalık.EndsWith("\\")) Fazlalık = Fazlalık.Remove(Fazlalık.Count() - 1);
                                HedefKlasör = HedefKlasör_ + "\\" + Fazlalık;

                                switch (SüBıİş.yöntem_kararverilen)
                                {
                                    case (ŞifrelemeYöntem.KararVer):
                                    case (ŞifrelemeYöntem.Çöz):
                                        if (!Aş.Düzelt(fileName, konum_sifrecozme + HedefKlasör + "\\" + DosyaAdı, SüBıİş.şifre))
                                        {
                                            if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.KararVer)
                                            {
                                                SüBıİş.yöntem_kararverilen = ŞifrelemeYöntem.Şifrele;
                                                goto SürükleBırakKlasörSifreleme_YenidenDene;
                                            }

                                            Liste_Hatalar.Add("HATA");
                                            Liste_Hatalar.Add(Aş.SonHatayıOku() + " " + fileName);
                                        }
                                        else
                                        {
                                            Talep.IslemormusBoyut += new FileInfo(fileName).Length;
                                            SüBıİş.yöntem_kararverilen = ŞifrelemeYöntem.Çöz;
                                        }
                                        break;

                                    case (ŞifrelemeYöntem.Şifrele):
                                        if (!Aş.Karıştır(fileName, konum_sifreleme + HedefKlasör + "\\" + DosyaAdı, SüBıİş.şifre, YığınKarıştırGirdi_2))
                                        {
                                            Liste_Hatalar.Add("HATA");
                                            Liste_Hatalar.Add(Aş.SonHatayıOku() + " " + fileName);
                                        }
                                        else Talep.IslemormusBoyut += new FileInfo(fileName).Length;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Liste_Hatalar.Add("HATA");
                                Liste_Hatalar.Add("Sürükle Bırak Karıştırma " + SüBıİş_SürükleBırakListesi_0_ + " kuyruktan ATILDI >>> EXCEPTION " + ex.Message);
                            }

                            if (Durdur) return;
                        }

                        if (!Sadece1dosya)
                        {
                            fileEntries = Directory.GetDirectories(SüBıİş_SürükleBırakListesi_0_, "*", System.IO.SearchOption.AllDirectories);
                            SüBıİş.DosyaAdet_Toplam = fileEntries.Length + 1000000;

                            //eksik klasörlerin oluşturulması
                            for (int i = fileEntries.Count() - 1; i >= 0; i--)
                            {
                                SüBıİş.DosyaAdet_İşlenen = i;

                                Fazlalık = fileEntries[i].Remove(0, SüBıİş_SürükleBırakListesi_0_.Length);
                                if (Fazlalık != "") HedefKlasör = HedefKlasör_ + "\\" + Fazlalık;
                                else HedefKlasör = "";

                                if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.Şifrele)
                                {
                                    if (!Directory.Exists(konum_sifreleme + HedefKlasör)) Directory.CreateDirectory(konum_sifreleme + HedefKlasör);
                                }
                                else
                                {
                                    if (!Directory.Exists(konum_sifrecozme + HedefKlasör)) Directory.CreateDirectory(konum_sifrecozme + HedefKlasör);
                                }
                                if (Durdur) return;
                            }

                            string Kaynak;
                            //klasör isimlerinini işlenmesi
                            if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.Şifrele && SüBıİş.İsimlerideŞifrele)
                            {
                                SüBıİş.DosyaAdet_Toplam = fileEntries.Length + 2000000;

                                for (int i = fileEntries.Count() - 1; i >= 0; i--)
                                {
                                    SüBıİş.DosyaAdet_İşlenen = i;

                                    Fazlalık = fileEntries[i].Remove(0, SüBıİş_SürükleBırakListesi_0_.Length);
                                    if (Fazlalık != "") HedefKlasör = HedefKlasör_ + "\\" + Fazlalık;
                                    else HedefKlasör = "";

                                    Kaynak = konum_sifreleme + HedefKlasör + "\\MupYedekleyiciKlasorAdiDosyasi.mup";
                                    File.WriteAllText(Kaynak, Path.GetFileName(konum_sifreleme + HedefKlasör));
                                    File.SetAttributes(Kaynak, FileAttributes.Hidden | FileAttributes.System);
                                    if (Aş.Karıştır((string)Kaynak, (string)Kaynak, SüBıİş.şifre, YığınKarıştırGirdi_2))
                                    {
                                        SilDosya(Kaynak);
                                        DirectoryInfo hedef__ = Directory.GetParent(konum_sifreleme + HedefKlasör);
                                        Directory.Move(konum_sifreleme + HedefKlasör, hedef__.FullName + "\\" + Path.GetRandomFileName().Replace(".", ""));
                                    }
                                    else if (File.Exists(Kaynak)) SilDosya(Kaynak);

                                    if (Durdur) return;
                                }

                                Kaynak = konum_sifreleme + HedefKlasör_ + "\\MupYedekleyiciKlasorAdiDosyasi.mup";
                                File.WriteAllText(Kaynak, Path.GetFileName(konum_sifreleme + HedefKlasör_));
                                if (Aş.Karıştır((string)Kaynak, (string)Kaynak, SüBıİş.şifre, YığınKarıştırGirdi_2))
                                {
                                    SilDosya(Kaynak);
                                    DirectoryInfo hedef__ = Directory.GetParent(konum_sifreleme + HedefKlasör_);
                                    Directory.Move(konum_sifreleme + HedefKlasör_, hedef__.FullName + "\\" + Path.GetRandomFileName().Replace(".", ""));
                                }
                                else if (File.Exists(Kaynak)) SilDosya(Kaynak);
                            }
                            else if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.Çöz)
                            {
                                fileEntries = Directory.GetFiles(konum_sifrecozme, "MupYedekleyiciKlasorAdiDosyasi.mup", System.IO.SearchOption.AllDirectories);
                                SüBıİş.DosyaAdet_Toplam = fileEntries.Length + 3000000;

                                for (int i = fileEntries.Count() - 1; i >= 0; i--)
                                {
                                    SüBıİş.DosyaAdet_İşlenen = i;

                                    if (File.Exists(fileEntries[i]))
                                    {
                                        string AsılKlasörAdı = File.ReadAllText(fileEntries[i]);
                                        SilDosya(fileEntries[i]);
                                        Kaynak = Path.GetDirectoryName(fileEntries[i]);
                                        string Hedef = Directory.GetParent(Kaynak).FullName + "\\" + AsılKlasörAdı;

                                        if (Directory.Exists(Hedef))
                                        {
                                            int gecici_klasör_no = 1;
                                            while (Directory.Exists(Hedef + "_" + gecici_klasör_no) && !Durdur) gecici_klasör_no++;
                                            Hedef = Hedef + "_" + gecici_klasör_no;
                                        }

                                        while (Directory.Exists(Kaynak)) { try { Directory.Move(Kaynak, Hedef); } catch (Exception) { Thread.Sleep(100); SüBıİş.DosyaAdet_Toplam += 1000000; } }
                                    }
                                    if (Durdur) return;
                                }
                            }

                        }

                        Liste_Hatalar.Add("Bilgi");
                        Liste_Hatalar.Add("Sürükle Bırak Karıştırma Kalan " + Convert.ToString(SüBıİş.SürükleBırakListesi.Count - 1) + ", Tamamlanan " + SüBıİş_SürükleBırakListesi_0_);
                    }
                    catch (Exception ex)
                    {
                        Liste_Hatalar.Add("HATA");
                        Liste_Hatalar.Add("Sürükle Bırak Karıştırma " + SüBıİş.SürükleBırakListesi[0] + " kuyruktan ATILDI >>> EXCEPTION " + ex.Message);
                    }

                    SüBıİş.SürükleBırakListesi.RemoveAt(0);
                    if (Durdur) return;
                }
            }
        }
        public class FazlaDosyalarıSilen
        {
            public void FazlaDosyalarıSil()
            {
                while (FaDo.Calistir)
                {
                    FaDo.biten = 0;
                    FaDo.Toplam = 0;
                    if (Directory.Exists(Talep.Hedef)) { if (Directory.Exists(Talep.Kaynak)) { ProcessDirectory(Talep.Hedef); } }
                    else Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;
                    FaDo.Calistir = false;
                }
            }

            public static void ProcessDirectory(string targetDirectory)
            {
                string[] fileEntries;
                try
                {
                    fileEntries = Directory.GetFiles(targetDirectory);
                    foreach (string fileName in fileEntries)
                    {
                        if (Durdur) return;

                        string Fazlalık = fileName.Remove(0, Talep.Hedef.Length);

                        IslemGorenDosya = Talep.Kaynak + "\\" + Fazlalık;

                        if (!File.Exists(Talep.Kaynak + "\\" + Fazlalık))
                        {
                            if (Fazlalık.Contains("_mup_"))
                            {
                                if (Fazlalık.EndsWith("._mup_Ayarlar_mup_"))
                                {
                                    //ayarlar dosyası işlem yapma
                                }
                                else
                                {
                                    if (File.Exists((Talep.Hedef + "\\" + Fazlalık).Substring(0, (Talep.Hedef + "\\" + Fazlalık).Length - ("_mup_").Length))) SilDosya(Talep.Hedef + "\\" + Fazlalık);
                                    else if (!File.Exists((Talep.Kaynak + "\\" + Fazlalık).Substring(0, (Talep.Kaynak + "\\" + Fazlalık).Length - ("_mup_").Length))) SilDosya(Talep.Hedef + "\\" + Fazlalık);
                                    else
                                    {
                                        TaKa.hatalı++;
                                        YazLog("Bilgi", "_mup_ Korumalı Dosya " + Talep.Hedef + "\\" + Fazlalık);
                                    }
                                }
                            }
                            else
                            {
                                if (SilDosya(Talep.Hedef + "\\" + Fazlalık)) FaDo.biten++;
                                else
                                {
                                    TaKa.hatalı++;
                                    YazLog("HATA", "Hatalı Silinmiyor 1 " + Talep.Hedef + "\\" + Fazlalık);
                                }
                            }
                        }
                        FaDo.Toplam++;
                    }

                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (Durdur) return;

                        ProcessDirectory(subdirectory);

                        string Fazlalık = subdirectory.Remove(0, Talep.Hedef.Length);
                        if (!Directory.Exists(Talep.Kaynak + "\\" + Fazlalık))
                        {
                            try
                            {
                                DirectorySecurity fSecurity = Directory.GetAccessControl(Talep.Hedef + "\\" + Fazlalık);
                                string acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //@"DomainName\AccountName"
                                fSecurity.AddAccessRule(new FileSystemAccessRule(@acc, FileSystemRights.FullControl, AccessControlType.Allow));
                                Directory.SetAccessControl(Talep.Hedef + "\\" + Fazlalık, fSecurity);
                                Directory.Delete(Talep.Hedef + "\\" + Fazlalık, true);
                            }
                            catch (Exception ex)
                            {
                                TaKa.hatalı++;
                                YazLog("HATA", "Hatalı Silinmiyor ex " + Talep.Hedef + "\\" + Fazlalık + " ----->>>>> " + ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Liste_Hatalar.Add("HATA");
                    Liste_Hatalar.Add(ex.ToString() + " " + targetDirectory);
                }
            }

            public void SilKlasör()
            {
                try
                {
                    if (Directory.Exists(Talep.SilinecekKlasör)) Directory.Delete(Talep.SilinecekKlasör, true);
                    if (Directory.Exists(Talep.SilinecekKlasör)) SilKlasör_(Talep.SilinecekKlasör);
                }
                catch (Exception)
                {
                    TaKa.hatalı++;
                }

                FaDo.Calistir = false;
            }

            public void SilKlasör_(string yol)
            {
                string[] fileEntries;
                try
                {
                    fileEntries = Directory.GetFiles(yol);
                    foreach (string fileName in fileEntries)
                    {
                        if (Durdur) return;

                        if (SilDosya(fileName)) FaDo.biten++;
                        else
                        {
                            TaKa.hatalı++;
                            YazLog("HATA", "Hatalı Silinmiyor 2 " + fileName);
                        }

                    }

                    string[] subdirectoryEntries = Directory.GetDirectories(yol);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (Durdur) return;

                        try
                        {
                            SilKlasör_(subdirectory);
                            DirectorySecurity fSecurity = Directory.GetAccessControl(subdirectory);
                            string acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //@"DomainName\AccountName"
                            fSecurity.AddAccessRule(new FileSystemAccessRule(@acc, FileSystemRights.FullControl, AccessControlType.Allow));
                            Directory.SetAccessControl(subdirectory, fSecurity);
                            Directory.Delete(subdirectory, true);
                        }
                        catch (Exception ex)
                        {
                            YazLog("HATA", "Hatalı Silinmiyor ex " + subdirectory + " ----->>>>> " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Liste_Hatalar.Add("HATA");
                    Liste_Hatalar.Add(ex.Message + " " + yol);
                }
            }
        }
        public class TarihselFarkArayan
        {
            public void TarihselFarkAra()
            {
                while (TaFaA.Calistir)
                {
                    TaFaA.KlasörlerAynı = false;

                    try
                    {
                        if (!Directory.Exists(TaFaA.A)) break;
                        if (!Directory.Exists(TaFaA.B)) break;

                        if (!ProcessDirectory(TaFaA.A, TaFaA.B)) break;
                        if (!ProcessDirectory(TaFaA.B, TaFaA.A)) break;

                        TaFaA.KlasörlerAynı = true;
                    }
                    catch (Exception)
                    {
                        //olmayan hedef atlanıyor
                    }
                    TaFaA.Calistir = false;
                }
                TaFaA.Calistir = false;
            }

            public static bool ProcessDirectory(string A, string B)
            {
                string[] fileEntries;
                try
                {
                    fileEntries = Directory.GetFiles(A);
                    foreach (string fileName in fileEntries)
                    {
                        if (Durdur) return false;

                        string Fazlalık = fileName.Remove(0, A.Length);

                        IslemGorenDosya = "Özdeşlik Kontrolü, Bekleyiniz>" + B + "\\" + Fazlalık;

                        if (!File.Exists(B + "\\" + Fazlalık))
                        {
                            return false;
                        }
                        else
                        {
                            DateTime ftime = File.GetLastWriteTime(fileName);
                            DateTime ftime2 = File.GetLastWriteTime(B + "\\" + Fazlalık);

                            TimeSpan ftimefark = ftime - ftime2;

                            if (System.Math.Abs(ftimefark.TotalSeconds) > 2) //ikli dosya arası fark 3 sn yi geçmiş ise 
                            {
                                return false;
                            }
                        }
                    }

                    string[] subdirectoryEntries = Directory.GetDirectories(A);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (Durdur) return false;

                        string Fazlalık = subdirectory.Remove(0, A.Length);
                        if (!Directory.Exists(B + "\\" + Fazlalık)) return false;

                        if (!ProcessDirectory(A + "\\" + Fazlalık + "\\", B + "\\" + Fazlalık + "\\"))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Liste_Hatalar.Add("HATA");
                    Liste_Hatalar.Add(ex.Message + " " + A);
                }
                return false;
            }
        }
        public class GereksizUygulamaKapatan
        {
            public void Calistir()
            {
                while (GeUyKa_Uygulama.Count > 0)
                {
                    if (Durdur) return;

                    try
                    {
                        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

                        foreach (var proc in processes)
                        {
                            if (Durdur) return;

                            for (int i = 0; i < GeUyKa_Uygulama.Count; i += 2)
                            {
                                if (Durdur) return;

                                if (proc.ProcessName.Trim().ToLower() == GeUyKa_Uygulama[i].Trim().ToLower())
                                {
                                    using (System.Diagnostics.PerformanceCounter pcProcess = new System.Diagnostics.PerformanceCounter("Process", "% Processor Time", proc.ProcessName))
                                    {
                                        pcProcess.NextValue();
                                        System.Threading.Thread.Sleep(1000);
                                        Double ss = pcProcess.NextValue();
                                        if (ss >= Convert.ToDouble(GeUyKa_Uygulama[i + 1]))
                                        {
                                            if (!proc.HasExited)
                                            {
                                                YazLog("Bilgi", "Uygulama Kapatıldı " + proc.ProcessName + " cpu %" + Convert.ToString(ss));
                                                proc.Kill();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                    System.Threading.Thread.Sleep(10000);
                }
            }
        }
        #endregion

        public AnaEkran()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime dates_simdi = new DateTime();
                DateTime date_terhis = new DateTime(2011, 05, 05, 09, 20, 10);
                dates_simdi = DateTime.Now;
                TimeSpan fark = dates_simdi.Subtract(date_terhis);

                ProgAdVer = "Mup Yedekleyici V" + Application.ProductVersion + " " + fark.Days;
                this.Text = ProgAdVer;

                Talep.SayacOtoYedek = 1000;
                dataGridView1.RowCount = 1;
                dataGridView1[0, 0].Value = "İşlenen";

                comboBox1.SelectedIndex = 0;
                SüBıİş.SürükleBırakListesi = new List<string>();
                SüBıİş.yöntem_seçilen = (ŞifrelemeYöntem)comboBox1.SelectedIndex;
                SüBıİş.FareTuşunaBasılıyor = false;

                SilDosya(pak + "Banka\\" + "Hatalar.csv");

                SürekliYdeklemeZamanlama = DateTime.Now;

                textBox5.Left = groupBox1.Left;
                textBox5.Top = groupBox1.Top;
                textBox5.Width = groupBox1.Width;
                textBox5.Height = checkedListBox1.Height + groupBox1.Height;

                flowLayoutPanel1.Height = 290;
                flowLayoutPanel1.Width = 3 + button7.Width + 6 + button8.Width +
                                            6 + button9.Width + 6 + button10.Width +
                                            6 + button11.Width + 6 + button12.Width +
                                            6 + checkBox2.Width + 25;
                flowLayoutPanel1.Left = 10;

                bool sonuç = false;
                Ayarlar = new Ayarlar_(out sonuç, Convert.ToString(195324687) + Environment.MachineName.ToUpper() + "." + Environment.UserName.ToUpper() + D_Parmakİzi.Metne(), pak + "Banka\\" + Environment.MachineName.ToUpper() + "." + Environment.UserName.ToUpper() + "._mup_Ayarlar_mup_", false, 30, 30, true);
          
                PeTeİkKo = new PencereVeTepsiIkonuKontrolu_(this, Ayarlar, false, "", Location.X, Location.Y, Width, Height);
                PeTeİkKo.TepsiİkonunuBaşlat();
                PeTeİkKo.Tepsiİkonu.ContextMenuStrip = contextMenuStrip1;
            }
            catch (Exception) { }
        }
        private void AnaEkran_Shown(object sender, EventArgs e)
        {
            TaleplerDosyasınıListele();
            TazeleChecedBox1();
            GosterTalepBilgileri(0);

            flowLayoutPanel1.Top = textBox5.Top + textBox5.Height;

            timer1.Enabled = true;
            timer3.Enabled = true;

            if (Directory.Exists(pak + "Banka\\_")) Directory.Delete(pak + "Banka\\_", true);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer2_Tick(null, null);
            Durdur = false;
            ListeyiTaleplerDosyasınaYaz();

            button2_Click(null, null);

            PeTeİkKo.Dispose();
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;

                textBox1.BackColor = Color.YellowGreen;
                textBox2.BackColor = Color.YellowGreen;
                textBox4.BackColor = Color.YellowGreen;
                textBox6.BackColor = Color.YellowGreen;
                checkedListBox1.BackColor = Color.YellowGreen;
                textBox5.BackColor = Color.YellowGreen;
                dataGridView1.DefaultCellStyle.BackColor = Color.YellowGreen;
            }
            else e.Effect = DragDropEffects.None;
        }
        private void Form1_DragLeave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
            textBox4.BackColor = Color.White;
            textBox6.BackColor = Color.White;
            checkedListBox1.BackColor = Color.White; ;
            textBox5.BackColor = Color.Gainsboro;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);
            textBox5.AppendText(Environment.NewLine);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (File.Exists(file) || Directory.Exists(file))
                {
                    bool aynısıvar = false;
                    int i = 0;
                    for (; i < SüBıİş.SürükleBırakListesi.Count; i++)
                    {
                        if (SüBıİş.SürükleBırakListesi[i] == file)
                        {
                            aynısıvar = true;
                            break;
                        }
                    }

                    if (!aynısıvar)
                    {
                        SüBıİş.SürükleBırakListesi.Add(file);
                        GosterLog("Bilgi", "Sürükle Bırak Karıştırma Kuyruğa Eklendi " + file + " Sıra No " + Convert.ToString(SüBıİş.SürükleBırakListesi.Count));
                    }
                    else GosterLog("UYARI", "Sürükle Bırak Karıştırma ZATEN KUYRUKTA " + file + " Sıra No " + Convert.ToString(i + 1));
                }
                else GosterLog("UYARI", "UYGUN DEGİL " + file);
            }
        }

        static public bool YazLog(string Tip, string mesaj)
        {
            try
            {
                DateTime saveNow = DateTime.Now;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pak + "Banka\\" + "Hatalar.csv", true))
                {
                    Liste_Hatalar.Add(Tip);
                    Liste_Hatalar.Add(mesaj);
                    file.WriteLine(saveNow.ToString() + ";" + Tip + " " + mesaj);
                }

                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show("İstenmeyen Durum " + ex.Message);
            }
            return false;
        }
        public void GosterLog(string Tip, string mesaj)
        {
            if (Tip != "HATA")
            {
                switch (textBox5.Lines.Count())
                {
                    case (0):
                        break;

                    case (1):
                    case (2):
                        if (textBox5.Lines[0].Contains(mesaj)) return;
                        break;

                    default:
                        if (textBox5.Lines[textBox5.Lines.Count() - 2].Contains(mesaj)) return;
                        break;
                }
            }

            textBox5.AppendText("- " + Tip + " " + Convert.ToString(DateTime.Now) + " " + mesaj + Environment.NewLine);
            textBox5.ScrollToCaret();
        }

        public void TaleplerDosyasınıListele()
        {
            TaleplerDosyası = Ayarlar.Listele();
            for (int i = 0; i < TaleplerDosyası.Count();)
            {
                if (!TaleplerDosyası[i].Parametre.StartsWith("Talep_")) TaleplerDosyası.RemoveAt(i);
                else i++;
            }

            string okunan = Ayarlar.Oku("Sürükle Bırak İşlemleri");
            if (okunan != "")
            {
                SüBıİş.şifre = Ayarlar.Oku_AltDal(okunan, "şifre");
                if (SüBıİş.şifre != "") textBox7.Text = ">>>>> Kullanıcının Parolası";
                else textBox7.Text = "";

                SüBıİş.konum_sifrecozme = Ayarlar.Oku_AltDal(okunan, "düzeltme konumu");
                if (SüBıİş.konum_sifrecozme == "") textBox6.Text = ">>>>> Kaynak Konumu + \"\\Duzeltme\"";
                else textBox6.Text = SüBıİş.konum_sifrecozme;

                SüBıİş.konum_sifreleme = Ayarlar.Oku_AltDal(okunan, "karıştırma konumu");
                if (SüBıİş.konum_sifreleme == "") textBox4.Text = ">>>>> Kaynak Konumu + \"\\Karistirma\"";
                else textBox4.Text = SüBıİş.konum_sifreleme;

                SüBıİş.yöntem_seçilen = (ŞifrelemeYöntem)Convert.ToInt32(Ayarlar.Oku_AltDal(okunan, "yöntem"));
                comboBox1.SelectedIndex = (int)SüBıİş.yöntem_seçilen;

                checkBox4.Checked = true;
                if (Ayarlar.Oku_AltDal(okunan, "isimleride şifrele") == "Hayır") checkBox4.Checked = false;
                SüBıİş.İsimlerideŞifrele = checkBox4.Checked;

                textBox8.Text = Ayarlar.Oku_AltDal(okunan, "ipucu");
                if (string.IsNullOrEmpty(textBox8.Text)) textBox8.Text = "İpucu";
                else textBox8.Text = textBox8.Text.Replace(";", ".,");
                if (textBox8.Text != "İpucu") SüBıİş.ipucu = textBox8.Text;
                else SüBıİş.ipucu = "";
            }
            else button13_Click(null, null); 
        }
        private void ListeyiTaleplerDosyasınaYaz()
        {
            if (Durdur) return;

            List<Ayarlar_.BirParametre_> gecici = Ayarlar.Listele();
            for (int i = 0; i < gecici.Count();i++)
            {
                if (gecici[i].Parametre.StartsWith("Talep_")) Ayarlar.Sil(gecici[i].Parametre);
            }

            Ayarlar.ListeyiYaz(TaleplerDosyası);

            string okunan = "";
            Ayarlar.Yaz_AltDal(ref okunan, "şifre", SüBıİş.şifre);
            Ayarlar.Yaz_AltDal(ref okunan, "düzeltme konumu", SüBıİş.konum_sifrecozme);
            Ayarlar.Yaz_AltDal(ref okunan, "karıştırma konumu", SüBıİş.konum_sifreleme);
            Ayarlar.Yaz_AltDal(ref okunan, "yöntem", Convert.ToString((Int32)(SüBıİş.yöntem_seçilen)));
            Ayarlar.Yaz_AltDal(ref okunan, "ipucu", SüBıİş.ipucu);
            Ayarlar.Yaz_AltDal(ref okunan, "isimleride şifrele", SüBıİş.İsimlerideŞifrele.ToString());
            Ayarlar.Yaz("Sürükle Bırak İşlemleri", okunan);
        }
        public bool GosterTalepBilgileri(int no)
        {
            if (TaleplerDosyası.Count == 0) return false;
            if (no < 0 || no > TaleplerDosyası.Count) return false;

            string okunan = TaleplerDosyası[no].Ayar;
            label14.Text = "Son Yedek " + Ayarlar.Oku_AltDal(okunan, "son yedek anı");
            textBox3.Text = Ayarlar.Oku_AltDal(okunan, "tanım");
            textBox1.Text = Ayarlar.Oku_AltDal(okunan, "kaynak");
            textBox2.Text = Ayarlar.Oku_AltDal(okunan, "hedef");
            numericUpDown1.Value = Convert.ToInt32(Ayarlar.Oku_AltDal(okunan, "dk da çalıştır"));
            if (Ayarlar.Oku_AltDal(okunan, "fazla dosyaları sil") == "Evet") checkBox1.Checked = true;
            else checkBox1.Checked = false;
            numericUpDown2.Value = Convert.ToInt32(Ayarlar.Oku_AltDal(okunan, "farklı klasör sayısı"));
            if (Ayarlar.Oku_AltDal(okunan, "uzun sürecek dosyalama işlemi") == "Evet") checkBox5.Checked = true;
            else checkBox5.Checked = false;

            switch (Convert.ToInt16(Ayarlar.Oku_AltDal(okunan, "şifreleme yöntemi")))
            {
                case (0): radioButton_sifrelemyok.Checked = true; break;
                case (1): radioButton_Sifrele.Checked = true; break;
                case (2): radioButton_Coz.Checked = true; break;
            }

            textBox_Sifre.Text = "";
            if (Ayarlar.Oku_AltDal(okunan, "şifre") != "") textBox_Sifre.Text = "Kullanıcının Parolası";

            if (Ayarlar.Oku_AltDal(okunan, "etkin mi") == "Evet") return true;
            else return false;
        }
        public bool GosterArkaPlanTalepBilgileri(int no)
        {
            if (TaleplerDosyası.Count == 0) return false;
            if (no < 0 || no > TaleplerDosyası.Count) return false;

            string okunan = TaleplerDosyası[no].Ayar;
            Talep.SonYedekTarihi = Ayarlar.Oku_AltDal(okunan, "son yedek anı");
            Talep.Tanım = Ayarlar.Oku_AltDal(okunan, "tanım");

            Talep.Kaynak = Ayarlar.Oku_AltDal(okunan, "kaynak");
            if (!Talep.Kaynak.EndsWith("\\")) Talep.Kaynak += "\\";

            Talep.Hedef = Ayarlar.Oku_AltDal(okunan, "hedef");
            if (!Talep.Hedef.EndsWith("\\")) Talep.Hedef += "\\";

            Talep.Süre = Convert.ToInt32(Ayarlar.Oku_AltDal(okunan, "dk da çalıştır"));
            if (Ayarlar.Oku_AltDal(okunan, "uzun sürecek dosyalama işlemi") == "Evet") Talep.UzunSürecekDosyalamaİşlemlerindeArkaplandaÇalıştır = true;
            else Talep.UzunSürecekDosyalamaİşlemlerindeArkaplandaÇalıştır = false;
            if (Ayarlar.Oku_AltDal(okunan, "fazla dosyaları sil") == "Evet") Talep.Sil = true;
            else Talep.Sil = false;
            Talep.HedefKlasörSayisi = Convert.ToInt16(Ayarlar.Oku_AltDal(okunan, "farklı klasör sayısı"));
            if (Talep.HedefKlasörSayisi > 1) Talep.Sil = false;
            Talep.şifrelemeYontemi = Convert.ToInt16(Ayarlar.Oku_AltDal(okunan, "şifreleme yöntemi"));
            Talep.Şifre = Ayarlar.Oku_AltDal(okunan, "şifre"); ;
            if (Ayarlar.Oku_AltDal(okunan, "etkin mi") == "Evet") return true;
            else return false;     
        }
        public void SilTalep(int no)
        {
            if (TaleplerDosyası.Count == 0) return;
            TaleplerDosyası.RemoveAt(no);
            ListeyiTaleplerDosyasınaYaz();
        }
        public void DegiştirTalep(int no, int tür, string bilgi)
        {
            if (Durdur) return;
            if (TaleplerDosyası.Count == 0) return;

            if (no >= TaleplerDosyası.Count) return;

            Ayarlar_.BirParametre_ gecici = new Ayarlar_.BirParametre_(TaleplerDosyası[no].Parametre, TaleplerDosyası[no].Ayar);

            switch (tür)
            {
                case (0)://durum
                    Ayarlar.Yaz_AltDal(ref gecici.Ayar, "etkin mi", bilgi); 
                    break;

                case (1)://son tarih
                    Ayarlar.Yaz_AltDal(ref gecici.Ayar, "son yedek anı", bilgi); 
                    break;
            }

            TaleplerDosyası[no] = gecici;
            ListeyiTaleplerDosyasınaYaz();
        }
        private bool YazTalep(string TalepDurumu)
        {
            try
            {
                string yazılan = "", dd = "Hayir";
                Ayarlar.Yaz_AltDal(ref yazılan, "son yedek anı", DateTime.Now.ToString());
                Ayarlar.Yaz_AltDal(ref yazılan, "tanım", textBox3.Text);
                Ayarlar.Yaz_AltDal(ref yazılan, "kaynak", textBox1.Text);
                Ayarlar.Yaz_AltDal(ref yazılan, "hedef", textBox2.Text);
                Ayarlar.Yaz_AltDal(ref yazılan, "dk da çalıştır", Convert.ToString(numericUpDown1.Value));

                if (checkBox1.Checked) dd = "Evet";
                Ayarlar.Yaz_AltDal(ref yazılan, "fazla dosyaları sil", dd);

                dd = "Hayir";
                if (checkBox5.Checked) dd = "Evet";
                Ayarlar.Yaz_AltDal(ref yazılan, "uzun sürecek dosyalama işlemi", dd);

                Ayarlar.Yaz_AltDal(ref yazılan, "farklı klasör sayısı", Convert.ToString(numericUpDown2.Value));

                if (radioButton_sifrelemyok.Checked) dd = "0";
                else if (radioButton_Sifrele.Checked) dd = "1";
                else if (radioButton_Coz.Checked) dd = "2";
                Ayarlar.Yaz_AltDal(ref yazılan, "şifreleme yöntemi", dd);

                if (textBox_Sifre.Text == "") Ayarlar.Yaz_AltDal(ref yazılan, "şifre", "");
                else Ayarlar.Yaz_AltDal(ref yazılan, "şifre", D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(textBox_Sifre.Text), 128)));
                Ayarlar.Yaz_AltDal(ref yazılan, "etkin mi", TalepDurumu);
  
                Ayarlar_.BirParametre_ yeni = new Ayarlar_.BirParametre_("Talep_" + Path.GetRandomFileName(), yazılan);
                TaleplerDosyası.Add(yeni);
                ListeyiTaleplerDosyasınaYaz();

                return true;
            }
            catch (Exception ex)
            {
                YazLog("HATA", "YazTalep " + ex.Message);
            }
            return false;
        }

        static public bool SilDosya(string ad)
        {
            try
            {
                if (!File.Exists(ad)) return true; 

                FileSecurity fSecurity = File.GetAccessControl(ad);
                string acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //@"DomainName\AccountName"
                fSecurity.AddAccessRule(new FileSystemAccessRule(@acc, FileSystemRights.FullControl, AccessControlType.Allow));
                File.SetAccessControl(ad, fSecurity);
                File.SetAttributes(ad, FileAttributes.Normal);
                File.Delete(ad);
                return true;
            }
            catch (Exception)  { }

            return false;
        }
        public void SilKlasör(string yol)
        {
            Yapilacak_İşler yedek = Yapilacak_İşler_Sayaç;
            Yapilacak_İşler_Sayaç = Yapilacak_İşler.DahaBekle;
            Talep.SilinecekKlasör = yol;
            FaDo.Calistir = true;

            Thread_FazlaDosyalarıSilen = new Thread(Object_FazlaDosyalarıSilen.SilKlasör);
            Thread_FazlaDosyalarıSilen.Start();

            while (FaDo.Calistir) Application.DoEvents();

            Yapilacak_İşler_Sayaç = yedek;
        }
        public bool KarsilastirKlasör(string A, string B)
        {
            //false farklı
            //true aynı
            Yapilacak_İşler yedek = Yapilacak_İşler_Sayaç;
            Yapilacak_İşler_Sayaç = Yapilacak_İşler.DahaBekle;

            TaFaA.Calistir = true;
            TaFaA.A = A;
            TaFaA.B = B;
            Thread_TarihselFarkArayan = new Thread(Object_TarihselFarkArayan.TarihselFarkAra);
            Thread_TarihselFarkArayan.Start();
            while (TaFaA.Calistir)
            {
                try
                {
                    if (IslemGorenDosya.Contains(">"))
                    {
                        string[] gecici = IslemGorenDosya.Split('>');
                        label5.Text = gecici[0];
                        label4.Text = gecici[1];
                    }
                    else
                    {
                        label5.Text = Path.GetFileName(IslemGorenDosya);
                        label4.Text = Path.GetDirectoryName(IslemGorenDosya);
                    }
                }
                catch (Exception)
                {
                    label5.Text = "";
                    label4.Text = IslemGorenDosya;
                }

                Thread.Sleep(100);
                Application.DoEvents();
            }

            Yapilacak_İşler_Sayaç = yedek;
            if (TaFaA.KlasörlerAynı)    return true;
            else                        return false;
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox5.Visible = false;
                checkedListBox1.Visible = true;
                groupBox1.Visible = true;
            }
            else
            {
                textBox5.Visible = true;
                checkedListBox1.Visible = false;
                groupBox1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Yapilacak_İşler_Sayaç != Yapilacak_İşler.Bekleme) return;
            checkBox3.Checked = false;
            Yapilacak_İşler_Sayaç = Yapilacak_İşler.Hazirlik;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Unchecked)
            {
                if (checkBox3.ForeColor == System.Drawing.Color.Red) checkBox3.ForeColor = System.Drawing.Color.Black;
                else checkBox3.ForeColor = System.Drawing.Color.Red;
            }

            timer1.Enabled = false;
            switch(Yapilacak_İşler_Sayaç)
            {
                case (Yapilacak_İşler.Bekleme):
                case (Yapilacak_İşler.DahaBekle):
                    timer1.Interval = 500;
                    Talep.SonTalepNo = 0;

                    dataGridView1[1, 0].Value = "...";
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    this.Text = ProgAdVer;
                    PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);

                    button1.Enabled = true;
                    button6.Enabled = true;
                    break;

                case (Yapilacak_İşler.Hazirlik):
                    timer1.Interval = 100;

                    if (TaleplerDosyası.Count == 0)
                    {
                        Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                        timer1.Enabled = true;
                        return;
                    }
                    FaDo.biten = 0;
                    TaKa.biten = 0;
                    TaKa.hatalı = 0;
                    Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepBul;
                    break;

                case (Yapilacak_İşler.TalepBul):
                    button1.Enabled = false;
                    button6.Enabled = false;

                    label5.Text = "...";
                    label4.Text = "Adım " + Convert.ToString(Talep.SonTalepNo+1) + " " + Talep.Tanım;

                    if (GosterArkaPlanTalepBilgileri(Talep.SonTalepNo)) 
                    {
                        if (Talep.Tanım.Contains(">>>>>Komut>"))
                        {
                            label5.Text = "Çalışıyor";

                            try
                            {
                                string[] kom = Talep.Tanım.Split('>');
                                if (kom[6] == "Bekle")
                                {
                                    decimal bekleme = Convert.ToInt32(kom[7]);
                                    if (bekleme >= 24 * 60) GosterLog("UYARI", Talep.Tanım + " Bekleme süresi 24 saatten fazla olduğu için atlandı.");
                                    {
                                        SürekliYdeklemeZamanlama = DateTime.Now;
                                        SürekliYdeklemeZamanlama = SürekliYdeklemeZamanlama.AddMinutes((double)bekleme);
                                        GosterLog("Bilgi", Talep.Tanım + " Sistem " + Convert.ToString(bekleme) + " dakikalığına donduruldu. " + Convert.ToString(SürekliYdeklemeZamanlama)); 
                                    }
                                }
                                else if (kom[6] == "Calistir")
                                {
                                    string ad = Path.GetFileNameWithoutExtension(kom[7]);
                                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                                    bool uygulamaacik = false;
                                    foreach (var proc in processes)
                                    {
                                        if (proc.ProcessName.Trim().ToLower() == ad.Trim().ToLower()) uygulamaacik = true;
                                    }

                                    if (uygulamaacik) GosterLog("Bilgi", Talep.Tanım + " belirtilen uygulama zaten çalıştığı için atlandı.");
                                    else
                                    { 
                                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                                        process.StartInfo.UseShellExecute = true;
                                        process.StartInfo.FileName = Path.GetFileName(kom[7]);
                                        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(kom[7]);          
                                        if (kom.Count() >= 9) process.StartInfo.Arguments = kom[8];
                                        if (kom.Count() >= 10)
                                        {
                                            if (kom[9] != "") process.StartInfo.Verb = kom[9];
                                        }
                                        if (kom.Count() >= 11) 
                                        { 
                                            if (kom[10] == "Hidden") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                            else if (kom[10] == "Maximized") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                                            else if (kom[10] == "Minimized") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                                            else if (kom[10] == "Normal") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                                        }
                                        if (kom.Count() >= 12) { if (kom[11] == "DontUseShellExecute") process.StartInfo.UseShellExecute = false; }
                                        process.Start();

                                        GosterLog("Bilgi", Talep.Tanım + " belirtilen uygulama çalıştırıldı.");
                                    } 
                                }
                                else if (kom[6] == "Kapat")
                                {
                                    if (kom.Count() == 7 || (kom.Count() > 7 && kom[7] == ""))
                                    {
                                        while (!string.IsNullOrEmpty((string)dataGridView1[6, 0].Value)) { Thread.Sleep(100); Application.DoEvents(); if (Durdur) return; }
                                        
                                        GosterLog("Bilgi", Talep.Tanım + " Uygulama kapatılıyor.");
                                        System.Threading.Thread.Sleep(1000);
                                        this.Close();
                                    }
                                    else
                                    {
                                        if (kom.Count() == 8)
                                        {
                                            string ad = Path.GetFileNameWithoutExtension(kom[7]);
                                            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                                            foreach (var proc in processes)
                                            {
                                                if (proc.ProcessName.Trim().ToLower() == ad.Trim().ToLower())
                                                {
                                                    if (!proc.HasExited) proc.Kill();
                                                }
                                            }
                                        }
                                        else if (kom.Count() > 8 && kom[8] == "")
                                        {
                                            string ad = Path.GetFileNameWithoutExtension(kom[7]);
                                            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                                            foreach (var proc in processes)
                                            {
                                                if (proc.ProcessName.Trim().ToLower() == ad.Trim().ToLower())
                                                {
                                                    if (!proc.HasExited) proc.Kill();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            string ad = Path.GetFileNameWithoutExtension(kom[7]);
                                            if (!GeUyKa_Uygulama.Contains(ad))
                                            {
                                                GeUyKa_Uygulama.Add(ad);
                                                GeUyKa_Uygulama.Add(kom[8]);
                                                GosterLog("Bilgi", "Kuyruğa Eklendi " + ad + "%" + kom[8]);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { GosterLog("HATA", Talep.Tanım + " " + ex.Message); }

                            DegiştirTalep(Talep.SonTalepNo, 1, Convert.ToString(DateTime.Now));
                            if (checkBox3.Checked) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                            else Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;
                            break;
                        }

                        try
                        {
                            if (!Directory.Exists(Talep.Kaynak)) throw new Exception();
                            Directory.CreateDirectory(Talep.Hedef);
                        }
                        catch (Exception)
                        {
                            Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;
                            timer1.Enabled = true;

                            label5.Text = "Kaynak/Hedef Kullanılamıyor";
                            return;
                        }

                        if (Talep.HedefKlasörSayisi > 1)
                        {
                            Talep.OrjinalHedef = Talep.Hedef;
                            string tarih = Convert.ToString(DateTime.Now);
                            tarih = tarih.Replace(".", "_");
                            tarih = tarih.Replace(":", "-");

                            Talep.Hedef += tarih + "\\";

                            FaDo.biten = 0;
                            string[] subdirectoryEntries = Directory.GetDirectories(Talep.OrjinalHedef);
                            if (subdirectoryEntries.Count() > 0)
                            {
                                //sıralama
                                var Tarihler = new List<DateTime>();
                                int fazla_adet = 0;
                                for (int i = 0; i < subdirectoryEntries.Count(); i++)
                                {
                                    subdirectoryEntries[i] = subdirectoryEntries[i].Remove(0, Talep.OrjinalHedef.Length);
                                    subdirectoryEntries[i] = subdirectoryEntries[i].Replace("_", " ");
                                    subdirectoryEntries[i] = subdirectoryEntries[i].Replace("-", " ");

                                    try
                                    {
                                        string[] ta1 = subdirectoryEntries[i].Split(' ');
                                        if (ta1.Count() != 6) continue;
                                        Tarihler.Add(new DateTime(Convert.ToInt16(ta1[2]), Convert.ToInt16(ta1[1]), Convert.ToInt16(ta1[0]), Convert.ToInt16(ta1[3]), Convert.ToInt16(ta1[4]), Convert.ToInt16(ta1[5])));
                                        fazla_adet++;
                                    }
                                    catch (Exception) { }

                                    Application.DoEvents();
                                }
                                //silmeye 0. elemandan başlanacak
                                Tarihler.Sort((a, b) => a.CompareTo(b));

                                if (fazla_adet > 0)
                                {
                                    //Değişiklik kontrolü
                                    string son_ornek = Tarihler[Tarihler.Count() - 1].ToString();
                                    son_ornek = son_ornek.Replace(".", "_");
                                    son_ornek = son_ornek.Replace(":", "-");
                                    son_ornek += "\\";
                                    if (KarsilastirKlasör(Talep.Kaynak, Talep.OrjinalHedef + son_ornek))
                                    {
                                        DegiştirTalep(Talep.SonTalepNo, 1, Convert.ToString(DateTime.Now));
                                        GosterLog("Bilgi", Talep.Tanım + " Klasörler özdeş " + Talep.Kaynak + " " + Talep.OrjinalHedef + son_ornek);
                                        Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;
                                        label5.Text = "Klasörler Özdeş";
                                        break;
                                    }

                                    fazla_adet -= Talep.HedefKlasörSayisi;
                                    for (int i = 0; i < fazla_adet; i++)
                                    {
                                        subdirectoryEntries[i] = Tarihler[i].ToString();
                                        subdirectoryEntries[i] = subdirectoryEntries[i].Replace(".", "_");
                                        subdirectoryEntries[i] = subdirectoryEntries[i].Replace(":", "-");

                                        //Fazlalık eskileri silme
                                        SilKlasör(Talep.OrjinalHedef + subdirectoryEntries[i]);

                                        Application.DoEvents();
                                    }
                                }

                                //yeniyi baslatma devam ediyor
                            }
                        }

                        DoSaBu.Calistir = true;
                        Thread_DosyaSayisiniBulan = new Thread(Object_DosyaSayisiniBulan.DosyaSayisiniBul);
                        Thread_DosyaSayisiniBulan.Start();
                        Yapilacak_İşler_Sayaç = Yapilacak_İşler.Sayma;
                        label5.Text = "Çalışıyor";
                    }
                    else
                    {
                        label5.Text = "Durgun";

                        if (checkBox3.Checked) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                        else                   Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;    
                    }
                    break;

                case (Yapilacak_İşler.TalepArttır):
                    if (checkBox3.Checked) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                    else
                    {
                        if (++Talep.SonTalepNo < TaleplerDosyası.Count)
                        {
                            Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepBul;
                        }
                        else
                        {
                            Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                            label4.Text = "...";
                            label5.Text = "...";
                        }
                    }
                    break;

                case (Yapilacak_İşler.Sayma):
                    timer1.Interval = 200;

                    if (!DoSaBu.Calistir)
                    {     
                        progressBar1.Maximum = Talep.ToplamDosyaSayısı+1; 
                        progressBar1.Value = 0;

                        TaKa.Calistir = true;
                        TaKa.checkbox3 = checkBox3.Checked;
                        Thread_TarihselKarşilaştiran = new Thread(Object_TarihselKarşilaştiran.TarihselKarşilaştir);
                        Thread_TarihselKarşilaştiran.Start();
                        Yapilacak_İşler_Sayaç = Yapilacak_İşler.tarihsel_karşilaştir;
                        progressBar1.Style = ProgressBarStyle.Blocks;
                    }
                    else
                    {
                        progressBar1.Style = ProgressBarStyle.Marquee;
                        this.Text = "Adım " + Convert.ToString(Talep.SonTalepNo + 1) + " " + Talep.Tanım + " / " + Convert.ToString(Talep.ToplamDosyaSayısı) + " / " + D_DosyaBoyutu.Metne(Talep.ToplamBoyut) + " " + ProgAdVer;

                        PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Belirsiz);
                    }
                    break;

                case (Yapilacak_İşler.tarihsel_karşilaştir):
                    if (!TaKa.Calistir)
                    {
                        dataGridView1[1, 0].Value = Convert.ToString(TaKa.biten);
                        dataGridView1[2, 0].Value = Convert.ToString(Talep.YedeklenenDosyaSayısı);
                        dataGridView1[5, 0].Value = Convert.ToString(TaKa.hatalı) + " / " + Convert.ToString(TaKa.GenelHatalı);
                        try
                        { progressBar1.Value = TaKa.biten; } catch (Exception) { progressBar1.Value = progressBar1.Maximum-1; }
                        
                        if (Talep.Sil)
                        {
                            FaDo.Calistir = true;
                            Thread_FazlaDosyalarıSilen = new Thread(Object_FazlaDosyalarıSilen.FazlaDosyalarıSil);
                            Thread_FazlaDosyalarıSilen.Start();
                            Yapilacak_İşler_Sayaç = Yapilacak_İşler.fazla_dosyaları_sil;
                        }
                        else
                        {
                            DegiştirTalep(Talep.SonTalepNo, 1, Convert.ToString(DateTime.Now));

                            if (checkBox3.Checked) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                            else Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;

                            string sonuc = Talep.Tanım;
                            if (TaKa.hatalı > 0) sonuc += " " + Convert.ToString(TaKa.hatalı) + " HATALI işlem";
                            sonuc += ", " + Convert.ToString(Talep.YedeklenenDosyaSayısı) + " yedeklenen dosya";
                            if (FaDo.biten > 0) sonuc += ", " + Convert.ToString(FaDo.biten) + " fazla dosya";
                            sonuc += ", toplamda " + Convert.ToString(Talep.ToplamDosyaSayısı) + " dosya ve ";
                            sonuc += Convert.ToString(Talep.ToplamKlasörSayısı) + " klasör işlem gördü.";
                            GosterLog("Rapor", sonuc);

                            this.Text = ProgAdVer;
                            PeTeİkKo.Tepsiİkonu.BalloonTipText = Talep.Tanım + " " + Convert.ToString(Talep.YedeklenenDosyaSayısı) + " / " + Convert.ToString(Talep.ToplamDosyaSayısı);
                            if (Talep.YedeklenenDosyaSayısı > 0) PeTeİkKo.Tepsiİkonu.ShowBalloonTip(500);

                            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);
                        }
                    }
                    else
                    {
                        dataGridView1[1, 0].Value = Convert.ToString(TaKa.biten);
                        dataGridView1[2, 0].Value = Convert.ToString(Talep.YedeklenenDosyaSayısı);
                        dataGridView1[5, 0].Value = Convert.ToString(TaKa.hatalı) + " / " + Convert.ToString(TaKa.GenelHatalı);
                        try
                        {
                            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Normal, (ulong)progressBar1.Value, (ulong)progressBar1.Maximum);
                            progressBar1.Value = TaKa.biten;
                        } catch (Exception) { progressBar1.Value = progressBar1.Maximum - 1; }

                        try
                        {
                            if (IslemGorenDosya.Contains(">"))
                            {
                                string[] gecici = IslemGorenDosya.Split('>');
                                label5.Text = gecici[0];
                                label4.Text = gecici[1];
                            }
                            else
                            {
                                label5.Text = Path.GetFileName(IslemGorenDosya);
                                label4.Text = Path.GetDirectoryName(IslemGorenDosya);
                            }
                        }
                        catch (Exception)
                        {
                            label5.Text = "";
                            label4.Text = IslemGorenDosya;
                        }
                        
                        this.Text = "Adım " + Convert.ToString(Talep.SonTalepNo + 1) + " " + Talep.Tanım + " " + D_DosyaBoyutu.Metne(Talep.IslemormusBoyut) + "/" + D_DosyaBoyutu.Metne(Talep.ToplamBoyut) + " " + ProgAdVer;
                    }
                    break;

                case (Yapilacak_İşler.fazla_dosyaları_sil):
                    if (!FaDo.Calistir)
                    {
                        dataGridView1[3, 0].Value = Convert.ToString(FaDo.Toplam);
                        dataGridView1[4, 0].Value = Convert.ToString(FaDo.biten);
                        dataGridView1[5, 0].Value = Convert.ToString(TaKa.hatalı) + " / " + Convert.ToString(TaKa.GenelHatalı);

                        DegiştirTalep(Talep.SonTalepNo, 1, Convert.ToString(DateTime.Now));

                        if (checkBox3.Checked) Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
                        else                   Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepArttır;

                        string sonuc = Talep.Tanım;
                        if (TaKa.hatalı > 0) sonuc += " " + Convert.ToString(TaKa.hatalı) + " HATALI işlem";
                        sonuc += ", " + Convert.ToString(Talep.YedeklenenDosyaSayısı) + " yedeklenen dosya";
                        if (FaDo.biten > 0) sonuc += ", " + Convert.ToString(FaDo.biten) + " fazla dosya";
                        sonuc += ", toplamda " + Convert.ToString(Talep.ToplamDosyaSayısı) + " dosya ve ";
                        sonuc += Convert.ToString(Talep.ToplamKlasörSayısı) + " klasör işlem gördü.";
                        GosterLog("Rapor", sonuc);

                        this.Text = ProgAdVer;
                        PeTeİkKo.Tepsiİkonu.BalloonTipText = Talep.Tanım + " " + Convert.ToString(Talep.YedeklenenDosyaSayısı) + " / " + Convert.ToString(Talep.ToplamDosyaSayısı);
                        if (Talep.YedeklenenDosyaSayısı > 0) PeTeİkKo.Tepsiİkonu.ShowBalloonTip(500);

                        PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);
                    }
                    else
                    {
                        dataGridView1[3, 0].Value = Convert.ToString(FaDo.Toplam);
                        dataGridView1[4, 0].Value = Convert.ToString(FaDo.biten);
                        dataGridView1[5, 0].Value = Convert.ToString(TaKa.hatalı) + " / " + Convert.ToString(TaKa.GenelHatalı);

                        PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Durgun, (ulong)FaDo.biten, (ulong)FaDo.Toplam);

                        try
                        {
                            label5.Text = Path.GetFileName(IslemGorenDosya);
                            label4.Text = Path.GetDirectoryName(IslemGorenDosya);
                        }
                        catch (Exception)
                        {
                            label5.Text = "";
                            label4.Text = IslemGorenDosya;
                        }

                        this.Text = "Adım " + Convert.ToString(Talep.SonTalepNo + 1) + " " + Talep.Tanım + " " + ProgAdVer;
                    }
                    break;
            }

            timer1.Enabled = true;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (SüBıİş.FareTuşunaBasılıyor) return;

            timer2.Enabled = false;
            flowLayoutPanel1.Visible = false; 
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                int tick = Environment.TickCount + 250;
                for (int i = 0; i < Liste_Hatalar.Count() && tick > Environment.TickCount; i++)
                {
                    GosterLog(Liste_Hatalar[0], Liste_Hatalar[1]);
                    Liste_Hatalar.RemoveAt(0);
                    Liste_Hatalar.RemoveAt(0);
                }
            }
            catch (Exception ex) { Liste_Hatalar.Clear(); GosterLog("HATA", "Liste_Hatalar da sorun oluştu" + ex.Message); }

            if (GeUyKa_Uygulama.Count > 0)
            {
                if (Thread_GereksizUygulamaKapatan == null) Thread_GereksizUygulamaKapatan = new Thread(Object_GereksizUygulamaKapatan.Calistir);
                else if (Thread_GereksizUygulamaKapatan.ThreadState != ThreadState.Running &&
                         Thread_GereksizUygulamaKapatan.ThreadState != ThreadState.WaitSleepJoin)
                {
                    Thread_GereksizUygulamaKapatan = new Thread(Object_GereksizUygulamaKapatan.Calistir);
                    Thread_GereksizUygulamaKapatan.Start();
                }
            }

            if (SüBıİş.SürükleBırakListesi.Count > 0)
            {
                groupBox2.Enabled = false;

                if (Thread_TaKa_SürükleBırakListesi == null) Thread_TaKa_SürükleBırakListesi = new Thread(Object_TarihselKarşilaştiran.SürükleBırakKlasörSifreleme);
                else if (Thread_TaKa_SürükleBırakListesi.ThreadState != ThreadState.Running &&
                         Thread_TaKa_SürükleBırakListesi.ThreadState != ThreadState.WaitSleepJoin)
                {
                    Thread_TaKa_SürükleBırakListesi = new Thread(Object_TarihselKarşilaştiran.SürükleBırakKlasörSifreleme);
                    Thread_TaKa_SürükleBırakListesi.Start();
                }

                if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.Çöz) label6.Text = "Düzeltme ";
                else if (SüBıİş.yöntem_kararverilen == ŞifrelemeYöntem.Şifrele) label6.Text = "Karıştırma ";
                else label6.Text = "..... ";
                try { label6.Text += Convert.ToString(SüBıİş.DosyaAdet_İşlenen) + " / " + Convert.ToString(SüBıİş.DosyaAdet_Toplam) + " / " + Convert.ToString(SüBıİş.SürükleBırakListesi.Count + " " + SüBıİş.SürükleBırakListesi[0]); } catch (Exception)  { }         
            }
            else { label6.Text = "Boşta"; groupBox2.Enabled = true; }

            dataGridView1[6, 0].Value = UzSüDoİş.DurumuOku();
            toolTip1.SetToolTip(dataGridView1, UzSüDoİş.DetaylıDurum);

            if (!checkBox3.Checked) return;
            if (Yapilacak_İşler_Sayaç != Yapilacak_İşler.Bekleme) return;
            if (TaleplerDosyası.Count == 0) return;
            if (SürekliYdeklemeZamanlama > DateTime.Now) return;

            if (++Talep.SayacOtoYedek >= TaleplerDosyası.Count) Talep.SayacOtoYedek = 0;
            GosterTalepBilgileri(Talep.SayacOtoYedek);

            label5.Text = "...";
            label4.Text = "Adım " + Convert.ToString(Talep.SayacOtoYedek + 1) + " " + textBox3.Text;

            if (!GosterArkaPlanTalepBilgileri(Talep.SayacOtoYedek))
            {
                label5.Text = "Durgun";
                return;
            }

            Talep.SonYedekTarihi = Talep.SonYedekTarihi.Replace(".", " ");
            Talep.SonYedekTarihi = Talep.SonYedekTarihi.Replace(":", " ");
            string[] ta1 = Talep.SonYedekTarihi.Split(' ');
            while (ta1.Length < 6)
            {
                Talep.SonYedekTarihi += " 00";
                ta1 = Talep.SonYedekTarihi.Split(' ');
            }

            System.DateTime dates_simdi = new System.DateTime();
            dates_simdi = DateTime.Now;
            System.DateTime date_talep = new System.DateTime(Convert.ToInt16(ta1[2]), Convert.ToInt16(ta1[1]), Convert.ToInt16(ta1[0]), Convert.ToInt16(ta1[3]), Convert.ToInt16(ta1[4]), Convert.ToInt16(ta1[5]));

            TimeSpan difference = dates_simdi - date_talep;
            if (difference.TotalMinutes >= Talep.Süre)
            {
                Yapilacak_İşler_Sayaç = Yapilacak_İşler.Hazirlik;
                Talep.SonTalepNo = Talep.SayacOtoYedek;

                label5.Text = "Çalışıyor";
            }
            else label5.Text = "Çalışmaya " + Convert.ToString(Math.Round(Talep.Süre - difference.TotalMinutes,2)) + " dk. var.";
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text)) openFileDialog1.InitialDirectory = textBox1.Text;
            openFileDialog1.ShowDialog();
            textBox1.Text = Path.GetDirectoryName(openFileDialog1.FileName) + "\\";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text)) openFileDialog1.InitialDirectory = textBox2.Text;
            openFileDialog1.ShowDialog();
            textBox2.Text = Path.GetDirectoryName(openFileDialog1.FileName) + "\\";
        }
        private void button6_Click(object sender, EventArgs e)
        {
            YazTalep("Evet");
            TazeleChecedBox1();
            GosterTalepBilgileri(0);
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > 1)   checkBox1.Enabled = false;
            else                            checkBox1.Enabled = true;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)  numericUpDown2.Enabled = false;
            else                    numericUpDown2.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Aş.AcilDur(); Durdur = true;
            GeUyKa_Uygulama.Clear();
            button2.Enabled = false;
            checkBox3.Checked = false;
            UzSüDoİş.ListeyiTemizle();
            PeTeİkKo.İlerlemeyiYüzdeOlarakGöster(PencereVeTepsiIkonuKontrolu_.GörevÇubuğundaYüzdeGösterimiDurumu.Kapalı);
            if (SüBıİş.SürükleBırakListesi.Count > 0) Thread.Sleep(2000);
            
            while (DoSaBu.Calistir || FaDo.Calistir || TaKa.Calistir || TaFaA.Calistir) { Durdur = true; Application.DoEvents(); }
            
            if (Thread_DosyaSayisiniBulan != null)
            {
                while (Thread_DosyaSayisiniBulan.IsAlive) { Thread_DosyaSayisiniBulan.Abort(); Application.DoEvents(); }
            }

            if (Thread_FazlaDosyalarıSilen != null)
            {
                while (Thread_FazlaDosyalarıSilen.IsAlive) { Thread_FazlaDosyalarıSilen.Abort(); Application.DoEvents(); }
            }

            if (Thread_TarihselKarşilaştiran != null)
            {
                while (Thread_TarihselKarşilaştiran.IsAlive) { Thread_TarihselKarşilaştiran.Abort(); Application.DoEvents(); }
            }

            if (Thread_TarihselFarkArayan != null)
            {
                while (Thread_TarihselFarkArayan.IsAlive) { Thread_TarihselFarkArayan.Abort(); Application.DoEvents(); }
            }

            if (Thread_TaKa_SürükleBırakListesi != null)
            {
                while (Thread_TaKa_SürükleBırakListesi.IsAlive) { Thread_TaKa_SürükleBırakListesi.Abort(); Application.DoEvents(); }
            }

            SüBıİş.SürükleBırakListesi.Clear();     
            Yapilacak_İşler_Sayaç = Yapilacak_İşler.Bekleme;
            button2.Enabled = true;
            Durdur = false;
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex < 0) return;
            if (checkedListBox1.SelectedIndex >= TaleplerDosyası.Count) return;

            GosterTalepBilgileri(checkedListBox1.SelectedIndex);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            KlasorGezgini frm2 = new KlasorGezgini(this, true);
            frm2.Show();
            frm2.Baslangiçİşlemleri(textBox1.Text);
        }
        private void textBox5_DoubleClick(object sender, EventArgs e)
        {
            textBox5.Clear();
            TaKa.GenelHatalı = 0;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex < 0) return;
            SilTalep(checkedListBox1.SelectedIndex);
            TazeleChecedBox1();
            GosterTalepBilgileri(0);
        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBox1_iptalEt) e.NewValue = e.CurrentValue;
        }
        private void radioButton_sifrelemyok_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Sifre.Text = "";
        }
        private void bekleSüreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox3.Text = ">>>>>Komut>Bekle>BURAYAsüreyiYAZINIZ";
            textBox1.Text = "Örnek Kullanım >>>>>Komut>Bekle>30";
            textBox2.Text = "Dakika olarak giriş yapılır, belirtilen süre kadar programı dondurur";

            numericUpDown1.Value = numericUpDown1.Minimum;
            numericUpDown2.Value = numericUpDown2.Minimum;
            checkBox1.Checked = false;
            radioButton_sifrelemyok.Checked = true;
            textBox_Sifre.Text = "";
        }
        private void uygulamayıAçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox3.Text = ">>>>>Komut>Calistir>kısayol>parametreler>boşVEYAverb>BoşVeyaNormalMaximizedMinimizedHidden>8oşVeyaDontUseShellExecute>";
            textBox1.Text = "Örnek Kullanım >>>>>Komut>Çalistir>C:\\a.exe>+p-s>runas>Normal>> >>>>>Komut>Çalistir>chrome>wwww.argemup.com>>Maximized";
            textBox2.Text = "Belirtilen uygulamayı ilgili seçenekler ile çalıştırır";

            numericUpDown1.Value = numericUpDown1.Minimum;
            numericUpDown2.Value = numericUpDown2.Minimum;
            checkBox1.Checked = false;
            radioButton_sifrelemyok.Checked = true;
            textBox_Sifre.Text = "";
        }
        private void programıKapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox3.Text = ">>>>>Komut>Kapat>";
            textBox1.Text = "Örnek Kullanım >>>>>Komut>Kapat yada >>>>>Komut>Kapat>abc.exe yada >>>>>Komut>Kapat>abc.exe>50";
            textBox2.Text = "parametre almaz is kendini, alır ise o programı cpu yüzdesi değerin üzerinde ise kapatır";

            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            textBox5.AppendText(Environment.NewLine);
            foreach (var proc in processes) textBox5.AppendText("Seçilebilir " + proc.ProcessName + Environment.NewLine);
            textBox5.ScrollToCaret();

            numericUpDown1.Value = numericUpDown1.Minimum;
            numericUpDown2.Value = numericUpDown2.Minimum;
            checkBox1.Checked = false;
            radioButton_sifrelemyok.Checked = true;
            textBox_Sifre.Text = "";
        }

        public void TazeleChecedBox1()
        {
            int yedek = checkedListBox1.SelectedIndex;

            try
            {
                checkedListBox1.Items.Clear();
                checkedListBox1_iptalEt = false;
                for (int i = 0; i < TaleplerDosyası.Count; i++)
                {
                    string okunan = TaleplerDosyası[i].Ayar;
                   
                    if (Ayarlar.Oku_AltDal(okunan, "etkin mi") == "Evet") checkedListBox1.Items.Add(Convert.ToString(checkedListBox1.Items.Count + 1) + " : " + Ayarlar.Oku_AltDal(okunan, "tanım"), true);
                    else checkedListBox1.Items.Add(Convert.ToString(checkedListBox1.Items.Count + 1) + " : " + Ayarlar.Oku_AltDal(okunan, "tanım"), false);
                }
                if (TaleplerDosyası.Count > 0) checkedListBox1.SelectedIndex = yedek;
            }
            catch (Exception ex)
            {
                YazLog("HATA", "DoldurTalepListesi " + ex.Message);
            }

            checkedListBox1_iptalEt = true;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1_MouseMove(null, null);

            if (TaleplerDosyası.Count == 0) return;
            if (checkedListBox1.SelectedIndex < 0) return;
            if (checkedListBox1.SelectedIndex >= TaleplerDosyası.Count) return;

            DegiştirTalep(checkedListBox1.SelectedIndex, 0, "Evet");
            TazeleChecedBox1();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1_MouseMove(null, null);

            if (TaleplerDosyası.Count == 0) return;
            if (checkedListBox1.SelectedIndex < 0) return;
            if (checkedListBox1.SelectedIndex >= TaleplerDosyası.Count) return;

            DegiştirTalep(checkedListBox1.SelectedIndex, 0, "Hayır");
            TazeleChecedBox1();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1_MouseMove(null, null);

            if (TaleplerDosyası.Count == 0) return;
            if (checkedListBox1.SelectedIndex < 0) return;
            if (Yapilacak_İşler_Sayaç != Yapilacak_İşler.Bekleme) return;

            button2_Click(null, null);
            checkBox3.Checked = false;

            Talep.SonTalepNo = checkedListBox1.SelectedIndex;
            Yapilacak_İşler_Sayaç = Yapilacak_İşler.TalepBul;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1_MouseMove(null, null);

            if (TaleplerDosyası.Count == 0) return;
            if (checkedListBox1.SelectedIndex < 1) return;

            try
            {
                TaleplerDosyası.Insert(checkedListBox1.SelectedIndex - 1, TaleplerDosyası[checkedListBox1.SelectedIndex]);
                TaleplerDosyası.RemoveAt(checkedListBox1.SelectedIndex + 1);
                ListeyiTaleplerDosyasınaYaz();
                checkedListBox1.SelectedIndex--;
            }

            catch (Exception ex)
            {
                YazLog("HATA", "toolStripMenuItem1_Click " + ex.Message);
            }

            TazeleChecedBox1();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1_MouseMove(null, null);

            if (TaleplerDosyası.Count == 0) return;
            if (checkedListBox1.SelectedIndex < 0) return;
            if (checkedListBox1.SelectedIndex + 1 >= TaleplerDosyası.Count) return;

            try
            {
                TaleplerDosyası.Insert(checkedListBox1.SelectedIndex + 2, TaleplerDosyası[checkedListBox1.SelectedIndex]);
                TaleplerDosyası.RemoveAt(checkedListBox1.SelectedIndex);
                ListeyiTaleplerDosyasınaYaz();
                checkedListBox1.SelectedIndex++;
            }

            catch (Exception ex)
            {
                YazLog("HATA", "toolStripMenuItem2_Click " + ex.Message);
            }

            TazeleChecedBox1();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox3.ForeColor = System.Drawing.Color.Black;
                Talep.SayacOtoYedek = TaleplerDosyası.Count + 1;
                SürekliYdeklemeZamanlama = DateTime.Now;
                progressBar1.MarqueeAnimationSpeed = 75;
            }
            else
            {
                checkBox3.ForeColor = System.Drawing.Color.Red;
                progressBar1.MarqueeAnimationSpeed = 0;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    textBox1.Text = file;
                    return;
                }
                else if (File.Exists(file))
                {
                    textBox1.Text = Path.GetDirectoryName(file);
                    return;
                }
            }
        }
        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    textBox2.Text = file;
                    return;
                }
                else if (File.Exists(file))
                {
                    textBox2.Text = Path.GetDirectoryName(file);
                    return;
                }
            }
        }
        private void textBox4_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    textBox4.Text = file;
                    return;
                }
                else if (File.Exists(file))
                {
                    textBox4.Text = Path.GetDirectoryName(file);
                    return;
                }
            }
        }
        private void textBox6_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    textBox6.Text = file;
                    return;
                }
                else if (File.Exists(file))
                {
                    textBox6.Text = Path.GetDirectoryName(file);
                    return;
                }
            }
        }

        private void textBox_sü_bı_girdi_TextChanged(object sender, EventArgs e)
        {
            string sonuç = Aş.Düzelt(textBox_sü_bı_girdi.Text, textBox_sü_bı_şifre.Text);
            if (sonuç == "") sonuç = Aş.Karıştır(textBox_sü_bı_girdi.Text, textBox_sü_bı_şifre.Text);
            textBox_sü_bı_çıktı.Text = sonuç;

            if (textBox_sü_bı_girdi.Text != "") textBox_sü_bı_çıktı2.Text = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(textBox_sü_bı_girdi.Text)));

            flowLayoutPanel1_MouseMove(null, null);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //ipucu
            if (string.IsNullOrEmpty(textBox8.Text)) textBox8.Text = "İpucu";
            else textBox8.Text = textBox8.Text.Replace(";", ".,");
            if (textBox8.Text != "İpucu") SüBıİş.ipucu = textBox8.Text;
            else SüBıİş.ipucu = "";

            //şifre
            if (textBox7.Text != "" && textBox7.Text != ">>>>> Kullanıcının Parolası") { SüBıİş.şifre = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(textBox7.Text), 128)); textBox7.Text = ">>>>> Kullanıcının Şifresi"; }
            else SüBıİş.şifre = "";

            if (string.IsNullOrEmpty(textBox4.Text)) textBox4.Text = ">>>>> Kaynak Konumu + \"\\Karistirma\"";
            if (string.IsNullOrEmpty(textBox6.Text)) textBox6.Text = ">>>>> Kaynak Konumu + \"\\Duzeltme\"";
            
            SüBıİş.konum_sifrecozme = textBox6.Text;
            SüBıİş.konum_sifreleme = textBox4.Text;
            SüBıİş.yöntem_seçilen = (ŞifrelemeYöntem)comboBox1.SelectedIndex;

            SüBıİş.İsimlerideŞifrele = checkBox4.Checked;
            ListeyiTaleplerDosyasınaYaz();
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            Form1_DragLeave(null, null);

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string alinan;
            foreach (string file in files)
            {
                if (File.Exists(file)) alinan = Path.GetDirectoryName(file);
                else if (Directory.Exists(file)) alinan = file;
                else alinan = "";

                if (alinan != "")
                {
                    KlasorGezgini frm2 = new KlasorGezgini(this);
                    frm2.Show();
                    frm2.Baslangiçİşlemleri(alinan);
                }
            }
        } 

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            SüBıİş.FareTuşunaBasılıyor = true;
            SüBıİş.FareTuşunaBasılıyorX = e.X;
            SüBıİş.FareTuşunaBasılıyorY = e.Y;
        }
        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            SüBıİş.FareTuşunaBasılıyor = false;
        }
        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (SüBıİş.FareTuşunaBasılıyor)
            {
                flowLayoutPanel1.Left -= SüBıİş.FareTuşunaBasılıyorX - e.X;
                flowLayoutPanel1.Top -= SüBıİş.FareTuşunaBasılıyorY - e.Y;
                Application.DoEvents();
            }

            timer2.Enabled = true;
            timer2.Stop();
            flowLayoutPanel1.Visible = true;
            timer2.Start();
        }
    }
}

        
           