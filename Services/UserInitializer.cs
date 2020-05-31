using crm.Data;
using crm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Services
{
    public class UserInitializer
    {

        private readonly IServiceProvider _serviceProvider;

        public UserInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //This example just creates an Administrator role and one Admin users
        public async void Initialize()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                try
                {
                    //create database schema if none exists
                    var context = serviceScope.ServiceProvider.GetService<CrmContext>();
                    //context.Database.EnsureCreated();

                    //If there is already an Administrator role, abort
                    var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                    if (!(await _roleManager.RoleExistsAsync("Boss")))
                    {
                        //Create the Administartor Role
                        await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                    }
                    if (!(await _roleManager.RoleExistsAsync("Administrator")))
                    {
                        //Create the Administartor Role
                        await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                    }
                    if (!(await _roleManager.RoleExistsAsync("Administrator")))
                    {
                        //Create the Administartor Role
                        await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                    }
                    //Create the default Admin account and apply the Administrator role
                    var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                    string user = "xxx@yyy.com";
                    string password = "AbC!12345";
                    var success = await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, EmailConfirmed = true }, password);
                    if (success.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
                    }
                }
                catch
                {

                }
                
            }
        }
    }
    
}
