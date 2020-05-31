using crm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace crm.Authentication
{
    //public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    //{
    //    public MyUserClaimsPrincipalFactory(
    //        UserManager<ApplicationUser> userManager,
    //        RoleManager<IdentityRole> roleManager,
    //        IOptions<IdentityOptions> optionsAccessor)
    //        : base(userManager, roleManager, optionsAccessor)
    //    {
    //    }

    //    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    //    {
    //        var identity = await base.GenerateClaimsAsync(user);
    //        identity.AddClaim(new Claim("ContactName", user.UserName ?? ""));
    //        return identity;
    //    }
    //}
}
