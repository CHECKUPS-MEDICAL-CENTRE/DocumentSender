using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

namespace DocumentSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.File(@"C:\DocumentLogs\Logs.txt")
            .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
            .UseWindowsService()
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddHostedService<Worker>();
                   services.AddModels(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("LocalDB"),
                        b => b.MigrationsAssembly("MigrationHandler")));
               })
            .ConfigureLogging(logging =>
            {
                logging.AddSerilog();
            })

           ;

    }
}
