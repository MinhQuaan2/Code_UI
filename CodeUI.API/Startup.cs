using Autofac;
using CodeUI.API.AppStart;
using CodeUI.API.Controllers.Moderator;
using CodeUI.API.Helpers;
using CodeUI.API.Mapper;
using CodeUI.Data.MakeConnection;
using CodeUI.Data.Repository;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Helpers;
using CodeUI.Service.Service;
using CodeUI.Service.Service.AdminServices;
using CodeUI.Service.Service.ModeratorServices;
using FirebaseAdmin;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Reso.Core.Extension;
using ServiceStack.Redis;
using StackExchange.Redis;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using Coravel;

namespace CodeUI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1041 // Provide ObsoleteAttribute message

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                        //.WithOrigins(GetDomain())
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(Configuration["Endpoint:RedisEndpoint"]));
            services.AddMemoryCache();
            services.ConfigMemoryCacheAndRedisCache(Configuration["Endpoint:RedisEndpoint"]);
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CodeUI API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer iJIUzI1NiIsInR5cCI6IkpXVCGlzIElzc2'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        securitySchema,
                    new string[] { "Bearer" }
                    }
                });
            });
            services.ConfigureAuthServices(Configuration);
            services.ConnectToConnectionString(Configuration);
            services.ConfigureHangfireServices(Configuration);

            ServiceHelpers.Initialize(Configuration);

            // Set the default time zone to UTC+7
            services.AddSingleton<TimeZoneInfo>(_ =>
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
            });


            #region Firebase
            var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "firebase.json");
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(pathToKey)
            });
            #endregion 

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #region Coravel
            services.AddScheduler();
            #endregion
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            //builder.RegisterType<Service>().As<IService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<ProfileService>().As<IProfileService>();
            builder.RegisterType<ElementService>().As<IElementService>();
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<PaymentService>().As<IPaymentService>();
            builder.RegisterType<ReactElementService>().As<IReactElementService>();
            builder.RegisterType<PackageService>().As<IPackageService>();
            builder.RegisterType<FollowCreatorService>().As<IFollowCreatorService>();
            builder.RegisterType<ReportService>().As<IReportService>();
            builder.RegisterType<FirebaseMessagingService>().As<IFirebaseMessagingService>();
            builder.RegisterType<StaffService>().As<IStaffService>();
            builder.RegisterType<DonationService>().As<IDonationService>();
            builder.RegisterType<RequestService>().As<IRequestService>();

            builder.RegisterType<ModeratorElementService>().As<IModeratorElementService>();
            builder.RegisterType<ModeratorReportService>().As<IModeratorReportService>();

            builder.RegisterType<AdminStaffService>().As<IAdminStaffService>();
            builder.RegisterType<AdminAccountService>().As<IAdminAccountService>();
            builder.RegisterType<AdminElementService>().As<IAdminElementService>();
            builder.RegisterType<AdminPackageService>().As<IAdminPackageService>();
            builder.RegisterType<AdminPointsService>().As<IAdminPointsService>();

            //builder.Register<IRedisClientsManager>(c =>
            //new RedisManagerPool(Configuration.GetConnectionString("RedisConnectionString")));
            //builder.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());

            builder.RegisterGeneric(typeof(GenericRepository<>))
            .As(typeof(IGenericRepository<>))
            .InstancePerLifetimeScope();
            var configbuilder = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json");
            var config = configbuilder.Build();

            var smtpClient = new SmtpClient(config["Mail:Host"])
            {
                Port = int.Parse(config["Mail:Port"]),
                Credentials = new NetworkCredential(config["Mail:Username"], config["Mail:Password"]),
                EnableSsl = true,
            };
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //app.ConfigMigration<>();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeUI_API V1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();
            AuthConfig.Configure(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
