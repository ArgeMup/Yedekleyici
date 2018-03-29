using System;
using System.IO;
using System.Security.AccessControl;

namespace Yedekleyici
{
    public static class İzinAl
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

                return true;
            }
            catch (Exception) { }

            return false;
        }
    }

    public static class Listele
    {
        public static string[] Dosya(string Klasör, string Filtre = "*", SearchOption Seçenekler = SearchOption.TopDirectoryOnly)
        {
            try
            {
                string[] Liste = Directory.GetFiles(Klasör, Filtre, Seçenekler);
                if (Liste != null) return Liste; 
                else return new string[0];
            }
            catch (Exception) { }

            try
            {
                İzinAl.Klasör(Klasör);
                string[] Liste = Directory.GetFiles(Klasör, Filtre, Seçenekler);
                if (Liste != null) return Liste;
            }
            catch (Exception) { }

            return new string[0];
        }

        public static string[] Klasör(string Klasör, string Filtre = "*", SearchOption Seçenekler = SearchOption.TopDirectoryOnly)
        {
            try
            {
                string[] Liste = Directory.GetDirectories(Klasör, Filtre, Seçenekler);
                if (Liste != null) return Liste;
                else return new string[0];
            }
            catch (Exception) { }

            try
            {
                İzinAl.Klasör(Klasör);
                string[] Liste = Directory.GetDirectories(Klasör, Filtre, Seçenekler);
                if (Liste != null) return Liste;
            }
            catch (Exception) { }

            return new string[0];
        }
    }
}
