using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("[Controller]")]
    public class BuscaPorAtivo : ControllerBase
    {

        private static readonly HttpClient client = new HttpClient();

        [HttpGet("{symbol}")]
        public async Task<ActionResult<string>> BuscandoPorAtivo(string symbol)
        {
            string apiKey = "0a9e1b13d9bded5b922da6b7a89620694c4bee4b8ad3d01a6f1dda9c534e1d49";
         
            string url = $"https://serpapi.com/search.json?q={symbol}&hl=pt&engine=google_finance&api_key={apiKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return Ok(responseBody);
        }
    }
}
