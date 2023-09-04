
namespace JackSParrot.Services
{
	public abstract class AStorageService : AService
	{
		public abstract void Save<T>(string    key, T      data);
		public abstract void SaveInt(string    key, int    data);
		public abstract void SaveString(string key, string data);
		public abstract void SaveFloat(string  key, float  data);
		public abstract void SaveBool(string   key, bool   data);

		public abstract T      Get<T>(string    key, T      defaultValue = default);
		public abstract int    GetInt(string    key, int    defaultValue = default);
		public abstract string GetString(string key, string defaultValue = default);
		public abstract float  GetFloat(string  key, float  defaultValue = default);
		public abstract bool   GetBool(string   key, bool   defaultValue = default);

		public abstract bool HasKey(string key);

		public abstract void Flush();
	}
}
