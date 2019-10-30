using BreadBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BreadBot
{
	public static class DailyBread
	{
		//environment variables
		private const string PostMessageUrl = "PostMessageUrl";
		private const string BotToken = "BotUserToken";
		private const string ChannelName = "Channel";

		[FunctionName("VibeCheck")]
		public static async void RunAsync([TimerTrigger("0 0 9 * * MON-FRI")]TimerInfo myTimer, ILogger log)
		{
			log.LogInformation("Firing chron trigger.");

			//get environment variables
			var postMessageUrl = Helper.GetEnvironmentVariable(PostMessageUrl);
			var botToken = Helper.GetEnvironmentVariable(BotToken);
			var channelName = Helper.GetEnvironmentVariable(ChannelName);

			try
			{
				var message = new PostMessageModel
				{
					text = "Give us this day our daily bread.",
					channel = channelName
				};

				var content = JsonConvert.SerializeObject(message);
				using (var client = new HttpClient())
				{
					log.LogInformation("Posting message.");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", botToken);
					await client.PostAsync(postMessageUrl, new StringContent(content, Encoding.UTF8, "application/json"));
				}
			}
			catch (Exception ex)
			{
				log.LogInformation(ex.Message);
			}
		}
	}
}
