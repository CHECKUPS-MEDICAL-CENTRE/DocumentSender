using DocumentSender.Models.Lab;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public interface ILabDocumentsService
    {
        Task<List<LabTestParticulars>> FecthLabTestParticularsPCR(object[] args);
        Task<List<LabTestParticulars>> FecthLabTestParticularsAntigen(object[] args);
        Task<LabCertDetails> FetchLabCertDetails(object[] args);
        Task<IEnumerable<TakenTests>> TestsDone(object[] args);
        Task SendToErrorLogLab(object[] args);
    }
}