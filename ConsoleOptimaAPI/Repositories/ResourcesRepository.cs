using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Repositories
{

    public class ResourcesRepository : IResourcesRepository
    {
        private readonly DapperContext _context;
        public ResourcesRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }
        public async Task<IEnumerable<Resource>> GetResources()
        {
            string query = @"SELECT Twr_TwrId AS TwrTwrId
							,Twr_Kod AS TwrKod
							,Twr_Nazwa AS TwrNazwa
							,Twr_JM as TwrJM
							,CASE Twr_Typ
								WHEN 0
									THEN CASE Twr_Produkt
											WHEN 0
												THEN CASE 
														WHEN Twr_Kaucja = 1
															AND 0 = 1
															THEN 'UPO'
														ELSE 'UP'
														END
											ELSE CASE 
													WHEN Twr_Kaucja = 1
														AND 0 = 1
														THEN 'UZO'
													ELSE 'UZ'
													END
											END
								WHEN 1
									THEN CASE Twr_Produkt
											WHEN 0
												THEN CASE 
														WHEN Twr_Kaucja = 1
															AND 0 = 1
															THEN 'TPO'
														ELSE 'TP'
														END
											ELSE CASE 
													WHEN Twr_Kaucja = 1
														AND 0 = 1
														THEN 'TZO'
													ELSE 'TZ'
													END
											END
								END AS TwrTypS
							,Twr_NumerKat AS TwrNumerKat
							,Twr_EAN AS TwrEAN
							,Twr_KodDostawcy AS TwrKodDostawcy
							,Knt_Kod AS KntKod
							,CASE 
								WHEN Twr_Typ = 0
									THEN 4
								WHEN Twr_Typ = 1
									AND ISNULL(TwI_Ilosc, 0) = 0
									THEN 0
								WHEN Twr_Typ = 1
									AND Twr_IloscMin > 0
									AND ISNULL(TwI_Ilosc, 0) < round(Twr_IloscMin * isnull(M.TwJZ_JMPrzelicznikL, 1) / isnull					(M.TwJZ_JMPrzelicznikM,	1), 4)
									THEN 2
								ELSE 3
								END AS StanZasobu
							,ISNULL(TwI_Ilosc, 0) AS TwI_Ilosc
							,ISNULL(TwI_Rezerwacje, 0) AS TwIRezerwacje
							,ISNULL(TwI_Braki, 0) AS TwIBraki
							,ISNULL(TwI_Zamowienia, 0) AS TwIZamowienia
							,CASE 
								WHEN TwI_Braki - TwI_Zamowienia < 0
									THEN 0
								ELSE ISNULL(TwI_Braki, 0) - ISNULL(TwI_Zamowienia, 0)
								END AS TwIBrakiPozostale
							,CASE 
								WHEN TwI_Ilosc - TwI_Rezerwacje - TwI_RezerwacjeZlecenia < 0
									THEN 0
								ELSE ISNULL(TwI_Ilosc, 0) - ISNULL(TwI_Rezerwacje, 0) - ISNULL(TwI_RezerwacjeZlecenia, 0)
								END AS TwIIloscDost
							,CONVERT(DECIMAL(28, 2), (ISNULL(TwC_Wartosc, 0))) AS TwCWartosc
							,CASE 
								WHEN ISNULL(TwC_Waluta, '') = ''
									THEN ''
								ELSE TwC_Waluta
								END AS TwCWaluta
							,CASE ISNULL(TwC_Waluta, '')
								WHEN 'PLN'
									THEN CONVERT(DECIMAL(28, 2), round(CONVERT(DECIMAL(28, 4), ISNULL(TwC_Wartosc, 0) * 1 / 1),		CASE 
													WHEN Twr_CenaZCzteremaMiejscami = 1
														THEN 4
													ELSE 2
													END))
								WHEN 'EUR'
									THEN CONVERT(DECIMAL(28, 2), round(CONVERT(DECIMAL(28, 4), ISNULL(TwC_Wartosc, 0) * 4.6340 /	1),	CASE 
													WHEN Twr_CenaZCzteremaMiejscami = 1
														THEN 4
													ELSE 2
													END))
								WHEN 'GBP'
									THEN CONVERT(DECIMAL(28, 2), round(CONVERT(DECIMAL(28, 4), ISNULL(TwC_Wartosc, 0) * 5.3631 /	1),	CASE 
													WHEN Twr_CenaZCzteremaMiejscami = 1
														THEN 4
													ELSE 2
													END))
								WHEN 'USD'
									THEN CONVERT(DECIMAL(28, 2), round(CONVERT(DECIMAL(28, 4), ISNULL(TwC_Wartosc, 0) * 4.4062 /	1),	CASE 
													WHEN Twr_CenaZCzteremaMiejscami = 1
														THEN 4
													ELSE 2
													END))
								END AS TwrWartoscPLN
							,CONVERT(DECIMAL(28, 2), (ISNULL(TwC_Wartosc, 0)) * ISNULL(TwI_Ilosc, 0)) AS Twr_WartoscWal
							,CASE ISNULL(TwC_Waluta, '')
								WHEN 'PLN'
									THEN CONVERT(DECIMAL(28, 2), ISNULL(TwC_Wartosc, 0) * ISNULL(TwI_Ilosc, 0) * 1 / 1)
								WHEN 'EUR'
									THEN CONVERT(DECIMAL(28, 2), ISNULL(TwC_Wartosc, 0) * ISNULL(TwI_Ilosc, 0) * 4.6340 / 1)
								WHEN 'GBP'
									THEN CONVERT(DECIMAL(28, 2), ISNULL(TwC_Wartosc, 0) * ISNULL(TwI_Ilosc, 0) * 5.3631 / 1)
								WHEN 'USD'
									THEN CONVERT(DECIMAL(28, 2), ISNULL(TwC_Wartosc, 0) * ISNULL(TwI_Ilosc, 0) * 4.4062 / 1)
								END AS TwrWartoscCalosciPLN
							,ISNULL(TwI_Wartosc, 0) AS TwIWartosc
							,KCN_Kod AS KCNKod
							,KV7_Kod AS KV7Kod
							,TwC_Typ AS TwCTyp
							,Twr_Typ AS TwrTyp
							,Twr_Kaucja AS TwrKaucja
							,Twr_Produkt AS TwrProdukt
							,Twr_KosztUslugiTyp AS TwrKosztUslugiTyp
							,Twr_NieAktywny AS TwrNieAktywny
							,TwC_TwCNumer AS TwCTwCNumer
							,TwCZV_TwCNumer AS TwCZVTwCNumer
							,Twr_GidNumer AS TwrGidNumer
						FROM CDN.Towary A
						LEFT OUTER JOIN (
							SELECT il.twi_twrid
								,il.TwI_Data
								,il.TwI_Ilosc
								,il.TwI_Rezerwacje
								,il.TwI_RezerwacjeZlecenia
								,il.TwI_Zamowienia
								,il.TwI_Braki
								,il.TwI_Wartosc
								,il.TwI_MagId
							FROM cdn.twrilosci il
							INNER JOIN (
								SELECT TwI_TwrId
									,max(TwI_Data) AS [data]
								FROM CDN.TwrIlosci
								WHERE TwI_MagId IS NULL
									AND TwI_Data <= Convert(DATETIME, '2023-09-28 00:00:00', 120)
								GROUP BY TwI_TwrId
								) AS il_data ON il.TwI_TwrId = il_data.TwI_TwrId
								AND il.twi_data = il_data.data
								AND IL.TwI_MagId IS NULL
							) B ON Twr_TwrId = b.TwI_TwrId
						LEFT OUTER JOIN CDN.TwrCeny C ON C.TwC_TwrID = A.Twr_TwrId
							AND Twr_TwCNumer = TwC_TwCNumer
						LEFT OUTER JOIN CDN.TwrCenyZakView D ON D.TwCZV_TwrID = C.TwC_TwrID
						LEFT OUTER JOIN CDN.Kontrahenci E ON E.Knt_KntId = A.Twr_KntId
						LEFT OUTER JOIN CDN.KodyCn L ON L.KCN_KCNId = A.Twr_KCNId
						LEFT OUTER JOIN CDN.KodyJPKV7 N ON A.Twr_KV7ID = N.KV7_KV7ID
						LEFT OUTER JOIN (
							SELECT Twr_TwrId AS TwrID
								,Twr_Kod AS Twr_FantomKod
							FROM CDN.Towary
							) K ON K.TwrId = A.Twr_ESklepFantomID
						LEFT JOIN CDN.TwrJMZ M ON A.Twr_IloscMinJM = M.TwJZ_JM
							AND A.Twr_Twrid = M.TwJZ_Twrid
							AND M.TwJZ_eSklepID = 0";
			try
			{
				using var connection = _context.CreateConnection();
				var form = await connection.QueryAsync<Resource>(query);

				return form.ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return new List<Resource>();

        }

    }
}
