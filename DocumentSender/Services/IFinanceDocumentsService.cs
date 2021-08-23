using DocumentSender.Models.Finance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public interface IFinanceDocumentsService
    {
        Task<InvoiceParams> GetInvoiceDetails(object[] args);
        Task<IEnumerable<InvoiceItemsList>> GetLabtestItemsPCR(object[] args);
    }
}