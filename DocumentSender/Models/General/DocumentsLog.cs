using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.General
{
    public class DocumentsLog
    {
        [Key]
        public Guid document_id { get; set; }
        public string cycle_id { get; set; }
        public string document_type { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_sent { get; set; }
        public bool is_sent { get; set; }
        public int number_of_resends { get; set; }
        public string email_to { get; set; }
        public string phone_to { get; set; }

    }
    public class Cycle_idVM
    {
        [Key]
        public string CycleId { get; set; }
        public string DocumentType { get; set; }
    }
    public class EmailPhone
    {
        [Key]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
    public class Email2
    {
        [Key]
        public string TempId { get; set; }
        public string Email { get; set; }
    }
}
