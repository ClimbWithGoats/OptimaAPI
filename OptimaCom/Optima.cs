using CDNBase;
using CDNDave;
using CDNHeal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CDNTwrb1;
using OptimaCom.Controller;

namespace OptimaCom
{
    public class Optima
    {
        public static ApplicationClass Application = null;
        public static ILogin Login = null;
        public static CDNBase.AdoSession Sesja = default;


        // Przyklad 1. - Logowanie do O! bez wyświetlania okienka logowania
        static public void LogowanieAutomatyczne(XLLoginInfo optima)
        {
            string Operator = optima.OpeIdent; //"ADMIN";
            string Haslo = optima.OpeHaslo;// ;		// operator nie ma hasła
            string Firma = optima.NazwaFirmy;// "Prezentacja_KH";    // nazwa firmy

            object[] hPar = new object[] {
                         1,  1,   0,  0,  1,   1,  0,    0,   0,   0,   0,   0,   1,   1,  1,   0,  0, 0 }; // do jakich modułów się logujemy
            /* Kolejno: KP, KH, KHP, ST, FA, MAG, PK, PKXL, CRM, ANL, DET, BIU, SRW, ODB, KB, KBP, HAP, CRMP
			 */

            // katalog, gdzie jest zainstalowana Optima (bez ustawienia tej zmiennej nie zadziała, chyba że program odpalimy z katalogu O!)
            System.Environment.CurrentDirectory = @"C:\Program Files (x86)\Comarch ERP Optima";//@"C:\Program Files\OPTIMA.NET";	

            // tworzymy nowy obiekt apliakcji
            Application = new CDNBase.ApplicationClass();

            // Jeśli proces nie ma dostępu do klucza w rejstrze 
            // HKCU\Software\CDN\CDN OPT!MA\CDN OPT!MA\Login\KonfigConnectStr
            // np. gdy pracuje jako aplikacji w IIS 
            // ciąg połączeniowy (ConnectString) podajemy bezpośrednio :
            // Application.KonfigConnectStr = "NET:CDN_KNF_Konfiguracja_DW,SERWERSQL,NT=0";
            // Application.HASPKeyServer =

            // blokujemy
            Login = Application.LockApp(256, 5000, null, null, null, null);
            //Login =  Application.LockApp(1, 5000, null, null, null, null);


            // logujemy się do podanej Firmy, na danego operatora, do podanych modułów
            Login = Application.Login(Operator, Haslo, Firma);


            //  Logowanie z pobraniem ustawienia modułów z karty Operatora
            //	Login = Application.Login(Operator, Haslo, Firma, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            //

            // tu jesteśmy zalogowani do O!
            Console.WriteLine("Jesteśmy zalogowani do O!");
            Sesja = Login.CreateSession();
        }
        // wylogowanie z O!
        public static void Wylogowanie()
        {
            // niszczymy Login
            Login = null;
            // odblokowanie (wylogowanie) O!
            Application.UnlockApp();
            // niszczymy obiekt Aplikacji
            Application = null;
        }

        // Przykład 2. - Dodanie dokumentu rejestru VAT
        protected static void DodanieRejestru()
        {
            // Tworzymy obiekt sesji
            ///Sesja = Login.CreateSession();
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            // tworzenie potrzebnych kolekcji

            CDNBase.ICollection FormyPlatnosci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.FormyPlatnosci", null));
            CDNBase.ICollection Waluty = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Waluty", null));
            CDNBase.ICollection RejestryVAT = (CDNBase.ICollection)(Sesja.CreateObject("CDN.RejestryVAT", null));
            CDNBase.ICollection Kontrahenci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Kontrahenci", null));
            // pobieranie kontrahenta, formy platnosci  i waluty
            CDNHeal.IKontrahent Kontrahent = (CDNHeal.IKontrahent)Kontrahenci["Knt_Kod='ALOZA'"];
            // w konfiguracji jest tylko jedna waluta (PLN)
            //			CDNHeal.Waluta Waluta			= (CDNHeal.Waluta)Waluty[ "WNa_Symbol='EUR'" ];
            OP_KASBOLib.FormaPlatnosci FPl = (OP_KASBOLib.FormaPlatnosci)FormyPlatnosci[1];
            // utworzenie nowego obiektu rejestru VAT
            CDNRVAT.VAT RejestrVAT = (CDNRVAT.VAT)RejestryVAT.AddNew(null);
            //ustawianie parametrów rejestru
            RejestrVAT.Typ = 2;//	1 - zakupu; 2 - sprzedaży
            Console.WriteLine("Typ ustawiony");

            RejestrVAT.Rejestr = "SPRZEDAŻ";    // nazwa rejestru
            Console.WriteLine("Rejestr ustawiony");

            RejestrVAT.Dokument = "DET01/05/2007";
            Console.WriteLine("Dokument ustawiony");

            RejestrVAT.IdentKsieg = "2007/05/28-oop";
            Console.WriteLine("IdentKsieg ustawiony");

            RejestrVAT.DataZap = new DateTime(2007, 05, 28);
            Console.WriteLine("DataZap ustawiona");

            RejestrVAT.FormaPlatnosci = FPl;
            Console.WriteLine("Forma platnosci ustawiona");

            RejestrVAT.Podmiot = (CDNHeal.IPodmiot)Kontrahent;
            Console.WriteLine("Podmiot ustawiony");
            // kategoria ustawia się sama, gdy ustawiany jest kontrahent

            // waluty nie ustawiam, bo w konf. jest na razie tylko jedna waluta (PLN)
            //			RejestrVAT.WalutaDoVAT = Waluta;	
            //			Console.WriteLine( "Waluta ustawiona " + RejestrVAT.Waluta.Symbol );

            // dodanie elementów rejestru VAT
            DodajElementyDoRejestru(RejestrVAT);

            // zapisanie zmian
            Sesja.Save();
            Sesja.ClearState();
        }

        protected static void DodajElementyDoRejestru(CDNRVAT.VAT RejestrVAT)
        {

            // pobranie kolekcji elementów rejestru
            CDNBase.ICollection Elementy = RejestrVAT.Elementy;
            Console.WriteLine("Dodawanie elementow: ");
            // dodanie elementów kolejno o stawkach 0%, 3%, 5%, 7%, 22% i zwolnionej
            DodajJeden(0, (CDNRVAT.VATElement)Elementy.AddNew(null));
            DodajJeden(3, (CDNRVAT.VATElement)Elementy.AddNew(null));
            DodajJeden(5, (CDNRVAT.VATElement)Elementy.AddNew(null));
            DodajJeden(7, (CDNRVAT.VATElement)Elementy.AddNew(null));
            DodajJeden(22, (CDNRVAT.VATElement)Elementy.AddNew(null));
            DodajJeden(-1, (CDNRVAT.VATElement)Elementy.AddNew(null));
        }

        protected static void DodajJeden(int Stawka, CDNRVAT.VATElement Element)
        {
            Console.WriteLine("\tDodaję element:");
            if (Stawka == -1)
            {
                // dodanie elementu o stawce zwolnionej z VAT
                Element.Flaga = 1;
                Element.Stawka = 0;
            }
            else
                Element.Stawka = Stawka;

            // Element.Flaga = 4;	// Flaga=4 oznacza stwkę NP.
            Console.WriteLine("\tStawka ustawiona");

            /*
			 * Rodzaje zakupów:
			 * Towary		 = 1;
			 * Inne			 = 2;
			 * Trwale		 = 3;
			 * Uslugi		 = 4;
			 * NoweSrTran	 = 5;
			 * Nieruchomosci = 6;
			 * Paliwo		 = 7;
			 */
            Element.RodzajZakupu = 1;
            Console.WriteLine("\tRodzaj zakupu ustawiony");

            Element.Netto = 23.57m; // m na końcu oznacza typ decimal
            Console.WriteLine("\tNetto ustawione");

            /* Porperty Odliczenia:
			 *  dla rej zakupu(odliczenia):		0-nie, 1-tak, 2-warunkowo
			 *	dla rej sprzed(uwz. w prop):	0-nie uwzgledniaj, 1-Uwzgledniaj w proporcji, 2- tylko w mianowniku
			 */
            Element.Odliczenia = 0;
            Console.WriteLine("\tOdliczenia ustawione\n");
        }

        public enum WydrukiUrzadzeniaEnum   // rodzaje urządzeń do wydruku
        {
            e_rpt_Urzadzenie_Ekran = 1,
            e_rpt_Urzadzenie_DrukarkaDomyslna = 2,
            e_rpt_Urzadzenie_DrukarkaInna = 3,
            e_rpt_Urzadzenie_SerwerWydrukow = 4
        }

        // Przykład 3. - Dodanie nowego kontrahenta
        protected static void DodajKontrahenta()
        {
            // //CDNBASE.//Sesja = Login.CreateSession();
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            OP_KASBOLib.Banki Banki = (OP_KASBOLib.Banki)Sesja.CreateObject("CDN.Banki", null);
            CDNHeal.Kategorie Kategorie = (CDNHeal.Kategorie)Sesja.CreateObject("CDN.Kategorie", null);

            CDNHeal.Kontrahenci Kontrahenci = (CDNHeal.Kontrahenci)Sesja.CreateObject("CDN.Kontrahenci", null);
            CDNHeal.IKontrahent Kontrahent = (CDNHeal.IKontrahent)Kontrahenci.AddNew(null);

            CDNHeal.IAdres Adres = Kontrahent.Adres;
            CDNHeal.INumerNIP NumerNIP = Kontrahent.NumerNIP;

            Adres.Gmina = "Kozia Wólka";
            Adres.KodPocztowy = "20-835";
            Adres.Kraj = "Polska";
            Adres.NrDomu = "17";
            Adres.Miasto = "Kozia Wólka";
            Adres.Powiat = "Lukrecjowo";
            Adres.Ulica = "Śliczna";
            Adres.Wojewodztwo = "Kujawsko-Pomorskie";

            NumerNIP.UstawNIP("PL", "281-949-89-45", 1);

            Kontrahent.Akronim = "NOWY";
            Kontrahent.Email = "nowy@nowy.eu";
            Kontrahent.Medialny = 0;
            Kontrahent.Nazwa1 = "Nowy kontrahent";
            Kontrahent.Nazwa2 = "dodany z C#";
            string Nazwa = Kontrahent.NazwaPelna;

            Kontrahent.Bank = (OP_KASBOLib.Bank)Banki["BNa_BNaID=2"];
            Kontrahent.Kategoria = (CDNHeal.Kategoria)Kategorie["Kat_KodSzczegol='ENERGIA'"];

            Sesja.Save();
            Sesja.ClearState();

            Console.WriteLine("Nazwa dodanego kotrahenta: " + Nazwa);
        }
        public static void EdytujAtrybutyTowaru(Commodity towar, ConfigRoot configuration, OptimaControllerRepository repository)
        {

            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            CDNTwrb1.Towary Towary = (CDNTwrb1.Towary)Sesja.CreateObject("CDN.Towary", null);
            CDNTwrb1.ITowar Towar = default;
            CDNBase.ICollection rAtrybuty = default;
            CDNTwrb1.IDefAtrybut rAtrybut = default;
            CDNTwrb1.ITwrAtrybut rAtrybutTowaru = default;

            try
            {
                Towar = (CDNTwrb1.ITowar)Towary[string.Format("Twr_GIDNumer='{0}'", towar.OptimaId)];
                var AllAttributes = configuration.Attributes.All.ToList();
                AllAttributes.AddRange(configuration.Attributes.CommodityAlias.Attribute.ToList());
                foreach (var item in AllAttributes)
                {
                    rAtrybuty = (CDNBase.ICollection)(Sesja.CreateObject("CDN.DefAtrybuty", null));
                    if (item == null) continue;
                    if (string.IsNullOrEmpty(item.Name) || item.Format == AttributeFormat.Ignore)
                        continue;
                    string name = configuration.Attributes.CommodityAlias.CanPrefixed ? configuration.Attributes.CommodityAlias.Prefix + item.Name : item.Name;
                    string sqlas = "SELECT TOP(1) DeA_Kod FROM cdn.DefAtrybuty WHERE DeA_Kod = '{0}' AND DeA_Typ = {1}";
                    string sql = string.Format(sqlas, name, (int)configuration.Attributes.CommodityAlias.Type);
                    List<dynamic> res = repository.GetData(sql);
                    if (res.Any())
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty[string.Format("DeA_Kod='{0}'", name)];
                    }
                    else
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty.AddNew(null);
                        var comm = configuration.Attributes.CommodityAlias;
                        rAtrybut.Typ = (int)comm.Type;
                        rAtrybut.Kod = name;
                        rAtrybut.Nazwa = name;
                        rAtrybut.Format = (int)item.Format;
                        Sesja.Save();
                        res = repository.GetData(sql);

                    }
                    string sqlasSubAtt = "SELECT top(1) DeA_Kod as Kod, Twr_GIDNumer as TwrId, DeA_DeAId as AtrId FROM cdn.TwrKarty tk JOIN cdn.TwrAtrybuty ta ON tk.Twr_GIDNumer = ta.TwA_TwrId join cdn.DefAtrybuty defa on ta.TwA_DeAId = defa.DeA_DeAId WHERE Twr_GIDNumer = {0} AND TwA_DeAId = {1} AND DeA_Kod='{2}'";
                    string sqlSubAtt = string.Format(sqlasSubAtt, towar.OptimaId, rAtrybut.ID, name);

                    List<dynamic> SubAtt = repository.GetData(sqlSubAtt);

                    if (rAtrybut != null && !SubAtt.Any())
                    {
                        rAtrybutTowaru = (CDNTwrb1.ITwrAtrybut)Towar.Atrybuty.AddNew(null);
                        rAtrybutTowaru.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutTowaru.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(towar, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();

                    }
                    else
                    {
                        rAtrybutTowaru = (CDNTwrb1.ITwrAtrybut)Towar.Atrybuty[string.Format("TwA_DeAId='{0}'", rAtrybut.ID)];
                        rAtrybutTowaru.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutTowaru.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(towar, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();
                    }
                }
                Sesja.ClearState();
                return;
            }
            catch (Exception ex)
            {
                if (Towar == null)
                {
                    Console.WriteLine("Problem z towarem ID : " + towar.OptimaId);
                    return;
                }
                Console.WriteLine(ex.Message);
                Sesja.ClearState();
            }
            Console.WriteLine("Towar Dispose() ");
        }
        public static void EdytujAtrybutyDokumentu(Document document, ConfigRoot configuration, OptimaControllerRepository repository)
        {

            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            CDNHlmn.DokumentyHaMag Dokumenty = (CDNHlmn.DokumentyHaMag)Sesja.CreateObject("CDN.DokumentyHaMag", null);
            CDNHlmn.IDokumentHaMag dokument = default;
            CDNBase.ICollection rAtrybuty = default;
            CDNTwrb1.IDefAtrybut rAtrybut = default;
            CDNTwrb1.IDokAtrybut rAtrybutDokumentu = default;
            try
            {
                //    Sesja = Login.CreateSession();
                dokument = (CDNHlmn.IDokumentHaMag)Dokumenty[string.Format("TrN_TrNID={0}", document.OptimaId)];

                var AllAttributes = configuration.Attributes.All.ToList();
                AllAttributes.AddRange(configuration.Attributes.DocumentAlias.Attribute.ToList());

                foreach (var item in AllAttributes)
                {
                    rAtrybuty = (CDNBase.ICollection)(Sesja.CreateObject("CDN.DefAtrybuty", null));
                    if (item == null) continue;
                    if (string.IsNullOrEmpty(item.Name) || item.Format == AttributeFormat.Ignore)
                        continue;
                    string name = configuration.Attributes.DocumentAlias.CanPrefixed ? configuration.Attributes.DocumentAlias.Prefix + item.Name : item.Name;
                    string sqlas = "SELECT TOP(1) DeA_Kod FROM cdn.DefAtrybuty WHERE DeA_Kod = '{0}' AND DeA_Typ = {1}";
                    string sql = string.Format(sqlas, name, (int)configuration.Attributes.DocumentAlias.Type);
                    List<dynamic> res = repository.GetData(sql);

                    if (res.Any())
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty[string.Format("DeA_Kod='{0}'", name)];
                    }
                    else
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty.AddNew(null);
                        var docSet = configuration.Attributes.DocumentAlias;
                        rAtrybut.Typ = (int)docSet.Type;
                        rAtrybut.Kod = name;
                        rAtrybut.Nazwa = name;
                        rAtrybut.Format = (int)item.Format;
                        Sesja.Save();
                        res = repository.GetData(sql);

                    }
                    string sqlasSubAtt = "SELECT TOP(1) DAt_Kod as Kod, DAt_WartoscTxt as Txt, DAt_WartoscDecimal as Num, DAt_DeAId as AtId FROM cdn.TraNag tn JOIN cdn.DokAtrybuty da ON tn.TrN_TrNID = da.DAt_TrNId WHERE DAt_TrNId = {0} AND DAt_DeAId = {1} AND DAt_Kod='{2}'";
                    string sqlSubAtt = string.Format(sqlasSubAtt, document.OptimaId, rAtrybut.ID, name);

                    List<dynamic> SubAtt = repository.GetData(sqlSubAtt);

                    if (rAtrybut != null && !SubAtt.Any())
                    {
                        rAtrybutDokumentu = (CDNTwrb1.IDokAtrybut)dokument.Atrybuty.AddNew(null);
                        rAtrybutDokumentu.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutDokumentu.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(document, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();

                    }
                    else
                    {
                        rAtrybutDokumentu = (CDNTwrb1.IDokAtrybut)dokument.Atrybuty[string.Format("DAt_DeAId ={0}", rAtrybut.ID)];
                        rAtrybutDokumentu.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutDokumentu.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(document, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();

                    }
                }
                Sesja.ClearState();
                return;

            }
            catch (Exception ex)
            {
                if (dokument == null)
                {
                    Console.WriteLine("Problem z dokumentem ID : " + document.OptimaId);
                    return;
                }
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("dokument Dispose() ");
        }

        public static void EdytujAtrybutyKontrahenta(Contractor kontrahent, ConfigRoot configuration, OptimaControllerRepository repository)
        {
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            // CDNBase.AdoSession Sesja = default;
            CDNHeal.Kontrahenci Kontrahenci = (CDNHeal.Kontrahenci)Sesja.CreateObject("CDN.Kontrahenci", null);
            CDNHeal.IKontrahent _Kontrahent = default;
            CDNBase.ICollection rAtrybuty = default;
            CDNTwrb1.IDefAtrybut rAtrybut = default;
            CDNTwrb1.IKntAtrybut rAtrybutKontrahenta = default;

            try
            {
                _Kontrahent = (CDNHeal.IKontrahent)Kontrahenci[string.Format("Knt_GIDNumer='{0}'", kontrahent.OptimaId)];

                var AllAttributes = configuration.Attributes.All.ToList();
                AllAttributes.AddRange(configuration.Attributes.contractorAlias.Attribute.ToList());

                foreach (var item in AllAttributes)
                {
                    rAtrybuty = (CDNBase.ICollection)(Sesja.CreateObject("CDN.DefAtrybuty", null));
                    if (item == null) continue;
                    if (string.IsNullOrEmpty(item.Name) || item.Format == AttributeFormat.Ignore)
                        continue;
                    string name = configuration.Attributes.contractorAlias.CanPrefixed ? configuration.Attributes.contractorAlias.Prefix + item.Name : item.Name;
                    string sqlas = "SELECT TOP(1) DeA_Kod FROM cdn.DefAtrybuty WHERE DeA_Kod = '{0}' AND DeA_Typ = {1}";
                    string sql = string.Format(sqlas, name, (int)configuration.Attributes.contractorAlias.Type);
                    List<dynamic> res = repository.GetData(sql);

                    if (res.Any())
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty[string.Format("DeA_Kod='{0}'", name)];
                    }
                    else
                    {
                        rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty.AddNew(null);
                        var kntSet = configuration.Attributes.contractorAlias;
                        rAtrybut.Typ = (int)kntSet.Type;
                        rAtrybut.Kod = name;
                        rAtrybut.Nazwa = name;
                        rAtrybut.Format = (int)item.Format;
                        Sesja.Save();
                        res = repository.GetData(sql);

                    }
                    string sqlasSubAtt = "SELECT  DeA_Kod as Kod, Knt_GIDNumer as KntId, DeA_DeAId as AtrId FROM cdn.KntKarty tn JOIN cdn.KntAtrybuty da ON tn.Knt_GIDNumer = da.KnA_PodmiotId join cdn.DefAtrybuty defa on da.KnA_DeAId = defa.DeA_DeAId WHERE Knt_GIDNumer = {0} AND KnA_DeAId = {1} AND DeA_Kod='{2}'";
                    string sqlSubAtt = string.Format(sqlasSubAtt, kontrahent.OptimaId, rAtrybut.ID, name);

                    List<dynamic> SubAtt = repository.GetData(sqlSubAtt);
                    if (rAtrybut != null && !SubAtt.Any())
                    {
                        rAtrybutKontrahenta = (CDNTwrb1.IKntAtrybut)_Kontrahent.Atrybuty.AddNew(null);
                        rAtrybutKontrahenta.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutKontrahenta.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(_Kontrahent, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();

                    }
                    else
                    {
                        rAtrybutKontrahenta = (CDNTwrb1.IKntAtrybut)_Kontrahent.Atrybuty[string.Format("KnA_DeAId='{0}'", rAtrybut.ID)];
                        rAtrybutKontrahenta.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
                        rAtrybutKontrahenta.Wartosc = !string.IsNullOrEmpty(item.JsonPropertyName) ? OwnReflections.GetPropertyValue(_Kontrahent, item.JsonPropertyName) : OwnReflections.GetDefaultValue(item.Value);
                        Sesja.Save();
                    }
                }
                Sesja.ClearState();
                return;

            }
            catch (Exception ex)
            {
                if (_Kontrahent == null)
                {
                    Console.WriteLine("Problem z kontrahentem ID : " + kontrahent.OptimaId);
                    return;
                }
                Console.WriteLine(ex.Message);
                Sesja.ClearState();
            }
            Console.WriteLine("kontrahent Dispose() ");

        }
        //SELECT  DeA_Kod as Kod, Knt_GIDNumer as KntId, DeA_DeAId as AtrId FROM cdn.KntKarty tn JOIN cdn.KntAtrybuty da ON tn.Knt_GIDNumer = da.KnA_PodmiotId join cdn.DefAtrybuty defa on da.KnA_DeAId = defa.DeA_DeAId WHERE Knt_GIDNumer = {0} AND KnA_DeAId = {1} AND DeA_Kod='{2}'

        // Przykład 4. - Dodanie towaru
        protected static void DodanieTowaru()
        {
            //  //CDNBASE.//Sesja = Login.CreateSession();
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            CDNTwrb1.Towary Towary = (CDNTwrb1.Towary)Sesja.CreateObject("CDN.Towary", null);
            CDNTwrb1.ITowar Towar = (CDNTwrb1.ITowar)Towary.AddNew(null);

            Towar.Nazwa = "Nowy towar dodany z C#";
            string nazwa = Towar.Nazwa1;
            Towar.Kod = "NOWY_C#";

            Towar.Stawka = 22.00m;
            Towar.Flaga = 2; // 2- oznacza stawkę opodatkowaną, pozostałe wartości 
                             // pola są podane w strukturze bazy tabela: [CDN.Towary].[Twr_Flaga]

            Towar.JM = "SZT";

            // Podstawienie grupy do towaru
            // Grupy dla towarów mają strukutrę drzewiastą która jest umieszczona w tabeli TwrGrupy
            CDNBase.ICollection Grupy = (CDNBase.ICollection)(Sesja.CreateObject("CDN.TwrGrupy", null));
            CDNTwrb1.TwrGrupa Grupa = (CDNTwrb1.TwrGrupa)Grupy["twg_kod = 'Grupa Główna'"];

            // Towar jest przypisany do jednje grupy domyślnej 
            Towar.TwGGIDNumer = Grupa.GIDNumer;
            // oraz do listy grup 
            CDNTwrb1.TwrGrupa GrupaZListy = (CDNTwrb1.TwrGrupa)Towar.Grupy.AddNew(null);
            GrupaZListy = Grupa;

            Console.WriteLine("podstawienie do grupy: " + Grupa.Kod);

            CDNTwrb1.ICena cana1 = (CDNTwrb1.ICena)Towar.Ceny[1];
            cana1.WartoscNetto = 29.29m;
            CDNTwrb1.ICena cana2 = (CDNTwrb1.ICena)Towar.Ceny[4];
            cana2.WartoscBrutto = 30.02m;


            // Zapis 
            Sesja.Save();
            Console.WriteLine("Towar dodany: " + nazwa);
        }

        // Przykład 5. - Dodanie faktury sprzedaży
        protected static void DodanieFaktury()
        {
            //  //CDNBASE.//Sesja = Login.CreateSession();
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);

            CDNHlmn.DokumentyHaMag Faktury = (CDNHlmn.DokumentyHaMag)Sesja.CreateObject("CDN.DokumentyHaMag", null);
            CDNHlmn.IDokumentHaMag Faktura = (CDNHlmn.IDokumentHaMag)Faktury.AddNew(null);

            CDNBase.ICollection Kontrahenci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Kontrahenci", null));
            CDNHeal.IKontrahent Kontrahent = (CDNHeal.IKontrahent)Kontrahenci["Knt_Kod='ALOZA'"];

            CDNBase.ICollection FormyPlatnosci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.FormyPlatnosci", null));
            OP_KASBOLib.FormaPlatnosci FPl = (OP_KASBOLib.FormaPlatnosci)FormyPlatnosci[1];

            // e_op_Rdz_FS 			302000
            Faktura.Rodzaj = 302000;
            // e_op_KlasaFS			302
            Faktura.TypDokumentu = 302;

            //Ustawiamy bufor
            Faktura.Bufor = 0;

            //Ustawiamy date
            Faktura.DataDok = new DateTime(2007, 06, 04);

            //Ustawiamy formę póatności
            Faktura.FormaPlatnosci = FPl;

            //Ustawiamy podmiot
            Faktura.Podmiot = (CDNHeal.IPodmiot)Kontrahent;

            //Ustawiamy magazyn
            Faktura.MagazynZrodlowyID = 1;

            //Dodajemy pozycje
            CDNBase.ICollection Pozycje = Faktura.Elementy;
            CDNHlmn.IElementHaMag Pozycja = (CDNHlmn.IElementHaMag)Pozycje.AddNew(null);
            Pozycja.TowarKod = "NOWY_C#";
            Pozycja.Ilosc = 2;
            //Pozycja.Cena0WD = 10;
            Pozycja.WartoscNetto = Convert.ToDecimal("123,13");

            //Dodanie atrybutu dokumentu TEKST
            CDNBase.ICollection rAtrybuty = (CDNBase.ICollection)(Sesja.CreateObject("CDN.DefAtrybuty", null));
            CDNTwrb1.IDefAtrybut rAtrybut = (CDNTwrb1.IDefAtrybut)rAtrybuty["dea_Kod = 'TEKST'"];
            CDNTwrb1.IDokAtrybut rAtrybutDokumentu = (CDNTwrb1.IDokAtrybut)Faktura.Atrybuty.AddNew(null);
            rAtrybutDokumentu.DefAtrybut = (CDNTwrb1.DefAtrybut)rAtrybut;
            rAtrybutDokumentu.Wartosc = "Nr:XP123456";

            // Atrybut można też podstawić za pomocą ID atrybutu bez tworzenia kolekcji atrybutów:
            // rAtrybutDokumentu.DeAID = 123

            //zapisujemy			
            Sesja.Save();

            Console.WriteLine("Dodano fakturę: " + Faktura.NumerPelny);
        }
        // Przykład 6. - Korekta FA
        protected static void DogenerujKorekteFA()
        {

            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            //  //CDNBASE.//Sesja = Login.CreateSession();

            CDNHlmn.DokumentyHaMag Faktury = (CDNHlmn.DokumentyHaMag)Sesja.CreateObject("CDN.DokumentyHaMag", null);
            CDNHlmn.IDokumentHaMag Faktura = (CDNHlmn.IDokumentHaMag)Faktury.AddNew(null);

            // FAKI 302001; 
            // FAKW 302002; 
            // FAKV 302003;

            //ustawiamy rodzaj i typ dokumentu
            Faktura.Rodzaj = 302001;
            Faktura.TypDokumentu = 302;

            //Rodzaj korekty
            Faktura.Korekta = 1;
            //Bufor
            Faktura.Bufor = 1;
            //Id korygowanej faktury
            Faktura.OryginalID = 1;

            //Zmieniamy ilosc
            CDNBase.ICollection Pozycje = Faktura.Elementy;
            CDNHlmn.IElementHaMag Pozycja = (CDNHlmn.IElementHaMag)Pozycje["Tre_TwrKod='NOWY_C#'"];
            Pozycja.Ilosc = 1;

            //Zapisujemy
            Sesja.Save();

            Console.WriteLine("Dogenerowano korektę: " + Faktura.NumerPelny);
        }
        // Przykład 7. - Dogenerowanie WZ do FA
        protected static void Dogenerowanie_WZ_Do_FA()
        {
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            // //CDNBASE.//Sesja = Login.CreateSession();
            //ADODB.Connection cnn	= new ADODB.ConnectionClass();
            ADODB.Connection cnn = Sesja.Connection;
            //cnn.Open( cnn.ConnectionString, "", "", 0);


            ADODB.Recordset rRs = new ADODB.RecordsetClass();
            string select = "select top 1 TrN_TrNId as ID from cdn.TraNag where TrN_Rodzaj = 302000 ";

            rRs.Open(select, cnn, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            // Wykreowanie obiektu serwisu:
            CDNHlmn.SerwisHaMag Serwis = (CDNHlmn.SerwisHaMag)Sesja.CreateObject("Cdn.SerwisHaMag", null);

            // Samo gemerowanie dok. FA
            int MAGAZYN_ID = 1;
            Serwis.AgregujDokumenty(302, rRs, MAGAZYN_ID, 306, null);

            //Zapisujemy
            Sesja.Save();
        }



        #region Przykład 8 - Dodanie zlecenia serwisowego
        #region COM_DOK
        /*<COM_DOK>
<OPIS>Dodanie zlecenia serwisowego</OPIS> 
<Uruchomienie>Przykład działa na bazie DEMO</Uruchomienie>
<Interfejs> ISrsZlecenie </<Interfejs>
<Interfejs> ISrsCzynnosc </<Interfejs>
<Interfejs> ISrsCzesc </<Interfejs>
<Osoba>WN</Osoba> 
<Jezyk>C#</Jezyk> 
<OPT_VER>17.3.1</OPT_VER>
</COM_DOK>*/
        #endregion


        public static void DodanieZleceniaSerwisowego()
        {
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            try
            {
                Console.WriteLine("##### dodawanie zlecenia serwisowego #####");
                Console.WriteLine("Kreuję sesję ...");
                //     //CDNBASE.//Sesja = Login.CreateSession();
                Console.WriteLine("Kreuję kolekcję zleceń serwisowych ...");

                OP_CSRSLib.SrsZlecenia zlecenia = (OP_CSRSLib.SrsZlecenia)Sesja.CreateObject("CDN.SrsZlecenia", null);
                Console.WriteLine("Dodaję nowe zlecenie do kolekcji zleceń serwisowych ...");
                OP_CSRSLib.ISrsZlecenie zlecenie = (OP_CSRSLib.ISrsZlecenie)zlecenia.AddNew(null);

                Console.WriteLine("Kreuję kolekcje kontrahentów ...");
                CDNBase.ICollection kontrahenci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Kontrahenci", null));
                Console.WriteLine("I pobieram z niej obiekt kontrahenta o kodzie 'ALOZA' ...");
                CDNHeal.IKontrahent kontrahent = (CDNHeal.IKontrahent)kontrahenci["Knt_Kod='ALOZA'"];

                Console.WriteLine("Teraz obiekt kontrahenta podstawiam do property Podmiot dla zlecenia ...");
                zlecenie.Podmiot = (CDNHeal.IPodmiot)kontrahent;

                Console.WriteLine("Dzisiejszą datę podstawiam jako datę utworzenia zlecenia...");
                zlecenie.DataDok = DateTime.Now;

                Console.WriteLine("Pobieram kolekcję czynności przypisanych do zlecenia...");
                CDNBase.ICollection czynnosci = zlecenie.Czynnosci;
                Console.WriteLine("I dodaję do niej nowy obiekt...");
                OP_CSRSLib.ISrsCzynnosc czynnosc = (OP_CSRSLib.ISrsCzynnosc)czynnosci.AddNew(null);

                Console.WriteLine("Przypisuję usługę o kodzie PROJ_ZIELENI do tej czynności...");
                czynnosc.TwrId = GetIdTowaru(Sesja, "PROJ_ZIELENI");
                Console.WriteLine("Ilość jednostek ustalam na dwie...");
                czynnosc.Ilosc = 2;
                czynnosc.CenaNetto = 100;
                czynnosc.CzasTrwania = new DateTime(1899, 12, 30, 12, 0, 0);   //12 godzin
                czynnosc.KosztUslugi = 48;

                Console.WriteLine("Teraz dodaję części ...");
                CDNBase.ICollection czesci = zlecenie.Czesci;
                Console.WriteLine("I dodaję do niej nowy obiekt...");
                OP_CSRSLib.ISrsCzesc czesc = (OP_CSRSLib.ISrsCzesc)czesci.AddNew(null);

                Console.WriteLine("Przypisuję towar o kodzie IGLAKI_CYPRYS ...");
                czesc.TwrId = GetIdTowaru(Sesja, "IGLAKI_CYPRYS");
                Console.WriteLine("Ilość jednostek ustalam na trzy...");
                czesc.Ilosc = 3;
                czesc.CenaNetto = 99.80m;
                czesc.Fakturowac = 1; //do fakturowania

                Console.WriteLine("Przypisuję towar o kodzie ZIEMIA_5 ...");
                czesc.TwrId = GetIdTowaru(Sesja, "ZIEMIA_5");
                Console.WriteLine("Ilość jednostek ustalam na pięć...");
                czesc.Ilosc = 5;
                czesc.CenaNetto = 4.90m;
                czesc.Fakturowac = 1; //do fakturowania


                Console.WriteLine("Zapisuję sesję...");
                Sesja.Save();
                zlecenie = (OP_CSRSLib.ISrsZlecenie)zlecenia[String.Format("SrZ_SrZId={0}", zlecenie.ID)];
                Console.WriteLine("Dodano zlecenie: {0}\nCzas trwania czynności: {1}:{2}\nKoszt: {3}\nWartość netto w cenach sprzedaży: {4}\nWartość netto do zafakturowania : {5}",
                    zlecenie.NumerPelny,
                    zlecenie.CzynnosciCzasTrwania / 100,
                    (zlecenie.CzynnosciCzasTrwania % 100).ToString("00"),
                    zlecenie.Koszt.ToString("#0.00"),
                    zlecenie.WartoscNetto.ToString("#0.00"),
                    zlecenie.WartoscNettoDoFA.ToString("#0.00"));
            }
            catch (COMException comError)
            {
                Console.WriteLine("###ERROR### Dodanie zlecenia nie powiodło się!\n{0}", ErrorMessage(comError));
            }
        }

        private static int GetIdTowaru(CDNBase.AdoSession Sesja, string kod)
        {
            if (String.IsNullOrEmpty(kod)) return 0;
            int Id = 0;
            Id = Convert.ToInt32(GetSingleValue(Sesja, String.Format("Select IsNull(Twr_TwrId, 0) From cdn.Towary Where Twr_Kod = '{0}'", kod), false));
            return Id;
        }

        private static object GetSingleValue(CDNBase.AdoSession Sesja, string query, bool configCnn)
        {
            ADODB.Connection cn = configCnn ? Sesja.ConfigConnection : Sesja.Connection;
            ADODB.Recordset rs = new ADODB.RecordsetClass();
            rs.Open(query, cn, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
                return rs.Fields[0].Value;
            else
                return null;
        }


        protected static string ErrorMessage(System.Exception e)
        {
            StringBuilder mess = new StringBuilder();
            if (e != null)
            {
                mess.Append(e.Message);
                while (e.InnerException != null)
                {
                    mess.Append(e.InnerException.Message);
                    e = e.InnerException;
                }
            }
            return mess.ToString();
        }
        #endregion Przykład 8 - Dodanie zlecenia serwisowaego

        #region Przykład 9 - Dodanie dokumentu OBD
        #region COM_DOK
        /*<COM_DOK>
<OPIS>Dodanie dokumentu OBD</OPIS> 
<Uruchomienie>Przykład działa na bazie DEMO. </Uruchomienie>
<Interfejs> IDokNag </<Interfejs>
<Interfejs> IKontrahent </<Interfejs>
<Interfejs> INumerator </<Interfejs>
<Osoba>WN</Osoba> 
<Jezyk>C#</Jezyk> 
<OPT_VER>17.3.1</OPT_VER>
</COM_DOK>*/
        #endregion

        public static void DodanieDokumentuOBD()
        {
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            try
            {
                Console.WriteLine("##### dodawanie dokumentu OBD #####");
                Console.WriteLine("Kreuję sesję ...");
                //CDNBASE.//Sesja = Login.CreateSession();
                Console.WriteLine("Kreuję kolekcję dokumentów ...");
                OP_SEKLib.DokNagColl dokumenty = (OP_SEKLib.DokNagColl)Sesja.CreateObject("CDN.DokNagColl", null);
                Console.WriteLine("Dodaję nowy dokument do kolekcji ...");
                OP_SEKLib.IDokNag dokument = (OP_SEKLib.IDokNag)dokumenty.AddNew(null);
                dokument.Typ = 1;  //firmowy 

                //jeśli seria dokumentu ma być inna niż domyślna
                const string symbolDD = "AUTO";
                CDNHeal.DefinicjeDokumentow definicjeDokumentow = (CDNHeal.DefinicjeDokumentow)Sesja.CreateObject("CDN.DefinicjeDokumentow", null);
                CDNHeal.DefinicjaDokumentu definicjaDokumentu = (CDNHeal.DefinicjaDokumentu)definicjeDokumentow[string.Format("Ddf_Symbol='OBD'", symbolDD)];
                //Ustawiamy numerator
                OP_KASBOLib.INumerator numerator = (OP_KASBOLib.INumerator)dokument.Numerator;

                numerator.DefinicjaDokumentu = definicjaDokumentu;

                numerator.Rejestr = "A1";//ustawiam serię dla wybranej definicji dokumentów
                //numerator.Rejestr = 133; //jeśli potrzeba ustawić numer

                Console.WriteLine("Dzisiejszą datę podstawiam jako datę utworzenia...");
                //dokument.DataDok = DateTime.Now;     //jeśli data dokumentu ma być inna niż bieżąca
                dokument.Tytul = "Rozpoczęcie procesu akwizycji";
                dokument.Dotyczy = "Opis kroków do wykonania. Proszę zacząć od dzisiaj!";

                //Dodanie podmiotu
                Console.WriteLine("Kreuję kolekcję kontrahentów ...");
                CDNBase.ICollection kontrahenci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Kontrahenci", null));
                Console.WriteLine("I pobieram z niej obiekt kontrahenta o kodzie 'ALOZA' ...");
                CDNHeal.IKontrahent kontrahent = (CDNHeal.IKontrahent)kontrahenci["Knt_Kod='ALOZA'"];

                dokument.PodmiotTyp = 1; //TypPodmiotuKontrahent
                dokument.PodId = kontrahent.ID;
                Console.WriteLine("Zapisuję dokument... " + dokument.Stempel.TSMod);


                // Dodanie nagłówka pliku
                OP_SEKLib.IDokNagPlik DokNagPlik = (OP_SEKLib.IDokNagPlik)dokument.PlikiArchiwalne.AddNew(null);

                // ustalenie koniecznych atrybutów 
                DokNagPlik.UstawStempel();
                DokNagPlik.DoNID = dokument.ID;
                DokNagPlik.FileMode = 2;
                DokNagPlik.WBazie = 1;
                DokNagPlik.NazwaPliku = "testfile.html";
                DokNagPlik.Wersja = 0;

                // Tworzenie danych binarnych 

                CDNTwrb1.IDanaBinarna PlikDaneBin = (CDNTwrb1.IDanaBinarna)DokNagPlik.PlikiBinarne.AddNew(null);

                //koniczne atrybuty pliku binarnego oraz import danych
                PlikDaneBin.TWAID = DokNagPlik.ID;
                PlikDaneBin.DodajPlik(@"C:\testfile.html");
                PlikDaneBin.Typ = 2;
                PlikDaneBin.NazwaPliku = "testfile.html";
                // Zapis dok.                                    


                Sesja.Save();
                dokument = (OP_SEKLib.IDokNag)dokumenty[String.Format("DoN_DoNId={0}", dokument.ID)];
                Console.WriteLine("Dodano dokument numer: {0}", dokument.NumerPelny);
            }
            catch (COMException comError)
            {
                Console.WriteLine("###ERROR### Dodanie dokumentu OBD nie powiodło się!\n{0}", ErrorMessage(comError));
            }
        }
        #endregion

        #region Przykład 10. - Utworzenie nowego polecenia księgowania

        protected static void UtworzeniePK()
        {
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            //Sesja = Login.CreateSession();
            /* 
             * Pobranie ID bieżącego okresu obrachunkowego z konfigracji Opt!my
             */
            CDNKONFIGLib.IKonfiguracja knf = (CDNKONFIGLib.IKonfiguracja)Sesja.Login.CreateObject("CDNKonfig.Konfiguracja");
            string val = knf.get_Value("BiezOkresObr");
            int OObID = (val.Length == 0) ? 0 : Convert.ToInt32(val);

            /* stworzenie nowego PK */
            CDNKH.Dekrety dekret = (CDNKH.Dekrety)Sesja.CreateObject("CDN.Dekrety", null);
            CDNKH.IDekret idekret = (CDNKH.IDekret)dekret.AddNew(null);

            /* pobranie obiektu okresu obrachunkowego (jeżeli konieczne) – tutaj już mamy okres do jakiego dodajemy PK
             * ICollection okresy = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Okresy", null));
             * CDNDave.IOkres iokres = (CDNDave.IOkres)okresy["OOb_OObID=" + OObID];
             */

            /* 
             * pobranie dziennika
             * pobieranie dzinnika po samym ID nie jest dobre: 
             *      dziennik jest przypisany do okresu obrachunkowego!
             *      lepiej po symbolu oraz właśnie ID okresu do jakiego będzie dodane PK
             */
            ICollection dziennik = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Dzienniki", null));
            //CDNDave.IDziennik idzienniki = (CDNDave.IDziennik)dziennik["Dzi_DziID = 26"];
            CDNDave.IDziennik idzienniki = (CDNDave.IDziennik)dziennik["Dzi_Symbol = 'BANK' AND Dzi_OObID=" + OObID];

            /* stworzenie kontrahenta */
            ICollection Kontrahenci = (CDNBase.ICollection)(Sesja.CreateObject("CDN.Kontrahenci", null));
            CDNHeal.IKontrahent kontrahent = (CDNHeal.IKontrahent)Kontrahenci["Knt_Kod='ADM'"];

            /*
             * podstawienie OObID nie jest konieczne - dziennik wie, do jakiego okresu należy
             */
            //idekret.OObId = iokres.ID;
            idekret.DziID = idzienniki.ID;
            idekret.DataDok = new DateTime(2010, 06, 9);

            /*
             * podstawienie dataOpe i DataWys nie jest konieczne, jeżeli mają być takie same jak DataDok 
             */
            //idekret.DataOpe = new DateTime(2010, 06, 9);
            //idekret.DataWys = new DateTime(2010, 06, 9);

            idekret.Bufor = 1;
            idekret.Podmiot = (CDNHeal.IPodmiot)kontrahent;
            idekret.Dokument = "A1234";

            // Dodawania pozycji do dokumentu
            ICollection elementy = idekret.Elementy;
            CDNKH.IDekretElement ielement = (CDNKH.IDekretElement)elementy.AddNew(null);

            ICollection kategorie = (ICollection)Sesja.CreateObject("CDN.Kategorie", null);
            CDNHeal.IKategoria ikategoria = (CDNHeal.IKategoria)kategorie["Kat_KodSzczegol = 'INNE'"];
            ielement.Kategoria = (CDNHeal.Kategoria)ikategoria;

            /*
             * Te daty przejmowane są z PK
             */
            // ielement.DataOpe = new DateTime(2010, 06, 9);
            // ielement.DataWys = new DateTime(2010, 06, 9);

            ICollection konta = (ICollection)Sesja.CreateObject("CDN.Konta", null);

            /*
             * Konta ksiegowe też są przypisane do okresu obrachunkowego!
             * Wybieranie konta po samym numerze to za mało: trzeba jeszcze wybrać z jakiego okresu ma być konto!
             */
            CDNKH.IKonto kontoWn = (CDNKH.IKonto)konta["Acc_Numer = '100' AND Acc_OObID=" + OObID];
            CDNKH.IKonto kontoMa = (CDNKH.IKonto)konta["Acc_Numer = '131' AND Acc_OObID=" + OObID];

            ielement.KontoWn = (CDNKH.Konto)kontoWn;
            ielement.KontoMa = (CDNKH.Konto)kontoMa;

            ielement.Kwota = 100;

            // Koniec dodawania pozycji do dokumentu
            Sesja.Save();
        }

        #endregion



        // dla obiektów Clarionowych, których nie można tak łatwo używać w C#
        public static void InvokeMethod(object o, string Name, object[] Params)
        {
            o.GetType().InvokeMember(Name, System.Reflection.BindingFlags.InvokeMethod, null, o, Params, null, null, null);
        }
        // dla obiektów Clarionowych, których nie można tak łatwo używać w C#
        public static void SetProperty(object o, string Name, object Value)
        {
            if (o == null)
                return;
            o.GetType().InvokeMember(Name, System.Reflection.BindingFlags.SetProperty, null, o, new object[] { Value });
        }
        // dla obiektów Clarionowych, których nie można tak łatwo używać w C#
        public static object GetProperty(object o, string Name)
        {
            if (o == null)
                return null;
            return o.GetType().InvokeMember(Name, System.Reflection.BindingFlags.GetProperty, null, o, null);
        }

        enum SposobOdbieraniaNadg
        {
            Brak = 1,
            OBM = 2,  //Nadgodziny do odbioru w bieżącym miesiącu 
            ONM = 3,  //Nadgodziny odbierane w kolejnym miesiącu
            WPL = 4,   //Wolne za nadgodziny (płatne)
            WNP = 5   //Wolne za nadgodziny (niepłatne)
        }


        public static void DodanieObecnosciPracownik()
        {
            //CDNBASE.//Sesja = Login.CreateSession();
            //var Sesja = CloneObject<CDNBase.AdoSession>(Sesja);
            //sprawdzam, czy jest modul PK XL
            Boolean CzyJestModulPKXL = (Sesja.Login.get_Moduly(9) == 1) ? true : false;

            //dzien, na który wprowadzamy obecnosc
            DateTime Data = new DateTime(2008, 10, 28);

            //Wykreowanie obiektu pracownika o akronimie 'TEST'
            CDNPrac.Pracownicy Pracownicy = (CDNPrac.Pracownicy)Sesja.CreateObject("CDN.Pracownicy", null);
            CDNPrac.IPracownik Pracownik = (CDNPrac.IPracownik)Pracownicy["Pra_Kod = 'TEST'"];


            //sprawdzam, czy pracowanik byl zatrudniony na dzien wprowadzania obecnosci
            CDNPrac.IHistoria Historia = Pracownik.GetHistoriaAktualna(Data, 0);
            if (Historia.Zatrudnienie.Period.ContainDate(Data) == 0
                || Data > Historia.Zatrudnienie.Period.To.AddDays(-1))
            {
                throw new System.Exception("Dzień poza okresem zatrudnienia.!");
            }

            //czas pracy pracownika
            OP_KALBLib.CzasPracy CzasPracy = (OP_KALBLib.CzasPracy)Pracownik.CzasPracy;
            OP_KALBLib.IDzienPracy DzienPracy;
            try
            {
                DzienPracy = (OP_KALBLib.IDzienPracy)CzasPracy[Data];
                throw new System.Exception("Na dany dzień wprowadzono już zapis!");
                /* Jesli chcemy taki zapis usunac to */
                //CzasPracy.Delete(Data);
            }
            catch {  /* nieładnie tak zostawiać wyjątek, ale... :) */ }

            //Dodanie dnia
            DzienPracy = (OP_KALBLib.IDzienPracy)CzasPracy.AddNew(Data);

            #region WEJSCIE - 1
            {
                //    //Ustawienie godzin wejścia/wyjścia
                //    DzienPracy.OdGodziny_1 = new DateTime(1899, 12, 30, 8, 12, 0);  //kreujemy datę 1899-12-30 z godzina wejscia
                //    DzienPracy.DoGodziny_1 = new DateTime(1899, 12, 30, 10, 23, 0); //kreujemy datę 1899-12-30 z godzina wyjscia

                //    //Ustawienie strefy
                //    OP_KALBLib.Strefa Strefa = (OP_KALBLib.Strefa)Sesja.CreateObject("CDN.Strefy", "DST_Akronim = 'praca.pdst'");
                //    //OP_KALBLib.IStrefa Strefa = (OP_KALBLib.IStrefa)Strefy[];
                //    DzienPracy.Strefa_1 = Strefa;

                //    //sprawdzic, czy jest licencja na duze place
                //    if (CzyJestModulPKXL)
                //    {
                //        //Ustawienie dzialu
                //        CDNPrac.IDzial Dzial = (CDNPrac.IDzial)Sesja.CreateObject("CDNPrac.Dzialy", "DZL_AdresWezla = '1'");
                //        //CDNPrac.IDzial Dzial = (CDNPrac.IDzial)Dzialy[];
                //        DzienPracy.Dzial_1 = Dzial;

                //        //Ustawienie projektu
                //        CDNDave.IProjekt Projekt = (CDNDave.IProjekt)Sesja.CreateObject("CDN.Projekty", "PRJ_Wezel = '  3.  1'");
                //        //CDNDave.IProjekt Projekt = (CDNDave.IProjekt)Projekty[];
                //        DzienPracy.Projekt_1 = Projekt;

                //        //Ustawienie sposobu odbioru nadgodzin
                //        DzienPracy.OdbNadg_1 = (int)SposobOdbieraniaNadg.Brak;

                //    }
            }


            #endregion WEJSCIE - 1


            #region WEJSCIE - 2
            {
                //sprawdzic, czy jest licencja na duze place
                //if (CzyJestModulPKXL)
                //{
                //    //Ustawienie godzin wejścia/wyjścia
                //    DzienPracy.OdGodziny_2 = new DateTime(1899, 12, 30, 8, 12, 0);  //kreujemy datę 1899-12-30 z godzina wejscia
                //    DzienPracy.DoGodziny_2 = new DateTime(1899, 12, 30, 10, 23, 0); //kreujemy datę 1899-12-30 z godzina wyjscia

                //    //Ustawienie strefy
                //    OP_KALBLib.Strefa Strefa = (OP_KALBLib.Strefa)Sesja.CreateObject("CDN.Strefy", "DST_Akronim = 'praca.pdst'");
                //    //OP_KALBLib.IStrefa Strefa = (OP_KALBLib.IStrefa)Strefy[];
                //    DzienPracy.Strefa_2 = Strefa;

                //    //Ustawienie dzialu
                //    CDNPrac.IDzial Dzial = (CDNPrac.IDzial)Sesja.CreateObject("CDNPrac.Dzialy", "DZL_AdresWezla = '1'");
                //    //CDNPrac.IDzial Dzial = (CDNPrac.IDzial)Dzialy[];
                //    DzienPracy.Dzial_2 = Dzial;

                //    //Ustawienie projektu
                //    CDNDave.IProjekt Projekt = (CDNDave.IProjekt)Sesja.CreateObject("CDN.Projekty", "PRJ_Wezel = '  3.  1'");
                //    //CDNDave.IProjekt Projekt = (CDNDave.IProjekt)Projekty[];
                //    DzienPracy.Projekt_2 = Projekt;

                //    //Ustawienie sposobu odbioru nadgodzin
                //    DzienPracy.OdbNadg_2 = (int)SposobOdbieraniaNadg.Brak;

                //}
            }
            #endregion WEJSCIE - 2
            //Zapis zmian
            Sesja.Save();
        }

        public static void DodajAtrybuty()
        {
            Console.WriteLine("Edycja atrybutu");
        }
    }
}
