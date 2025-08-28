using App.Core.Interfaces;
using App.Core.Services;
using App.Core.UnitOfWorks;
using DataAccess.EFCore;
using DataAccess.EFCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedKernel.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Http;
using Hangfire.SqlServer;
using App.Core.Models.Authentications;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc.Filters;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.Options;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            #region Security Setting Avoid AntiForgeyToken and addhsts
            //add middleware HTTP Strict Transport Security
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            #endregion

            #region Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TakafulRemunJobs", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[]{}
                    }
                });
            });
            #endregion

            services.AddRazorPages();

            #region Register hangfire
            services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                            .UseSimpleAssemblyNameTypeSerializer()
                                            .UseDefaultTypeSerializer()
                                            .UseMemoryStorage()
                                            .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                                            {
                                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                                QueuePollInterval = TimeSpan.Zero,
                                                UseRecommendedIsolationLevel = true,
                                                UsePageLocksOnDequeue = true,
                                                DisableGlobalLocks = true
                                            }));
            /*services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                            .UseSimpleAssemblyNameTypeSerializer()
                                            .UseDefaultTypeSerializer()
                                            .UseMemoryStorage());*/
            //services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));
            //services.AddHangfireServer();
            // Konfigurasi BackgroundJobServerOptions
            var serverOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 5,
                Queues = new[] { "default", "jobsmailapproval" },
                ServerName = string.Format("{0}_jobsmailapproval", Environment.MachineName)
            };

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = serverOptions.WorkerCount;
                options.Queues = serverOptions.Queues;
                options.ServerName = serverOptions.ServerName;
                options.ShutdownTimeout = serverOptions.ShutdownTimeout;
            });
            #endregion Register hangfire

            #region Register ApplicationContext
            services.AddDbContext<ApplicationContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                , b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)));
            #endregion

            #region Newtonsoft Configuration
            //services.AddControllers().AddNewtonsoftJson(o =>
            //{
            //    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});
            #endregion

            #region Register Configuration
            //configure strongly typed settings object
            var appSettingsSection = Configuration.GetSection("ServiceConfiguration");
            services.Configure<ServiceConfiguration>(appSettingsSection);
            #endregion Register Configuration

            #region Configure Authorization
            //services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder()
            //    .RequireAuthenticatedUser()
            //    .AddAuthenticationSchemes(ActivationBearer.ToArray())
            //    .Build();
            //});
            #endregion

            #region Configure JWT AUTH
            var serviceConfiguration = appSettingsSection.Get<ServiceConfiguration>();
            var jwtSecretKey = Encoding.UTF8.GetBytes(serviceConfiguration.JwtSettings.Secret);
            var jwtIssuer = serviceConfiguration.JwtSettings.Issuer;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                ValidateIssuer = false, //set true
                ValidateAudience = false, //set true
                //ValidIssuer = jwtIssuer,
                //ValidAudience = jwtIssuer,
                RequireExpirationTime = false,
                ValidateLifetime = true //set false
            };
            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication( x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                                builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });
            #endregion Configure JWT AUTH

            #region Interface and Repository
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IRepository, EFRepository>();
            services.AddTransient<IMailSender, MailSenderService>();            
            #endregion

            #region Register Login Service
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<ILogger, LoggerService>();
            services.AddTransient<IAuthenticationHandler, AuthenticationHandlerService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IIdentityService, IdentityService>();
            #endregion

            #region Register Scheduler Service
            services.AddScoped<ISchedulerService, SchedulerService>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
                              ,IBackgroundJobClient backgroundJobClient
                              ,IRecurringJobManager recurringJobManager
                              ,IServiceProvider serviceProvider
                              ,ISchedulerService schedulerService)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TakafulRemunJobs v1"));
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            #region Configure Cors
            //to activate Cross-Origin Resource Sharing (CORS)
            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
            #endregion

            //app.UseServerHeader(false);           

            #region Setting Cookies
            //https://www.codeproject.com/Articles/1259066/10-Points-to-Secure-Your-ASP-NET-Core-MVC-Applicat
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.None
            });
            #endregion

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            #region add CSP security
            app.Use(async (context, next) =>
            {
                if (env.IsDevelopment() || env.IsProduction())
                {
                    context.Response.OnStarting(() =>
                    {
                        //https://stackoverflow.com/questions/39174888/asp-net-core-remove-x-powered-by-cannot-be-done-in-middleware
                        //https://stackoverflow.com/questions/79338418/removing-server-header-not-working-in-asp-net-core-8-does-not-work-with-middle
                        context.Response.Headers.Remove("Server");
                        context.Response.Headers.Remove("X-AspNetWebPages-Version");
                        context.Response.Headers.Remove("X-AspNet-Version");
                        context.Response.Headers.Remove("X-Powered-By");
                        context.Response.Headers.Remove("X-AspNetMvc-Version");

                        return Task.CompletedTask;
                    });

                   
                    //Anti-clickjacking Header
                    //https://dzone.com/articles/secure-net-core-applications-from-click-jacking-ne
                    context.Response.Headers.Append("X-Frame-Options", "DENY");
                    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

                    //https://stackoverflow.com/questions/71209211/how-to-add-to-a-asp-net-core-net6-project-owasp-recommendation
                    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                    //https://www.stackhawk.com/blog/net-content-security-policy-guide-what-it-is-and-how-to-enable-it/
                    //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
                    //context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
                    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self' https: 'unsafe-inline' 'unsafe-eval'; img-src 'self' data: ;font-src 'self' data:;");

                    //context.Response.Headers.Remove("X-AspNetMvc-Version");                    
                    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

                    //From Engine AI
                    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
                    context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

                    context.Response.Headers.Append("access-control-allow-credentials", "true");
                    context.Response.Headers.Append("access-control-allow-headers", "Authorization, Content-Type, Origin, x-requested-with, x-signalr-user-agent");
                    context.Response.Headers.Append("access-control-allow-methods", "POST, PUT, GET, PATCH, DELETE, OPTIONS");
                    //context.Response.Headers.Append("access-control-allow-origin", AllowedOrigin);
                    context.Response.Headers.Append("access-control-max-age", "3600");

                    context.Response.Headers.Append("Feature-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'");

                    await next();
                }
            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                //endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                //{
                //    Authorization = new[] { new HangfireAuthorizationFilter() },
                //    IgnoreAntiforgeryToken = true
                //});

                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("Welcome to remun service");
                });
            });

            #region use hangfire dashboard
            var jobsDashboardUsername = Configuration.GetSection("HangfireConfiguration").GetSection("Username").Value;
            var jobsDashboardPassword = Configuration.GetSection("HangfireConfiguration").GetSection("Password").Value;
            var jobsDashboardUrl = Configuration.GetSection("HangfireConfiguration").GetSection("Url").Value;

            jobsDashboardUsername = jobsDashboardUsername == null ? "admin" : jobsDashboardUsername;
            jobsDashboardPassword = jobsDashboardPassword == null ? "P@ssw0rd" : jobsDashboardPassword;
            jobsDashboardUrl = jobsDashboardUrl == null ? "RemunJobs" : jobsDashboardUrl;

            schedulerService.Run();
            app.UseHangfireDashboard("/" + jobsDashboardUrl, new DashboardOptions
            {
               //Authorization = new[] { new HangfireAuthorizationFilter() }
               //AsyncAuthorization = new[] { new HangfireAsyncAuthorizationFilter() },
               //install package Hangfire.Dashboard.Basic.Authentication
               Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = jobsDashboardUsername,
                        Pass = jobsDashboardPassword
                    }
                },
                IgnoreAntiforgeryToken = true
            });
            //app.UseHangfireDashboard();
            #endregion use hangfire dashboard
        }
    }
}
