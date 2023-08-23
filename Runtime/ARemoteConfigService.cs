using JackSParrot.Services;

namespace JackSparrot.Services
{
	public abstract class ARemoteConfigService : AService
	{
		public bool Fetching { get; protected set; } = false;
        
		public abstract bool IsLocal { get; }
		public abstract int GetInt(string name, int defaultValue = 0);
		public abstract float GetFloat(string name, float defaultValue = 0f);
		public abstract bool GetBool(string name, bool defaultValue = false);
		public abstract string GetString(string name, string defaultValue = "");
	}
}
