using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TMS_MobileCredentials
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //读取端口配置文件
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("host.json")
                .Build();
            //使用配置文件中的端口地址
            return WebHost.CreateDefaultBuilder(args)
                //.ConfigureLogging(logger =>
                //{
                //    logger.ClearProviders();
                //})
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
        }
    }
}
