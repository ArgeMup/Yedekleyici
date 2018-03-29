using System.Collections.Generic;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using System;

namespace Yedekleyici
{
    public class DosyaSistemiIzleyici_
    {
        List<Talep> Liste = new List<Talep>();

        public bool DeğişiklikVarMı(string Ad, string Kaynak, string Hedef)
        {
            if (!Directory.Exists(Kaynak)) return false;

            string imza = Ad + Kaynak + Hedef;
            imza = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(imza)));

            foreach (Talep biri in Liste)
            {
                if (biri.İmza == imza) return biri.DeğişiklikVarMı();
            }

            Liste.Add(new Talep(imza, Kaynak));
            return true;
        }
        public void Durdur(string Ad, string Kaynak, string Hedef)
        {
            string imza = Ad + Kaynak + Hedef;
            imza = D_HexMetin.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Metin.BaytDizisine(imza)));

            foreach (Talep biri in Liste)
            {
                if (biri.İmza == imza)
                {
                    biri.Dispose();
                    Liste.Remove(biri);
                    return;
                }
            }
        }

        class Talep : IDisposable
        {
            public string İmza;

            FileSystemWatcher İzleyici = null;
            int DeğişenDosyaSayısı = 0;
            string Kaynak;

            public Talep(string imza, string Kaynak)
            {
                this.İmza = imza;
                this.Kaynak = Kaynak;

                DeğişiklikVarMı();
            }
            ~Talep()
            {
                Dispose();
            }
            public void Dispose()
            {
                if (İzleyici != null) { İzleyici.Dispose(); İzleyici = null; }
            }
            public bool DeğişiklikVarMı()
            {
                if (İzleyici == null)
                {
                    İzleyici = new FileSystemWatcher(Kaynak);
                    İzleyici.Changed += İzleyici_YapısıDeğişti;
                    İzleyici.Created += İzleyici_YapısıDeğişti;
                    İzleyici.Deleted += İzleyici_YapısıDeğişti;
                    İzleyici.Disposed += İzleyici_Disposed;
                    İzleyici.Error += İzleyici_Error;
                    İzleyici.Renamed += İzleyici_AdıDeğişti;

                    İzleyici.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                    İzleyici.IncludeSubdirectories = true;

                    string kay = "";
                    try
                    {
                        İzleyici.EnableRaisingEvents = true;
                        DeğişenDosyaSayısı = 0;

                        kay = Kaynak + "\\" + Path.GetRandomFileName();
                        Directory.CreateDirectory(kay, new System.Security.AccessControl.DirectorySecurity(Kaynak, System.Security.AccessControl.AccessControlSections.Access));

                        System.Threading.Thread.Sleep(1);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    catch (Exception) { }

                    if (Directory.Exists(kay)) AnaEkran.SilKlasör(kay);
                    if (DeğişenDosyaSayısı == 0) { AnaEkran.YazLog("Bilgi", "Dosya sistemi izleyicisi oluşturulamadı " + Kaynak); DeğişenDosyaSayısı = 1; }
                    else DeğişenDosyaSayısı = 0;
                }

                return DeğişenDosyaSayısı > 0;
            }

            void İzleyici_AdıDeğişti(object sender, RenamedEventArgs e)
            {
                DeğişenDosyaSayısı++;
            }
            void İzleyici_YapısıDeğişti(object sender, FileSystemEventArgs e)
            {
                DeğişenDosyaSayısı++;
            }
            void İzleyici_Error(object sender, ErrorEventArgs e)
            {
                try { İzleyici.Dispose(); } catch (System.Exception) { }
            }
            void İzleyici_Disposed(object sender, System.EventArgs e)
            {
                İzleyici = null;
            }  
        }
    }
}