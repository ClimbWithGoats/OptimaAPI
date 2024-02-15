using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Interfaces
{

    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategories();
    }
}
