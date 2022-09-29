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
        Task<IEnumerable<GetVisitsVM>> Getvisits(object[] args);
        Task<GetBalanceVM> GetPharmacyBalance(object[] args);
        Task<GetMemberDetails> GetMemberDetails(object[] args);
        Task<PatientDetailsVM> GetPatientDetails(object[] args);
        Task<IEnumerable<ChargeItemsVM>> GetPatientConsultations(object[] args);
        Task<IEnumerable<ChargeItemsVM>> GetPatientLabs(object[] args);
        Task<IEnumerable<ChargeItemsVM>> GetPatientDiagnostics(object[] args);
        Task<IEnumerable<ChargeItemsVM>> GetPatientPharmacyItems(object[] args);
        Task<IEnumerable<GetVisitsVM>> GetPatientvisits(object[] args);
    }
}