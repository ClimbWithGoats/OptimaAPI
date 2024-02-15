using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
//KartyTowarowe
namespace OptimaAPI.Repositories
{

    public class MerchandiseCardsRepository : IMerchandiseCardsRepository
    {
        private readonly DapperContext _context;
        public MerchandiseCardsRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }
        public async Task<IEnumerable<MerchandiseCardExt>> GetMerchandiseCards()
        {

            string query = @"SELECT DISTINCT t.Twr_TwrId AS TwrId
	,t.Twr_Kod AS TwrKod
    ,t.Twr_JM as TwrJM
	,t.Twr_NumerKat AS TwrNrKatalogowy
	,(
		SELECT TOP (1) tg.TwG_Kod
		FROM cdn.TwrGrupy tg
		WHERE tg.TwG_GIDNumer = t.Twr_TwGGIDNumer
		) AS TwgKodGrupy
	,(
		SELECT TOP (1) TwG_Nazwa
		FROM cdn.TwrGrupy tg
		WHERE tg.TwG_GIDNumer = t.Twr_TwGGIDNumer
		) AS TwgNazwa
	,t.Twr_Typ AS TwrTyp
	,t.Twr_EAN AS TwrEan
	,t.Twr_SWW AS TwrSSW
	,(SELECT TOP(1)  KCN_Kod FROM CDN.KodyCN where KCN_KCNId = t.Twr_KCNId)  AS KodCN
	,t.Twr_Stawka AS StawkaVatSprz
	,t.Twr_StawkaZak AS StawkaVatZak
	,Twr_TwCNumer AS TwcNumerSelect
	,t.Twr_SplitPay AS TwrSplitPay
	,t.Twr_OdwrotneObciazenie as TwrOdwrotneObciazenie
	,t.Twr_Nazwa as TwrNazwa
	,Twr_Kategoria AS TwrKat
	,Twr_KategoriaZak AS TwrKatZak
	,t.Twr_JM AS TwrJM
	,t.Twr_JMCalkowite AS TwrJmCalowite
	,(
SELECT   
    TwC_TwCNumer AS TwcNumer,
    DfC_Typ AS TypCeny,
    ISNULL(DfC_Nieaktywna, 0) AS TwCNieaktywna,
    TwC_Marza AS TwcMarza,
    DfC_Zaokraglenie AS DfcZaokraglenie,
    DfC_Offset AS Offset,
    TwC_Wartosc AS Wartosc ,   
    CASE 
        WHEN DfC_Typ = 1 THEN TwC_Wartosc  -- Jeśli typ ceny to netto, zwróć cenę netto
        WHEN DfC_Typ = 2 THEN (TwC_Wartosc / (1 + Twr_StawkaZak))  -- Jeśli typ ceny to brutto, oblicz cenę netto
    END AS CenaNetto,
	  CASE 
        WHEN DfC_Typ = 1 THEN (TwC_Wartosc * (1 + Twr_StawkaZak))  -- Jeśli typ ceny to brutto, oblicz cenę netto
        WHEN DfC_Typ = 2 THEN TwC_Wartosc  -- Jeśli typ ceny to netto, zwróć cenę netto
    END AS CenaBrutto,
    DfC_Waluta as DFCWaluta
FROM 
    CDN.TwrCeny B 
JOIN 
    CDN.DEFCeny CDNDEFCeny ON DfC_Lp = TwC_TwCNumer 
WHERE 
    Twc_TwrID = t.Twr_TwrId
ORDER BY 
    CASE 
        WHEN Twc_TwCNumer >= 0 THEN Twc_TwCNumer 
        ELSE -Twc_TwCNumer + (SELECT MAX(twc_twcnumer) FROM cdn.twrceny) 
    END
	For Json Auto
) as ListaCen,

(SELECT TwV_TwVId as Id, TwV_KodKraju as KodKraju, (select Top (1) Kra_Nazwa from CDN_KNF_Konfiguracja.CDN.Kraje where Kra_Kod = TwV_KodKraju) as NazwaKraju,  TwV_Stawka as Stawka FROM CDN.TwrStawkiVAT WHERE TwV_TwrId= t.Twr_TwrId for json auto) as StawkiVAT
,
(SELECT TwJZ_TwJZId as Id, TwJZ_JM as JednostkaPomocnicza, TwJZ_JMPrzelicznikL as Licznik, TwJZ_JMPrzelicznikM as Mianownik, '1 ' + TwJZ_JM+ ' = '+ CAST(TwJZ_JMPrzelicznikL AS VARCHAR(20)) + ' szt.' as Przelicznik, TwJZ_CenaJednostkowa, TwJZ_WysCm as Wysokosc, TwJZ_SzerCm as Szerokosc, TwJZ_DlugCm as Dlugosc FROM CDN.TwrJMZ WHERE TwJZ_TwrId= t.Twr_TwrId  For json Auto) as PomJednMiary ,
(SELECT TwE_TwEId as Id, TwE_EAN as KodEAN, TwE_Opis as Opis, TwE_JM as Jednostka, TwE_Domyslny as Domyslny FROM CDN.TwrEan WHERE TwE_TwrId= t.Twr_TwrId for json auto) as KodyEan
,
(
SELECT tgv.GIDNumer as TwGID, tgv.Sciezka as Sciezka, tgv.Nazwa as Nazwa, tgv.Kod as Kod FROM CDN.TwrGrupy JOIN CDN.TwrGrupyView tgv on TwG_GRONumer = GIDNumer   
WHERE TwG_GIDNumer= t.Twr_TwrId AND TwG_GIDTyp = t.Twr_GIDTyp For json Auto) as Grupy 
FROM cdn.TwrKarty tk
LEFT OUTER JOIN cdn.Towary t ON tk.Twr_GIDNumer = t.Twr_GIDNumer
LEFT OUTER JOIN cdn.TwrAtrybuty da ON da.TwA_TwrId = t.Twr_GIDNumer
LEFT OUTER JOIN CDN.DefAtrybuty defa ON da.TwA_DeAId = defa.DeA_DeAId";
            try
            {
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<MerchandiseCard>(query);

                List<MerchandiseCardExt> merchandiseCardExts = form
          .Select(mc => new MerchandiseCardExt(mc))
          .ToList();

                return merchandiseCardExts;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return new List<MerchandiseCardExt>();

        }



        //public async Task<IEnumerable<MerchandiseCard>> GetMerchandiseCards()
        //{
        //    string query = @"SELECT 
        //                    Twr_GIDTyp as TwrGIDTyp 
        //                    ,Twr_GIDFirma as TwrGIDFirma
        //                    ,Twr_GIDNumer as TwrGIDNumer
        //                    ,Twr_GIDLp as TwrGIDLp
        //                    ,Twr_Typ as TwrTyp
        //                    ,Twr_Kod as TwrKod
        //                    ,Twr_KodFormat as TwrKodFormat
        //                    ,Twr_FPPKod as TwrFPPKod
        //                    ,Twr_Nazwa as TwrNazwa
        //                    ,Twr_Nazwa1 as TwrNazwa1
        //                    ,Twr_Certyfikat as TwrCertyfikat
        //                    ,Twr_Sww as TwrSww
        //                    ,Twr_Ean as TwrEan
        //                    ,Twr_Jm as TwrJm
        //                    ,Twr_Waga as TwrWaga
        //                    ,Twr_WJm as TwrWJm
        //                    ,Twr_JmFormat as TwrJmFormat
        //                    ,Twr_IloscZam as TwrIloscZam
        //                    ,Twr_IloscMin as TwrIloscMin
        //                    ,Twr_IloscMax as TwrIloscMax
        //                    ,Twr_Ubytek as TwrUbytek
        //                    ,Twr_Prog as TwrProg
        //                    ,Twr_Upust as TwrUpust
        //                    ,Twr_UpustData as TwrUpustData
        //                    ,Twr_UpustDataOd as TwrUpustDataOd
        //                    ,Twr_UpustDataDo as TwrUpustDataDo
        //                    ,Twr_UpustGodz as TwrUpustGodz
        //                    ,Twr_UpustGodzOd as TwrUpustGodzOd
        //                    ,Twr_UpustGodzDo as TwrUpustGodzDo
        //                    ,Twr_AutoZam as TwrAutoZam
        //                    ,Twr_IloscAZam as TwrIloscAZam
        //                    ,Twr_CzasDst as TwrCzasDst
        //                    ,Twr_CenaSpr as TwrCenaSpr
        //                    ,Twr_JmDomyslna as TwrJmDomyslna
        //                    ,TWR_JMDomyslnaZak as TWRJMDomyslnaZak
        //                    ,Twr_DstDomyslny as TwrDstDomyslny
        //                    ,Twr_RozliczMag as TwrRozliczMag
        //                    ,Twr_Zakup as TwrZakup
        //                    ,Twr_Sprzedaz as TwrSprzedaz
        //                    ,Twr_GrupaPod as TwrGrupaPod
        //                    ,Twr_Akcyza as TwrAkcyza
        //                    ,Twr_OpeTyp as TwrOpeTyp
        //                    ,Twr_OpeFirma as TwrOpeFirma
        //                    ,Twr_OpeNumer as TwrOpeNumer
        //                    ,Twr_OpeLp as TwrOpeLp
        //                    ,Twr_PrcTyp as TwrPrcTyp
        //                    ,Twr_PrcFirma as TwrPrcFirma
        //                    ,Twr_PrcNumer as TwrPrcNumer
        //                    ,Twr_PrcLp as TwrPrcLp
        //                    ,Twr_KontaktTyp as TwrKontaktTyp
        //                    ,Twr_KontaktZa as TwrKontaktZa
        //                    ,Twr_KontaktJC as TwrKontaktJC
        //                    ,Twr_Okresowy as TwrOkresowy
        //                    ,Twr_Cel as TwrCel
        //                    ,Twr_Atrybut1 as TwrAtrybut1
        //                    ,Twr_Format1 as TwrFormat1
        //                    ,Twr_Wartosc1 as TwrWartosc1
        //                    ,Twr_Atrybut2 as TwrAtrybut2
        //                    ,Twr_Format2 as TwrFormat2
        //                    ,Twr_Wartosc2 as TwrWartosc2
        //                    ,Twr_Atrybut3 as TwrAtrybut3
        //                    ,Twr_Format3 as TwrFormat3
        //                    ,Twr_Wartosc3 as TwrWartosc3
        //                    ,Twr_Punkty as TwrPunkty
        //                    ,Twr_Koncesja as TwrKoncesja
        //                    ,Twr_Konto1 as TwrKonto1
        //                    ,Twr_Konto2 as TwrKonto2
        //                    ,Twr_Konto3 as TwrKonto3
        //                    ,Twr_Polozenie as TwrPolozenie
        //                    ,Twr_Katalog as TwrKatalog
        //                    ,Twr_WCenniku as TwrWCenniku
        //                    ,Twr_EdycjaNazwy as TwrEdycjaNazwy
        //                    ,Twr_BezRabatu as TwrBezRabatu
        //                    ,Twr_KopiujOpis as TwrKopiujOpis
        //                    ,Twr_URL as TwrURL
        //                    ,Twr_Warunek as TwrWarunek
        //                    ,Twr_ObjetoscL as TwrObjetoscL
        //                    ,Twr_ObjetoscM as TwrObjetoscM
        //                    ,Twr_LastModL as TwrLastModL
        //                    ,Twr_LastModO as TwrLastModO
        //                    ,Twr_LastModC as TwrLastModC
        //                    ,Twr_TerminZwrotu as TwrTerminZwrotu
        //                    ,Twr_ZakupAutoryz as TwrZakupAutoryz
        //                    ,Twr_MagTyp as TwrMagTyp
        //                    ,Twr_MagFirma as TwrMagFirma
        //                    ,Twr_MagNumer as TwrMagNumer
        //                    ,Twr_MagLp as TwrMagLp
        //                    ,Twr_MarzaMin as TwrMarzaMin
        //                    ,Twr_KosztUslugi as TwrKosztUslugi
        //                    ,Twr_KosztUTyp as TwrKosztUTyp
        //                    ,Twr_Clo as TwrClo
        //                    ,Twr_PodatekImp as TwrPodatekImp
        //                    ,Twr_StanInfoLimit as TwrStanInfoLimit
        //                    ,Twr_StanInfoMax as TwrStanInfoMax
        //                    ,Twr_StanInfoProcent as TwrStanInfoProcent
        //                    ,Twr_Aktywna as TwrAktywna
        //                    ,Twr_Wsk as TwrWsk
        //                    ,Twr_CCKTyp as TwrCCKTyp
        //                    ,Twr_CCKFirma as TwrCCKFirma
        //                    ,Twr_CCKNumer as TwrCCKNumer
        //                    ,Twr_CCKLp as TwrCCKLp
        //                    ,Twr_PrdTyp as TwrPrdTyp
        //                    ,Twr_PrdFirma as TwrPrdFirma
        //                    ,Twr_PrdNumer as TwrPrdNumer
        //                    ,Twr_PrdLp as TwrPrdLp
        //                    ,Twr_OpeTypM as TwrOpeTypM
        //                    ,Twr_OpeFirmaM as TwrOpeFirmaM
        //                    ,Twr_OpeNumerM as TwrOpeNumerM
        //                    ,Twr_OpeLpM as TwrOpeLpM
        //                    ,Twr_PCN as TwrPCN
        //                    ,Twr_Notowania as TwrNotowania
        //                    ,Twr_WagaBrutto as TwrWagaBrutto
        //                    ,Twr_WJmBrutto as TwrWJmBrutto
        //                    ,Twr_GrupaPodSpr as TwrGrupaPodSpr
        //                    ,Twr_Licencja as TwrLicencja 
        //                    FROM cdn.TwrKarty";
        //    using var connection = _context.CreateConnection();
        //    var form = await connection.QueryAsync<MerchandiseCard>(query);

        //    return form.ToList();

        //}
    }
}


/*
 * SELECT t.Twr_TwrId AS TwrId
	,t.Twr_Kod AS TwrKod
	,t.Twr_NumerKat AS TwrNrKatalogowy
	,(
		SELECT TOP (1) tg.TwG_Kod
		FROM cdn.TwrGrupy tg
		WHERE tg.TwG_GIDNumer = t.Twr_TwGGIDNumer
		) AS TwgKodGrupy
	,(
		SELECT TOP (1) TwG_Nazwa
		FROM cdn.TwrGrupy tg
		WHERE tg.TwG_GIDNumer = t.Twr_TwGGIDNumer
		) AS TwgNazwa
	,t.Twr_Typ AS TwrTyp
	,t.Twr_EAN AS TwrEan
	,t.Twr_SWW AS TwrSSW
	,(SELECT TOP(1)  KCN_Kod FROM CDN.KodyCN where KCN_KCNId = t.Twr_KCNId)  AS KodCN
	,t.Twr_Stawka AS StawkaVatSprz
	,t.Twr_StawkaZak AS StawkaVatZak
	,Twr_TwCNumer AS TwcNumerSelect
	,t.Twr_SplitPay AS TwrSplitPay
	,t.Twr_OdwrotneObciazenie
	,t.Twr_Nazwa
	,Twr_Kategoria AS TwrKat
	,Twr_KategoriaZak AS TwrKatZak
	,t.Twr_JM AS TwrJM
	,t.Twr_JMCalkowite AS TwrJmCalowite
	,(
SELECT   
    TwC_TwCNumer AS TwcNumer,
    DfC_Typ AS TypCeny,
    ISNULL(DfC_Nieaktywna, 0) AS TwCNieaktywna,
    TwC_Marza AS TwcMarza,
    DfC_Zaokraglenie AS DfcZaokraglenie,
    DfC_Offset AS Offset,
    TwC_Wartosc AS Wartosc ,   
    CASE 
        WHEN DfC_Typ = 1 THEN TwC_Wartosc  -- Jeśli typ ceny to netto, zwróć cenę netto
        WHEN DfC_Typ = 2 THEN (TwC_Wartosc / (1 + Twr_StawkaZak))  -- Jeśli typ ceny to brutto, oblicz cenę netto
    END AS CenaNetto,
	  CASE 
        WHEN DfC_Typ = 1 THEN (TwC_Wartosc * (1 + Twr_StawkaZak))  -- Jeśli typ ceny to brutto, oblicz cenę netto
        WHEN DfC_Typ = 2 THEN TwC_Wartosc  -- Jeśli typ ceny to netto, zwróć cenę netto
    END AS CenaBrutto,
    DfC_Waluta
FROM 
    CDN.TwrCeny B 
JOIN 
    CDN.DEFCeny ON DfC_Lp = TwC_TwCNumer 
WHERE 
    Twc_TwrID = t.Twr_TwrId
ORDER BY 
    CASE 
        WHEN Twc_TwCNumer >= 0 THEN Twc_TwCNumer 
        ELSE -Twc_TwCNumer + (SELECT MAX(twc_twcnumer) FROM cdn.twrceny) 
    END
	For Json Auto
) as ListaCen,

(SELECT TwV_TwVId as Id, TwV_KodKraju as KodKraju, (select Top (1) Kra_Nazwa from CDN_KNF_Konfiguracja.CDN.Kraje where Kra_Kod = TwV_KodKraju) as NazwaKraju,  TwV_Stawka as Stawka FROM CDN.TwrStawkiVAT WHERE TwV_TwrId= t.Twr_TwrId for json auto) as StawkiVAT
,
(SELECT TwJZ_TwJZId as Id, TwJZ_JM as JednostkaPomocnicza, TwJZ_JMPrzelicznikL as Licznik, TwJZ_JMPrzelicznikM as Mianownik, '1 ' + TwJZ_JM+ ' = '+ CAST(TwJZ_JMPrzelicznikL AS VARCHAR(20)) + ' szt.' as Przelicznik, TwJZ_CenaJednostkowa, TwJZ_WysCm as Wysokosc, TwJZ_SzerCm as Szerokosc, TwJZ_DlugCm as Dlugosc FROM CDN.TwrJMZ WHERE TwJZ_TwrId= t.Twr_TwrId  For json Auto) as PomJednMiary ,
(SELECT TwE_TwEId as Id, TwE_EAN as KodEAN, TwE_Opis as Opis, TwE_JM as Jednostka, TwE_Domyslny as Domyslny FROM CDN.TwrEan WHERE TwE_TwrId= t.Twr_TwrId for json auto) as KodyEan
,
(
SELECT tgv.GIDNumer as TwGID, tgv.Sciezka as Sciezka, tgv.Nazwa as Nazwa, tgv.Kod as Kod FROM CDN.TwrGrupy JOIN CDN.TwrGrupyView tgv on TwG_GRONumer = GIDNumer   
WHERE TwG_GIDNumer= t.Twr_TwrId AND TwG_GIDTyp = t.Twr_GIDTyp For json Auto) as Grupy 
FROM cdn.TwrKarty tk
JOIN cdn.Towary t ON tk.Twr_GIDNumer = t.Twr_GIDNumer */