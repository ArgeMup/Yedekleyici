// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Senaryo
{
    public enum Tür { Koşul, Komut };
    public enum AltTür { Zamanlama, Degisken, Klasor, Mesaj, Senaryo, Talep, Etiket, Uygulama };

    public class Değişken
    {
        static Mutex Mtx = new Mutex();
        const char Liste_AyırmaKarakteri = '|';
        static Dictionary<string, string> Tümü = new Dictionary<string, string>();
        public static bool UygunMu(string Adı)
        {
            return  Adı.StartsWith("<") && 
                    Adı.EndsWith(">") &&
                    Adı.Length > 2;
        }
        public static bool MevcutMu(string Adı)
        {
            return Tümü.ContainsKey(Adı);
        }
        public static void Yaz(string Adı, int Girdi)
        {
            Yaz(Adı, Girdi.ToString());
        }
        public static void Yaz(string Adı, string Girdi)
        {
            Adı = Adı.Trim(' ');

            if (Adı == "<SenaryoSonrakiAdimaGecmeSuresi>")
            {
                Ortak.SonrakiAdımaGeçmeSüresi_Msn = Convert.ToInt32(Girdi);

                return;
            }

            Mtx.WaitOne();
            Tümü[Adı.Trim(' ')] = Girdi;
            Mtx.ReleaseMutex();
        }
        static string Oku_Metin(string Adı)
        {
            Adı = Adı.Trim(' ');

            if (Adı == "<Saat>") return DateTime.Now.ToString("HH:mm:ss");
            else if (Adı == "<Tarih>") return DateTime.Now.ToString("dd.MM.yyyy");
            else if (Adı == "<UygulamaninKonumu>") return Yedekleyici.Ortak.pak;
            else if (Adı == "<UygulamaninAdi>") return Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            else if (Adı == "<BilgisayarAdi>") return Environment.MachineName;
            else if (Adı == "<KullaniciAdi>") return Environment.UserName;
            else if (Adı == "<SenaryoSonrakiAdimaGecmeSuresi>") return (Ortak.SonrakiAdımaGeçmeSüresi_Msn).ToString();
            else if (Adı.Contains("(") && Adı.Contains(")"))
            {
                string Paremetre = "";
                string AltParemetre = "";

                int bitiş = Adı.IndexOf(')');
                for (int i = bitiş; i >= 0; i--)
                {
                    if (Adı[i] == '(')
                    {
                        AltParemetre = Adı.Substring(i + 1, bitiş - i - 1);
                        Paremetre = Adı.Remove(i, bitiş - i + 1);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Paremetre) || string.IsNullOrEmpty(AltParemetre)) throw new Exception(Adı + " isimli değişken bulunamadı");

                if (Paremetre == "<CpuYuzdesi>")
                {
                    if (AltParemetre == "Bilgisayar") return Yedekleyici.Ortak.CpuYüzdesi().ToString();

                    return Yedekleyici.Ortak.CpuYüzdesi(AltParemetre).ToString();
                }
                else if (Paremetre == "<HddYuzdesi>")
                {
                    if (AltParemetre == "Bilgisayar") return Yedekleyici.Ortak.HddYüzdesi().ToString();
                }
                else if (Paremetre == "<KlasorMevcutMu>")
                {
                    return Directory.Exists(AltParemetre.Trim('\\', ' ')) ? "Evet" : "Hayir";
                }
                else if (Paremetre == "<SenaryodaCalisanAdimSayisi>")
                {
                    Bir_Senaryo_ sny = Ortak.Senaryo_Bul(AltParemetre);
                    return sny != null ? sny.ÇalışanGörevAdedi.ToString() : "";
                }
                else if (Paremetre == "<ListedekiElemanSayisi>")
                {
                    return AltParemetre.Split(Liste_AyırmaKarakteri).Length.ToString();
                }
                else if (Paremetre == "<ListedekiEleman>")
                {
                    string[] Liste = AltParemetre.Split(Liste_AyırmaKarakteri);
                    int SıraNo = Convert.ToInt32(Liste[Liste.Length-1]);
                    if (SıraNo >= Liste.Length - 1) throw new Exception("SıraNo : " + SıraNo + " uygun değil");
                    return Liste[SıraNo];
                }
                else if (Paremetre == "<KlasorleriListele>")
                {
                    if (!Directory.Exists(AltParemetre)) throw new Exception("Yola ulaşılamıyor : " + AltParemetre);
                    string[] liste = Yedekleyici.Ortak.Listele.Klasör(AltParemetre, SearchOption.TopDirectoryOnly);
                    
                    string Çıktı = "";
                    foreach (string biri in liste) { Çıktı += Directory.GetParent(biri + Path.DirectorySeparatorChar).Name + Liste_AyırmaKarakteri; }

                    return Çıktı.TrimEnd(Liste_AyırmaKarakteri);
                }
                else if (Paremetre == "<DosyalariListele>")
                {
                    if (!Directory.Exists(AltParemetre)) throw new Exception("Yola ulaşılamıyor : " + AltParemetre);
                    string[] liste = Yedekleyici.Ortak.Listele.Dosya(AltParemetre, SearchOption.TopDirectoryOnly);

                    string Çıktı = "";
                    foreach (string biri in liste) { Çıktı += Path.GetFileName(biri) + Liste_AyırmaKarakteri; }

                    return Çıktı.TrimEnd(Liste_AyırmaKarakteri);
                }
                else if (Paremetre == "<RastgeleMetin>")
                {
                    int Adet = Convert.ToInt32(AltParemetre);
                    return ArgeMup.HazirKod.Dönüştürme.D_GeriDönülemezKarmaşıklaştırmaMetodu.Metinden(Path.GetRandomFileName(), Adet);
                }
                else if (Paremetre == "<KullaniciSecimi>")
                {
                    string[] dizi = AltParemetre.Split('|');

                    string Tür = dizi[0].Trim();
                    MessageBoxButtons btn = MessageBoxButtons.OK; MessageBoxDefaultButton var_btn = MessageBoxDefaultButton.Button1;
                    if (Tür == "TamamIptal") { btn = MessageBoxButtons.OKCancel; var_btn = MessageBoxDefaultButton.Button2; }
                    else if (Tür == "YenidendeneIptal") { btn = MessageBoxButtons.RetryCancel; var_btn = MessageBoxDefaultButton.Button2; }
                    else if (Tür == "EvetHayir") { btn = MessageBoxButtons.YesNo; var_btn = MessageBoxDefaultButton.Button2; }
                    else if (Tür == "EvetHayirIptal") { btn = MessageBoxButtons.YesNoCancel; var_btn = MessageBoxDefaultButton.Button3; }
                    else if (Tür == "DurdurYenidendeneYoksay") { btn = MessageBoxButtons.AbortRetryIgnore; var_btn = MessageBoxDefaultButton.Button3; }

                    //Süreli kapatma imkanı
                    //var w = new Form() { Size = new System.Drawing.Size(0, 0) };
                    //System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5))
                    //    .ContinueWith((t) => w.Close(), System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                    //MessageBox.Show(w, dizi[1], Application.ProductName, btn, MessageBoxIcon.Question, var_btn);

                    switch (MessageBox.Show(dizi[1], Application.ProductName, btn, MessageBoxIcon.Question, var_btn, MessageBoxOptions.ServiceNotification))
                    {
                        case DialogResult.Abort: return "Durdur";
                        case DialogResult.Cancel: return "Iptal";
                        case DialogResult.Ignore: return "Yoksay";
                        case DialogResult.No: return "Hayir";
                        case DialogResult.None: return "Hicbiri";
                        case DialogResult.OK: return "Tamam";
                        case DialogResult.Retry: return "Yenidendene";
                        case DialogResult.Yes: return "Evet";
                    }
                }
                else if (Paremetre == "<InternetPing>")
                {
                    try
                    {
                        System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
                        System.Net.NetworkInformation.PingReply reply = myPing.Send(AltParemetre, 3500);
                        return (reply.Status == System.Net.NetworkInformation.IPStatus.Success) ? "Evet" : "Hayir";
                    }
                    catch (Exception) { return "Hayir"; }
                }
            }

            Mtx.WaitOne();
            string girdi;
            bool sonuç = Tümü.TryGetValue(Adı, out girdi);
            Mtx.ReleaseMutex();

            if (sonuç) return girdi;

            throw new Exception(Adı + " isimli değişken bulunamadı");
        }
        public static string Oku_MetinVeyaDeğişken(string Girdi)
        {
            while (Girdi.Contains("<") && Girdi.Contains(">"))
            {
                int buyuktur = Girdi.IndexOf('>');
                for (int i = buyuktur; i >= 0; i--)
                {
                    if (Girdi[i] == '<')
                    {
                        string değişken = Girdi.Substring(i, buyuktur - i + 1);
                        değişken = Oku_Metin(değişken);
                        Girdi = Girdi.Remove(i, buyuktur - i + 1);
                        Girdi = Girdi.Insert(i, değişken);
                        break;
                    }
                }
            }

            return Girdi;
        }
        public static int Oku_SayiVeyaDeğişken(string Girdi)
        {
            int sayı_olarak;
            string metin_olarak = Oku_MetinVeyaDeğişken(Girdi);

            if (int.TryParse(metin_olarak, out sayı_olarak)) return sayı_olarak;

            throw new Exception(Girdi + " bilgisi sayı olarak kullanılamıyor");
        }
    }

    public class Bir_Adım_ 
    {
        public Bir_Senaryo_ Senaryo;

        public string Açıklama = "";
        public int Seviye, SatırNo;
        public bool Etkin;
        public Tür Tür;
        public AltTür AltTür;
        public string[] İçerik;
        public List<Bir_Adım_> Adımlar = new List<Bir_Adım_>();

        public TreeNode Dal;
        public Thread Görev;
        string Gidilecek_Etiket = "";

        public Bir_Adım_(string Metin, Bir_Senaryo_ Senaryo, int SatırNo)
        {
            this.Senaryo = Senaryo;
            this.SatırNo = SatırNo;

            string[] TekTek = Metin.Split(';');
            if (TekTek.Length < 4) { Seviye = -1; return; }

            for (int i = 0; i < TekTek.Length; i++) TekTek[i] = TekTek[i].Trim();

            İçerik = new string[TekTek.Length - 4];
            for (int i = 0; i < İçerik.Length; i++) İçerik[i] = TekTek[i + 4];

            Etkin = TekTek[1].Trim() == "E";
            Seviye = TekTek[0].Length;

            switch (TekTek[2])
            {
                default: { Seviye = -2; return; }

                case "Kosul": Tür = Tür.Koşul; break;
                case "Komut": Tür = Tür.Komut; break;
            }

            switch (TekTek[3])
            {
                default: { Seviye = -3; return; }

                case "Senaryo": AltTür = AltTür.Senaryo; break;
                case "Zamanlama": AltTür = AltTür.Zamanlama; break;
                case "Degisken": AltTür = AltTür.Degisken; break;
                case "Talep": AltTür = AltTür.Talep; break;
                case "Etiket": AltTür = AltTür.Etiket; break;
                case "Klasor": AltTür = AltTür.Klasor; break;
                case "Mesaj": AltTür = AltTür.Mesaj; break;
                case "Uygulama": AltTür = AltTür.Uygulama; break;
            }

            if (!DenetleVeAçıklamayıÜret(Senaryo)) { Seviye = -4; return; }
            return;
        }

        bool DenetleVeAçıklamayıÜret(Bir_Senaryo_ SenaryoMlz)
        {
            try
            {
                if (Tür == Tür.Koşul)
                {
                    if (AltTür == AltTür.Zamanlama)
                    {
                        if (İçerik.Length == 4)
                        {
                            if (İçerik[0] == "GunSaat")
                            {
                                Açıklama = AçıklamayıÜret_GünSaat(ref İçerik[1]);
                                if (Açıklama == "") return false;

                                goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Degisken)
                    {
                        if (İçerik.Length == 5)
                        {
                            string d1 = AçıklamayıÜret_İçerikVeyaDeğişken(İçerik[0]);
                            string d2 = AçıklamayıÜret_İçerikVeyaDeğişken(İçerik[2]);

                            if (!string.IsNullOrEmpty(d1) && !string.IsNullOrEmpty(d2))
                            {
                                if (İçerik[1] == "Esit")
                                {
                                    Açıklama = d1 + " eşittir " + d2;
                                    goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                                }
                                else if (İçerik[1] == "Kucuk")
                                {
                                    Açıklama = d1 + " küçüktür " + d2;
                                    goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                                }
                                else if (İçerik[1] == "Buyuk")
                                {
                                    Açıklama = d1 + " büyüktür " + d2;
                                    goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                                }
                                else if (İçerik[1] == "Ayni")
                                {
                                    Açıklama = d1 + " ile " + d2 + " aynı";
                                    goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                                }
                                else if (İçerik[1] == "Icerir")
                                {
                                    Açıklama = d1 + " bilgisi " + d2 + " bilgisini içeriyor";
                                    goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                                }
                            }
                        }
                        else if(İçerik.Length == 4)
                        {
                            string d1 = AçıklamayıÜret_İçerikVeyaDeğişken(İçerik[0]);
                           
                            if (İçerik[1] == "MevcutMu")
                            {
                                Açıklama = d1 + " değişkeni tanımlandı";
                                goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Klasor)
                    {
                        if (İçerik.Length == 4)
                        {
                            if (İçerik[0] == "Mevcut")
                            {
                                Açıklama = İçerik[1] + " yoluna erişilebiliyor";
                                goto Kosullar_İçin_Ek_Açıklamayı_Ekle;
                            }
                        }
                    }
                }
                else if (Tür == Tür.Komut)
                {
                    if (AltTür == AltTür.Talep)
                    {
                        if (İçerik.Length == 1)
                        {
                            Açıklama = "Talebi çalıştır -> " + İçerik[0];

                            return true;
                        }
                    }
                    else if (AltTür == AltTür.Etiket)
                    {
                        if (İçerik.Length == 2)
                        {
                            if (İçerik[0] == "Ata" || İçerik[0] == "AtaVeDur")
                            {
                                SenaryoMlz.Etiketler[İçerik[1]] = this;
                                
                                Açıklama = "Etiket -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Git")
                            {
                                Açıklama = "Etikete git -> " + İçerik[1];

                                return true;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Senaryo)
                    {
                        if (İçerik.Length == 2)
                        {
                            if (İçerik[0] == "Calistir")
                            {
                                Açıklama = "Senaryoyu çalıştır -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Durdur")
                            {
                                Açıklama = "Senaryoyu durdur -> " + İçerik[1];

                                return true;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Mesaj)
                    {
                        if (İçerik.Length == 2)
                        {
                            if (İçerik[0] == "Uyari")
                            {
                                Açıklama = "Uyarı mesajı göster -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Hata")
                            {
                                Açıklama = "Hata mesajı göster -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Tepsi")
                            {
                                Açıklama = "Tepside döner mesaj göster -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Gunluk")
                            {
                                Açıklama = "Günlüğe ekle -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Ses")
                            {
                                Açıklama = "Sesi oynat -> " + İçerik[1];

                                return true;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Zamanlama)
                    {
                        if (İçerik.Length == 3)
                        {
                            if (İçerik[0] == "Bekle")
                            {
                                int süre;
                                if (!İçerik[2].StartsWith("<")) süre = int.Parse(İçerik[2]);

                                Açıklama = AçıklamayıÜret_İçerikVeyaDeğişken(İçerik[2]) + " " + İçerik[1] + " boyunca bekle";

                                return true;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Degisken)
                    {
                        if (İçerik.Length == 3)
                        {
                            if (İçerik[0] == "Arttir")
                            {
                                Açıklama = İçerik[1] + " değişkenini " + İçerik[2] + " kadar arttır";

                                return true;
                            }
                            else if (İçerik[0] == "Azalt")
                            {
                                Açıklama = İçerik[1] + " değişkenini " + İçerik[2] + " kadar azalt";

                                return true;
                            }
                            else if (İçerik[0] == "Kopyala")
                            {
                                Açıklama = İçerik[2] + " -> Kopyala -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "IceriginiKopyala")
                            {
                                Açıklama = İçerik[2] + " -> İçeriğini kopyala -> " + İçerik[1];

                                return true;
                            }
                        }
                    }
                    else if (AltTür == AltTür.Uygulama)
                    {
                        if (İçerik.Length == 6)
                        {
                            if (İçerik[0] == "Calistir")
                            {
                                Açıklama = "Uygulamayı çalıştır -> " + İçerik[1];

                                return true;
                            }
                        }
                        else if (İçerik.Length == 2)
                        {
                            if (İçerik[0] == "Durdur")
                            {
                                Açıklama = "Uygulamayı durdur -> " + İçerik[1];

                                return true;
                            }
                            else if (İçerik[0] == "Yoket")
                            {
                                Açıklama = "Uygulamayı yoket -> " + İçerik[1];

                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Yedekleyici.Ortak.Günlük_Ekle(ex.ToString()); }

            return false;


            Kosullar_İçin_Ek_Açıklamayı_Ekle:

            string KoşulUygun = "alt dala";
            string KoşulZıt = "alt dala";

            if (!string.IsNullOrEmpty(İçerik[İçerik.Length - 2])) KoşulUygun = İçerik[İçerik.Length - 2];
            if (!string.IsNullOrEmpty(İçerik[İçerik.Length - 1])) KoşulZıt = İçerik[İçerik.Length - 1];

            if (KoşulUygun == KoşulZıt) Açıklama = "Her durumda " + KoşulUygun + " [" + Açıklama + "]";
            else
            {
                Açıklama += " ise " + KoşulUygun +
                            " veya tersi durumda " + KoşulZıt;
            }

            return true;
        }
        string AçıklamayıÜret_GünSaat(ref string Girdi)
        {
            string normalleştirilmiş = "";
            string Çıktı = "";
            string[] GünSaat = Girdi.Split(',');
            if (GünSaat.Length != 2) return "";

            while (GünSaat[0].Length < 7) GünSaat[0] += "0";
            if (GünSaat[0][6] == '1') { Çıktı += ", Paz"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][0] == '1') { Çıktı += ", Pzt"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][1] == '1') { Çıktı += ", Sal"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][2] == '1') { Çıktı += ", Çrş"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][3] == '1') { Çıktı += ", Per"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][4] == '1') { Çıktı += ", Cum"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;
            if (GünSaat[0][5] == '1') { Çıktı += ", Cts"; normalleştirilmiş = "1" + normalleştirilmiş; }
            else normalleştirilmiş = "0" + normalleştirilmiş;

            string[] Saatler = GünSaat[1].Split('-');
            TimeSpan başlangıç = TimeSpan.Parse(Saatler[0]); string bas = başlangıç.Hours.ToString("D2") + ":" + başlangıç.Minutes.ToString("D2");
            TimeSpan bitiş = TimeSpan.Parse(Saatler[1]); string bit = bitiş.Hours.ToString("D2") + ":" + bitiş.Minutes.ToString("D2");
            if (başlangıç >= bitiş) return "";

            Girdi = normalleştirilmiş + "," + bas + "-" + bit;
            return "Bugün " + Çıktı.Trim(',', ' ') + " ise ve saat " + bas + " ile " + bit + " arasında";
        }
        string AçıklamayıÜret_İçerikVeyaDeğişken(string Girdi)
        {
            return "\"" + Girdi + "\""; //Metin
        }

        public void İşlem_()
        {
            Interlocked.Increment(ref Senaryo.ÇalışanGörevAdedi);
             
            try
            {
            YenidenDene:
                Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Çalışıyor);
                bool sonuç = true;
                Gidilecek_Etiket = "";
                int zaman_damgası = Environment.TickCount;

                if (Tür == Tür.Koşul) sonuç = İşlem_Koşul();
                else İşlem_Komut();

                zaman_damgası = Environment.TickCount - zaman_damgası;
                if (!Senaryo.Durdurmaİsteği && zaman_damgası < Ortak.SonrakiAdımaGeçmeSüresi_Msn) Thread.Sleep(Ortak.SonrakiAdımaGeçmeSüresi_Msn - zaman_damgası);

                if (Senaryo.Durdurmaİsteği) Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Hata, "Senaryo durduruldu " + DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"));
                else if (Tür == Tür.Komut) Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Tamam, DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"));
                else
                {
                    if (sonuç) Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Tamam, DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"));
                    else Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Tamam_Hayır, "Koşul uygun değil " + DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"));
                }
                  
                if (!Senaryo.Durdurmaİsteği)
                {
                    if (Gidilecek_Etiket == "")
                    {
                        for (int i = 0; i < Adımlar.Count && !Senaryo.Durdurmaİsteği; i++)
                        {
                            Ortak.Adım_Başlat(Adımlar[i]);
                        }
                    }
                    else if (Gidilecek_Etiket == "Dur") { }
                    else if (Gidilecek_Etiket.StartsWith("Tekrarla"))
                    {
                        Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Çalışıyor);
                        DateTime Süre;

                        if (!Gidilecek_Etiket.Contains(",")) Süre = DateTime.Now.AddMilliseconds(Ortak.SonrakiAdımaGeçmeSüresi_Msn);
                        else Süre = DateTime.Now.AddSeconds(Değişken.Oku_SayiVeyaDeğişken(Gidilecek_Etiket.Split(',')[1]));

                        İşlem_Bekle(Süre);

                        goto YenidenDene;
                    }
                    else
                    {
                        Bir_Adım_ Adm;
                        if (Senaryo.Etiketler.TryGetValue(Gidilecek_Etiket, out Adm))
                        {
                            Ortak.Adım_Başlat(Adm);
                        }
                        else throw new Exception(Gidilecek_Etiket + " isimli etiket bulunamadı");
                    }
                }
            }
            catch (Exception ex) 
            { 
                Yedekleyici.Ortak.Günlük_Ekle(ex.Message.ToString());
                Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Hata, ex.Message + " " + DateTime.Now.ToString("d MMMM ddd HH:mm:ss:fff"));

                if (Değişken.UygunMu(Senaryo.BeklenmeyenDurumSatırNonunKopyalanacağıDeğişkeninAdı))
                {
                    Değişken.Yaz(Senaryo.BeklenmeyenDurumSatırNonunKopyalanacağıDeğişkeninAdı, SatırNo);
                }

                Bir_Adım_ Adm;
                if (Senaryo.Etiketler.TryGetValue(Senaryo.BeklenmeyenDurumEtiketi, out Adm))
                {
                    Ortak.Adım_Başlat(Adm);
                }
            }

            Görev = null;

            Interlocked.Decrement(ref Senaryo.ÇalışanGörevAdedi);
        }
        bool İşlem_Koşul()
        {
            bool SonuçUygun = false;

            if (AltTür == AltTür.Zamanlama)
            {
                if (İçerik[0] == "GunSaat")
                {
                    string[] Dizi = İçerik[1].Split(',');

                    int kullanıcı = Convert.ToInt32(Dizi[0], 2);
                    int bugün = 1 << (int)DateTime.Now.DayOfWeek;
                    bugün &= kullanıcı;

                    if (bugün > 0)
                    {
                        Dizi = Dizi[1].Split('-');
                        TimeSpan başlangıç = TimeSpan.Parse(Dizi[0]);
                        TimeSpan bitiş = TimeSpan.Parse(Dizi[1]);
                        TimeSpan şimdi = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));

                        SonuçUygun = (başlangıç <= şimdi && şimdi <= bitiş);
                    }
                }
            }
            else if (AltTür == AltTür.Degisken)
            {
                if (İçerik[1] == "Ayni" || İçerik[1] == "Icerir")
                {
                    string d1 = Değişken.Oku_MetinVeyaDeğişken(İçerik[0]);
                    string d2 = Değişken.Oku_MetinVeyaDeğişken(İçerik[2]);

                    if (İçerik[1] == "Ayni")
                    {
                        SonuçUygun = (d1 == d2);
                    }
                    else if (İçerik[1] == "Icerir")
                    {
                        SonuçUygun = (d1.Contains(d2));
                    }
                }
                else if (İçerik[1] == "MevcutMu") SonuçUygun = Değişken.MevcutMu(İçerik[0]);
                else
                {
                    int d1 = Değişken.Oku_SayiVeyaDeğişken(İçerik[0]);
                    int d2 = Değişken.Oku_SayiVeyaDeğişken(İçerik[2]);

                    if (İçerik[1] == "Esit")
                    {
                        SonuçUygun = (d1 == d2);
                    }
                    else if (İçerik[1] == "Kucuk")
                    {
                        SonuçUygun = (d1 < d2);
                    }
                    else if (İçerik[1] == "Buyuk")
                    {
                        SonuçUygun = (d1 > d2);
                    }
                } 
            }
            else if (AltTür == AltTür.Klasor)
            {
                if (İçerik[0] == "Mevcut")
                {
                    SonuçUygun = Directory.Exists(Değişken.Oku_MetinVeyaDeğişken(İçerik[1]));
                }
            }
            
            Gidilecek_Etiket = Değişken.Oku_MetinVeyaDeğişken(İçerik[İçerik.Length - (SonuçUygun ? 2 : 1)]);

            return SonuçUygun;
        }
        void İşlem_Komut()
        {
            if (AltTür == AltTür.Talep)
            {
                Talep.Bir_Talep_ T = Talep.Ortak.Aç(Değişken.Oku_MetinVeyaDeğişken(İçerik[0]));
                if (T == null) throw new Exception(İçerik[0] + " isimli talep açılamadı");
                else if (!string.IsNullOrEmpty(T.Hata)) throw new Exception(İçerik[0] + " -> " + T.Hata);

                Talep.İşlem_ i = new Talep.İşlem_();
                i.Senaryo = Senaryo;
                i.Talep = T;
                i.Çalıştır();
            }
            else if (AltTür == AltTür.Etiket)
            {
                if (İçerik[0] == "Git")
                {
                    Gidilecek_Etiket = Değişken.Oku_MetinVeyaDeğişken(İçerik[1]);
                    return;
                }
            }
            else if (AltTür == AltTür.Senaryo)
            {
                string adı = Değişken.Oku_MetinVeyaDeğişken(İçerik[1]);
                Bir_Senaryo_ sny = Ortak.Senaryo_Bul(adı);
                if (sny == null) throw new Exception(İçerik[1] + " isimli senaryo bulunamadı");

                if (İçerik[0] == "Calistir")
                {
                    Ortak.Senaryo_Başlat(adı);
                }
                else if (İçerik[0] == "Durdur")
                {
                    Ortak.Senaryo_Durdur(adı);
                }
            }
            else if (AltTür == AltTür.Mesaj)
            {
                if (İçerik[0] == "Uyari")
                {
                    Yedekleyici.Ortak.PeTeİkKo.MetniBaloncuktaGöster(Değişken.Oku_MetinVeyaDeğişken(İçerik[1]), ToolTipIcon.Warning);
                }
                else if (İçerik[0] == "Hata")
                {
                    Yedekleyici.Ortak.PeTeİkKo.MetniBaloncuktaGöster(Değişken.Oku_MetinVeyaDeğişken(İçerik[1]), ToolTipIcon.Error);
                }
                else if (İçerik[0] == "Tepsi")
                {
                    Yedekleyici.Ortak.PeTeİkKo.MetniTepsiİkonundaGöster(Değişken.Oku_MetinVeyaDeğişken(İçerik[1]));
                }
                else if (İçerik[0] == "Gunluk")
                {
                    Yedekleyici.Ortak.Günlük_Ekle(Değişken.Oku_MetinVeyaDeğişken(İçerik[1]), "Komut");
                }
                else if (İçerik[0] == "Ses")
                {
                    switch (İçerik[1])
                    {
                        case "Melodi1": System.Media.SystemSounds.Hand.Play(); break;
                        case "Melodi2": System.Media.SystemSounds.Asterisk.Play(); break;
                        default: if (File.Exists(İçerik[1])) new System.Media.SoundPlayer(İçerik[1]).Play(); break;
                    }
                }
            }
            else if (AltTür == AltTür.Zamanlama)
            {
                if (İçerik[0] == "Bekle")
                {
                    DateTime süre = DateTime.Now;
                    int miktar = Değişken.Oku_SayiVeyaDeğişken(İçerik[2]);
                    if (İçerik[1] == "Saniye") süre = süre.AddSeconds(miktar);
                    else if (İçerik[1] == "Dakika") süre = süre.AddMinutes(miktar);
                    else if (İçerik[1] == "Saat") süre = süre.AddHours(miktar);
                    else if (İçerik[1] == "Gun") süre = süre.AddDays(miktar);

                    İşlem_Bekle(süre);
                }
            }
            else if (AltTür == AltTür.Degisken)
            {
                if (İçerik[0] == "Arttir")
                {
                    int değer = Değişken.Oku_SayiVeyaDeğişken(İçerik[2]);
                    değer = Değişken.Oku_SayiVeyaDeğişken(İçerik[1]) + değer;
                    Değişken.Yaz(İçerik[1], değer);
                }
                else if (İçerik[0] == "Azalt")
                {
                    int değer = Değişken.Oku_SayiVeyaDeğişken(İçerik[2]);
                    değer = Değişken.Oku_SayiVeyaDeğişken(İçerik[1]) - değer;
                    Değişken.Yaz(İçerik[1], değer);
                }
                else if (İçerik[0] == "Kopyala")
                {
                    Değişken.Yaz(İçerik[1], İçerik[2]);
                }
                else if (İçerik[0] == "IceriginiKopyala")
                {
                    Değişken.Yaz(İçerik[1], Değişken.Oku_MetinVeyaDeğişken(İçerik[2]));
                }
            }
            else if (AltTür == AltTür.Uygulama)
            {
                string UygulamaYolu = Değişken.Oku_MetinVeyaDeğişken(İçerik[1]);

                if (İçerik[0] == "Calistir")
                {
                    System.Diagnostics.Process[] uyglm = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(UygulamaYolu));
                    if (uyglm.Length > 0) Yedekleyici.Ortak.Günlük_Ekle(UygulamaYolu + " belirtilen uygulama zaten çalıştığı için atlandı.", "Bilgi");
                    else
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = Path.GetFileName(UygulamaYolu);
                        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(UygulamaYolu);
                        process.StartInfo.Arguments = İçerik[3];
                        if (İçerik[4] != "") process.StartInfo.Verb = İçerik[4];

                        if (İçerik[2] == "TamEkran") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                        else if (İçerik[2] == "Kucultulmus") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                        else if (İçerik[2] == "Gizli") process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        else process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                        if (İçerik[5] == "DontUseShellExecute") process.StartInfo.UseShellExecute = false;
                        process.Start();
                    }
                }
                else if (İçerik[0] == "Durdur")
                {
                    if (UygulamaYolu == "Kendi")
                    {
                        Yedekleyici.Ortak.UygulamayıKapatmaSayacı = 5000;
                    }
                    else
                    {
                        System.Diagnostics.Process[] uyglm = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(UygulamaYolu));
                        foreach (System.Diagnostics.Process p in uyglm)
                        {
                            if (!p.HasExited) p.CloseMainWindow();
                        }
                    }
                }
                else if (İçerik[0] == "Yoket")
                {
                    System.Diagnostics.Process[] uyglm = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(UygulamaYolu));
                    foreach (System.Diagnostics.Process p in uyglm)
                    {
                        if (!p.HasExited) p.Kill();
                    }
                }
            }
        }
        void İşlem_Bekle(DateTime Süre)
        {
            if (Dal != null) Dal.ToolTipText = Süre.ToString("d MMMM ddd HH:mm:ss:fff") + " anına kadar kilitli";

            int fark = (int)(Süre - DateTime.Now).TotalMilliseconds;
            
            if (fark >= 5000)
            {
                while (!Senaryo.Durdurmaİsteği && Süre > DateTime.Now)
                {
                    Thread.Sleep(5000);
                }
            }
            else Thread.Sleep(fark);
        }
    }

    public class Bir_Senaryo_
    {
        public string DosyaYolu;
        public List<Bir_Adım_> Adımlar = new List<Bir_Adım_>();
        public Dictionary<string, Bir_Adım_> Etiketler = new Dictionary<string, Bir_Adım_>();
        public TreeNode Ağaç = null;
        public bool Durdurmaİsteği = false;
        public int ÇalışanGörevAdedi = 0;
        public int TekrarÇalıştırmaZamanAralığı_msn = 0, TekrarÇalıştırmaZamanAralığı_msn_sayac = 0;
        public string Tanim = "", Hata = "", BeklenmeyenDurumEtiketi = "", BeklenmeyenDurumSatırNonunKopyalanacağıDeğişkeninAdı = "";

        public Bir_Senaryo_(string DosyaYolu)
        {
            this.DosyaYolu = DosyaYolu;

            if (!File.Exists(DosyaYolu)) { Hata = "Dosya bulunamadı";  return; }

            Bir_Senaryo_İlkAdım(File.ReadAllLines(DosyaYolu));
        }
        public Bir_Senaryo_(string[] İçerik)
        {
            Bir_Senaryo_İlkAdım(İçerik);
        }
        void Bir_Senaryo_İlkAdım(string[] İçerik)
        {
            if (İçerik == null || İçerik.Length == 0) { Hata += "\r\nİçerik boş"; return; }

            for (int i = 0; i < İçerik.Length; i++)
            {
                İçerik[i] = İçerik[i].Split('#')[0].Trim();
                if (string.IsNullOrEmpty(İçerik[i])) continue;

                if (İçerik[i].StartsWith("Tanim;"))
                {
                    Tanim = İçerik[i].Substring(İçerik[i].IndexOf(';')+1).Trim();
                }
                else if (İçerik[i].StartsWith("BeklenmeyenDurumEtiketi;"))
                {
                    string[] dizi = İçerik[i].Split(';');
                    if (dizi.Length != 3) Hata += "\r\nSatır " + (i + 1) + " hatalı";
                    else
                    {
                        BeklenmeyenDurumEtiketi = dizi[1].Trim();
                        BeklenmeyenDurumSatırNonunKopyalanacağıDeğişkeninAdı = dizi[2].Trim();
                    } 
                }
                else if (İçerik[i].StartsWith("TekrarCalistirmaZamanAraligi;"))
                {
                    string g = İçerik[i].Substring(İçerik[i].IndexOf(';') + 1).Trim();
                    if (!int.TryParse(g, out int ç)) Hata += "\r\nSatır " + (i + 1) + " hatalı";
                    else TekrarÇalıştırmaZamanAralığı_msn = (int)TimeSpan.FromSeconds(ç).TotalMilliseconds;
                }
                else
                {
                    Bir_Adım_ Adm = new Bir_Adım_(İçerik[i], this, i+1);
                    if (Adm.Seviye > 0) Adımlar.Add(Adm);
                    else Hata += "\r\nSatır " + (i + 1) + " hatalı";
                }
            }

            if (Tanim == "") Hata += "\r\nTanim boş olamaz";

            bool SeviyeHatası = false;
        YenidenDene:
            if (Adımlar.Count > 1)
            {
                for (int i = Adımlar.Count - 1; i > 0; i--)
                {
                    if (Adımlar[i].Seviye > Adımlar[i - 1].Seviye)
                    {
                        if (Adımlar[i].Seviye - 1 != Adımlar[i - 1].Seviye) SeviyeHatası = true;
                        Adımlar[i - 1].Adımlar.Add(Adımlar[i]);
                        Adımlar.RemoveAt(i);
                        goto YenidenDene;
                    }
                }
            }

            if (SeviyeHatası) Hata += "\r\nSenaryodaki seviyeler uygun değil";
            Hata = Hata.Trim('\r', '\n', ' ');
        }
    }

    public class GörselÖge 
    {
        public static TreeNode Oluştur(Bir_Senaryo_ Senaryo)
        {
            TreeNode[] Dizi = new TreeNode[Senaryo.Adımlar.Count];

            for (int i = 0; i < Dizi.Length; i++)
            {
                Dizi[i] = new TreeNode();
                Oluştur_AltDal(Dizi[i], Senaryo.Adımlar[i]);
            }

            Senaryo.Ağaç = new TreeNode(Senaryo.Tanim, Dizi);
            Senaryo.Ağaç.Checked = true;
            Yedekleyici.Ortak.AğaçGörseliniGüncelle(Senaryo.Ağaç, (int)Yedekleyici.Ortak.Resimler.Yeni);
            Senaryo.Ağaç.Expand();
            return Senaryo.Ağaç;
        }
        private static void Oluştur_AltDal(TreeNode Dal, Bir_Adım_ Adım)
        {
            Dal.Text = Adım.Açıklama;
            Dal.Checked = Adım.Etkin;
            Dal.Tag = Adım;
            Yedekleyici.Ortak.AğaçGörseliniGüncelle(Dal, (int)Yedekleyici.Ortak.Resimler.Yeni);
            Dal.Expand();
            Adım.Dal = Dal;

            foreach (var Birisi in Adım.Adımlar)
            {
                TreeNode Yeni = new TreeNode();
                Dal.Nodes.Add(Yeni);

                Oluştur_AltDal(Yeni, Birisi);
            }
        }
    }

    public class Ortak
    {
        public static int SonrakiAdımaGeçmeSüresi_Msn = 500;
        public static List<Bir_Senaryo_> Senaryo_Listesi = new List<Bir_Senaryo_>();
        
        public static void Senaryo_Başlat(string Tanım)
        {
            List<Bir_Senaryo_> snyler = Senaryo_Listesi.FindAll(x => x.Tanim == Tanım);
            foreach (var sny in snyler) Senaryo_Başlat(sny);
        }
        public static void Senaryo_Başlat(Bir_Senaryo_ sny)
        {
            if (string.IsNullOrEmpty(sny.Hata))
            {
                sny.Durdurmaİsteği = false;
                foreach (Bir_Adım_ a in sny.Adımlar)
                {
                    if (a.Tür == Tür.Komut && a.AltTür == AltTür.Etiket && a.İçerik[0] == "AtaVeDur") continue;

                    Adım_Başlat(a);
                }

                string ipucu = DateTime.Now.AddMilliseconds(sny.TekrarÇalıştırmaZamanAralığı_msn).ToString("d MMMM ddd HH:mm:ss:fff");
                if (sny.TekrarÇalıştırmaZamanAralığı_msn > 0) ipucu = "Tekrar çalıştırılacağı an : " + ipucu;

                Yedekleyici.Ortak.AğaçGörseliniGüncelle(sny.Ağaç, (int)Yedekleyici.Ortak.Resimler.Yeni, ipucu);
            }
            else Yedekleyici.Ortak.AğaçGörseliniGüncelle(sny.Ağaç, (int)Yedekleyici.Ortak.Resimler.Hata, sny.Hata);
        }
        public static void Senaryo_Durdur(string Tanım)
        {
            foreach (var s in Senaryo_Listesi)
            {
                if (s.Tanim == Tanım)
                {
                    s.Durdurmaİsteği = true;
                    break;
                }
            }
        }
        public static Bir_Senaryo_ Senaryo_Bul(string Tanım)
        {
            foreach (var s in Senaryo_Listesi)
            {
                if (s.Tanim == Tanım) return s;
            }

            return null;
        }

        public static void Adım_Başlat(Bir_Adım_ A)
        {
            if (A.Etkin && A.Görev == null)
            {
                A.Görev = new Thread(() => A.İşlem_());
                A.Görev.Start();
            }
        }

        public static void BilinenleriAç(TreeView tv)
        {
            string[] SenaryoYolları = Yedekleyici.Ortak.Listele.Dosya(Yedekleyici.Ortak.pak_Banka, SearchOption.AllDirectories, "*.Yedekleyici_Senaryo");

            List<Bir_Senaryo_> SnryLsts = new List<Bir_Senaryo_>();
            tv.Nodes.Clear();
            tv.SuspendLayout();
            for (int i = 0; i < SenaryoYolları.Length; i++)
            {
                Bir_Senaryo_ sny = new Bir_Senaryo_(SenaryoYolları[i]);
                tv.Nodes.Add(GörselÖge.Oluştur(sny));
                SnryLsts.Add(sny);
            }
            tv.ResumeLayout();

            for (int i = 0; i < SnryLsts.Count; i++)
            { 
                if (SnryLsts[i].Hata != "")
                {
                    SnryLsts[i].Hata = SnryLsts[i].Hata.Trim('\r', '\n', ' ');

                    Yedekleyici.Ortak.Günlük_Ekle(SnryLsts[i].DosyaYolu +
                                                  Environment.NewLine + SnryLsts[i].Hata);
                }
            }
            Senaryo_Listesi = SnryLsts;
        }
    }
}
