using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace sh.vcp.sso.server
{
    public class Program
    {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), config.GetValue<string>("WebRootFolder"));

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseKestrel()
                .UseWebRoot(webRoot)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();
        }
    }
}
