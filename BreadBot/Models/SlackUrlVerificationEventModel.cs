namespace BreadBot.Models
{
	public class SlackUrlVerificationEventModel
	{
		public string Token { get; set; }
		public string Challenge { get; set; }
		public string Type { get; set; }
	}
}
