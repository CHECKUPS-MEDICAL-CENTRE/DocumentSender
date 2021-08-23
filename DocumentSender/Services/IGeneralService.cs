using DocumentSender.Models.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public interface IGeneralService
    {
        Task<IEnumerable<Cycle_idVM>> GetUnsentDocuments();
    }
}