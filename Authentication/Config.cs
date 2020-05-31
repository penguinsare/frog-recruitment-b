using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Authentication
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource(
                    "customProfile",
                    new[] { "name", "access", "recruiterId" })
        };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource(
                    IdentityServerConstants.LocalApi.ScopeName,
                    new[] { "name", "access", "recruiterId" })
                {
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                                
                new Client
                {
                    ClientName = "vuejs_code_client",
                    ClientId = "vuejs_code_client",
                    AccessTokenType = AccessTokenType.Reference,
                    RequireConsent = false,
                    AccessTokenLifetime = 330,// 330 seconds, default 60 minutes
                    IdentityTokenLifetime = 300,
                    //AbsoluteRefreshTokenLifetime = 11400,
                    //Slidin
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://petkov.z6.web.core.windows.net",
                        "https://petkov.z6.web.core.windows.net/#callback",
                        "https://petkov.z6.web.core.windows.net/#silent-renew",
                        "https://polytech-software.com",
                        "https://polytech-software.com/#callback",
                        "https://polytech-software.com/#silent-renew",
                        "https://localhost:8080",
                        "https://localhost:8080/#callback",
                        "https://localhost:8080/#silent-renew"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://petkov.z6.web.core.windows.net",
                        "https://polytech-software.com",
                        "https://localhost:8080",
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://petkov.z6.web.core.windows.net",
                        "https://polytech-software.com",
                        "https://localhost:8080",
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "customProfile"
                    },
                    AllowOfflineAccess = true
                }
    
            };
        }
    }
}
