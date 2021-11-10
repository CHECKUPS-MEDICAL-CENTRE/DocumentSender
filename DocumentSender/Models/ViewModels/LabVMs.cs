using DocumentSender.Models.Lab;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.ViewModels
{
    public class LabCertsVM
    {
        public string PatientNumber { get; set; }
        public string PatientName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string IdNumber { get; set; }
        public string LabSerial { get; set; }
        public string Sample { get; set; }
        public string Investigation { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime SampleCollectionTime { get; set; }
        public string TestValue { get; set; }
        public IEnumerable<LabTestParticulars> labTestParticulars { get; set; }
    }
    public class ConsentFormVM
    {
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Telephone { get; set; }
        [Key]
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public int IsVaccinated { get; set; }
        public int IsDoseComplete { get; set; }
        public string TestingReason { get; set; }
        public int TypeOfTest { get; set; }
        public string TypeOfTestDescription { get; set; }
        public string PaymentMethod { get; set; }
        public string InsuranceId { get; set; }
        public string Insurance { get; set; }
        public string SchemeId { get; set; }
        public string Scheme { get; set; }
        public string Town { get; set; }
        public string Residence { get; set; }
        public string Occupation { get; set; }
       public DateTime VisitDate { get; set; }
        public string PassportId { get; set; }
       public string FlightNumber { get; set; }
        public string CountryTo { get; set; }
        public string WhereTestWasDone { get; set; }
        public string CollectionSlot { get; set; }
        public string CollectionLocation { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string OrderID { get; set; }
        public string VisitTime { get; set; }
        public string MemberNumber { get; set; } 
        public string Airline { get; set; }
    }
}
