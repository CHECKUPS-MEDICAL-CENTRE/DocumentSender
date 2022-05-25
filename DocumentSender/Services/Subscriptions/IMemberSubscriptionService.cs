using DocumentSender.Models.SubscriptionModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services.Subscriptions
{
    public interface IMemberSubscriptionService
    {
        Task<IEnumerable<SubscriptionModelsVM>> GetConsultations(object[] args);
        Task<IEnumerable<SubscriptionModelsVM>> GetDiagnostics(object[] args);
        Task<IEnumerable<SubscriptionModelsVM>> GetLabs(object[] args);
        Task<IEnumerable<SubscriptionModelsVM>> GetPharmacyItems(object[] args);
    }
}