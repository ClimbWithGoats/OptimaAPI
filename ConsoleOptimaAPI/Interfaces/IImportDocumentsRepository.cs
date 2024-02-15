using OptimaAPI.Models;
using static OptimaAPI.Repositories.ImportDocumentsRepository;

namespace OptimaAPI.Interfaces
{
    public interface IImportDocumentsRepository
    {
        Task<InputMessage> TestingRunProcces();
        Task<InputMessage> PostPZ();
        Task<InputMessage> PostFZ();
        Task<InputMessage> PostWZ();
        Task<InputMessage> PostFA();
        Task<InputMessage> PutWMSStatus();
        Task<InputMessage> PutWMSSignature();
        Task<InputMessage> PutWMSDiscrepancies();

    }
}
