using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace OptimaAPI.Repositories
{
    public class DocumentsRepository : IDocumentsRepository
    {
        readonly DapperContext _context;
        public DocumentsRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
            _context.CreateConnection();
        }
        readonly string sql = @"SELECT 
Distinct TrN_TrNId AS TrnId
,pod.pod_kod AS Akronim 
	,def.DDf_Numeracja as NumeracjaDok
	,trn_rodzaj AS Rodzaj	
	,TRN_Numerpelny AS TrnNumerPelny
	,TrN_NumerString AS TrnNumerString
	,TrN_NumerNr AS TrnNumer
	,TrN_Anulowany AS TrnAnulowany
	,DATEDIFF(dd, '1800-12-28', coalesce(TRN_Dataope, getdate())) AS TrnDataOperacji
	,MAG_Symbol AS MagSymbol
	,Trn_PodNazwa1 + SPACE(SIGN(LEN(Trn_PodNazwa2))) + Trn_PodNazwa2 + SPACE(SIGN(LEN(Trn_PodNazwa3))) + Trn_PodNazwa3 AS NazwaPodmiotu
	,TRN_Podmiasto AS Miasto
	,TrN_RazemNetto AS TrnRazemNetto
	,TrN_Bufor AS Bufor
	,DATEDIFF(dd, '1800-12-28', coalesce(TrN_DataWys, getdate())) AS TrnDataWystawienia
	,(
		SELECT TrE_Lp AS LP
			,TrE_TwrNazwa AS TowarNazwa
			,TrE_TwrKod AS TowarKod
			,TrE_Ilosc AS Ilosc
			,TrE_Jm AS JM
			,TrE_Cena0 AS cena
			,TrE_WartoscNetto AS Wartosc
		FROM cdn.TraElem
		WHERE TrE_TrNId = TrN_TrNID
		FOR json auto
		) AS Lista		
FROM CDN.TraNag A
LEFT OUTER JOIN CDN.Kategorie B ON B.KAT_Katid = A.TRN_Katid
LEFT OUTER JOIN CDN.Magazyny C ON C.MAG_Magid = A.TRN_Magzrdid
LEFT OUTER JOIN CDN.PodmiotyView Pod ON Pod.Pod_PodId = A.TrN_PodId
	AND A.Trn_PodmiotTyp = Pod.Pod_PodmiotTyp
LEFT OUTER JOIN cdn.DokAtrybuty da ON da.DAt_TrNId = A.TrN_TrNID
LEFT OUTER JOIN CDN.DefAtrybuty defa ON da.DAt_DeAId = defa.DeA_DeAId
LEFT OUTER JOIN CDN.DokDefinicje def on A.TrN_DDfId = def.DDf_DDfID
WHERE (TrN_TypDokumentu = {0} and TrN_Rodzaj = {1} and TrN_Bufor = {2} and  TrN_DataWys >= DATEFROMPARTS(YEAR(GETDATE())-1, 12, 10)) {3}
ORDER BY TrnDataWystawienia desc, TrN_NumerString desc,  TrN_NumerNr , TrN_Anulowany ";

        readonly string sqlALl = string.Format(@" select 1");
        public async Task<IEnumerable<Document>> GetDocuments()
        {
            var lista = _context.GetConfiguration.Attributes.All.ToList();
            lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());

            // var ttt = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias,lista);

            string query = sqlALl;
            using var connection = _context.CreateConnection();
            var form = await connection.QueryAsync<BaseDocument>(query);
            if (form == null)
                return new List<Document>();

            List<Document> documents = form
      .Select(mc => new Document(mc))
      .ToList();

            return documents;
        }


        #region 302 - Faktury spredaży
        // Faktura sprzedaży
        public async Task<IEnumerable<Document>> GetFADocuments()
        {
            //tutaj wsadzam te całe ustawienia z parametrami, trzeba konfig z aplikacji Optimy połączyć z configiem API.
            try
            {
                //Fa- Faktura sprzedaży
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
                string query = string.Format(sql, 302, 302000, 0, moreQuery);

                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
                    .Select(mc => new Document(mc))
                    .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        public async Task<IEnumerable<Document>> GetFZDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 301, 301000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Faktura sprzedaży z paragonu
        public async Task<IEnumerable<Document>> GetFPADocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {

                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 302, 302006, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();

        }

        // Faktura sprzedaży z RO
        public async Task<IEnumerable<Document>> GetFRODocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 302, 302008, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Faktura sprzedaży z pro-formy
        public async Task<IEnumerable<Document>> GetFPFDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 302, 302009, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        #endregion

        #region 303 - Przychód wewnętrzny
        // Przychód wewnętrzny
        public async Task<IEnumerable<Document>> GetPWDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 303, 303000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }

        #endregion

        #region Rozchód wewnętrzny
        // Rozchód wewnętrzny
        public async Task<IEnumerable<Document>> GetRWDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 304, 304000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }

        #endregion

        #region 305 - Paragon
        // paragon 
        public async Task<IEnumerable<Document>> GetPADocuments()
        {
            //Fa- Paragon
            try
            {
                //Fa- Faktura sprzedaży
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 305, 305000, 0, moreQuery);

                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Paragon sprzedaży przekształcony z WZ
        public async Task<IEnumerable<Document>> GetPAWZDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 305, 305004, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Paragon sprzedaży przekształcony z RO
        public async Task<IEnumerable<Document>> GetPAROocuments()
        {
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 305, 305008, 0, moreQuery);

                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }

        #endregion


        #region 306 - Wydanie zewnętrzne
        // Wydanie zewnętrzne 
        public async Task<IEnumerable<Document>> GetWZDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 306, 306000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }


        #endregion

        #region 307 - przyjęcie zewnętrzne
        // przyjęcie zewnętrzne
        public async Task<IEnumerable<Document>> GetPZDocuments()
        {
            var lista = _context.GetConfiguration.Attributes.All.ToList();
            lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
            string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            //Fa- Faktura sprzedaży
            string query = string.Format(sql, 307, 307000, 0, moreQuery);
            try
            {
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Przyjęcie zewnętrzne powstałe z zamówienia dostawcy
        public async Task<IEnumerable<Document>> GetPZZDDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 307, 307010, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        #endregion

        #region 308 - Rezerwacja odbiorcy
        // Rezerwacja odbiorcy
        public async Task<IEnumerable<Document>> GetRODocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 308, 308000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Rezerwacja odbiorcy przeksztalcona z PF
        public async Task<IEnumerable<Document>> GetROPFDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 308, 308011, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        #endregion


        #region 309 - Zamówienie dostawcy
        // Zamówienie dostawcy
        public async Task<IEnumerable<Document>> GetZDDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 309, 309000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        // Zamówienie dostawcy wygenrowane z RO
        public async Task<IEnumerable<Document>> GetZDRODocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 309, 309308, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }
        #endregion

        #region 312 - Przesuniecie MM
        // Przesuniecie MM
        public async Task<IEnumerable<Document>> GetMMDocuments()
        {
            //Fa- Faktura sprzedaży
            try
            {
                var lista = _context.GetConfiguration.Attributes.All.ToList();
                lista.AddRange(_context.GetConfiguration.Attributes.DocumentAlias.Attribute.ToList());
                string moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.DocumentAlias, lista);
            string query = string.Format(sql, 312, 312000, 0, moreQuery);
                //Fa- Faktura sprzedaży
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<BaseDocument>(query);
                if (form == null)
                    return new List<Document>();
                List<Document> documents = form
          .Select(mc => new Document(mc))
          .ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Document>();
        }

        #endregion

    }
}
