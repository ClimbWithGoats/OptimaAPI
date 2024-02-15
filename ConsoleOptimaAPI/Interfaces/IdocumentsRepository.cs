using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Interfaces
{

    public interface IDocumentsRepository
    {
        Task<IEnumerable<Document>> GetDocuments();
        Task<IEnumerable<Document>> GetFZDocuments();
        Task<IEnumerable<Document>> GetFADocuments();
        Task<IEnumerable<Document>> GetFPADocuments();
        Task<IEnumerable<Document>> GetFRODocuments();
        Task<IEnumerable<Document>> GetFPFDocuments();
        Task<IEnumerable<Document>> GetPWDocuments();
        Task<IEnumerable<Document>> GetRWDocuments();
        Task<IEnumerable<Document>> GetPADocuments();
        Task<IEnumerable<Document>> GetPAWZDocuments();
        Task<IEnumerable<Document>> GetPAROocuments();
        Task<IEnumerable<Document>> GetWZDocuments();
        Task<IEnumerable<Document>> GetPZDocuments();
        Task<IEnumerable<Document>> GetPZZDDocuments();
        Task<IEnumerable<Document>> GetRODocuments();
        Task<IEnumerable<Document>> GetROPFDocuments();
        Task<IEnumerable<Document>> GetZDDocuments();
        Task<IEnumerable<Document>> GetZDRODocuments();
        Task<IEnumerable<Document>> GetMMDocuments();
    }
}
