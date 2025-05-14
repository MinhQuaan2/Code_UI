using CodeUI.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CodeUI.Data.MakeConnection
{
    public static class MakeConnection
    {
        public static IServiceCollection ConnectToConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CodeUiDevContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(configuration.GetConnectionString("SQLServerDatabase_dev"), sql => sql.UseNetTopologySuite());
            });
            services.AddDbContext<CodeUiDevContext>(ServiceLifetime.Transient);

            return services;
        }
    }
}