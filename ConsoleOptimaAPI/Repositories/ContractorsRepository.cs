using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
//Kontrahenci
namespace OptimaAPI.Repositories
{
    public class ContractorsRepository : IContractorsRepository
    {
        private readonly DapperContext _context;
        public ContractorsRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task<IEnumerable<Contractor>> GetContractors()
        {
            try
            {
                string sql = @"SELECT DISTINCT k.Knt_GIDNumer AS KntId
							  ,kk.knt_typ AS KntTyp
							  ,k.Knt_Kod AS KntKod
							  ,k.Knt_Grupa AS KntGrupa
							  ,k.Knt_Rodzaj AS KntRodzaj
							  ,k.Knt_DokumentTozsamosci AS Knt_DokumentTozsamosci
							  ,k.Knt_Nazwa1 AS KntNazwa1
							  ,k.Knt_Nazwa2 AS KntNazwa2
							  ,k.Knt_Nazwa3 AS KntNazwa3
							  ,k.Knt_NipKraj AS KntNipKraj
							  ,k.Knt_Nip AS KntNip
							  ,k.Knt_NipE AS KntNipE
							  ,k.Knt_IdSisc AS IdSISC
							  ,k.Knt_Regon AS KntRegon
							  ,k.Knt_Pesel AS KntPesel
							  ,k.Knt_Kraj AS KntKraj
							  ,k.Knt_Miasto AS KntMiasto
							  ,k.Knt_Poczta AS KntPoczta
							  ,k.Knt_Ulica AS KntUlica
							  ,k.Knt_Adres2 AS KntAdres2
							  ,k.Knt_Telefon1 AS KntTelefon1
							  ,k.Knt_Telefon2 AS KntTelefon2
							  ,k.Knt_TelefonSms AS KntTelefonSms
							  ,k.Knt_KrajISO AS KntKrajISO
							  ,k.Knt_Wojewodztwo AS KntWojewodztwo
							  ,k.Knt_KodPocztowy AS KntKodPocztowy
							  ,k.Knt_NrDomu AS KntNrDomu
							  ,k.Knt_NrLokalu AS KntNrLokalu
							  ,k.Knt_Fax AS KntFax
							  ,k.Knt_Email AS KntEmail
							  ,k.Knt_URL AS KntURL
							  ,k.Knt_KorMiasto AS KntKorMiasto
							  ,k.Knt_KorPoczta AS KntKorPoczta
							  ,k.Knt_KorUlica AS KntKorUlica
							  ,k.Knt_KorKraj AS KntKorKraj
							  ,k.Knt_KorKodPocztowy AS KntKorKodPocztowy
							  ,k.Knt_KorNrDomu AS KntKorNrDomu
							  ,k.Knt_KorNrLokalu AS KntKorNrLokalu
							  ,(
							  	SELECT Kat_KodOgolny
							  	FROM cdn.Kategorie kat
							  	WHERE kat.Kat_KatID = k.Knt_KatID
							  	) AS KntKatID
							  ,(
							  	SELECT Kat_KodOgolny
							  	FROM cdn.Kategorie kat
							  	WHERE kat.Kat_KatID = k.Knt_KatZakID
							  	) AS KntKatZakID
							  ,k.Knt_Zezwolenie AS KntZezwolenie
							  ,k.Knt_EAN AS KntEAN
							  ,k.Knt_GLN AS KntGLN
							  ,fp.FPl_Nazwa AS FPlNazwa
							  ,k.Knt_Waluta AS KntWaluta
							  ,'' AS NieRozliczajPlatnosc
							  ,k.Knt_NaliczajPlatnosc AS KntNaliczajPlatnosc
							  ,k.Knt_SplitPay AS KntSplitPay
							  ,k.Knt_NieNaliczajOdsetek AS KntNieNaliczajOdsetek
							  ,k.Knt_TerminPlat AS KntTerminPlatb
							  ,k.Knt_Termin AS knttermin
							  ,k.Knt_MaxZwloka AS KntMaxZwloka
							  ,k.Knt_Termin AS KntTermin
							  ,(
							  	SELECT sp.SPL_LiczbaPorz
							  		,(
							  			SELECT TOP (1) BNa_Nazwa1
							  			FROM cdn.BnkNazwy
							  			WHERE BNa_BNaId = sp.SPL_BnaId
							  			) AS NazwaBanku
							  		,sp.SPL_RachunekNr0
							  		,sp.SPL_TS_Mod
							  	FROM cdn.SchematPlatnosci sp
							  	WHERE SPL_PodmiotTyp = k.Knt_Rodzaj
							  		AND SPL_PodmiotID = k.Knt_KntId
							  	FOR JSON AUTO
							  	) AS ListaRachBank
							  ,k.Knt_WindykacjaSchematId AS KntWindykacjaSchematId
							  ,k.Knt_WindykacjaOsobaId AS KntWindykacjaOsobaId
							  ,k.Knt_WindykacjaEMail AS KntWindykacjaEMail
							  ,k.Knt_WindykacjaTelefonSms AS KntWindykacjaTelefonSms
							  ,k.Knt_PodmiotTyp AS KntPodmiotTyp
							  ,k.Knt_Export AS KntExport
							  ,k.Knt_MetodaKasowa AS KntMetodaKasowa
							  ,k.Knt_Rolnik AS KntRolnik
							  ,k.Knt_PowiazanyUoV AS KntPowiazanyUoV
							  ,k.Knt_Chroniony AS KntChroniony
							  ,k.Knt_BlokadaDok AS KntBlokadaDok
							  ,k.Knt_Nieaktywny AS KntNieaktywny
							  ,k.Knt_LimitFlag AS KntLimitFlag
							  ,k.Knt_LimitKredytu AS KntLimitKredytu
							  ,k.Knt_LimitKredytuWykorzystany AS KntLimitKredytuWykorzystany
							  ,k.Knt_LimitPrzeterKredytWartosc AS KntLimitPrzeterKredytWartosc
							  ,kk.Knt_Rabat AS FPlRabat
							  ,k.Knt_Opis AS KntOpis
							  ,k.Knt_KontoDost AS KontoDost
							  ,k.Knt_KontoOdb AS KontoOdb
							  FROM cdn.Kontrahenci k
							  JOIN cdn.FormyPlatnosci fp ON k.Knt_FplID = FPl_FPlId
							  JOIN CDN.KntKarty KK ON K.Knt_KntId = KK.Knt_GIDNumer";

                using var connection = _context.CreateConnection();
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.contractorAlias.Attribute.ToList());
                string? moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.contractorAlias, lista);
                string query = sql;
                if (string.IsNullOrEmpty(moreQuery))
                {
                    query = string.Format("{0} {1} {2}", sql, "WHERE", moreQuery);
                }

                var form = await connection.QueryAsync<Contractor>(query);

                return form.ToList();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Contractor>();
        }
        public async Task<IEnumerable<SQLContractor>> GetSQLContractors()
        {
            try
            {
                string sql = @"SELECT DISTINCT 
							 k.Knt_GIDNumer as KntId
							,kk.knt_typ as KntTyp
							,k.Knt_Kod as KntKod
							,k.Knt_Grupa as KntGrupa
							,k.Knt_Rodzaj as KntRodzaj
							,k.Knt_DokumentTozsamosci as Knt_DokumentTozsamosci
							,k.Knt_Nazwa1 as KntNazwa1
							,k.Knt_Nazwa2 as KntNazwa2
							,k.Knt_Nazwa3 as KntNazwa3
							,k.Knt_NipKraj as KntNipKraj
							,k.Knt_Nip as KntNip 
							,k.Knt_NipE as KntNipE 
							,k.Knt_IdSisc as IdSISC
							,k.Knt_Regon as KntRegon
							,k.Knt_Pesel as KntPesel
							,k.Knt_Kraj as KntKraj
							,k.Knt_Miasto as KntMiasto
							,k.Knt_Poczta as KntPoczta
							,k.Knt_Ulica as KntUlica
							,k.Knt_Adres2 as KntAdres2
							,k.Knt_Telefon1 as KntTelefon1
							,k.Knt_Telefon2 as KntTelefon2
							,k.Knt_TelefonSms as KntTelefonSms
							,k.Knt_KrajISO as KntKrajISO
							,k.Knt_Wojewodztwo as KntWojewodztwo
							,k.Knt_KodPocztowy as KntKodPocztowy
							,k.Knt_NrDomu as KntNrDomu
							,k.Knt_NrLokalu as KntNrLokalu
							,k.Knt_Fax as KntFax
							,k.Knt_Email as KntEmail
							,k.Knt_URL as KntURL
							,k.Knt_KorMiasto as KntKorMiasto
							,k.Knt_KorPoczta as KntKorPoczta
							,k.Knt_KorUlica as KntKorUlica
							,k.Knt_KorKraj as KntKorKraj
							,k.Knt_KorKodPocztowy as KntKorKodPocztowy
							,k.Knt_KorNrDomu as KntKorNrDomu
							,k.Knt_KorNrLokalu as KntKorNrLokalu
							,(select Kat_KodOgolny from cdn.Kategorie kat where kat.Kat_KatID = k.Knt_KatID) as KntKatID
							,(select Kat_KodOgolny from cdn.Kategorie kat where kat.Kat_KatID = k.Knt_KatZakID)  as KntKatZakID
							,k.Knt_Zezwolenie as KntZezwolenie
							,k.Knt_EAN as KntEAN
							,k.Knt_GLN as KntGLN
							,fp.FPl_Nazwa as FPlNazwa
							,k.Knt_Waluta as KntWaluta
							, '' as NieRozliczajPlatnosc
							,k.Knt_NaliczajPlatnosc as KntNaliczajPlatnosc
							, k.Knt_SplitPay as KntSplitPay
							,k.Knt_NieNaliczajOdsetek as KntNieNaliczajOdsetek
							,k.Knt_TerminPlat as KntTerminPlatb
							,k.Knt_Termin as knttermin
							,k.Knt_MaxZwloka as KntMaxZwloka
							,k.Knt_Termin as KntTermin
								, (select sp.SPL_LiczbaPorz, (select top(1) BNa_Nazwa1 from cdn.BnkNazwy where BNa_BNaId = sp.SPL_BnaId) as NazwaBanku, sp.SPL_RachunekNr0, sp.SPL_TS_Mod  from  cdn.SchematPlatnosci sp where SPL_PodmiotTyp = k.Knt_Rodzaj and SPL_PodmiotID = k.Knt_KntId 
							FOR JSON AUTO) as ListaRachBank
							,k.Knt_WindykacjaSchematId as KntWindykacjaSchematId
							,k.Knt_WindykacjaOsobaId as KntWindykacjaOsobaId
							,k.Knt_WindykacjaEMail as KntWindykacjaEMail
							,k.Knt_WindykacjaTelefonSms as KntWindykacjaTelefonSms
							,k.Knt_PodmiotTyp as KntPodmiotTyp
							,k.Knt_Export as KntExport
							,k.Knt_MetodaKasowa as KntMetodaKasowa
							,k.Knt_Rolnik as KntRolnik
							,k.Knt_PowiazanyUoV as KntPowiazanyUoV
							,k.Knt_Chroniony as KntChroniony
							,k.Knt_BlokadaDok as KntBlokadaDok
							,k.Knt_Nieaktywny as KntNieaktywny
							,k.Knt_LimitFlag as KntLimitFlag
							,k.Knt_LimitKredytu as KntLimitKredytu
							,k.Knt_LimitKredytuWykorzystany as KntLimitKredytuWykorzystany
							,k.Knt_LimitPrzeterKredytWartosc as KntLimitPrzeterKredytWartosc	
							,kk.Knt_Rabat as FPlRabat
							,k.Knt_Opis as KntOpis
							,k.Knt_KontoDost as KontoDost
							,k.Knt_KontoOdb as KontoOdb
							FROM cdn.Kontrahenci k
							JOIN cdn.FormyPlatnosci fp ON k.Knt_FplID = FPl_FPlId
							JOIN CDN.KntKarty KK ON K.Knt_KntId =  KK.Knt_GIDNumer";

                using var connection = _context.CreateConnection();
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.contractorAlias.Attribute.ToList());
                string? moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.contractorAlias, lista);
                string query = sql;
                if (string.IsNullOrEmpty(moreQuery))
                {
                    query = string.Format("{0} {1} {2}", sql, "WHERE", moreQuery);
                }

                var form = await connection.QueryAsync<SQLContractor>(query);
                return form.ToList();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<SQLContractor>();
        }

        public async Task<IEnumerable<MiniContractor>> GetMiniInfoAboutContractors()
        {
            try
            {
                string query = @"SELECT [Knt_GIDNumer] as KntGIDNumer,[Knt_Akronim] as KntAkronim ,[Knt_Nazwa1]   as KntNazwa1
                             FROM [CDN].[KntKarty]";
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<MiniContractor>(query);

                return form.ToList();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<MiniContractor>();

        }
    }
}
