using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//Kontrahenci
namespace OptimaAPI.Interfaces
{
    public interface IContractorsRepository
    {
        Task<IEnumerable<Contractor>> GetContractors();
        Task<IEnumerable<MiniContractor>> GetMiniInfoAboutContractors();
        Task<IEnumerable<SQLContractor>> GetSQLContractors();
    }
}
