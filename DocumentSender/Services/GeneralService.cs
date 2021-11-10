using DocumentSender.CheckDbContext;
using DocumentSender.Models.General;
using DocumentSender.RepositoryMixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public class GeneralService : Repository, IGeneralService
    {
        public async Task<IEnumerable<Cycle_idVM>> GetUnsentDocuments()
        {
            string query = @"select cycle_id CycleId, document_type DocumentType from DocumentsLog where is_sent=1";

            return await FindOptimisedAsync<Cycle_idVM>(query);
        }
        public async Task<EmailPhone> GetEmailPhone(object[] args)
        {
            string query = @"select (PR.patient_first_name+' '+ pr.patient_middle_name +' '+ PR.patient_last_name) AS Name,Pr.email Email, pr.patient_tel Phone from visit_cycle vc
inner join patient_registration pr on pr.patient_id= vc.patient_id
where vc.cycle_id={0}";
            return await FirstOrDefaultOptimisedAsync<EmailPhone>(query, args);
        }
        public async Task <Email2> GetSecondaryEmail(object[] args)
        {
            string query = @"select 'tempid' as tempid, Email2 Email from onlinepatients where email={0} and telephone={1}";
            return await FirstOrDefaultOptimisedAsync<Email2>(query, args);
        }
        public async Task UpdateSentCertificate(object[] args)
        {
            string query = @"Update DocumentsLog set is_sent=0 where cycle_id={0} and document_type={1}";
            await UpdateAsync(query, args);
        }
        public async Task<EmailPhone> GetPatientDetails(object[] args)
        {
            string query = @"select (FirstName+' '+ OtherNames +' '+ Surname) AS Name,Pr.email Email, pr.patient_tel Phone from visit_cycle vc
inner join onlinepatients";
            return await FirstOrDefaultOptimisedAsync<EmailPhone>(query, args);
        }
        public GeneralService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
