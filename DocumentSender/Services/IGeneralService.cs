using DocumentSender.Models.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public interface IGeneralService
    {
        Task<IEnumerable<Cycle_idVM>> GetUnsentDocuments();
         Task<EmailPhone> GetEmailPhone(object[] args);
        Task UpdateSentCertificate(object[] args);
        Task SendToErrorLog(object[] args);
        //Task<Email2> GetSecondaryEmail(object[] args);
    }
}