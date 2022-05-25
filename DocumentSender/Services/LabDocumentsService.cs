using DocumentSender.CheckDbContext;
using DocumentSender.Models.Lab;
using DocumentSender.Models.ViewModels;
using DocumentSender.RepositoryMixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public class LabDocumentsService : Repository, ILabDocumentsService
    {
        public async Task<LabCertDetails> FetchLabCertDetails(object[] args)
        {
            string query = @"select (PR.patient_first_name+' '+ pr.patient_middle_name +' '+ PR.patient_last_name) AS PatientName, 
pr.patient_facility_number PatientNumber,pr.gender Gender,pr.id_no, DATEDIFF(year, pr.dob, getdate()) as Age,pr.dob DOB,
inv.invoice_number InvoiceNumber, tsc.test_serial_no LabSerial, tsc.sample Sample,t.samplecollectiontime SampleCollectionTime, vc.cycle_created_time VisitDate,
lt.lab_test_desc Investigation, pr.id_no IdNumber,t.value TestValue
 from visit_cycle vc
inner join test t on t.cycle_id=vc.cycle_id
inner join lab_test lt on lt.lab_test_id=t.lab_test_id
inner join patient_registration pr on pr.patient_id=vc.patient_id
inner join invoices inv on inv.cycle_id=vc.cycle_id
inner join test_sample_collection tsc on tsc.cycle_id=vc.cycle_id
where vc.cycle_id={0} and t.done=1
 ";
            return await FirstOrDefaultOptimisedAsync<LabCertDetails>(query, args);
        }
        public async Task<List<LabTestParticulars>> FecthLabTestParticularsPCR(object[] args)
        {
            string query = @"select  slt.sub_lab_test_description Test, tr.value Value
 from visit_cycle vc
inner join test_results tr on tr.cycle_id=vc.cycle_id
inner join lab_test lt on lt.lab_test_id=tr.lab_test_id
inner join sub_lab_test slt on slt.sub_lab_test_id= tr.sub_lab_test_id
where vc.cycle_id={0} and tr.cycle_id={0} and (tr.cancelled is null or tr.cancelled=0)
and lt.lab_test_id=3530
order by slt.sub_lab_test_description asc
";
            return await FindOptimisedAsync<LabTestParticulars>(query, args);
        }
        public async Task<List<LabTestParticulars>> FecthLabTestParticularsAntigen(object[] args)
        {
            string query = @"select  slt.sub_lab_test_description Test, tr.value Value
 from visit_cycle vc
inner join test_results tr on tr.cycle_id=vc.cycle_id
inner join lab_test lt on lt.lab_test_id=tr.lab_test_id
inner join sub_lab_test slt on slt.sub_lab_test_id= tr.sub_lab_test_id
where vc.cycle_id={0} and tr.cycle_id={0} and (tr.cancelled is null or tr.cancelled=0)
and lt.lab_test_id=3558
order by slt.sub_lab_test_description asc
";
            return await FindOptimisedAsync<LabTestParticulars>(query, args);
        }
        public async Task<IEnumerable <TakenTests >> TestsDone(object[] args)
        {
            string query = @"select lab_test_id Test from test where cycle_id={0} and done=1";
            return await FindOptimisedAsync<TakenTests>(query, args);
        }
        public async Task<ConsentFormVM> GetConsentForm(object[] args)
        {
            string query = @"select * from onlinepatients where ";
            return await FirstOrDefaultOptimisedAsync<ConsentFormVM>(query, args);
        }


        public async Task SendToErrorLogLab(object[] args)
        {
            string query = @"insert into error_log(error_log_id,error_desc,module,center_id,recorded_date,recorded_by)
values(newid(),{0},'DocumentSender',77,getdate(),12)";
            await UpdateAsync(query, args);
        }
        public LabDocumentsService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
