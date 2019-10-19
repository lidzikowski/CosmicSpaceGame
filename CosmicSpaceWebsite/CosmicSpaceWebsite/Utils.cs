using CosmicSpaceWebsiteDll;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CosmicSpaceWebsite
{
    public class Utils
    {
        public static async Task<T> HttpGetAsync<T>(HttpClient Http, ApiService service, Dictionary<string, object> parameters)
        {
            string api = $"https://localhost:44396/api/user/{service.ToString()}";
            bool first = true;
            foreach (var item in parameters)
            {
                api += $"{(first ? "?" : "&")}{item.Key}={item.Value}";
                first = false;
            }

            var response = await Http.GetAsync(api);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }
    }
}