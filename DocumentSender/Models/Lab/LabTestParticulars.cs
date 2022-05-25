using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.Lab
{
    public class LabTestParticulars
    {
        [Key]
        public string Test { get; set; }
        public string Value { get; set; }
    }
    public class TakenTests
    {
        [Key]
        public int Test { get; set; }
    }
}
