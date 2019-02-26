using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AccessControlClient
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpContextAccessor();
            ConfigureDatabase(services);

            var config = Configuration.GetSection("CognitoConfig").Get<CognitoConfig>();

            services.AddOptions();
            services.Configure<CognitoConfig>(Configuration.GetSection("CognitoConfig"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.Audience = "3tit9k2l04h1dnebbab5hj69re";
                o.Authority = "https://cognito-idp.eu-west-1.amazonaws.com/eu-west-1_7Uuksl4YT";
            })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ResponseType = "code";//config.ResponseType;//
                    options.MetadataAddress = "https://cognito-idp.eu-west-1.amazonaws.com/eu-west-1_7Uuksl4YT/.well-known/openid-configuration";//config.MetadataAddress;//
                    options.ClientId = "3tit9k2l04h1dnebbab5hj69re";// config.ClientId;//
                    options.ClientSecret = "1fcq7fqlchpam0g96i9h0iahp7206ts17k2c5mqd68q5lakuo1ql";//config.ClientSecret;// 
                    options.Events = new OpenIdConnectEvents
                    {
                        // this makes signout working
                        OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut
                    };
                });



            //services.AddCognitoIdentity();
            ConfigurePermissionCheck(services);
            services.AddTransient<IAuthorizationPolicyProvider, ExternalPermissisonPolicyProvider>();

            services.AddTransient<IAuthorizationHandler, ExternalPermissionHandler>();
            //services.AddSingleton<ICallPermissionCheck, RemotePermissionCheck>();
            services.AddScoped<ICallPermissionCheck, PermissionCheck>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Access Control", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "AccessControl.xml");
                c.IncludeXmlComments(filePath);
            });



        }
        public virtual void ConfigurePermissionCheck(IServiceCollection services)
        {
            //services.Configure<RemotePermissionCheckConfig>(Configuration.GetSection("RemotePermissionCheck"));
            var config = Configuration.GetSection("RemotePermissionCheck").Get<RemotePermissionCheckConfig>();
            services.AddSingleton<RemotePermissionCheckConfig>(config);
        }
        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            //var connection = Configuration.GetConnectionString("CustomDatabase");
            //services.AddDbContext<AccessControlContext>
            //    (options => options.UseMySQL(connection));
            //services.AddScoped<PermissionCheck>();

        }
        private Task OnRedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            var cognitoConfig = Configuration.Get<CognitoConfig>();
            context.ProtocolMessage.Scope = "openid";
            context.ProtocolMessage.ResponseType = "code";

            var logoutEndpoint = $"{context.Request.Scheme}://{context.Request.Host}/home/signOut";
            var clientId = "3tit9k2l04h1dnebbab5hj69re"; //cognitoConfig.ClientId;//

            var logoutUrl = $"{context.Request.Scheme}://{context.Request.Host}/home/signedOut";

            context.ProtocolMessage.IssuerAddress = $"{logoutEndpoint}?client_id={clientId}&logout_uri={logoutUrl}&redirect_uri={logoutUrl}";

            // delete cookies
            context.Properties.Items.Remove(CookieAuthenticationDefaults.AuthenticationScheme);
            // close openid session
            context.Properties.Items.Remove(OpenIdConnectDefaults.AuthenticationScheme);

            return Task.CompletedTask;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
