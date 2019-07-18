using System.Collections.Generic;

namespace BreadBot.Models
{
	public class EventRequestModel
	{
		public string Token { get; set; }
		public string Team_id { get; set; }
		public string Api_app_id { get; set; }
		public EventModel Event { get; set; }
		public string Type { get; set; }
		public List<string> authed_users { get; set; }
		public string Event_id { get; set; }
		public int Event_time { get; set; }
	}

	public class EventModel
	{
		public string Type { get; set; }
		public string Event_ts { get; set; }
		public string User { get; set; }
		public string Ts { get; set; }
		public string Item { get; set; }
	}
}
