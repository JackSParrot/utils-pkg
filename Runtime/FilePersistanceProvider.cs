using System;
using System.Collections;
using JackSParrot.JSON;
using UnityEngine;

namespace JackSParrot.Data
{
	[CreateAssetMenu(fileName = "FilePersistanceProvider", menuName = "JackSParrot/Data/FilePersistanceProvider")]
	public class FilePersistanceProvider: ADataPersistanceProvider
	{
		private string     _path;
		private JSONObject _data = null;

		public override IEnumerator Initialize()
		{
			_path = Application.persistentDataPath + "/data.json";

			if (System.IO.File.Exists(_path))
			{
				try
				{
					_data = JSON.JSON.LoadString(System.IO.File.ReadAllText(_path));
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					_data = new JSONObject();
				}
			}

			yield return null;
		}

		public override void Save(string key, string data)
		{
			_data[key] = data;
		}

		public override string Load(string key)
		{
			return _data.Has(key) ? _data[key].ToString() : string.Empty;
		}

		public override void DeleteAll()
		{
			_data = new JSONObject();
			Flush();
		}

		public override void Flush()
		{
			System.IO.File.WriteAllText(_path, _data.ToString());
		}
	}
}
