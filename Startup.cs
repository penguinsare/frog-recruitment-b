using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using crm.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using crm.Helpers;
using crm.Authentication;
using System.IdentityModel.Tokens.Jwt;
using crm.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;
using crm.Services;
using IdentityServerHost;
using System.Reflection;
using IdentityServer4;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using IdentityModel;

namespace crm
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
            services.AddCors(t => t.AddPolicy(
                "CorsPolicy", policy => policy
                 .WithOrigins(
                    Configuration["AllowedHosts:0"],
                    Configuration["AllowedHosts:1"])                    
                 .AllowAnyHeader()
                 .AllowAnyMethod()
            ));

            services.AddDbContext<CrmContext>(options =>
                options.UseSqlite(Configuration["Database:Connection"]));
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });


            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric= false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<CrmContext>()
                .AddDefaultTokenProviders();
          
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            services.AddTransient<ISavedDocumentHandler, SavedDocumentHandler>();
            services.AddTransient<IRecoveryCodeValidator, RecoveryCodeValidator>();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            //.AddSigningCredential(IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey())    
            .AddDeveloperSigningCredential()
            // this adds the config data from DB (clients, resources)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                b.UseSqlite(Configuration["Database:Connection"],
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                b.UseSqlite(Configuration["Database:Connection"],
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<ApplicationUser>();

            services.AddLocalApiAuthentication();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("access", "elevated");
                    policyBuilder.RequireUserName("administrator");
                });
                options.AddPolicy("ElevatedPolicy", policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("access", "elevated");
                });
                options.AddPolicy("NormalPolicy", policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("access", "elevated", "normal");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            
            //InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseDefaultFiles();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
            app.UseIdentityServer();

            app.UseMvc(routes => {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            });           
        }        

        //private void InitializeDatabase(IApplicationBuilder app)
        //{
        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        //        context.Database.Migrate();
        //        if (!context.Clients.Any())
        //        {
        //            foreach (var client in Config.GetClients())
        //            {
        //                context.Clients.Add(client.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.IdentityResources.Any())
        //        {
        //            foreach (var resource in Config.GetIdentityResources())
        //            {
        //                //context.IdentityResources.Remove(resource.ToEntity());
        //                context.IdentityResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.ApiResources.Any())
        //        {
        //            foreach (var resource in Config.GetApiResources())
        //            {
        //                context.ApiResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //}
    }
}
