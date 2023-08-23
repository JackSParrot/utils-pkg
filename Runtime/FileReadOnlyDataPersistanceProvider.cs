using System;
using System.Collections;
using JackSParrot.JSON;
using UnityEngine;

namespace JackSParrot.Data
{
	[CreateAssetMenu(fileName = "FileReadOnlyDataPersistanceProvider", menuName = "JackSParrot/Data/FileReadOnlyDataPersistanceProvider")]
	public class FileReadOnlyDataPersistanceProvider: AReadOnlyDataPersistanceProvier
	{
		[SerializeField]
		private TextAsset defaultConfig;

		private JSONObject _data;

		public override IEnumerator Initialize()
		{
			try
			{
				_data = JSON.JSON.LoadString(defaultConfig.text);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				_data = new JSONObject();
			}

			yield break;
		}

		public override string Load(string key)
		{
			return _data.Has(key) ? _data[key].ToString() : string.Empty;
		}
	}
}
