using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Interfaces
{
    public interface IResourcesRepository
    {
        Task<IEnumerable<Resource>> GetResources();
    }
}
