using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using TMS_MobileAPI.Core;
using TMS_MobileAPI.Credentials;
using TMS_MobileAPI.DependencyInjection;
using TMS_MobileAPI.HangFire;
using TMS_MobileRepository.Helpers;


namespace TMS_MobileAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationHelper.config = configuration;//配置信息
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Hangfire 数据库配置
            services.AddHangfire(x => x.UseSqlServerStorage(
                Configuration.GetSection("ConnectionStrings:SqlServerConnection").Value));
            //仓储在此注入
            RepositoryInjection.ConfigureRepository(services);
            //配置信息对象
            services.Configure<AppSetting>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppSetting>(Configuration.GetSection("Emailpost"));
            services.Configure<AppSetting>(Configuration.GetSection("AuthorityUrl"));

            

            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            //跨域
            //companycode,mobiletype,token
            services.AddCors(options => options.AddPolicy("Mobile", p => p.AllowAnyOrigin().AllowAnyHeader()
            .WithMethods("Get","Post","Put","Delete")));

            //services.AddCors(options => options.AddPolicy("Csrf", p => p.WithOrigins(
            //    Configuration.GetSection("Cores:Origins").Value.Split(';')).AllowAnyHeader()
            //   .WithMethods("Get").AllowCredentials()));
            //services.Configure<CookiePolicyOptions>(p =>
            //{
            //    p.CheckConsentNeeded = context => false;
            //    p.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddResponseCompression();//响应压缩
            services.AddAntiforgery(option =>
            {
                //option.Cookie.Name = "XSRF-TOKEN";
                //option.FormFieldName = "TMSFieldName";
                option.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddMvc(Options =>
            {
            Options.Filters.Add<MyActionFilter>();//加载自定义过滤器
            Options.Filters.Add(typeof(ResultMiddleware));
            Options.RespectBrowserAcceptHeader = true;

            //Options.Filters.Add(new ValidateAntiForgeryTokenAttribute());//AutoValidateAntiforgery
        }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("API", new Info { Title = "统一视图日历API", Version = "v1" });
                var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                c.IncludeXmlComments(Path.Combine(basePath, "TMS_MobileAPI.xml"));
                c.OperationFilter<AddAuthTokenHeaderParameter>();//token
            });
            services.AddMvcCore().AddJsonFormatters();

            services.AddAuthentication((options) =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                

            })
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters();
                Options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(0);
                
                Options.RequireHttpsMetadata = false;
                Options.Audience = "api1";
                Options.Authority = Configuration.GetSection("AuthorityUrl:Url").Value;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory, IServiceProvider serviceProvider,
            IAntiforgery antiforgery)
        {
           
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                //app.UseRequestHeaderMiddleware();
                app.UseExceptionHandler(p => p.Run(async context =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await MyErrorEvent.ErrorEvent(context);
                }));
            }
            GlobalConfiguration.Configuration.UseActivator<MyActivator>(new MyActivator(serviceProvider));
            app.UseHangfireServer();//启动Hangfire服务
            app.UseHangfireDashboard("/Dashboard", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizeFilter() }
            });//启动Hangfire后台管理面板
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");

            //app.Use(async (context, next) =>
            //{

            //    context.Response.Headers["Server"] = "Dnc1.0";


            //    ////antiforgery设置
            //    //string path = context.Request.Path.Value;
            //    //if (string.Equals(path, "/Antiforgery", StringComparison.OrdinalIgnoreCase))
            //    //{
            //    //    var domain = Configuration.GetSection("Domain:url").Value;
            //    //    //var tokens = antiforgery.GetAndStoreTokens(context);
            //    //    context.Response.Cookies.Append("CSRF-TOKEN", "12313",
            //    //        new CookieOptions() { HttpOnly = false,Domain=domain });

            //    //}

            //    await next();
            //});


            
            app.UseResponseCompression();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/API/swagger.json", "统一视图日历API V1");
            });
            app.UseAuthentication();//权限验证
            
            //允许跨域
            //app.UseCors("Mobile");
            //app.UseCookiePolicy();
            app.UseMvc(router => {
                router.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}"
                    );
            });
        }
        public class AddAuthTokenHeaderParameter : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();
                var attrs = context.ApiDescription.RelativePath;
                //foreach (var attr in attrs)
                //{
                //    // 如果 Attribute 是我们自定义的验证过滤器
                //    if (attr.GetType() == typeof(IdentityController))
                //    {
                //        operation.Parameters.Add(new NonBodyParameter()
                //        {
                //            Name = "AuthToken",
                //            In = "header",
                //            Type = "string",
                //            Required = false
                //        });
                //    }
                //}
                if (attrs != "api/Credential")
                {
                    operation.Parameters.Add(new NonBodyParameter()
                    {
                        Description = "权限认证(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey",
                        Required = false
                    });
                }
            }
        }
    }
}
