using DocumentSender.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.ViewModels
{
    public class InvoiceParticularsVM
    {
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public DateTime DateGenerated { get; set; }
        public string InvoiceNumber { get; set; }
        public IEnumerable<InvoiceItemsList> InvoiceItems { get; set; }
    }
}
