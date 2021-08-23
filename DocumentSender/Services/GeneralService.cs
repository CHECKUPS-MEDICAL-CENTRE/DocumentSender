using DocumentSender.CheckDbContext;
using DocumentSender.Models.General;
using DocumentSender.RepositoryMixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public class GeneralService : Repository, IGeneralService
    {
        public async Task<IEnumerable<Cycle_idVM>> GetUnsentDocuments()
        {
            string query = @"select cycle_id CycleId, document_type DocumentType from DocumentsLog where is_sent=1";

            return await FindOptimisedAsync<Cycle_idVM>(query);
        }
        public GeneralService(CheckupsDbContext context) : base(context)
        {

        }
    }
}
