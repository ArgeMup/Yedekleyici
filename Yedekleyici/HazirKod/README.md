# HazirKod Genel Amaçlı Kütüphane
ArgeMup@yandex.com

Ayarlar_ Ayarlar = new Ayarlar_();                              //create settings class based on xml
string yol = Ayarlar.AyarlarDosyasıYolunuAl();                  //get standart file location    
bool sonuc_bool = Ayarlar.Yaz("parametre", "bilgi");            //write
string okunan = Ayarlar.Oku("parametre");                       //read
List<Ayarlar_.BirParametre_> liste = Ayarlar.Listele();         //list
sonuc_bool = Ayarlar.Sil("parametre");                          //delete                   

DahaCokKarmasiklastirma_ DahaCokKarmasiklastirma = new DahaCokKarmasiklastirma_();  //create different type crypt class
string sonuc = DahaCokKarmasiklastirma.Karıştır("bilgi", "parola");                 //encrypt everytime  different output, using my algo 
sonuc = DahaCokKarmasiklastirma.Düzelt(sonuc, "parola");                            //decrypt

DurumBildirimi_ DurumBildirimi = new DurumBildirimi_(this);       //create warning class
DurumBildirimi.BaloncukluUyarı("mesaj");                          //message on watch
DurumBildirimi.BaloncukluUyarı("mesaj", textBox1);                //message on component

Karmasiklastirma_ Karmasiklastirma = new Karmasiklastirma_();     //create crypt class
string sonuc = Karmasiklastirma.Karıştır("bilgi", "parola");      //encrypt everytime same output, using aes
sonuc = Karmasiklastirma.Düzelt(sonuc, "parola");                 //decrypt

KayitDefteri_ KayitDefteri = new KayitDefteri_();                 //create registry class 
bool sonuc_bool = KayitDefteri.Yaz("dal", "parametre", "bilgi");  //write
string sonuc = (string)KayitDefteri.Oku("dal", "parametre");      //read

//create multi word auto complete class 
KelimeTamamlayici_ KelimeTamamlayici = new KelimeTamamlayici_(this, null, new List<string>() { "123", "1234" });    
KelimeTamamlayici.Başlat(textBox1);                                           //tells that it will work on textbox1

Sabitler_ Sabitler = new Sabitler_();                                         //create #define like global constants

TiplerArasiDonusturme_ TiplerArasiDonusturme = new TiplerArasiDonusturme_();  //create convert between types class which for strings, byte arrays, memory streams, hex strings 

UygulamaBostaBekliyor_ UygulamaBostaBekliyor = new UygulamaBostaBekliyor_();  //create advertisement class
UygulamaBostaBekliyor.BelirtilenSüreSonundaGörüntüOynat(this);                //play your videos if pc idle

//create other instances finder class
UygulamaOncedenCalistirildiMi_ UygulamaOncedenCalistirildiMi = new UygulamaOncedenCalistirildiMi_(this);           
if (UygulamaOncedenCalistirildiMi.KontrolEt())                                //look for other instance of app
{                                                                             //yes found
    UygulamaOncedenCalistirildiMi.DiğerUygulamayıÖneGetir();                  //bring to front other instances
    UygulamaOncedenCalistirildiMi.DiğerUygulamayıKapat();                     //close other instances
}

UygulamalarArasıHaberlesme_ Hab = new UygulamalarArasıHaberlesme_();    //create ipc class for network or internal queue
Hab.Baslat("A Cihazı");                                                 //fill sender name
Gönderilecek_Mesaj_ Mesaj_1 = new Gönderilecek_Mesaj_();                //create sender
Mesaj_1.AlıcınınAdı = "B Cihazı";                                       //fill receiver name
Mesaj_1.Konu = "K1";                                                    //fill subject
Hab.HemenGonder(ref Mesaj_1);                                           //send
Alınan_Mesaj_ MesaJ_2 = new Alınan_Mesaj_();                            //create receiver 
MesaJ_2 = Hab.GelenKutusu[0];                                           //read received message if there is one
