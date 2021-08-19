using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;

namespace DFar.Passthrough
{
    public static class Passthrough
    {
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("Pass")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pass/{passId}")] HttpRequest req,
            ILogger log, 
            string passId)
        {
            var url = Environment.GetEnvironmentVariable(passId);
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception($"Unable to find URL provided by passId: {passId}");
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            return response;
        }
    }
}
