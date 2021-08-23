using DocumentSender.CheckDbContext;
using DocumentSender.Models.Lab;
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
pr.patient_facility_number PatientNumber,pr.gender Gender,pr.id_no, DATEDIFF(year, pr.dob, getdate()) as Age,
inv.invoice_number InvoiceNumber, tsc.test_serial_no LabSerial, tsc.sample Sample,tsc.samplecollectiontime SampleCollectionTime, vc.cycle_created_time VisitDate,
pr.dob,lt.lab_test_desc Investigation, pr.id_no IdNumber,t.value TestValue
 from visit_cycle vc
inner join test t on t.cycle_id=vc.cycle_id
inner join lab_test lt on lt.lab_test_id=t.lab_test_id
inner join patient_registration pr on pr.patient_id=vc.patient_id
inner join invoices inv on inv.cycle_id=vc.cycle_id
inner join test_sample_collection tsc on tsc.cycle_id=vc.cycle_id
where vc.cycle_id={0}
 ";
            return await FirstOrDefaultOptimisedAsync<LabCertDetails>(query, args);
        }
        public async Task<IEnumerable<LabTestParticulars>> FecthLabTestParticulars(object[] args)
        {
            string query = @"select  slt.sub_lab_test_description Test, tr.value Value
 from visit_cycle vc
inner join test_results tr on tr.cycle_id=vc.cycle_id
inner join lab_test lt on lt.lab_test_id=tr.lab_test_id
inner join sub_lab_test slt on slt.sub_lab_test_id= tr.sub_lab_test_id
where vc.cycle_id={0} and tr.cycle_id={0} and (tr.cancelled is null or tr.cancelled=0)";
            return await FindOptimisedAsync<LabTestParticulars>(query, args);
        }

        public LabDocumentsService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
