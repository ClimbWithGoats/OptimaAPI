using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//KartyTowarowe
namespace OptimaAPI.Interfaces
{

    public interface IMerchandiseCardsRepository
    {
         Task<IEnumerable<MerchandiseCardExt>> GetMerchandiseCards();
    }
}
