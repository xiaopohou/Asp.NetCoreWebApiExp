using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_MobileRepository.Helpers;

namespace TMS_MobileAPI.Core
{
    public class TokenClientHelper
    {
        public static string tokenEndPoint { get; set; }
        public static async Task GetTokenClient()
        {
            if (string.Equals(TokenClientHelper.tokenEndPoint, null))
            {
                var discoveryClient = new DiscoveryClient(ConfigurationHelper.GetValue("AuthorityUrl:Url"))
                { Policy = { RequireHttps = false } };
                var disco = await discoveryClient.GetAsync();
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }
                tokenEndPoint = disco.TokenEndpoint;
                return;
            }
        }
    }
}
