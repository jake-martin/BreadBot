using BreadBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace BreadBot
{
	public static class VerificationEndpoint
	{
		[FunctionName("VerificationEndpoint")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
		{
			log.LogInformation("Verification Endpoint received a request.");

			try
			{
				log.LogInformation("Attempting to verify Endpoint");

				//var url = Helper.GetEnvironmentVariable("SlackVerificationUrl");

				var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

				var verificationEventModel = JsonConvert.DeserializeObject<SlackUrlVerificationEventModel>(requestBody);

				//using (var httpClient = new HttpClient())
				//{
				//	var response = await httpClient.PostAsync(url, new StringContent(verificationEventModel.Challenge, Encoding.UTF8, "application/json"));
				//	response.EnsureSuccessStatusCode();
				//}

				log.LogInformation("Endpoint Verification sent.");

				return new OkObjectResult(verificationEventModel.Challenge);
			}
			catch (Exception ex)
			{
				log.LogError("Failed to verify: " + ex.Message);
				return new InternalServerErrorResult();
			}
		}
	}
}
