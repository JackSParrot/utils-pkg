using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Data
{
	public abstract class AGameConfigProvider: ScriptableObject
	{
		public abstract List<AGameConfig> GetConfigs();
	}
}
