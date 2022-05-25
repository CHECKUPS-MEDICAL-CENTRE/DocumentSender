using DocumentSender.CheckDbContext;
using DocumentSender.Services;
using DocumentSender.Services.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender
{
    public static class ExtensionManager
    {
        public static IServiceCollection AddModels(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddScoped<IFinanceDocumentsService, FinanceDocumentsService>();
            services.AddScoped<ILabDocumentsService, LabDocumentsService>();
            services.AddScoped<IGeneralService, GeneralService>();
            services.AddScoped<IMemberSubscriptionService, MemberSubscriptionService>();
            services.AddDbContextPool<CheckupsDbContext>(options);

            return services;
        }
    }
}
