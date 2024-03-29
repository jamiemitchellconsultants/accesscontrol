﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Helper;
using AccessControl.Models;
using AccessControl.Services;
using Amazon.CognitoIdentityProvider;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;
using Microsoft.OpenApi.Models;


namespace AccessControl
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AccessControl.Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">Services.</param>
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
            services.AddOptions();
            services.Configure<CognitoConfig>(Configuration.GetSection("CognitoConfig"));
            services.Configure<AuthorityConfiguration>(Configuration.GetSection("IdentityServer"));
            ConfigureAuthentication(services);
            services.AddTransient<IAuthorizationPolicyProvider, ExternalPermissisonPolicyProvider>();
            services.AddTransient<IAuthorizationHandler, ExternalPermissionHandler>();
            services.AddScoped<ICallPermissionCheck, PermissionCheck>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Access Control", Version = "v1" });
                c.EnableAnnotations();
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "AccessControl.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        public virtual void ConfigureAuthentication(IServiceCollection services)
        {
            var config = Configuration.GetSection("CognitoConfig").Get<CognitoConfig>();
            if (config != null)
            {
                services.AddAuthentication("Bearer").AddJwtBearer(o =>
                    {
                        o.Audience = "3tit9k2l04h1dnebbab5hj69re";
                        o.Authority = "https://cognito-idp.eu-west-1.amazonaws.com/eu-west-1_7Uuksl4YT";

                    })
                    .AddCookie()
                    .AddOpenIdConnect(options =>
                    {
                        options.ResponseType = "code"; //config.ResponseType;//
                        options.MetadataAddress =
                            "https://cognito-idp.eu-west-1.amazonaws.com/eu-west-1_7Uuksl4YT/.well-known/openid-configuration"; //config.MetadataAddress;//
                        options.ClientId = "3tit9k2l04h1dnebbab5hj69re"; // config.ClientId;//
                        options.ClientSecret =
                            "1fcq7fqlchpam0g96i9h0iahp7206ts17k2c5mqd68q5lakuo1ql"; //config.ClientSecret;// 
                        options.Events = new OpenIdConnectEvents
                        {
                            // this makes signout working
                            OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut
                        };
                    });
            }

            var is4Config = Configuration.GetSection("IdentityServer").Get<AuthorityConfiguration>();
            if (is4Config != null)
            {
                services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = $"{is4Config.Authority}:{is4Config.AuthorityPort}";
                        options.RequireHttpsMetadata = false;

                        options.ApiName = is4Config.ApiName;
                        options.NameClaimType = is4Config.NameClaimType;
                        options.RoleClaimType = is4Config.RoleClaimType;

                    });
            }
        }

        /// <summary>
        /// Configures the database.
        /// </summary>
        /// <param name="services">Services.</param>
        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("CustomDatabase");
            services.AddDbContext<AccessControlContext>
                (options => options.UseMySQL(connection));
            services.AddScoped<PermissionCheck>();

        }
        private Task OnRedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            var cognitoConfig = Configuration.Get<CognitoConfig>();
            context.ProtocolMessage.Scope = "openid";
            context.ProtocolMessage.ResponseType = "code";

            var logoutEndpoint = $"{context.Request.Scheme}://{context.Request.Host}/home/signOut";
            var clientId =  "3tit9k2l04h1dnebbab5hj69re"; //cognitoConfig.ClientId;//

            var logoutUrl = $"{context.Request.Scheme}://{context.Request.Host}/home/signedOut";

            context.ProtocolMessage.IssuerAddress = $"{logoutEndpoint}?client_id={clientId}&logout_uri={logoutUrl}&redirect_uri={logoutUrl}";

            // delete cookies
            context.Properties.Items.Remove(CookieAuthenticationDefaults.AuthenticationScheme);
            // close openid session
            context.Properties.Items.Remove(OpenIdConnectDefaults.AuthenticationScheme);

            return Task.CompletedTask;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
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

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Access Control V1");
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
