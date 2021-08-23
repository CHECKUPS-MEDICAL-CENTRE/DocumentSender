using DocumentSender.Models.Lab;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public interface ILabDocumentsService
    {
        Task<IEnumerable<LabTestParticulars>> FecthLabTestParticulars(object[] args);
        Task<LabCertDetails> FetchLabCertDetails(object[] args);
    }
}