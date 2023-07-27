using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Presentation
{
    public static class TimerFunctions
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("SendNotification")]
        public static async Task Run([TimerTrigger("%SendNotificationCron%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var requestUri = Environment.GetEnvironmentVariable("SendNotificationRequestedUri");

            log.LogInformation($"Put request was sended to {requestUri}");
            var res = await httpClient.PutAsync(requestUri, null);
            
            if (res.IsSuccessStatusCode)
            {
                log.LogInformation($"Put request was success");
            }
            else
            {
                var text = new StringBuilder($"Request has response with status code {res.StatusCode} and content:\n");

                using var sr = new StreamReader(await res.Content.ReadAsStreamAsync());
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    text.Append(line + "\n");
                }
                log.LogWarning(text.ToString());
            }
        }
    }
}
