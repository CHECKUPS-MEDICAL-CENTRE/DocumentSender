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
    public class ChargeItemsVM
    {
        [Key]
        public string Invoice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public decimal Savings { get; set; }
    }
    public class GetVisitsVM
    {
        [Key]
        public string invoice_number { get; set; }
        public DateTime cycle_created_time { get; set; }
    }
    public class GetBalanceVM
    {
        [Key]
        public decimal PharmacyBalance { get; set; }
        public int SubscriptionPackageId { get; set; }
    }
    public class GetMemberDetails
    {
        [Key]
        public string MemberNumber { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string Package { get; set; }
        public DateTime EnrolledOn { get; set; }
        public string Gender { get; set; }
    }
    public class PatientDetailsVM
    {
        [Key]
        public string PatNo { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
    }
}
