using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.Finance
{
    public class InvoiceParams
    {
        [Key]
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public DateTime DateGenerated { get; set; }
        public string InvoiceNumber { get; set; }

    }
}
