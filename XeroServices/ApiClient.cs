using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace XeroServices
{
    public class ApiClient
    {
        public static string GetTenantId(string accessToken)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            List<Connection> connections = client.GetFromJsonAsync<List<Connection>>("https://api.xero.com/connections")
                .ConfigureAwait(true).GetAwaiter().GetResult();
            return connections.FirstOrDefault().TenantId;
        }
    }
}
