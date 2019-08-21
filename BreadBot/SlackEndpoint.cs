using BreadBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
		//environment variables
		private const string PostMessageUrl = "PostMessageUrl";
		private const string BotToken = "BotUserToken";
		private const string ChannelName = "Channel";


		[FunctionName("SlackEndpoint")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
		{
			log.LogInformation("Slack Endpoint received a request.");
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

				//get environment variables
				var postMessageUrl = Helper.GetEnvironmentVariable(PostMessageUrl);
				var botToken = Helper.GetEnvironmentVariable(BotToken);
				var channelName = Helper.GetEnvironmentVariable(ChannelName);

				//deserialize request to a model
				//var eventRequest = JsonConvert.DeserializeObject<EventRequestModel>(requestBody);

				//check if the event request was an app_mention
				//if (eventRequest.Type == "app_mention")
				//{
				log.LogInformation("Receieved app mention request type.");

				var random = new Random();
				var key = random.Next(0, 11);

				var botText = new Dictionary<int, string>
				{
					{0, "Let's get this bread!"},
					{1, "Let's yeet this wheat."},
					{2, "There is not a thing that is more positive than bread. – Fyodor Dostoevsky"},
					{3, "With bread, all sorrows are less - from Don Quixote"},
					{4, "Rather a piece of bread with a happy heart than wealth with grief. – Egyptian Proverb"},
					{5, "Check before you bite if it is bread or a stone. - Croatian Proverb"},
					{6, "Let them eat cake! - Marie Antoinnette"},
					{7, "The sky is the daily bread of the eyes. - Ralph Waldo Emerson"},
					{8, "Bread is the king of the table, and all else is merely the court that surrounds the king. - Louis Bromfield"},
					{9, "Without bread, all is misery. - William Cobbett"},
					{10, "How do you get a raise at the bread factory? Butter up your boss."}
				};

				var message = new PostMessageModel
				{
					text = botText[key],
					channel = channelName
				};

				var content = JsonConvert.SerializeObject(message);
				using (var client = new HttpClient())
				{
					log.LogInformation("Posting message.");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", botToken);
					await client.PostAsync(postMessageUrl, new StringContent(content, Encoding.UTF8, "application/json"));
				}
				return new OkResult();
				//}

				//log.LogInformation("Request does not match a valid criteria");
				//return new BadRequestResult();
			}
			catch (Exception ex)
			{
				log.LogError(ex.Message);
				return new InternalServerErrorResult();
			}
		}
	}
}
