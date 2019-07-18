using BreadBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BreadBot
{
	public static class SlackEndpoint
	{
		[FunctionName("SlackEndpoint")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
		{
			log.LogInformation("Verification Endpoint received a request.");
			try
			{
				var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

				//determine if this is a verification challenge request
				if (requestBody.Contains("challenge"))
				{
					log.LogInformation("Attempting to verify Endpoint");

					var verificationEventModel = JsonConvert.DeserializeObject<SlackUrlVerificationEventModel>(requestBody);

					log.LogInformation("Endpoint Verification sent.");

					return new OkObjectResult(verificationEventModel.Challenge);
				}

				var eventRequest = JsonConvert.DeserializeObject<EventRequestModel>(requestBody);
				if (eventRequest.Type == "app_mention")
				{
					var message = new PostMessageModel
					{
						Text = "Let's get this bread!",
						Channel = ""
					};

					var postMessageUrl = Helper.GetEnvironmentVariable("PostMessageUrl");
					var botToken = Helper.GetEnvironmentVariable("BotToken");

					var content = JsonConvert.SerializeObject(message);
					using (var client = new HttpClient())
					{
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", botToken);
						await client.PostAsync(postMessageUrl, new StringContent(content, Encoding.UTF8, "application/json"));
					}
					return new OkResult();
				}

				return new BadRequestResult();

			}
			catch (Exception ex)
			{
				log.LogError(ex.Message);
				return new InternalServerErrorResult();
			}
		}
	}
}
