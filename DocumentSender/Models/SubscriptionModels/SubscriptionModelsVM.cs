using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Models.SubscriptionModels
{
    public class SubscriptionModelsVM
    {
        [Key]
        public string Invoice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string Charges { get; set; }
        public decimal Savings { get; set; }
    }
}
