using System.Collections.Generic;

namespace JackSParrot.Services
{
	public interface ITrackEvent
	{
		string EventName { get; }
		Dictionary<string, object> Parameters { get; }
	}
	public class TrackEvent : ITrackEvent
	{
		public string EventName { get; private set; } 
		public Dictionary<string, object> Parameters { get; private set; } 

		public TrackEvent(string name, Dictionary<string, object> parameters = null)
		{
			EventName = name;
			Parameters = parameters ?? new Dictionary<string, object>();
			Parameters.Add("custom", "");
		}
	}
}
