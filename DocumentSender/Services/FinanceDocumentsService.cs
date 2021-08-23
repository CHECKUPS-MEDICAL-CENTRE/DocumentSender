using DocumentSender.CheckDbContext;
using DocumentSender.Models.Finance;
using DocumentSender.RepositoryMixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public class FinanceDocumentsService : Repository, IFinanceDocumentsService
    {
        public async Task<IEnumerable<InvoiceItemsList>> GetLabtestItemsPCR(object[] args)
        {
            string query = @"select lt.lab_test_desc Description,Savings,vat_amount VAT,regular_retail_price Retail, Quantity,unit_price Price,(quantity* unit_price) as Total from visit_charge_items vci 
inner join lab_test lt on lt.lab_test_code=vci.item_code
where vci.cycle_id={0}";
            return await FindOptimisedAsync<InvoiceItemsList>(query, args);
        }
        public async Task<InvoiceParams> GetInvoiceDetails(object[] args)
        {
            string query = @"select (PR.patient_first_name+' '+ pr.patient_middle_name +' '+ PR.patient_last_name) AS PatientName, pr.patient_facility_number PatientNumber,
dt.invoice_number InvoiceNumber, dt.date_created DateGenerated from visit_cycle vc
inner join deposit dt on vc.cycle_id=dt.cycle_id
inner join patient_registration pr on pr.patient_id= vc.patient_id
where vc.cycle_id={0}";
            return await FirstOrDefaultOptimisedAsync<InvoiceParams>(query, args);
        }
        public FinanceDocumentsService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
