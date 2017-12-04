// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/Yedekleyici>

using System;
using System.Windows.Forms;
using System.IO;

namespace Yedekleyici
{
    static class Program
    {
        static string pak = Directory.GetCurrentDirectory() + "\\YedekleyiciDosyalari\\"; //programanaklasör

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

            Directory.CreateDirectory(pak + "Banka");
            //Directory.CreateDirectory(pak + "Kutuphane");

            //AnaEkran_GerekliDosyaKontrolü(Yedekleyici.Properties.Resources.ArgemupSifreleme, "ArgemupSifreleme.dll");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AnaEkran());
        }
        //private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        //{
        //    string strTempAssmbPath = pak + "Kutuphane\\" + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
        //    if (!File.Exists(strTempAssmbPath)) return null;
        //    return Assembly.LoadFrom(strTempAssmbPath);
        //}
        //static int AnaEkran_GerekliDosyaKontrolü(byte[] Kaynak, string AsılDosyaAdı)
        //{
        //    try
        //    {
        //        string asıl = pak + "Kutuphane\\" + AsılDosyaAdı;
        //        string gecici = pak + "Kutuphane\\" + "GerekliDosya.Gecici";

        //        if (!File.Exists(asıl)) { File.WriteAllBytes(asıl, Kaynak); return 1; }

        //        File.WriteAllBytes(gecici, Kaynak);
        //        if (System.Diagnostics.FileVersionInfo.GetVersionInfo(gecici).FileVersion != System.Diagnostics.FileVersionInfo.GetVersionInfo(asıl).FileVersion ||
        //            new FileInfo(gecici).Length != new FileInfo(asıl).Length)
        //        {
        //            File.Delete(asıl);
        //            File.Move(gecici, asıl);
        //            return 2;
        //        }

        //        File.Delete(gecici);
        //    }
        //    catch (Exception) { }

        //    return 0;
        //}
    }
}
