using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Credentials
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","myapi")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client{
                    ClientId="client",
                     // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={"api1"}

                },
                new Client
                {
                    ClientId="tms",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes={"api1"},
                    AccessTokenLifetime=600,
                    //AbsoluteRefreshTokenLifetime=40,

                }

            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="1",
                    Username="TMSMOBILE",
                    Password="1qaz",
                },
                new TestUser
                {
                    SubjectId="2",
                    Username="ITMS",
                    Password="Clic",
                }

            };
        }
    }
}
