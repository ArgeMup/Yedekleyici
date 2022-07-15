// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ArgeMup.HazirKod.Dönüştürme;
using ArgeMup.HazirKod;
using System.IO.Compression;

namespace Talep
{
    public enum Dosyaları { Boşta, Kopyala, Karıştır, Düzelt, Sıkıştır, Aç, Sil, SıkıştırKarıştır, DüzeltAç }

    public class Bir_Talep_
    {
        public string Tanım = "", DosyaYolu = "", Kaynak = "", Hedef = "", Hata = "", ParolaŞablonu = "";
        public bool TamKopya = false;
        public int Versiyonla_adet = 1, Versiyonla_mb = 0, CpuYuzdesi = 100, HddYuzdesi = 100;
        public Dosyaları Dosyaları = Dosyaları.Boşta;
        public string[][] Filtreler_Dosya, Filtreler_Klasör;

        public Bir_Talep_(string[] Satırlar)
        {
            Bir_Talep_İlkAdım(Satırlar);
        }
        public Bir_Talep_(string DosyaYolu)
        {
            if (!File.Exists(DosyaYolu)) { Hata = "Dosya bulunamadı"; return; }
            this.DosyaYolu = DosyaYolu;
            Bir_Talep_İlkAdım(File.ReadAllLines(DosyaYolu));
        }
        void Bir_Talep_İlkAdım(string[] Satırlar)
        {
            List<string> DosAtlLis = null;
            string DenetlenenDosya = ".Yedekleyici_Talep";
            
            List<string[]> Filt_Kla = new List<string[]>(), Filt_Dos = new List<string[]>();
            
            YenidenDene:
            if (Satırlar == null) { Hata += "[" + DenetlenenDosya + "]" + " dosya açılamadı"; }
            else
            {
                for (int i = 0; i < Satırlar.Length; i++)
                {
                    Satırlar[i] = Satırlar[i].Split('#')[0].Trim();
                    if (string.IsNullOrEmpty(Satırlar[i])) continue;

                    string[] TekTek = Satırlar[i].Trim().Split(';');
                    for (int ii = 0; ii < TekTek.Length; ii++) TekTek[ii] = TekTek[ii].Trim();

                    if (TekTek.Length < 2) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }

                    string AyıklanmışGirdi = TekTek[1];
                    try { AyıklanmışGirdi = Senaryo.Değişken.Oku_MetinVeyaDeğişken(TekTek[1]); }
                    catch (Exception ex) { Hata += "\r\n[" + DenetlenenDosya + "] " + ex.ToString(); }

                    if (DosAtlLis == null && TekTek[0] == "Tanim")
                    {
                        Tanım = TekTek[1];
                    }
                    else if (DosAtlLis == null && TekTek[0] == "Kaynak")
                    {
                        Kaynak = AyıklanmışGirdi.Trim(' ').TrimEnd('\\') + "\\";
                    }
                    else if (DosAtlLis == null && TekTek[0] == "Hedef")
                    {
                        Hedef = AyıklanmışGirdi.Trim(' ').TrimEnd('\\') + "\\";
                    }
                    else if (TekTek[0] == "ParolaSablonu")
                    {
                        ParolaŞablonu = AyıklanmışGirdi;
                    }
                    else if (TekTek[0] == "TamKopya")
                    {
                        TamKopya = AyıklanmışGirdi != "Hayir";
                    }
                    else if (TekTek[0] == "Dosyalari")
                    {
                        if (AyıklanmışGirdi == "Kopyala") Dosyaları = Dosyaları.Kopyala;
                        else if (AyıklanmışGirdi == "Karistir") Dosyaları = Dosyaları.Karıştır;
                        else if (AyıklanmışGirdi == "Duzelt") Dosyaları = Dosyaları.Düzelt;
                        else if (AyıklanmışGirdi == "Sikistir") Dosyaları = Dosyaları.Sıkıştır;
                        else if (AyıklanmışGirdi == "Ac") Dosyaları = Dosyaları.Aç;
                        else if (AyıklanmışGirdi == "Sil") Dosyaları = Dosyaları.Sil;
                        else if (AyıklanmışGirdi == "SikistirKaristir") Dosyaları = Dosyaları.SıkıştırKarıştır;
                        else if (AyıklanmışGirdi == "DuzeltAc") Dosyaları = Dosyaları.DüzeltAç;
                        else { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                    }
                    else if (TekTek[0] == "Versiyonla")
                    {
                        if (TekTek.Length < 3) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }

                        if (!int.TryParse(AyıklanmışGirdi, out Versiyonla_adet)) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                        if (!int.TryParse(Senaryo.Değişken.Oku_MetinVeyaDeğişken(TekTek[2]), out Versiyonla_mb)) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                    }
                    else if (TekTek[0] == "CpuYuzdesi")
                    {
                        if (!int.TryParse(AyıklanmışGirdi, out CpuYuzdesi)) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                    }
                    else if (TekTek[0] == "HddYuzdesi")
                    {
                        if (!int.TryParse(AyıklanmışGirdi, out HddYuzdesi)) { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                    }
                    else if (TekTek[0] == "AtlaKlasor")
                    {
                        string[] dizi = AyıklanmışGirdi.Trim(' ').TrimEnd('\\').Split('\\');
                        if (dizi.Length > 0)
                        {
                            for (int iii = 0; iii < dizi.Length; iii++) dizi[iii] = dizi[iii].Trim('\\', ' ');
                            Filt_Kla.Add(dizi);
                        }
                    }
                    else if (TekTek[0] == "AtlaDosya")
                    {
                        string[] dizi = AyıklanmışGirdi.Trim('.', ' ').Split('.');
                        if (dizi.Length >= 2)
                        {
                            for (int iii = 0; iii < dizi.Length; iii++) dizi[iii] = dizi[iii].Trim('.', ' ');
                            if (dizi.Length > 2)
                            {
                                string isim = "";
                                for (int iiiii = 0; iiiii < dizi.Length - 1; iiiii++)
                                {
                                    isim += "." + dizi[iiiii];
                                }
                                isim = isim.Trim('.');

                                string soyisim = dizi[dizi.Length - 1];

                                dizi = new string[2];
                                dizi[0] = isim;
                                dizi[1] = soyisim;
                            }
                            Filt_Dos.Add(dizi);
                        }
                    }
                    else { Hata += "\r\n[" + DenetlenenDosya + "]" + " Satır " + (i + 1) + " hatalı"; continue; }
                }
            }
            
            if (DosAtlLis == null) DosAtlLis = Yedekleyici.Ortak.Listele.Dosya(Kaynak, SearchOption.TopDirectoryOnly, "*.Yedekleyici_DosyaAtlamaListesi").ToList();
            if (DosAtlLis != null && DosAtlLis.Count > 0)
            {
                DenetlenenDosya = Path.GetFileName(DosAtlLis[0]);
                Satırlar = File.ReadAllLines(DosAtlLis[0]);
                
                DosAtlLis.RemoveAt(0);
                goto YenidenDene;
            }

            if (Tanım == "") Hata += "\r\n\"Tanım\" boş olamaz";
            if (Kaynak == "\\") Hata += "\r\n\"Kaynak\" boş olamaz";
            if (Hedef == "\\") Hata += "\r\n\"Hedef\" boş olamaz";
            if (Dosyaları == Dosyaları.Boşta) Hata += "\r\n\"Dosyalari\" boş olamaz";
            else if (Dosyaları == Dosyaları.Sil) Versiyonla_adet = 1;
            if (ParolaŞablonu == "")
            {
                if (Dosyaları == Dosyaları.Düzelt || 
                    Dosyaları == Dosyaları.Karıştır ||
                    Dosyaları == Dosyaları.SıkıştırKarıştır || 
                    Dosyaları == Dosyaları.DüzeltAç ) Hata += "\r\nKarıştırma / Düzeltme işlemlerinde \"ParolaŞablonu\" boş olamaz";
            }

            Filtreler_Klasör = Filt_Kla.ToArray();
            Filtreler_Dosya = Filt_Dos.ToArray();

            Hata = Hata.Trim('\r', '\n', ' ');
        }
    }

    class Bir_Dosya_
    {
        public string Yolu;
        public long Boyutu = 0;
        public DateTime DeğiştirilmeTarihi;

        public Bir_Dosya_()
        {
            Yolu = "";
            Boyutu = 0;
            DeğiştirilmeTarihi = DateTime.Now;
        }
        public Bir_Dosya_(string Kök, string DosyaYolu)
        {
            Yolu = DosyaYolu.Remove(0, Kök.Length).Trim(' ').TrimEnd('\\');

            if (File.Exists(DosyaYolu))
            {
                FileInfo DosyaBilgisi = new FileInfo(DosyaYolu);
                Boyutu = DosyaBilgisi.Length;
                DeğiştirilmeTarihi = DosyaBilgisi.LastWriteTime;
            }
        }
    }
    class Bir_Dosya_Listesi_
    {
        public string Kök;
        public long Boyutu = 0;
        public Bir_Dosya_[] Dosyalar = new Bir_Dosya_[0];

        public Bir_Dosya_ Bul(string Yolu)
        {
            foreach (Bir_Dosya_ biri in Dosyalar)
            {
                if (biri.Yolu == Yolu) return biri;
            }

            return null;
        }
    }

    public class Ortak
    {
        public static Dictionary<string, string[]> Liste = new Dictionary<string, string[]>();
        public static void Listele()
        {
            Liste.Clear();
            foreach (string t in Yedekleyici.Ortak.Listele.Dosya(Yedekleyici.Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Talep"))
            {
                Bir_Talep_ Yeni = new Bir_Talep_(t);

                string[] eski;
                if (!Liste.TryGetValue(Yeni.Tanım, out eski))
                {
                    Liste.Add(Yeni.Tanım, new string[] { t, Yeni.Kaynak, Yeni.Hedef });
                }
                else Yedekleyici.Ortak.Günlük_Ekle(Yeni.Tanım + " tanımlı talep bir sefer daha gösterildi" +
                                                   Environment.NewLine + Yeni.DosyaYolu +
                                                   Environment.NewLine + eski[0]);
            }

        }
        public static Bir_Talep_ Aç(string Tanım)
        {
            string[] dosyayolu;

            if (Liste.TryGetValue(Tanım, out dosyayolu))
            {
                return new Bir_Talep_(dosyayolu[0]);
            }

            return null;
        }
    }

    public class İşlem_
    {
        public Senaryo.Bir_Senaryo_ Senaryo;
        public Bir_Talep_ Talep;
        public Görsel_ Görsel = null;

        decimal İşlenenDosyaBoyutu = 0, SonGeçerliİşlenenDosyaBoyutu = 0, Başarılıİşlem = 0, Hatalıİşlem = 0;
        Bir_Dosya_Listesi_ Kaynak_DosyaListesi = null;
        int tick = 0, SıraNo = 0;
        DahaCokKarmasiklastirma_ Dçk;
        DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_ YığınKarıştırGirdi;
        DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_ YığınDüzeltGirdi;
        string BirdenFazlaAşamalıİşlem_GeciciKlasör = "";
        İşlem_ BirdenFazlaAşamalıİşlem = null;

        public void Çalıştır()
        {
            İşlenenDosyaBoyutu = 0;

            #region Görsel Üretimi
            if (Görsel == null)
            {
                Görsel = new Görsel_();
                Yedekleyici.Ortak.Düzlem.Invoke((MethodInvoker)delegate () { Yedekleyici.Ortak.Düzlem.Controls.Add(Görsel); });
                Görsel.Başlat(Senaryo.Tanim, Talep.Tanım, Yedekleyici.Ortak.KarakterBüyüklüğü, Yedekleyici.Ortak.Düzlem.Width - 25, Talep.Kaynak, Talep.Hedef);
            }
            #endregion

            if (!Directory.Exists(Talep.Kaynak))
            {
                Görsel.Güncelle(-1, -1, Talep.Kaynak, "Kaynak yola erişilemiyor");
                throw new Exception(Senaryo.Tanim + " / " + Talep.Tanım + " / Kaynak yola erişilemiyor" + Environment.NewLine + Talep.Kaynak);
            }

            Görsel.Güncelle(-1, -1, Talep.Kaynak, "Dosya adedi sayılıyor");
            if (!Yedekleyici.Ortak.Dsi.DeğişiklikVarMı(Talep.Kaynak, Talep.Hedef)) goto KaynakVeHedefÖzdeş;

            //Kaynak Dosya Listesi
            Kaynak_DosyaListesi = DosyaListesiniOluştur(Talep.Kaynak, Talep.Dosyaları != Dosyaları.Sil); //sil ise tam dosya listesini oluştur

            //Hedef Klasörünün belirlenmesi
            string KıyaslanacakHedef_Klasör_ZipDosyası = Talep.Hedef;
            List<DateTime> Tarihler = new List<DateTime>();

            if (Talep.Versiyonla_adet > 1)
            {
                KıyaslanacakHedef_Klasör_ZipDosyası = Talep.Hedef + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "\\";

                string[] klasörler = Yedekleyici.Ortak.Listele.Klasör(Talep.Hedef, SearchOption.TopDirectoryOnly);
                if (klasörler.Count() > 0)
                {
                    foreach (string klasör in klasörler) { try { Tarihler.Add(DateTime.ParseExact(klasör.Remove(0, Talep.Hedef.Length).Trim(' ').TrimEnd('\\'), "dd_MM_yyyy_HH_mm_ss", System.Globalization.CultureInfo.InvariantCulture)); } catch (Exception) { } }

                    Tarihler.Sort((a, b) => a.CompareTo(b)); //silmeye 0. elemandan başlanacak

                    //Değişiklik kontrolü
                    if (Tarihler.Count > 0) KıyaslanacakHedef_Klasör_ZipDosyası = Talep.Hedef + Tarihler[Tarihler.Count() - 1].ToString("dd_MM_yyyy_HH_mm_ss") + "\\";
                }
            }

            //Hedef klasörün dosya listesinin oluşturulması
            Bir_Dosya_Listesi_ HedefinListesi = new Bir_Dosya_Listesi_();
            if (Talep.Dosyaları == Dosyaları.Kopyala || Talep.Dosyaları == Dosyaları.Karıştır || Talep.Dosyaları == Dosyaları.Düzelt) HedefinListesi = DosyaListesiniOluştur(KıyaslanacakHedef_Klasör_ZipDosyası);
            else if (Talep.Dosyaları == Dosyaları.Sıkıştır)
            {
                string SıkıştırılılacakDosyaAdı = Path.GetDirectoryName(Talep.Kaynak);
                int konum = SıkıştırılılacakDosyaAdı.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                KıyaslanacakHedef_Klasör_ZipDosyası = KıyaslanacakHedef_Klasör_ZipDosyası + SıkıştırılılacakDosyaAdı.Substring(konum) + ".zip";

                if (File.Exists(KıyaslanacakHedef_Klasör_ZipDosyası)) HedefinListesi = DosyaListesiniOluştur_Zip(KıyaslanacakHedef_Klasör_ZipDosyası);
            }
            else if (Talep.Dosyaları == Dosyaları.Aç)
            {
                Kaynak_DosyaListesi = DosyaListesiniOluştur_Karışık(Talep.Kaynak);
                HedefinListesi = DosyaListesiniOluştur(KıyaslanacakHedef_Klasör_ZipDosyası);
            }
            else if (Talep.Dosyaları == Dosyaları.SıkıştırKarıştır)
            {
                if (!Directory.Exists(KıyaslanacakHedef_Klasör_ZipDosyası)) Directory.CreateDirectory(KıyaslanacakHedef_Klasör_ZipDosyası);

                BirdenFazlaAşamalıİşlem_GeciciKlasör = Yedekleyici.Ortak.pak_Geçici + D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), 4);

                BirdenFazlaAşamalıİşlem = new İşlem_();
                BirdenFazlaAşamalıİşlem.Görsel = Görsel;
                BirdenFazlaAşamalıİşlem.Senaryo = Senaryo;
                BirdenFazlaAşamalıİşlem.Talep = new Bir_Talep_((string[])null);
                BirdenFazlaAşamalıİşlem.Talep.Tanım = D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), 8);
                BirdenFazlaAşamalıİşlem.Talep.Kaynak = KıyaslanacakHedef_Klasör_ZipDosyası;
                BirdenFazlaAşamalıİşlem.Talep.Hedef = BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\A\\";
                BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Düzelt;
                BirdenFazlaAşamalıİşlem.Talep.ParolaŞablonu = Talep.ParolaŞablonu;

                BirdenFazlaAşamalıİşlem.Talep.TamKopya = Talep.TamKopya;
                BirdenFazlaAşamalıİşlem.Talep.CpuYuzdesi = Talep.CpuYuzdesi;
                BirdenFazlaAşamalıİşlem.Talep.HddYuzdesi = Talep.HddYuzdesi;
                BirdenFazlaAşamalıİşlem.Talep.Filtreler_Dosya = Talep.Filtreler_Dosya;
                BirdenFazlaAşamalıİşlem.Talep.Filtreler_Klasör = Talep.Filtreler_Klasör;

                BirdenFazlaAşamalıİşlem.Çalıştır();

                HedefinListesi = DosyaListesiniOluştur_Karışık(BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\A\\");

                string ÜstKlasör = Directory.GetParent(Kaynak_DosyaListesi.Kök).Name + Path.DirectorySeparatorChar;
                foreach (var biri in Kaynak_DosyaListesi.Dosyalar) biri.Yolu = ÜstKlasör + biri.Yolu;
            }
            else if (Talep.Dosyaları == Dosyaları.DüzeltAç)
            {
                BirdenFazlaAşamalıİşlem_GeciciKlasör = Yedekleyici.Ortak.pak_Geçici + D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), 4);
               
                BirdenFazlaAşamalıİşlem = new İşlem_();
                BirdenFazlaAşamalıİşlem.Görsel = Görsel;
                BirdenFazlaAşamalıİşlem.Senaryo = Senaryo;
                BirdenFazlaAşamalıİşlem.Talep = new Bir_Talep_((string[])null);
                BirdenFazlaAşamalıİşlem.Talep.Tanım = D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), 8);
                BirdenFazlaAşamalıİşlem.Talep.Kaynak = Talep.Kaynak;
                BirdenFazlaAşamalıİşlem.Talep.Hedef = BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\A\\";
                BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Düzelt;
                BirdenFazlaAşamalıİşlem.Talep.ParolaŞablonu = Talep.ParolaŞablonu;

                BirdenFazlaAşamalıİşlem.Talep.TamKopya = Talep.TamKopya;
                BirdenFazlaAşamalıİşlem.Talep.CpuYuzdesi = Talep.CpuYuzdesi;
                BirdenFazlaAşamalıİşlem.Talep.HddYuzdesi = Talep.HddYuzdesi;
                BirdenFazlaAşamalıİşlem.Talep.Filtreler_Dosya = Talep.Filtreler_Dosya;
                BirdenFazlaAşamalıİşlem.Talep.Filtreler_Klasör = Talep.Filtreler_Klasör;

                BirdenFazlaAşamalıİşlem.Çalıştır();

                Kaynak_DosyaListesi = DosyaListesiniOluştur_Karışık(BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\A\\");

                HedefinListesi = DosyaListesiniOluştur(KıyaslanacakHedef_Klasör_ZipDosyası);
            }

            //Kaynak ile Hedef karşılaştırması
            Bir_Dosya_Listesi_ Fark = KlasörFarkListesiniÇıkar(Kaynak_DosyaListesi, HedefinListesi);
            if (Talep.Versiyonla_adet > 1)
            {
                if (Fark.Dosyalar.Length == 0) goto Dsi_Baslat;
                KıyaslanacakHedef_Klasör_ZipDosyası = Talep.Hedef + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "\\" + Path.GetFileName(KıyaslanacakHedef_Klasör_ZipDosyası);

                while (Tarihler.Count > Talep.Versiyonla_adet && !Senaryo.Durdurmaİsteği)
                {
                    string Silinecek = Talep.Hedef + Tarihler[0].ToString("dd_MM_yyyy_HH_mm_ss");
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Versiyonlama için fazlalık dosyalar siliniyor", Silinecek)) break;
                    if (!Yedekleyici.Ortak.Sil.Klasör(Silinecek))
                    {
                        Hatalıİşlem++;
                        Yedekleyici.Ortak.Günlük_Ekle("Klasör Silinemedi -> " + Silinecek);
                    }
                    Tarihler.RemoveAt(0);
                }

                if (Talep.Versiyonla_mb > 0)
                {
                    List<Bir_Dosya_Listesi_> ttt = new List<Bir_Dosya_Listesi_>();
                    foreach (DateTime biri in Tarihler)
                    {
                        string okunacak = Talep.Hedef + biri.ToString("dd_MM_yyyy_HH_mm_ss") + "\\";
                        ttt.Add(DosyaListesiniOluştur(okunacak));
                    }

                    long toplam = long.MaxValue;
                    long en_Fazla = (long)Talep.Versiyonla_mb * 1024 * 1024;
                    while (toplam > en_Fazla && !Senaryo.Durdurmaİsteği)
                    {
                        toplam = 0;
                        foreach (Bir_Dosya_Listesi_ biri in ttt) toplam += biri.Boyutu;
                        if (toplam > en_Fazla)
                        {
                            if (GörseliGüncelle_ÇıkışİsteniyorMu("Versiyonlama için fazlalık dosyalar siliniyor", ttt[0].Kök, (int)(en_Fazla * 100 / toplam))) break;
                            if (!Yedekleyici.Ortak.Sil.Klasör(ttt[0].Kök))
                            {
                                Hatalıİşlem++;
                                Yedekleyici.Ortak.Günlük_Ekle("Klasör Silinemedi -> " + ttt[0].Kök);
                            }
                            ttt.RemoveAt(0);
                        }
                    }
                }
            }
            else
            {
                if (Talep.Dosyaları == Dosyaları.Kopyala || Talep.Dosyaları == Dosyaları.Karıştır || Talep.Dosyaları == Dosyaları.Düzelt) Kaynak_DosyaListesi = Fark;
                else if (Fark.Dosyalar.Length == 0) goto Dsi_Baslat;
            }
            Talep.Hedef = KıyaslanacakHedef_Klasör_ZipDosyası;
            if (Talep.Dosyaları == Dosyaları.Aç) Kaynak_DosyaListesi = DosyaListesiniOluştur(Talep.Kaynak);

            //Hedef klasörün kullanılabilirliği
            string KontrolEdilecekOlan = Path.GetDirectoryName(KıyaslanacakHedef_Klasör_ZipDosyası);
            if (!Directory.Exists(KontrolEdilecekOlan))
            {
                Directory.CreateDirectory(KontrolEdilecekOlan);
                if (!Directory.Exists(KontrolEdilecekOlan))
                {
                    Görsel.Güncelle(-1, -1, KontrolEdilecekOlan, "Hedef klasör kullanılamıyor");
                    throw new Exception(Senaryo.Tanim + " / " + Talep.Tanım + " / Hedef klasör kullanılamıyor" + Environment.NewLine + KontrolEdilecekOlan);
                }
            }

            //Karıştırma düzeltme için gerekli işlemler
            if (Talep.Dosyaları == Dosyaları.Karıştır || Talep.Dosyaları == Dosyaları.Düzelt)
            {
                Dçk = new DahaCokKarmasiklastirma_();
                YığınKarıştırGirdi = new DahaCokKarmasiklastirma_._Yığın_Karıştır_Girdi_("", false, YüzdeDeğeriniDeğerlendir_Sifrele);
                YığınDüzeltGirdi = new DahaCokKarmasiklastirma_._Yığın_Düzelt_Girdi_(YüzdeDeğeriniDeğerlendir_SifresiniCoz);

                Depo.Biri birisi = Yedekleyici.Ortak.ParolaŞablonu.Find(x => x.Adı == Talep.ParolaŞablonu);
                if (birisi.Adı != Talep.ParolaŞablonu || string.IsNullOrEmpty(birisi.İçeriği))
                {
                    string msg = Senaryo.Tanim + " / " + Talep.Tanım + " / Parola şablonu bulunamadı" + Environment.NewLine + Talep.ParolaŞablonu;
                    Görsel.Güncelle(-1, -1, "", msg);
                    throw new Exception(msg);
                }

                Talep.ParolaŞablonu = Dçk.Düzelt(birisi.İçeriği, Yedekleyici.Ortak.Parola);
            }

            switch (Talep.Dosyaları)
            {
                case Dosyaları.Kopyala:
                case Dosyaları.Karıştır:
                case Dosyaları.Düzelt:              Kopyala_Karıştır_Düzelt();  break;
                case Dosyaları.Sıkıştır:            Sıkıştır();                 break;
                case Dosyaları.Aç:                  Aç();                       break;
                case Dosyaları.Sil:                 Sil();                      break;
                case Dosyaları.SıkıştırKarıştır:    SıkıştırKarıştır();         break;
                case Dosyaları.DüzeltAç:            DüzeltAç();                 break;
            }
            
        Dsi_Baslat:
            if (Directory.Exists(BirdenFazlaAşamalıİşlem_GeciciKlasör)) 
            {
                BirdenFazlaAşamalıİşlem.Talep.Tanım += "-";
                BirdenFazlaAşamalıİşlem.Talep.Kaynak = BirdenFazlaAşamalıİşlem_GeciciKlasör;
                BirdenFazlaAşamalıİşlem.Talep.Hedef = BirdenFazlaAşamalıİşlem_GeciciKlasör;
                BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Sil;
                BirdenFazlaAşamalıİşlem.Çalıştır();
            }

            Yedekleyici.Ortak.Dsi.Başlat(Talep.Kaynak, Talep.Hedef);

        KaynakVeHedefÖzdeş:
            string msga = "Kaynak ve hedef özdeş.";
            if (Kaynak_DosyaListesi != null) msga = "Tamamlandı. " + D_DosyaBoyutu.Yazıya(Kaynak_DosyaListesi.Boyutu) + ", " + Kaynak_DosyaListesi.Dosyalar.Length + " dosya işlem gördü.";
            if (Senaryo.Durdurmaİsteği) msga = "Senaryo durduruldu";

            Görsel.Güncelle(100, 100, Talep.Kaynak + Environment.NewLine + Başarılıİşlem + " başarılı, " + Hatalıİşlem + " hatalı işlem. " + DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"), msga);
            Yedekleyici.Ortak.Günlük_Ekle("Kaynak : " + Talep.Kaynak + Environment.NewLine +
                                          "Hedef : " + Talep.Hedef + Environment.NewLine +
                                          Başarılıİşlem + " başarılı, " + Hatalıİşlem + " hatalı işlem." + Environment.NewLine +
                                          msga, "Bilgi");
        }
        void Kopyala_Karıştır_Düzelt()
        {
            for (SıraNo = 0; SıraNo < Kaynak_DosyaListesi.Dosyalar.Length; SıraNo++)
            {
                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Kopyalanıyor")) break;

                string K = Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu;
                string H = Talep.Hedef + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu;

                SonGeçerliİşlenenDosyaBoyutu = İşlenenDosyaBoyutu;

                if (!File.Exists(H + "_mup_")) { if (File.Exists(H)) File.Move(H, H + "_mup_"); }
                else if (!Yedekleyici.Ortak.Sil.Dosya(H)) Yedekleyici.Ortak.Günlük_Ekle("Dosya silinemedi -> " + H);

                bool Sonuç = false;
                string HataMesajı = "";
                switch (Talep.Dosyaları)
                {
                    case Dosyaları.Kopyala:
                        Sonuç = Argemup_Dosyalama_Kopyala(K, H);
                        HataMesajı = "Dosya Kopyalanamadı -> ";
                        break;
                    case Dosyaları.Karıştır:
                        Sonuç = Dçk.Karıştır(K, H, Talep.ParolaŞablonu, YığınKarıştırGirdi);
                        HataMesajı = "Dosya Karıştırılamadı -> " + Dçk.SonHatayıOku() + " -> ";
                        break;
                    case Dosyaları.Düzelt:
                        Sonuç = Dçk.Düzelt(K, H, Talep.ParolaŞablonu, YığınDüzeltGirdi);
                        HataMesajı = "Dosya Düzeltilemedi -> " + Dçk.SonHatayıOku() + " -> ";
                        break;
                }

                if (Sonuç)
                {
                    Başarılıİşlem++;
                    if (File.Exists(H + "_mup_")) Yedekleyici.Ortak.Sil.Dosya(H + "_mup_");
                }
                else
                {
                    Yedekleyici.Ortak.Günlük_Ekle(HataMesajı + K);
                    Hatalıİşlem++;
                    if (File.Exists(H)) Yedekleyici.Ortak.Sil.Dosya(H);
                    if (File.Exists(H + "_mup_")) File.Move(H + "_mup_", H);

                    if (HataMesajı.Contains("-Şifre Hatalı"))
                    {
                        if (Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu > 0) throw new Exception(Senaryo.Tanim + " / " + Talep.Tanım + " / Parola hatalı");
                    }
                }

                İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu;
            }

            if (Talep.TamKopya)
            {
                string[] Klasörler = Yedekleyici.Ortak.Listele.Klasör(Talep.Hedef, SearchOption.AllDirectories);
                for (SıraNo = 0; SıraNo < Klasörler.Length; SıraNo++)
                {
                    int yüzde = SıraNo * 100 / Klasörler.Length;
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Tam kopya hazırlanıyor 1 / 3", Klasörler[SıraNo], yüzde, (yüzde + 0) * 100 / 300)) break;

                    string fark = Klasörler[SıraNo].Remove(0, Talep.Hedef.Length);
                    if (!Directory.Exists(Talep.Kaynak + fark))
                    {
                        if (!Yedekleyici.Ortak.Sil.Klasör(Klasörler[SıraNo]))
                        {
                            Hatalıİşlem++;
                            Yedekleyici.Ortak.Günlük_Ekle("Fazla klasör Silinemedi -> " + Klasörler[SıraNo]);
                        }
                    }
                    else
                    {
                        if (!FiltrelemeSonucundaKlasörüKullan(fark, Talep.Filtreler_Klasör))
                        {
                            if (!Yedekleyici.Ortak.Sil.Klasör(Klasörler[SıraNo]))
                            {
                                Hatalıİşlem++;
                                Yedekleyici.Ortak.Günlük_Ekle("Fazla klasör Silinemedi -> " + Klasörler[SıraNo]);
                            }
                        }
                    }
                }

                Klasörler = Yedekleyici.Ortak.Listele.Klasör(Talep.Kaynak, SearchOption.AllDirectories);
                for (SıraNo = 0; SıraNo < Klasörler.Length; SıraNo++)
                {
                    int yüzde = SıraNo * 100 / Klasörler.Length;
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Tam kopya hazırlanıyor 2 / 3", Klasörler[SıraNo], yüzde, (yüzde + 100) * 100 / 300)) break;

                    string fark = Klasörler[SıraNo].Remove(0, Talep.Kaynak.Length);
                    if (!FiltrelemeSonucundaKlasörüKullan(fark, Talep.Filtreler_Klasör)) continue;

                    if (!Directory.Exists(Talep.Hedef + fark)) Directory.CreateDirectory(Talep.Hedef + fark);
                }

                string[] Dosyalar = Yedekleyici.Ortak.Listele.Dosya(Talep.Hedef, SearchOption.AllDirectories);
                for (SıraNo = 0; SıraNo < Dosyalar.Length; SıraNo++)
                {
                    int yüzde = SıraNo * 100 / Dosyalar.Length;
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Tam kopya hazırlanıyor 3 / 3", Dosyalar[SıraNo], yüzde, (yüzde + 200) * 100 / 300)) break;

                    string fark = Dosyalar[SıraNo].Remove(0, Talep.Hedef.Length);
                    if (!File.Exists(Talep.Kaynak + fark))
                    {
                        if (!Yedekleyici.Ortak.Sil.Dosya(Dosyalar[SıraNo]))
                        {
                            Hatalıİşlem++;
                            Yedekleyici.Ortak.Günlük_Ekle("Fazla dosya silinemedi -> " + Dosyalar[SıraNo]);
                        }
                    }
                    else
                    {
                        if (!FiltrelemeSonucundaDosyayıKullan(Dosyalar[SıraNo].Remove(0, Talep.Hedef.Length)))
                        {
                            if (!Yedekleyici.Ortak.Sil.Dosya(Dosyalar[SıraNo]))
                            {
                                Hatalıİşlem++;
                                Yedekleyici.Ortak.Günlük_Ekle("Fazla dosya silinemedi -> " + Dosyalar[SıraNo]);
                            }
                        }
                    } 
                }
            }
        }
        void Sıkıştır()
        {
            int ttt = Environment.TickCount + 250;

            ZipArchiveMode Tip = ZipArchiveMode.Create;
            if (!File.Exists(Talep.Hedef + "_mup_")) { if (File.Exists(Talep.Hedef)) File.Move(Talep.Hedef, Talep.Hedef + "_mup_"); }
            else if (!Yedekleyici.Ortak.Sil.Dosya(Talep.Hedef)) { Tip = ZipArchiveMode.Update; Yedekleyici.Ortak.Günlük_Ekle("Dosya silinemedi -> " + Talep.Hedef); }

            try
            {
                using (ZipArchive archive = ZipFile.Open(Talep.Hedef, Tip))
                {
                    for (SıraNo = 0; SıraNo < Kaynak_DosyaListesi.Dosyalar.Length; SıraNo++)
                    {
                        ZipArchiveEntry biri = null;
                        SonGeçerliİşlenenDosyaBoyutu = İşlenenDosyaBoyutu;

                        try
                        {
                            using (FileStream K = new FileStream(Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu, FileMode.Open, FileAccess.Read))
                            {
                                biri = archive.CreateEntry(Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu, CompressionLevel.Optimal);
                                biri.LastWriteTime = Kaynak_DosyaListesi.Dosyalar[SıraNo].DeğiştirilmeTarihi;
                                using (Stream H = biri.Open())
                                {
                                    long KaynakDosyaBoyutu = K.Length, KaynakOkunmuşAdet = 0;
                                    int KaynakOkunacakDosyaBoyutu;
                                    byte[] Tampon = new byte[4 * 1024];

                                    while (KaynakOkunmuşAdet < KaynakDosyaBoyutu)
                                    {
                                        if (KaynakDosyaBoyutu - KaynakOkunmuşAdet > Tampon.Length) KaynakOkunacakDosyaBoyutu = Tampon.Length;
                                        else KaynakOkunacakDosyaBoyutu = (int)KaynakDosyaBoyutu - (int)KaynakOkunmuşAdet;

                                        KaynakOkunacakDosyaBoyutu = K.Read(Tampon, 0, KaynakOkunacakDosyaBoyutu);
                                        H.Write(Tampon, 0, KaynakOkunacakDosyaBoyutu);

                                        KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;
                                        İşlenenDosyaBoyutu += KaynakOkunacakDosyaBoyutu;

                                        if (ttt < Environment.TickCount) { H.Flush(); ttt = Environment.TickCount + 250; }
                                        if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Sıkıştırılıyor")) break;
                                    }
                                }
                            }

                            Başarılıİşlem++;
                            İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu;
                        }
                        catch (Exception ex)
                        {
                            İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu;
                            Hatalıİşlem++;
                            Yedekleyici.Ortak.Günlük_Ekle(ex.ToString());

                            if (biri != null) biri.Delete();
                        }
                    }
                }

                if (File.Exists(Talep.Hedef + "_mup_")) Yedekleyici.Ortak.Sil.Dosya(Talep.Hedef + "_mup_");
            }
            catch (Exception ex)
            {
                Hatalıİşlem++;
                Yedekleyici.Ortak.Günlük_Ekle(ex.ToString());

                if (File.Exists(Talep.Hedef)) Yedekleyici.Ortak.Sil.Dosya(Talep.Hedef);
                if (File.Exists(Talep.Hedef + "_mup_")) File.Move(Talep.Hedef + "_mup_", Talep.Hedef);
            } 
        }  
        void Aç()
        {
            for (SıraNo = 0; SıraNo < Kaynak_DosyaListesi.Dosyalar.Length; SıraNo++)
            {
                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Açılıyor")) break;

                SonGeçerliİşlenenDosyaBoyutu = İşlenenDosyaBoyutu;

                try
                {
                    if (Path.GetExtension(Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu) != ".zip")
                    {
                        string K = Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu;
                        string H = Talep.Hedef + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu;

                        bool Sonuc = true;
                        if (File.Exists(H)) if (Math.Abs((Kaynak_DosyaListesi.Dosyalar[SıraNo].DeğiştirilmeTarihi - File.GetLastWriteTime(H)).TotalSeconds) <= 3) goto DevamEt;

                        if (!File.Exists(H + "_mup_")) { if (File.Exists(H)) File.Move(H, H + "_mup_"); }
                        else if (!Yedekleyici.Ortak.Sil.Dosya(H)) Yedekleyici.Ortak.Günlük_Ekle("Dosya silinemedi -> " + H);

                        Sonuc = Argemup_Dosyalama_Kopyala(K, H);

                    DevamEt:
                        if (Sonuc)
                        {
                            Başarılıİşlem++;
                            if (File.Exists(H + "_mup_")) Yedekleyici.Ortak.Sil.Dosya(H + "_mup_");
                        }
                        else
                        {
                            Yedekleyici.Ortak.Günlük_Ekle("Dosya Kopyalanamadı -> " + K);
                            Hatalıİşlem++;
                            if (File.Exists(H)) Yedekleyici.Ortak.Sil.Dosya(H);
                            if (File.Exists(H + "_mup_")) File.Move(H + "_mup_", H);
                        }
                    }
                    else
                    {
                        Bir_Dosya_Listesi_ SıkıstırılmışınListesi = DosyaListesiniOluştur_Zip(Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu);
                        Kaynak_DosyaListesi.Boyutu = Kaynak_DosyaListesi.Boyutu - Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu + SıkıstırılmışınListesi.Boyutu;

                        string AçılacakKlasörAdı = Talep.Hedef + Path.GetFileNameWithoutExtension(Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu) + Path.DirectorySeparatorChar;
                        int ttt = Environment.TickCount + 250;

                        using (ZipArchive archive = ZipFile.Open(Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu, ZipArchiveMode.Read))
                        {
                            foreach (Bir_Dosya_ biri in SıkıstırılmışınListesi.Dosyalar)
                            {
                                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Açılıyor")) break;

                                string H = AçılacakKlasörAdı + biri.Yolu;

                                try
                                {
                                    if (File.Exists(H)) if (Math.Abs((biri.DeğiştirilmeTarihi - File.GetLastWriteTime(H)).TotalSeconds) <= 3) goto DevamEt;

                                    if (!File.Exists(H + "_mup_")) { if (File.Exists(H)) File.Move(H, H + "_mup_"); }
                                    else if (!Yedekleyici.Ortak.Sil.Dosya(H)) Yedekleyici.Ortak.Günlük_Ekle("Dosya silinemedi -> " + H);

                                    Directory.CreateDirectory(Path.GetDirectoryName(H));
                                    using (FileStream Str_H = new FileStream(H, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        using (Stream Str_K = archive.GetEntry(biri.Yolu).Open())
                                        {
                                            long KaynakDosyaBoyutu = biri.Boyutu, KaynakOkunmuşAdet = 0;
                                            int KaynakOkunacakDosyaBoyutu;
                                            byte[] Tampon = new byte[4 * 1024];

                                            while (KaynakOkunmuşAdet < KaynakDosyaBoyutu)
                                            {
                                                if (KaynakDosyaBoyutu - KaynakOkunmuşAdet > Tampon.Length) KaynakOkunacakDosyaBoyutu = Tampon.Length;
                                                else KaynakOkunacakDosyaBoyutu = (int)KaynakDosyaBoyutu - (int)KaynakOkunmuşAdet;

                                                KaynakOkunacakDosyaBoyutu = Str_K.Read(Tampon, 0, KaynakOkunacakDosyaBoyutu);
                                                Str_H.Write(Tampon, 0, KaynakOkunacakDosyaBoyutu);

                                                KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;
                                                İşlenenDosyaBoyutu += KaynakOkunacakDosyaBoyutu;

                                                if (ttt < Environment.TickCount) { Str_H.Flush(); ttt = Environment.TickCount + 250; }
                                                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Açılıyor")) break;
                                            }
                                        }
                                    }
                                    File.SetLastWriteTime(H, biri.DeğiştirilmeTarihi);
                                    Başarılıİşlem++;

                                DevamEt:
                                    if (File.Exists(H + "_mup_")) Yedekleyici.Ortak.Sil.Dosya(H + "_mup_");
                                }
                                catch (Exception ex)
                                {
                                    Hatalıİşlem++;
                                    Yedekleyici.Ortak.Günlük_Ekle(ex.ToString());

                                    if (File.Exists(H)) Yedekleyici.Ortak.Sil.Dosya(H);
                                    if (File.Exists(H + "_mup_")) File.Move(H + "_mup_", H);
                                }
                            }
                        }
                        Başarılıİşlem++;
                    }

                    İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu;
                }
                catch (Exception ex)
                {
                    İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu;
                    Hatalıİşlem++;
                    Yedekleyici.Ortak.Günlük_Ekle(ex.ToString());
                }
            }
        }
        void Sil()
        {
            if (Talep.Kaynak != Talep.Hedef) throw new Exception(Senaryo.Tanim + " / " + Talep.Tanım + " / Kaynak ile Hedef parametreleri farklı");

            for (SıraNo = 0; SıraNo < Kaynak_DosyaListesi.Dosyalar.Length; SıraNo++)
            {
                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Siliniyor")) break;

                string K = Kaynak_DosyaListesi.Kök + Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu;
                if (!Yedekleyici.Ortak.Sil.Dosya(K))
                {
                    Yedekleyici.Ortak.Günlük_Ekle("Dosya silinemedi -> " + K);
                    Hatalıİşlem++;
                }
                else Başarılıİşlem++;

                İşlenenDosyaBoyutu += Kaynak_DosyaListesi.Dosyalar[SıraNo].Boyutu;
            }

            Yedekleyici.Ortak.Sil.Klasör(Talep.Kaynak);
        }
        void SıkıştırKarıştır()
        {
            BirdenFazlaAşamalıİşlem.Talep.Tanım += "+";
            BirdenFazlaAşamalıİşlem.Talep.Kaynak = Talep.Kaynak;
            BirdenFazlaAşamalıİşlem.Talep.Hedef = BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\B\\";
            BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Sıkıştır;
            BirdenFazlaAşamalıİşlem.Çalıştır();

            BirdenFazlaAşamalıİşlem.Talep.Tanım += "+";
            BirdenFazlaAşamalıİşlem.Talep.Kaynak = BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\B\\";
            BirdenFazlaAşamalıİşlem.Talep.Hedef = Talep.Hedef;
            BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Karıştır;
            BirdenFazlaAşamalıİşlem.Talep.ParolaŞablonu = Talep.ParolaŞablonu;
            BirdenFazlaAşamalıİşlem.Çalıştır();
        }
        void DüzeltAç()
        {
            BirdenFazlaAşamalıİşlem.Talep.Tanım += "+";
            BirdenFazlaAşamalıİşlem.Talep.Kaynak = BirdenFazlaAşamalıİşlem_GeciciKlasör + "\\A\\";
            BirdenFazlaAşamalıİşlem.Talep.Hedef = Talep.Hedef;
            BirdenFazlaAşamalıİşlem.Talep.Dosyaları = Dosyaları.Aç;
            BirdenFazlaAşamalıİşlem.Talep.ParolaŞablonu = Talep.ParolaŞablonu;
            BirdenFazlaAşamalıİşlem.Çalıştır();
        }

        #region Görsel Yönetimi
        bool GörseliGüncelle_ÇıkışİsteniyorMu(string Durum)
        {
            bool iptal_talebi_görsel = false;
            if (tick < Environment.TickCount)
            {
                if (Talep.CpuYuzdesi < 100)
                {
                    while (Yedekleyici.Ortak.CpuYüzdesi() > Talep.CpuYuzdesi && !iptal_talebi_görsel && !Senaryo.Durdurmaİsteği)
                    {
                        iptal_talebi_görsel = Görsel.Güncelle(-1, -1, "!!! İşlemci %" + Talep.CpuYuzdesi + " yoğun olduğundan talep beklemede !!!",
                                                Durum + " -> " + (SıraNo + 1) + " / " + Kaynak_DosyaListesi.Dosyalar.Length + ", " + D_DosyaBoyutu.Yazıya(İşlenenDosyaBoyutu) + " / " + D_DosyaBoyutu.Yazıya(Kaynak_DosyaListesi.Boyutu));
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (Talep.HddYuzdesi < 100)
                {
                    while (Yedekleyici.Ortak.HddYüzdesi() > Talep.HddYuzdesi && !iptal_talebi_görsel && !Senaryo.Durdurmaİsteği)
                    {
                        iptal_talebi_görsel = Görsel.Güncelle(-1, -1, "!!! Sabit Disk %" + Talep.HddYuzdesi + " yoğun olduğundan talep beklemede !!!",
                                                Durum + " -> " + (SıraNo + 1) + " / " + Kaynak_DosyaListesi.Dosyalar.Length + ", " + D_DosyaBoyutu.Yazıya(İşlenenDosyaBoyutu) + " / " + D_DosyaBoyutu.Yazıya(Kaynak_DosyaListesi.Boyutu));
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (Yedekleyici.Ortak.AnaEkran.WindowState != FormWindowState.Minimized && Yedekleyici.Ortak.EtkinPanel == Yedekleyici.Ortak.PanelListesi.Talepler)
                {
                 
                    iptal_talebi_görsel = Görsel.Güncelle(SıraNo * 100 / Kaynak_DosyaListesi.Dosyalar.Length,
                                                        (int)(İşlenenDosyaBoyutu * 100 / Kaynak_DosyaListesi.Boyutu),
                                                        Kaynak_DosyaListesi.Dosyalar[SıraNo].Yolu,
                                                        Durum + " -> " + (SıraNo + 1) + " / " + Kaynak_DosyaListesi.Dosyalar.Length + " -> " + D_DosyaBoyutu.Yazıya(İşlenenDosyaBoyutu) + " / " + D_DosyaBoyutu.Yazıya(Kaynak_DosyaListesi.Boyutu));
                }

                tick = Environment.TickCount + 250;
            }

            return Senaryo.Durdurmaİsteği || iptal_talebi_görsel;
        }
        bool GörseliGüncelle_ÇıkışİsteniyorMu(string ÜstYazı, string AltYazı, int ÜstYüzde = -1, int AltYüzde = -1)
        {
            bool iptal_talebi_görsel = false;
            if (tick < Environment.TickCount)
            {
                if (AltYüzde == -1) AltYüzde = ÜstYüzde;

                if (Talep.CpuYuzdesi < 100)
                {
                    while (Yedekleyici.Ortak.CpuYüzdesi() > Talep.CpuYuzdesi && !iptal_talebi_görsel && !Senaryo.Durdurmaİsteği)
                    {
                        iptal_talebi_görsel = Görsel.Güncelle(ÜstYüzde, AltYüzde, "!!! İşlemci %" + Talep.CpuYuzdesi + " yoğun olduğundan talep beklemede !!!" + Environment.NewLine + AltYazı, ÜstYazı);
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (Talep.HddYuzdesi < 100)
                {
                    while (Yedekleyici.Ortak.HddYüzdesi() > Talep.HddYuzdesi && !iptal_talebi_görsel && !Senaryo.Durdurmaİsteği)
                    {
                        iptal_talebi_görsel = Görsel.Güncelle(ÜstYüzde, AltYüzde, "!!! Sabit Disk %" + Talep.HddYuzdesi + " yoğun olduğundan talep beklemede !!!" + Environment.NewLine + AltYazı, ÜstYazı);
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (Yedekleyici.Ortak.AnaEkran.WindowState != FormWindowState.Minimized && Yedekleyici.Ortak.EtkinPanel == Yedekleyici.Ortak.PanelListesi.Talepler)
                {
                    iptal_talebi_görsel = Görsel.Güncelle(ÜstYüzde, AltYüzde, AltYazı, ÜstYazı);
                }

                tick = Environment.TickCount + 250;
            }

            return Senaryo.Durdurmaİsteği || iptal_talebi_görsel;
        }
        #endregion

        #region Dosya Listesi
        bool KlasörİçerikleriAynıMı(Bir_Dosya_Listesi_ A, Bir_Dosya_Listesi_ B)
        {
            if (A.Dosyalar.Length != B.Dosyalar.Length) return false;

            for (int i = 0; i < A.Dosyalar.Length; i++)
            {
                int yüzde = i * 100 / A.Dosyalar.Length;

                if (GörseliGüncelle_ÇıkışİsteniyorMu("Kaynak ve hedef karşılaştırılıyor",
                    A.Kök + Environment.NewLine + B.Kök, yüzde)) break;

                Bir_Dosya_ B_nin_örneği = B.Bul(A.Dosyalar[i].Yolu);
                if (B_nin_örneği == null) return false;

                TimeSpan Fark_Süre = A.Dosyalar[i].DeğiştirilmeTarihi - B_nin_örneği.DeğiştirilmeTarihi;
                if (Math.Abs(Fark_Süre.TotalSeconds) > 2) return false;
            }

            return true;
        }
        Bir_Dosya_Listesi_ KlasörFarkListesiniÇıkar(Bir_Dosya_Listesi_ Kaynak, Bir_Dosya_Listesi_ Hedef)
        {
            Bir_Dosya_Listesi_ Çıktı = new Bir_Dosya_Listesi_();
            List<Bir_Dosya_> Dosyalar = new List<Bir_Dosya_>();

            List<Bir_Dosya_> Hedefin_kopyası = Hedef.Dosyalar.ToList();

            for (int i = 0; i < Kaynak.Dosyalar.Length; i++)
            {
                int yüzde = i * 100 / Kaynak.Dosyalar.Length;

                if (GörseliGüncelle_ÇıkışİsteniyorMu("Kaynak ve hedef karşılaştırılıyor",
                    Kaynak.Kök + Environment.NewLine + Hedef.Kök, yüzde)) break;

                Bir_Dosya_ Hedef_in_örneği = Hedefin_kopyası.Find(x => (x.Yolu == Kaynak.Dosyalar[i].Yolu) );
                if (Hedef_in_örneği != null)
                {
                    Hedefin_kopyası.Remove(Hedef_in_örneği);

                    TimeSpan Fark_Süre = Kaynak.Dosyalar[i].DeğiştirilmeTarihi - Hedef_in_örneği.DeğiştirilmeTarihi;
                    if (Math.Abs(Fark_Süre.TotalSeconds) <= 3) continue;
                }

                Dosyalar.Add(Kaynak.Dosyalar[i]);
                Çıktı.Boyutu += Kaynak.Dosyalar[i].Boyutu;
            }

            Çıktı.Kök = Kaynak.Kök;
            Çıktı.Dosyalar = Dosyalar.ToArray();
            return Çıktı;
        }
        Bir_Dosya_Listesi_ DosyaListesiniOluştur(string Kök, bool Ayıkla = true)
        {
            Bir_Dosya_Listesi_ DosyaListesi = new Bir_Dosya_Listesi_();
            List<Bir_Dosya_> Dosyalar = new List<Bir_Dosya_>();
            
            string[] K_ler = Yedekleyici.Ortak.Listele.Klasör(Kök, SearchOption.AllDirectories);
            foreach (var k in K_ler)
            {
                if (Senaryo.Durdurmaİsteği) break;
                if (Ayıkla && !FiltrelemeSonucundaKlasörüKullan(k.Remove(0, Kök.Length), Talep.Filtreler_Klasör)) continue;

                string[] KlasördekiDosyalar = Yedekleyici.Ortak.Listele.Dosya(k, SearchOption.TopDirectoryOnly);
                foreach (var d in KlasördekiDosyalar)
                {
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosya adedi sayılıyor" + " -> " + Dosyalar.Count + " -> " + D_DosyaBoyutu.Yazıya(DosyaListesi.Boyutu), d)) break;
                    if (Ayıkla && !FiltrelemeSonucundaDosyayıKullan(d.Remove(0, Kök.Length))) continue;

                    Bir_Dosya_ Yeni = new Bir_Dosya_(Kök, d);
                    Dosyalar.Add(Yeni);
                    DosyaListesi.Boyutu += Yeni.Boyutu;
                }
            }

            string[] KlasördekiDosyalar_ = Yedekleyici.Ortak.Listele.Dosya(Kök, SearchOption.TopDirectoryOnly);
            foreach (var d in KlasördekiDosyalar_)
            {
                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosya adedi sayılıyor" + " -> " + Dosyalar.Count + " -> " + D_DosyaBoyutu.Yazıya(DosyaListesi.Boyutu), d)) break;
                if (Ayıkla && !FiltrelemeSonucundaDosyayıKullan(d.Remove(0, Kök.Length))) continue;

                Bir_Dosya_ Yeni = new Bir_Dosya_(Kök, d);
                Dosyalar.Add(Yeni);
                DosyaListesi.Boyutu += Yeni.Boyutu;
            }

            DosyaListesi.Kök = Kök;
            DosyaListesi.Dosyalar = Dosyalar.ToArray();
            return DosyaListesi;
        }
        Bir_Dosya_Listesi_ DosyaListesiniOluştur_Zip(string Dosya, bool Ayıkla = true)
        {
            Bir_Dosya_Listesi_ DosyaListesi = new Bir_Dosya_Listesi_();
            DosyaListesi.Kök = Path.GetDirectoryName(Dosya);
            DosyaListesi.Dosyalar = new Bir_Dosya_[0];

            if (!File.Exists(Dosya)) return DosyaListesi;

            List<Bir_Dosya_> Dosyalar = new List<Bir_Dosya_>();

            using (ZipArchive Arşiv = ZipFile.OpenRead(Dosya))
            {
                foreach (ZipArchiveEntry Biri in Arşiv.Entries)
                {
                    if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosya adedi sayılıyor" + " -> " + Dosyalar.Count + " -> " + D_DosyaBoyutu.Yazıya(DosyaListesi.Boyutu), Biri.Name)) break;
                    if (string.IsNullOrEmpty(Biri.Name)) continue; //klasör

                    if (Ayıkla && !FiltrelemeSonucundaKlasörüKullan(Path.GetDirectoryName(Biri.FullName), Talep.Filtreler_Klasör)) continue;
                    if (Ayıkla && !FiltrelemeSonucundaDosyayıKullan(Biri.FullName)) continue;

                    Bir_Dosya_ Yeni = new Bir_Dosya_();
                    Yeni.Boyutu = Biri.Length;
                    Yeni.DeğiştirilmeTarihi = Biri.LastWriteTime.DateTime;
                    Yeni.Yolu = Biri.FullName;

                    Dosyalar.Add(Yeni);
                    DosyaListesi.Boyutu += Yeni.Boyutu;
                }
            }

            DosyaListesi.Dosyalar = Dosyalar.ToArray();
            return DosyaListesi;
        }
        Bir_Dosya_Listesi_ DosyaListesiniOluştur_Karışık(string Kök, bool Ayıkla = true)
        {
            List<Bir_Dosya_> Dosyalar = new List<Bir_Dosya_>();
            Bir_Dosya_Listesi_ KökünListesi = DosyaListesiniOluştur(Kök, Ayıkla);
            Bir_Dosya_Listesi_ Çıktı = new Bir_Dosya_Listesi_();

            foreach (Bir_Dosya_ biri in KökünListesi.Dosyalar)
            {
                if (!biri.Yolu.Contains('\\') && biri.Yolu.EndsWith(".zip"))
                {
                    Bir_Dosya_Listesi_ zipli = DosyaListesiniOluştur_Zip(KökünListesi.Kök + biri.Yolu, Ayıkla);
                    string eklenecek_olan = Path.GetFileNameWithoutExtension(biri.Yolu) + Path.DirectorySeparatorChar;
                    foreach (Bir_Dosya_ zip_biri in zipli.Dosyalar) zip_biri.Yolu = eklenecek_olan + zip_biri.Yolu;
                    Dosyalar.AddRange(zipli.Dosyalar);
                    Çıktı.Boyutu += zipli.Boyutu;
                }
                else
                {
                    Dosyalar.Add(biri);
                    Çıktı.Boyutu += biri.Boyutu;
                }
            }

            Çıktı.Kök = KökünListesi.Kök;
            Çıktı.Dosyalar = Dosyalar.ToArray();
            return Çıktı;
        }

        bool FiltreKapsamındaMı(string Girdi, string Filtre)
        {
            if (Filtre == "*") return true;
            else if (Filtre == Girdi) return true;
            else if (Filtre.StartsWith("*") && Girdi.EndsWith(Filtre.Remove(0, 1))) return true;
            else if (Filtre.EndsWith("*") && Girdi.StartsWith(Filtre.Substring(0, Filtre.Length - 1))) return true;
            else return false;
        }
        bool FiltrelemeSonucundaKlasörüKullan(string Kaynak, string[][] Filtreler)
        {
            if (string.IsNullOrEmpty(Kaynak)) return true;

            string[] KaynaktakiKlasörler = Kaynak.Split('\\');

            foreach (var filtre in Filtreler)
            {
                for (int i = 0; i < filtre.Length; i++)
                {
                    if (i >= KaynaktakiKlasörler.Length) goto Diğerine_Geç;

                    if (FiltreKapsamındaMı(KaynaktakiKlasörler[i], filtre[i])) continue;
                    else goto Diğerine_Geç;
                }
                return false;

            Diğerine_Geç:
                continue;
            }

            return true;
        }
        bool FiltrelemeSonucundaDosyayıKullan(string Kaynak)
        {
            string isim = Path.GetFileNameWithoutExtension(Kaynak);
            string soyisim = Path.GetExtension(Kaynak).TrimStart('.');

            if (string.IsNullOrEmpty(isim) || string.IsNullOrEmpty(soyisim)) return true;

            foreach (var filtre in Talep.Filtreler_Dosya)
            {
                if (filtre[0].Contains('\\'))
                {
                    string[] YeniFiltre = Path.GetDirectoryName(filtre[0]).Trim('\\', ' ').Split('\\');
                    if (!FiltrelemeSonucundaKlasörüKullan(Path.GetDirectoryName(Kaynak), new string[][] { YeniFiltre } ))
                    {
                        if (filtre[0].Count(x => x == '\\') == Kaynak.Count(x => x == '\\'))
                        {
                            if (FiltreKapsamındaMı(isim, Path.GetFileNameWithoutExtension(filtre[0])))
                            {
                                if (FiltreKapsamındaMı(soyisim, filtre[1])) return false;
                            }
                        }
                    }
                }
                else
                {
                    if (FiltreKapsamındaMı(isim, filtre[0]) &&
                        FiltreKapsamındaMı(soyisim, filtre[1])) return false;
                }
            }
            return true;
        }
        #endregion

        #region İç Kullanım
        int YüzdeDeğeriniDeğerlendir_Sifrele(string Adı, long İşlenenBoyut, long ToplamBoyut)
        {
            İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + İşlenenBoyut;
            if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Karıştırılıyor")) Dçk.AcilDur();

            return 250;
        }
        int YüzdeDeğeriniDeğerlendir_SifresiniCoz(string Adı, long İşlenenBoyut, long ToplamBoyut)
        {
            İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + İşlenenBoyut;
            if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Düzeltiliyor")) Dçk.AcilDur();

            return 250;
        }
        bool Argemup_Dosyalama_Kopyala(string Kaynak, string Hedef)
        {
            FileStream Kaynak_FileStream = null, Hedef_FileStream = null;

            try
            {
                Kaynak_FileStream = new FileStream(Kaynak, FileMode.Open, FileAccess.Read);
                if (!Directory.Exists(Path.GetDirectoryName(Hedef))) Directory.CreateDirectory(Path.GetDirectoryName(Hedef));
                Hedef_FileStream = new FileStream(Hedef, FileMode.Create, FileAccess.Write);

                long KaynakOkunmuşAdet = 0;
                int KaynakOkunacakDosyaBoyutu;
                byte[] Tampon = new byte[4 * 1024];

                int Tick = Environment.TickCount + 1000;
                while (KaynakOkunmuşAdet < Kaynak_FileStream.Length)
                {
                    if (Kaynak_FileStream.Length - KaynakOkunmuşAdet > Tampon.Length) KaynakOkunacakDosyaBoyutu = Tampon.Length;
                    else KaynakOkunacakDosyaBoyutu = (int)Kaynak_FileStream.Length - (int)KaynakOkunmuşAdet;

                    Kaynak_FileStream.Read(Tampon, 0, KaynakOkunacakDosyaBoyutu);
                    Hedef_FileStream.Write(Tampon, 0, KaynakOkunacakDosyaBoyutu);

                    KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;

                    if (Environment.TickCount > Tick)
                    {
                        İşlenenDosyaBoyutu = SonGeçerliİşlenenDosyaBoyutu + KaynakOkunmuşAdet;
                        if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Kopyalanıyor")) break;

                        Hedef_FileStream.Flush(true);

                        Tick = Environment.TickCount + 250;
                    }
                }

                Kaynak_FileStream.Close();
                Hedef_FileStream.Close();

                File.SetCreationTime(Hedef, File.GetCreationTime(Kaynak));
                File.SetLastAccessTime(Hedef, File.GetLastAccessTime(Kaynak));
                File.SetLastWriteTime(Hedef, File.GetLastWriteTime(Kaynak));
                File.SetAccessControl(Hedef, File.GetAccessControl(Kaynak));
                File.SetAttributes(Hedef, File.GetAttributes(Kaynak));

                if (GörseliGüncelle_ÇıkışİsteniyorMu("Dosyalar Kopyalanıyor")) goto Cik;

                return true;
            }
            catch (Exception ex) { Yedekleyici.Ortak.Günlük_Ekle(ex.ToString()); }

        Cik:
            try { Kaynak_FileStream.Close(); } catch (Exception) { }
            try { Hedef_FileStream.Close(); } catch (Exception) { }
            try { Yedekleyici.Ortak.Sil.Dosya(Hedef); } catch (Exception) { }

            return false;
        }
        #endregion
    }
}
