using DocumentSender.CheckDbContext;
using DocumentSender.Models.SubscriptionModels;
using DocumentSender.RepositoryMixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services.Subscriptions
{
    public class MemberSubscriptionService : Repository, IMemberSubscriptionService
    {
        public async Task<IEnumerable<SubscriptionModelsVM>> GetConsultations(object[] args)
        {
            string query = @"
select z.*,Case when ZidiiTemlink is NULL then 'Billable' else 'Non-Billable' end Charges from
(select invoice_number Invoice,quantity,item_description Category,full_item_description Item,charge_amount Amount,savings, 
item_id
from visit_cycle vc
inner join patient_payment_accounts ppa on ppa.patient_payment_account_id= vc.payer_id
inner join visit_charge_items vci on vci.cycle_id=vc.cycle_id
where ppa.account_no={0} and vci.item_description='Consultation' and vci.invoice_number={1})z
left join(select * from SubscriptionPackageItem where SubscriptionPackageId='1')p on z.item_id=ZidiiTemlink
group by Invoice,Category,quantity,Item, Amount,savings, 
item_id,ZidiiTemlink
";
            return await FindOptimisedAsync<SubscriptionModelsVM>(query, args);
        }
        public async Task<IEnumerable<SubscriptionModelsVM>> GetLabs(object[] args)
        {
            string query = @"
select z.*,Case when ZidiiTemlink is NULL then 'Billable' else 'Non-Billable' end Charges from
(select invoice_number Invoice,quantity,item_description Category,full_item_description Item,charge_amount Amount,savings, 
item_id
from visit_cycle vc
inner join patient_payment_accounts ppa on ppa.patient_payment_account_id= vc.payer_id
inner join visit_charge_items vci on vci.cycle_id=vc.cycle_id
where ppa.account_no={0} and vci.item_description='Lab' and vci.invoice_number={1})z
left join(select * from SubscriptionPackageItem where SubscriptionPackageId='1')p on z.item_id=ZidiiTemlink
group by Invoice,Category,quantity,Item, Amount,savings, 
item_id,ZidiiTemlink
";
            return await FindOptimisedAsync<SubscriptionModelsVM>(query, args);
        }
        public async Task<IEnumerable<SubscriptionModelsVM>> GetDiagnostics(object[] args)
        {
            string query = @"
select z.*,Case when ZidiiTemlink is NULL then 'Billable' else 'Non-Billable' end Charges from
(select invoice_number Invoice,quantity,item_description Category,full_item_description Item,charge_amount Amount,savings, 
item_id
from visit_cycle vc
inner join patient_payment_accounts ppa on ppa.patient_payment_account_id= vc.payer_id
inner join visit_charge_items vci on vci.cycle_id=vc.cycle_id
where ppa.account_no={0} and vci.item_description='Service' and vci.invoice_number={1})z
left join(select * from SubscriptionPackageItem where SubscriptionPackageId='1')p on z.item_id=ZidiiTemlink
group by Invoice,Category,quantity,Item, Amount,savings, 
item_id,ZidiiTemlink
";
            return await FindOptimisedAsync<SubscriptionModelsVM>(query, args);
        }
        public async Task<IEnumerable<SubscriptionModelsVM>> GetPharmacyItems(object[] args)
        {
            string query = @"
select z.*,Case when ZidiiTemlink is NULL then 'Billable' else 'Non-Billable' end Charges from
(select invoice_number Invoice,quantity,item_description Category,full_item_description Item,charge_amount Amount,savings, 
item_id
from visit_cycle vc
inner join patient_payment_accounts ppa on ppa.patient_payment_account_id= vc.payer_id
inner join visit_charge_items vci on vci.cycle_id=vc.cycle_id
where ppa.account_no={0} and vci.item_description='Pharmacy' and vci.invoice_number={1})z
left join(select * from SubscriptionPackageItem where SubscriptionPackageId='1')p on z.item_id=ZidiiTemlink
group by Invoice,Category,quantity,Item, Amount,savings, 
item_id,ZidiiTemlink
";
            return await FindOptimisedAsync<SubscriptionModelsVM>(query, args);
        }
        public MemberSubscriptionService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
