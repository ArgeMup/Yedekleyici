// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ArgeMup.HazirKod.Dönüştürme
{
    public static class D_Metin
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisine(string Girdi)
        {
            if (string.IsNullOrEmpty(Girdi)) return null;
            return Encoding.UTF8.GetBytes(Girdi);
        }
        public static string BaytDizisinden(byte[] Girdi, int Boyut = int.MinValue)
        {
            if (Boyut == int.MinValue) Boyut = Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = Girdi.Length;
            if (Girdi == null) return "";

            return Encoding.UTF8.GetString(Girdi, 0, Boyut).TrimEnd('\0');
        }
    }

    public static class D_HexMetin
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisine(string Girdi)
        {
            if (string.IsNullOrEmpty(Girdi)) return null;
            return Enumerable.Range(0, Girdi.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(Girdi.Substring(x, 2), 16))
                     .ToArray();
        }
        public static string BaytDizisinden(byte[] Girdi, int Boyut = int.MinValue)
        {
            if (Boyut == int.MinValue) Boyut = Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = Girdi.Length;
            if (Girdi == null) return "";

            string Çıktı = "";
            for (int w = 0; w < Boyut; w++) Çıktı += Girdi[w].ToString("X2");
            return Çıktı;
        }
    }

    public static class D_Akış
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisine(MemoryStream Girdi, int Boyut = int.MinValue)
        {
            if (Boyut == int.MinValue) Boyut = (int)Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = (int)Girdi.Length;
            if (Girdi == null) return null;

            byte[] Çıktı = new byte[Boyut];
            Girdi.Read(Çıktı, 0, Boyut);
            return Çıktı;
        }
        public static void BaytDizisinden(byte[] Girdi, ref MemoryStream Çıktı, int Boyut = int.MinValue)
        {
            if (Boyut == int.MinValue) Boyut = Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = Girdi.Length;
            if (Girdi == null) return;

            Çıktı.Write(Girdi, 0, Boyut);
        }
    }

    public static class D_Nesne
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisine(object Girdi)
        {
            if (Girdi == null) return null;

            byte[] Çıktı;
            using (var mS = new MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(mS, Girdi);
                Çıktı = mS.ToArray();
            }
            return Çıktı;
        }
        public static object BaytDizisinden(byte[] Girdi)
        {
            if (Girdi == null) return null;

            object Çıktı;
            using (var mS = new MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                mS.Write(Girdi, 0, Girdi.Length);
                mS.Seek(0, SeekOrigin.Begin);
                Çıktı = bf.Deserialize(mS);
            }
            return Çıktı;
        }
    }

    public static class D_WwwAdresi
    {
        public const string Sürüm = "V1.0";

        public static string IpAdresine(string Girdi)
        {
            return System.Net.Dns.GetHostAddresses(Girdi)[0].ToString();
        }
    }

    public static class D_İkon
    {
        public const string Sürüm = "V1.0";

        public static Icon Metinden(string Metin, Icon ikon, Font font, Color Renk, Point Konum, Color ArkaPlan)
        {
            Brush brush = new SolidBrush(Renk);
            Bitmap bitmap = new Bitmap(ikon.Width, ikon.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(ArkaPlan);
            graphics.DrawString(Metin, font, brush, Konum.X, Konum.Y);
            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            brush.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

        public static void Yoket(Icon ikon)
        {
            W32_8.DestroyIcon(ikon.Handle);
            ikon.Dispose();
            ikon = null;
        }
    }

    public static class D_DosyaBoyutu
    {
        public const string Sürüm = "V1.0";

        public static string Metne(decimal d)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (d < 1) return "0B";

            int place = Convert.ToInt32(Math.Floor(Math.Log((Double)d, 1024)));
            double num = Math.Round((Double)d / Math.Pow(1024, place), 1);
            return (num).ToString() + suf[place];
        }
    }

    public static class D_Süre
    {
        public const string Sürüm = "V1.0";

        public static class Saniye
        {
            public const string Sürüm = "V1.0";

            public static string Metne(double süre)
            {
                if (süre < 59) return süre.ToString("0") + "sn";
                süre /= 60;//dakika

                if (süre < 60) return süre.ToString("0.#") + "dk";
                süre /= 60;//saat

                return süre.ToString("0.##") + "sa";
            }
        }
    }

    public static class D_Parmakİzi
    {
        public const string Sürüm = "V1.0";

        public static string Metne()
        {
            /* 
             * Kullanılacak ise  
             * Solution Explorer -> Proje -> References -> Add Reference
             * Assemblies -> Framework -> System.Management
             */

            string Çıktı = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_DiskDrive");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementBaseObject mo in moc)
            {
                var gecici = mo["InterfaceType"];
                if (gecici == null || gecici.ToString() == "USB") continue;

                gecici = mo["Model"];
                if (gecici != null) Çıktı += gecici.ToString() + ", ";

                gecici = mo["SerialNumber"];
                if (gecici != null) Çıktı += gecici.ToString() + ", ";

                gecici = mo["Signature"];
                if (gecici != null) Çıktı += gecici.ToString();

                if (Çıktı != "") break;
            }

            moc.Dispose();
            mc.Dispose();

            return Çıktı;
        }
    }
}