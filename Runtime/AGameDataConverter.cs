using UnityEngine;

namespace JackSParrot.Data
{
	public abstract class AGameDataConverter: ScriptableObject
	{
		public abstract void Parse(string data);
		public abstract string Stringify();

#if REMOTE_SETTINGS
        public abstract void CheckAndUpdateFromRemoteSettings();
#endif
	}
}
