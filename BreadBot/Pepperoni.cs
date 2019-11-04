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
	public static class Pepperoni
	{
		//environment variables
		private const string PostMessageUrl = "PostMessageUrl";
		private const string BotToken = "BotUserToken";
		private const string ChannelName = "Channel";

		[FunctionName("Pepperoni")]
		public static async void RunAsync([TimerTrigger("0 0 21 * * MON-FRI")]TimerInfo myTimer, ILogger log)
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
					text = ":thats_a_lotta_pepperoni: :thats_a_lotta_pepperoni: :thats_a_lotta_pepperoni:",
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
