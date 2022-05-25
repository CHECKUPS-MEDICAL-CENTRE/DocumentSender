using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.Lab
{
    public class LabCertDetails
    {
        [Key]
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
        public DateTime DOB { get; set; }
    }
}
