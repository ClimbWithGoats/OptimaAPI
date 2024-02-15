using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Repositories
{
    public class CommodityGoupsRepository : ICommodityGoupsRepository
    {
        private readonly DapperContext _context;
        public CommodityGoupsRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }
        public async Task<IEnumerable<CommodityGroup>> GetCommodityGroups()
        {
            try
            {
                string query = @"SELECT TwG_GIDNumer as TwGGIDNumer, TwG_Kod as TwGKod, TwG_Nazwa as TwGNazwa, TwG_TwGId as TwGTwGId, TwG_GrONumer as TwGGrONumer FROM CDN.TwrGrupy A 
                             WHERE (TwG_GIDTyp = -16)
                             ORDER BY TwG_Kod, TwG_TwGId";
                using var connection = _context.CreateConnection();




                var form = await connection.QueryAsync<CommodityGroup>(query);

                //string sql = @"SELECT TwG_GIDNumer as TwGGIDNumer, TwG_Kod as TwGKod, TwG_Nazwa as TwGNazwa, TwG_TwGId as TwGTwGId, TwG_GrONumer as TwGGrONumer FROM CDN.TwrGrupy A 
                //             WHERE (TwG_GIDTyp = -16)
                //             ORDER BY TwG_Kod, TwG_TwGId";
                //using var connection = _context.CreateConnection();

                //var lista = _context.GetConfiguration.Attributes.All.ToList();
                //lista.AddRange(_context.GetConfiguration.Attributes.CommodityAlias.Attribute.ToList());
                //string? moreQuery = _context.GetAttributeFilter(_context.GetConfiguration.Attributes.CommodityAlias, lista);
                //string query = sql;
                //if (string.IsNullOrEmpty(moreQuery))
                //{
                //    query = string.Format("{0} {1} {2}", sql, "WHERE", moreQuery);
                //}

                //var form = await connection.QueryAsync<CommodityGroup>(query);
                //return form.ToList();

                return form.ToList();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return new List<CommodityGroup>();
        }
    }
}
