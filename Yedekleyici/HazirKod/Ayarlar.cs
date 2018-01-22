// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using ArgeMup.HazirKod.Dönüştürme;
using System.Reflection;

namespace ArgeMup.HazirKod
{
    public class Ayarlar_ : IDisposable 
    {
        public const string Sürüm = "V2.3";
        #region Değişkenler
        int DeğişiklikleriKaydetmeAralığı_Sn;
        int KaynaklarıBoşaltmaAralığı_Dk;
        bool AltDallarıdaKarıştır, DosyadaDeğişiklikYapıldı;
        string AyarlarDosyasıYolu;
        string Parola;

        DateTime KaynaklarıBoşaltmaAnı;
        Mutex Mutex_;
        System.Timers.Timer Zamanlayıcı;
        public struct BirParametre_
        {
            public string Parametre;
            public string Ayar;

            public BirParametre_(string Parametre_, string Ayar_)
            {
                Parametre = Parametre_;
                Ayar = Ayar_;
            }
        }
        DahaCokKarmasiklastirma_ Karmaşıklaştırma;
        XmlDocument Döküman = null; XmlNode AyarlarDalı = null;
        #endregion
       
        public Ayarlar_(out bool Sonuç, string AyarlarİçinParola = "", string AyarlarDosyası = "", bool Izin_AltDallarıdaKarıştır = false, int Süre_DeğişiklikleriKaydetmeAralığı_Sn = 30, int Süre_KaynaklarıBoşaltmaAralığı_Dk = 30)
        {
            Sonuç = YenidenBaşlat(AyarlarİçinParola, AyarlarDosyası, Izin_AltDallarıdaKarıştır, Süre_DeğişiklikleriKaydetmeAralığı_Sn, Süre_KaynaklarıBoşaltmaAralığı_Dk);
        }
        public bool YenidenBaşlat(string AyarlarİçinParola = "", string AyarlarDosyası = "", bool Izin_AltDallarıdaKarıştır = false, int Süre_DeğişiklikleriKaydetmeAralığı_Sn = 30, int Süre_KaynaklarıBoşaltmaAralığı_Dk = 30)
        {
            try
            {
                Dispose();
                disposedValue = false;

				#if UUNNIITTYY
					#if UNITY_EDITOR
						AyarlarDosyası = AyarlarDosyası.Replace('/', '\\');
					#endif
					AyarlarDosyasıYolu = AyarlarDosyası;
				#else
					if (AyarlarDosyası == "") AyarlarDosyasıYolu = GetType().Assembly.Location + ".Ayarlar";    
					else if (AyarlarDosyası.Contains("\\")) AyarlarDosyasıYolu = AyarlarDosyası;               
					else AyarlarDosyasıYolu = Path.GetDirectoryName(GetType().Assembly.Location) + "\\" + AyarlarDosyası + ".Ayarlar";
				#endif
			
				if (!File.Exists(AyarlarDosyasıYolu)) 
                {
                    if (!Directory.Exists(Path.GetDirectoryName(AyarlarDosyasıYolu))) Directory.CreateDirectory(Path.GetDirectoryName(AyarlarDosyasıYolu));
                    FileStream gecici = File.Create(AyarlarDosyasıYolu);
                    gecici.Close();
                    File.Delete(AyarlarDosyasıYolu);
                }
            }
            catch (Exception) { AyarlarDosyasıYolu = ""; }

            if (string.IsNullOrEmpty(AyarlarİçinParola))
            {
                Parola = "";
                AltDallarıdaKarıştır = false; 
            }
            else
            {
                Parola = AyarlarİçinParola;
                AltDallarıdaKarıştır = Izin_AltDallarıdaKarıştır;
                if (Karmaşıklaştırma == null) Karmaşıklaştırma = new DahaCokKarmasiklastirma_();
            }

            DeğişiklikleriKaydetmeAralığı_Sn = Süre_DeğişiklikleriKaydetmeAralığı_Sn;
            KaynaklarıBoşaltmaAralığı_Dk = Süre_KaynaklarıBoşaltmaAralığı_Dk;

            try
            {
                Döküman = new XmlDocument();
                
                if (!File.Exists(AyarlarDosyasıYolu))
                {
                    XmlElement AnaKatman = Döküman.CreateElement("AnaKatman");

                    XmlElement Kendi = Döküman.CreateElement("Kendi");
                    XmlElement Kendi_1 = Döküman.CreateElement("Olusturulma");
                    Kendi_1.InnerText = DateTime.Now.ToString();
                    XmlElement Kendi_2 = Döküman.CreateElement("Konum");
                    Kendi_2.InnerText = AyarlarDosyasıYolu;
                    XmlElement Kendi_3 = Döküman.CreateElement("Surum");
                    Kendi_3.InnerText = Sürüm;
                    XmlElement Kendi_4 = Döküman.CreateElement("BilgisayarVeKullanıcıAdı");
                    Kendi_4.InnerText = Environment.MachineName + "/" + Environment.UserName;
                    Kendi.AppendChild(Kendi_1);
                    Kendi.AppendChild(Kendi_2);
                    Kendi.AppendChild(Kendi_3);
                    Kendi.AppendChild(Kendi_4);

                    XmlElement Uygulama = Döküman.CreateElement("Uygulama");
                    XmlElement Uygulama_1 = Döküman.CreateElement("Ad");
					#if UUNNIITTYY
					Uygulama_1.InnerText = UnityEngine.Application.productName;
					#else
					Uygulama_1.InnerText = System.Windows.Forms.Application.ProductName;
					#endif
                    XmlElement Uygulama_2 = Döküman.CreateElement("Surum");
					#if UUNNIITTYY
					Uygulama_2.InnerText = "V" + UnityEngine.Application.version;
					#else
					Uygulama_2.InnerText = "V" + System.Windows.Forms.Application.ProductVersion;
					#endif
                    Uygulama.AppendChild(Uygulama_1);
                    Uygulama.AppendChild(Uygulama_2);

                    XmlElement Ayarlar = Döküman.CreateElement("Ayarlar");

                    XmlElement DogrulamaKontrolu = Döküman.CreateElement("DogrulamaKontrolu");
                    XmlElement DogrulamaKontrolu_1 = Döküman.CreateElement("ButunlukKontrolu");
                    DogrulamaKontrolu_1.InnerText = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(Ayarlar.OuterXml), 32));
                    XmlElement DogrulamaKontrolu_2 = Döküman.CreateElement("ParolaKontrolu");
                    if (Parola == "") DogrulamaKontrolu_2.InnerText = "ParolaKontroluBilgisi";
                    else DogrulamaKontrolu_2.InnerText = Karmaşıklaştırma.Karıştır("ParolaKontroluBilgisi", Parola);
                    DogrulamaKontrolu.AppendChild(DogrulamaKontrolu_1);
                    DogrulamaKontrolu.AppendChild(DogrulamaKontrolu_2);

                    AnaKatman.AppendChild(Kendi);
                    AnaKatman.AppendChild(Uygulama);
                    AnaKatman.AppendChild(Ayarlar);
                    AnaKatman.AppendChild(DogrulamaKontrolu);

                    Döküman.AppendChild(AnaKatman);
                    Döküman.Save(AyarlarDosyasıYolu);
                }
                
                if (EtkinMi()) return true;
            }
            catch (Exception) { }

            Durdur();
            return false;
        }
        public string AyarlarDosyasıYolunuAl()
        {
            return AyarlarDosyasıYolu;
        }
        public bool DeğişiklikleriKaydet(bool VeDurdur = false)
        {
            try
            {
                if (File.Exists(AyarlarDosyasıYolu) && !File.Exists(AyarlarDosyasıYolu + ".yedek")) File.Copy(AyarlarDosyasıYolu, AyarlarDosyasıYolu + ".yedek", false);

                Mutex_.WaitOne();
                Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[0].InnerText = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(Döküman.ChildNodes[0].ChildNodes[2].OuterXml), 32));
                Döküman.Save(AyarlarDosyasıYolu);

                AyarlarDalı = null;
                if (!EtkinMi()) { Mutex_.ReleaseMutex(); return false; }
                DosyadaDeğişiklikYapıldı = false;
                Mutex_.ReleaseMutex();

                File.Delete(AyarlarDosyasıYolu + ".yedek");

                if (VeDurdur) return Durdur();
                else return true;
            }
            catch (Exception) { return false; }
        }

        private bool EtkinMi()
        {
            try
            {
                if (AyarlarDalı == null)
                {
                    if (Döküman == null) Döküman = new XmlDocument();
                    else Döküman.RemoveAll(); 
                    Döküman.Load(AyarlarDosyasıYolu);

                    if (Döküman.ChildNodes[0].Name                              != "AnaKatman"          ||
                        Döküman.ChildNodes[0].ChildNodes[0].Name                != "Kendi"              ||
                        Döküman.ChildNodes[0].ChildNodes[1].Name                != "Uygulama"           ||
                        Döküman.ChildNodes[0].ChildNodes[2].Name                != "Ayarlar"            ||
                        Döküman.ChildNodes[0].ChildNodes[3].Name                != "DogrulamaKontrolu"  ||
                        Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[0].Name  != "ButunlukKontrolu"   ||
                        Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[1].Name  != "ParolaKontrolu") return false;

                    if (Parola == "") { if (Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[1].InnerText != "ParolaKontroluBilgisi") return false; }
                    else if (Karmaşıklaştırma.Düzelt(Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[1].InnerText, Parola) != "ParolaKontroluBilgisi") return false;

                    string Hesaplanan_DoğrulukKontrolüBilgisi = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(Döküman.ChildNodes[0].ChildNodes[2].OuterXml), 32));
                    string Okunan_DoğrulukKontrolüBilgisi = Döküman.ChildNodes[0].ChildNodes[3].ChildNodes[0].InnerText;
                    if (Hesaplanan_DoğrulukKontrolüBilgisi != Okunan_DoğrulukKontrolüBilgisi) return false;

                    if (Zamanlayıcı == null && DeğişiklikleriKaydetmeAralığı_Sn > 0)
                    {
                        Zamanlayıcı = new System.Timers.Timer();
                        Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                        Zamanlayıcı.Interval = DeğişiklikleriKaydetmeAralığı_Sn * 1000;
                        Zamanlayıcı.AutoReset = false;
                        Zamanlayıcı.Enabled = true;
                    }

                    if (Mutex_ == null) Mutex_ = new Mutex();

                    AyarlarDalı = Döküman.ChildNodes[0].ChildNodes[2];
                }

                KaynaklarıBoşaltmaAnı = DateTime.Now + TimeSpan.FromMinutes(KaynaklarıBoşaltmaAralığı_Dk);
                return true;
            }
            catch (Exception) { Durdur(); return false; }
        }
        private bool Durdur()
        {
            try
            {
				#if UUNNIITTYY
				if (Mutex_ != null) { Mutex_.Close(); Mutex_ = null; }
				#else
				if (Mutex_ != null) { Mutex_.Dispose(); Mutex_ = null; }
				#endif

                if (Zamanlayıcı != null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
                if (Karmaşıklaştırma != null) { Karmaşıklaştırma.Dispose(); Karmaşıklaştırma = null; }

                if (AyarlarDalı != null) { AyarlarDalı.RemoveAll(); AyarlarDalı = null; }
                if (Döküman != null) { Döküman.RemoveAll(); Döküman = null; }

                return true;
            }
            catch (Exception) { return false; }
        }
        private void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            try { if (DosyadaDeğişiklikYapıldı) DeğişiklikleriKaydet(DateTime.Now >= KaynaklarıBoşaltmaAnı); } catch (Exception) { }
            Zamanlayıcı.Enabled = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Oku(string Parametre, string BulunamamasıDurumundakiDeğeri = "")
        {
            try
            {
                if (!EtkinMi()) throw new Exception();

                foreach (XmlNode Par in AyarlarDalı.ChildNodes)
                {
                    if (Par.InnerText == Parametre)
                    {
                        if (string.IsNullOrEmpty(Parola)) return Par.Attributes["C"].Value;
                        else return Karmaşıklaştırma.Düzelt(Par.Attributes["C"].Value, Parola);
                    }
                }
            }
            catch (Exception) { }
            return BulunamamasıDurumundakiDeğeri;
        }
        public string Oku_AltDal(string Xml, string Parametre, string BulunamamasıDurumundakiDeğeri = "")
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Xml);

                //                          AnaKatman     Ayarlar       Parametre
                foreach (XmlNode Par in doc.ChildNodes[0].ChildNodes)
                {
                    if (Par.InnerText == Parametre)
                    {
                        if (string.IsNullOrEmpty(Parola) || !AltDallarıdaKarıştır) return Par.Attributes["C"].Value;
                        else return Karmaşıklaştırma.Düzelt(Par.Attributes["C"].Value, Parola);
                    }
                }
            }
            catch (Exception) { }
            return BulunamamasıDurumundakiDeğeri;
        }
        public bool Oku_AltDal_Yaz_SınıfDeğişkenleri(string Xml, ref object Sınıf)
        {
            try
            {
                foreach (FieldInfo field in Sınıf.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    string AltDal = Oku_AltDal(Xml, field.Name);
                    if (!string.IsNullOrEmpty(AltDal))
                    {
                        if (field.FieldType.ToString() == Oku_AltDal(AltDal, "Tip"))
                        {
                            field.SetValue(Sınıf, D_Nesne.BaytDizisinden(D_HexMetin.BaytDizisine(Oku_AltDal(AltDal, "Bilgi"))));
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Yaz(string Parametre, string Ayar)
        {
            try
            {
                if (!EtkinMi()) throw new Exception();

                foreach (XmlNode Par in AyarlarDalı.ChildNodes)
                {
                    if (Par.InnerText == Parametre)
                    {
                        //güncelleme
                        Mutex_.WaitOne();
                        if (string.IsNullOrEmpty(Parola)) Par.Attributes["C"].Value = Ayar;
                        else Par.Attributes["C"].Value = Karmaşıklaştırma.Karıştır(Ayar, Parola);
                        DosyadaDeğişiklikYapıldı = true;
                        Mutex_.ReleaseMutex();
                        return true;
                    }
                }

                //yeni
                XmlElement Yeni = Döküman.CreateElement("B");
                Yeni.InnerText = Parametre;
                if (string.IsNullOrEmpty(Parola)) Yeni.SetAttribute("C", Ayar);
                else Yeni.SetAttribute("C", Karmaşıklaştırma.Karıştır(Ayar, Parola));

                Mutex_.WaitOne();
                AyarlarDalı.AppendChild(Yeni);
                DosyadaDeğişiklikYapıldı = true;
                Mutex_.ReleaseMutex();
                return true;
            }
            catch (Exception) { }
            return false;
        }
        public bool Yaz_AltDal(ref string Xml, string Parametre, string Ayar)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (string.IsNullOrEmpty(Xml))
                {
                    XmlElement AnaKatman = doc.CreateElement("A");
                    doc.AppendChild(AnaKatman);
                    Xml = doc.InnerXml;
                    doc.LoadXml(Xml);
                }
                else
                {
                    doc.LoadXml(Xml);
                    if (doc.ChildNodes[0].Name != "A") return false;
                }

                //                          AnaKatman     Ayarlar       Parametre
                foreach (XmlNode Par in doc.ChildNodes[0].ChildNodes)
                {
                    if (Par.InnerText == Parametre)
                    {
                        //güncelleme
                        if (string.IsNullOrEmpty(Parola) || !AltDallarıdaKarıştır) Par.Attributes["C"].Value = Ayar;
                        else Par.Attributes["C"].Value = Karmaşıklaştırma.Karıştır(Ayar, Parola);
                        Xml = doc.InnerXml;
                        return true;
                    }
                }

                //yeni
                XmlElement Yeni = doc.CreateElement("B");
                Yeni.InnerText = Parametre;
                if (string.IsNullOrEmpty(Parola) || !AltDallarıdaKarıştır) Yeni.SetAttribute("C", Ayar);
                else Yeni.SetAttribute("C", Karmaşıklaştırma.Karıştır(Ayar, Parola));

                //  AnaKatman     Ayarlar     Parametre
                doc.ChildNodes[0].AppendChild(Yeni);

                Xml = doc.InnerXml;
                return true;
            }
            catch (Exception) { }
            return false;
        }
        public bool Yaz_AltDal_Oku_SınıfDeğişkenleri(ref string Xml, object Sınıf)
        {
            foreach (FieldInfo field in Sınıf.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                string AltDal = "";
                if (!Yaz_AltDal(ref AltDal, "Tip", field.FieldType.ToString())) return false;
                if (!Yaz_AltDal(ref AltDal, "Bilgi", D_HexMetin.BaytDizisinden(D_Nesne.BaytDizisine(field.GetValue(Sınıf))))) return false;

                if (!Yaz_AltDal(ref Xml, field.Name, AltDal)) return false;
            }

            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BirParametre_> Listele()
        {
            List<BirParametre_> Liste = new List<BirParametre_>();
            try
            {
                if (!EtkinMi()) throw new Exception();

                foreach (XmlNode Par in AyarlarDalı.ChildNodes)
                {
                    BirParametre_ Yeni = new BirParametre_();
                    Yeni.Parametre = Par.InnerText;
                    if (string.IsNullOrEmpty(Parola)) Yeni.Ayar = Par.Attributes["C"].Value;
                    else Yeni.Ayar = Karmaşıklaştırma.Düzelt(Par.Attributes["C"].Value, Parola);
                    Liste.Add(Yeni);
                }
            }
            catch (Exception) { }
            return Liste;
        }
        public List<BirParametre_> Listele_AltDal(string Xml)
        {
            List<BirParametre_> Liste = new List<BirParametre_>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Xml);

                //                          AnaKatman     Ayarlar       Parametre
                foreach (XmlNode Par in doc.ChildNodes[0].ChildNodes)
                {
                    BirParametre_ Yeni = new BirParametre_();
                    Yeni.Parametre = Par.InnerText;
                    if (string.IsNullOrEmpty(Parola) || !AltDallarıdaKarıştır) Yeni.Ayar = Par.Attributes["C"].Value;
                    else Yeni.Ayar = Karmaşıklaştırma.Düzelt(Par.Attributes["C"].Value, Parola);
                    Liste.Add(Yeni);
                }
            }
            catch (Exception) { }
            return Liste;
        }

        public bool ListeyiYaz(List<BirParametre_> Liste)
        {
            try
            {
                foreach (var eleman in Liste) if (!Yaz(eleman.Parametre, eleman.Ayar)) return false;
                return true;
            }
            catch (Exception) { }
            return false;
        }
        public bool ListeyiYaz_AltDal(ref string Xml, List<BirParametre_> Liste)
        {
            try
            {
                foreach (var eleman in Liste) if (!Yaz_AltDal(ref Xml, eleman.Parametre, eleman.Ayar)) return false;
                return true;
            }
            catch (Exception) { }
            return false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int SıraNo(string Parametre)
        {
            try
            {
                if (!EtkinMi()) throw new Exception();
                int sırano = 0;

                foreach (XmlNode Par in AyarlarDalı.ChildNodes)
                {
                    if (Par.InnerText == Parametre) return sırano;
                    sırano++;
                }
            }
            catch (Exception) { }
            return -1;
        }
        public int SıraNo_AltDal(string Xml, string Parametre)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Xml);
                int sırano = 0;

                //                          AnaKatman     Ayarlar       Parametre
                foreach (XmlNode Par in doc.ChildNodes[0].ChildNodes)
                {
                    if (Par.InnerText == Parametre) return sırano;
                    sırano++;
                }
            }
            catch (Exception) { }
            return -1;
        }

        public bool Taşı(int ŞuandakiSıraNo, int HedefSıraNo)
        {
            try
            {
                if (!EtkinMi()) throw new Exception();

                if (ŞuandakiSıraNo >= AyarlarDalı.ChildNodes.Count) return false;
                if (HedefSıraNo >= AyarlarDalı.ChildNodes.Count) return false;

                Mutex_.WaitOne();
                if (ŞuandakiSıraNo > HedefSıraNo) AyarlarDalı.InsertBefore(AyarlarDalı.ChildNodes[ŞuandakiSıraNo], AyarlarDalı.ChildNodes[HedefSıraNo]);
                else AyarlarDalı.InsertAfter(AyarlarDalı.ChildNodes[ŞuandakiSıraNo], AyarlarDalı.ChildNodes[HedefSıraNo]);
                DosyadaDeğişiklikYapıldı = true;
                Mutex_.ReleaseMutex();
                return true;
            }
            catch (Exception) { }
            return false;
        }
        public bool Taşı_AltDal(ref string Xml, int ŞuandakiSıraNo, int HedefSıraNo)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (string.IsNullOrEmpty(Xml))
                {
                    XmlElement AnaKatman = doc.CreateElement("A");
                    doc.AppendChild(AnaKatman);
                    Xml = doc.InnerXml;
                    doc.LoadXml(Xml);
                }
                else
                {
                    doc.LoadXml(Xml);
                    if (doc.ChildNodes[0].Name != "A") return false;
                }

                if (ŞuandakiSıraNo >= doc.ChildNodes[0].ChildNodes.Count) return false;
                if (HedefSıraNo >= doc.ChildNodes[0].ChildNodes.Count) return false;

                if (ŞuandakiSıraNo > HedefSıraNo) doc.ChildNodes[0].InsertBefore(doc.ChildNodes[0].ChildNodes[ŞuandakiSıraNo], doc.ChildNodes[0].ChildNodes[HedefSıraNo]);
                else doc.ChildNodes[0].InsertAfter(doc.ChildNodes[0].ChildNodes[ŞuandakiSıraNo], doc.ChildNodes[0].ChildNodes[HedefSıraNo]);

                Xml = doc.InnerXml;
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public bool Sil(string Parametre)
        {
            try
            {
                if (!EtkinMi()) throw new Exception();

                for (int i = 0; i < AyarlarDalı.ChildNodes.Count; i++)
                {
                    if (AyarlarDalı.ChildNodes[i].InnerText == Parametre)
                    {
                        //sil
                        Mutex_.WaitOne();
                        AyarlarDalı.ChildNodes[i].ParentNode.RemoveChild(AyarlarDalı.ChildNodes[i]);
                        DosyadaDeğişiklikYapıldı = true;
                        Mutex_.ReleaseMutex();
                        break;
                    }
                }

                return true;
            }
            catch (Exception) { }
            return false;
        }
        public bool Sil_AltDal(ref string Xml, string Parametre)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (string.IsNullOrEmpty(Xml)) return true;
                else
                {
                    doc.LoadXml(Xml);
                    if (doc.ChildNodes[0].Name != "A") return false;
                }

                //                      AnaKatman     Ayarlar       Parametre
                for (int i = 0; i < doc.ChildNodes[0].ChildNodes.Count; i++)
                {
                    if (doc.ChildNodes[0].ChildNodes[i].InnerText == Parametre)
                    {
                        //sil
                        doc.ChildNodes[0].ChildNodes[i].ParentNode.RemoveChild(doc.ChildNodes[0].ChildNodes[i]);
                        if (doc.InnerXml == "<A></A>") Xml = "";
                        else Xml = doc.InnerXml;
                        break;
                    }
                }

                return true;
            }
            catch (Exception) { }
            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (DeğişiklikleriKaydetmeAralığı_Sn > 0 && DosyadaDeğişiklikYapıldı) DeğişiklikleriKaydet(true);

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    Durdur();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Ayarlar_()
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