using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.Registration
{
    public class OnlinePatient
    {
        [Key]
        public string Names { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string Insurance { get; set; }
        public string Scheme { get; set; }
        public string Town { get; set; }
        public string Residence { get; set; }
        public DateTime VisitDate { get; set; }
        public string PassportId { get; set; }
       
        public string CollectionLocation { get; set; }
        public string MemberNumber { get; set; }

    }
}
