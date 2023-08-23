using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Data
{
	public abstract class AGameProgressionProvider : ScriptableObject
	{
		public abstract List<AGameProgression> GetProgressions(GameConfigService gameConfigService);
	}
}
