using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly DapperContext _context;
        public CategoriesRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            try
            {
                string query = @"SELECT Kat_KatId as KatKatId, Kat_KodSzczegol as KatKodSzczegol, Kat_Opis as KatOpis, (SELECT STUFF((SELECT '; ' + KV7_Kod FROM CDN.DokKodyJPKV7 JOIN CDN.KodyJPKV7 ON DKV7_KV7ID = KV7_KV7ID WHERE DKV7_ParentTyp = 3 AND DKV7_ParentID = Kat_KatID FOR XML PATH('')) , 1, 2, '')) AS KodyJPKV7, Kat_KodOgolny as KatKodOgolny, Kat_Poziom as KatPoziom, Kat_ParentID as KatParentID, Kat_Nieaktywny as KatNieaktywny FROM CDN.Kategorie A";
                using var connection = _context.CreateConnection();
                var form = await connection.QueryAsync<Category>(query);

                return form.ToList();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<Category>();
        }
    }
}
