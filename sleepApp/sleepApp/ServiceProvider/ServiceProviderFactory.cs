using Microsoft.Extensions.DependencyInjection;
using sleepApp.Repository;
using sleepApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sleepApp.ServiceProvider
{
    public static class ServiceProviderFactory //регистрация зависимостей для внедрения
    {
        public static IServiceProvider ConfigureServices(string login, string password, string port, string dataBase, string host)
        {
            var services = new ServiceCollection();
            // Регистрация AppDbContext
            services.AddScoped<AppDbContext>(provider =>
            {
                return new AppDbContext(login, password, port, dataBase, host);
            });
            services.AddScoped<RespodentService>(); //scoped создается один раз для запроса
            services.AddScoped<SleepDataService>();

            services.AddScoped<RespondentRepository>();
            services.AddScoped<SleepDataRepository>();

            services.AddTransient<DashboardWindow>(); //создается каждый раз

            return services.BuildServiceProvider();
        }
    }
}
