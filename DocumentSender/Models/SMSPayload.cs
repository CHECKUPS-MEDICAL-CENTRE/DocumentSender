using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models
{
    public class SMSPayload
    {
        [Key]
        public Guid id { get; set; }
        public string phone { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? dateSent { get; set; }
    }
}
