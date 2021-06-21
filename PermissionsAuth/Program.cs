using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;


namespace PermissionsAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "appsettings.json");
            string logPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "logs");

            var configurationBuilder = new ConfigurationBuilder().AddJsonFile(configPath);
            var configuration = configurationBuilder.Build();

            GlobalDiagnosticsContext.Set("configDir", logPath);
            GlobalDiagnosticsContext.Set("connectionString", configuration["DefaultConnection"]);

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            var host = CreateHostBuilder(args).Build();

            try
            {
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                logger.Debug("init main");
                host.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Stopped program because of exception.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection
    }
}
