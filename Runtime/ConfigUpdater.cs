using System;
using UnityEngine;
using UnityEngine.Networking;

namespace JackSParrot.Data
{
	[CreateAssetMenu(fileName = "ConfigUpdater", menuName = "JackSParrot/Data/ConfigUpdater")]
	public class ConfigUpdater : ScriptableObject
	{
		[SerializeField]
		private AGameDataConverter dataConverter;
		
		[SerializeField]
		private string url = "";
		
		public void UpdateGameConfig(Action onDone = null)
		{
			DownloadLatest(onDone);
		}

		private void DownloadLatest(Action onDone)
		{
			UnityWebRequest request = new UnityWebRequest(url, "GET", new DownloadHandlerBuffer(), null);
			request.SendWebRequest().completed += r => RequestResponseReceived(r, onDone);
		}

		private void RequestResponseReceived(AsyncOperation obj, Action onDone)
		{
			UnityWebRequest request = (obj as UnityWebRequestAsyncOperation)?.webRequest;
			if (request == null || request.result == UnityWebRequest.Result.ConnectionError ||
				request.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError("Network error: " + request?.downloadHandler?.text);
				return;
			}

			OnConfigDownloaded(request.downloadHandler.text);
			onDone?.Invoke();
		}

		private void OnConfigDownloaded(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				Debug.LogError("Error downloading the config");
				return;
			}
			
			if (dataConverter != null)
			{
				dataConverter.Parse(data);
			}

			UpdateGameConfigFile(dataConverter != null ? dataConverter.Stringify() : data);
		}

		public void UpdateGameConfigFile(string contents)
		{
#if UNITY_EDITOR
			System.IO.File.WriteAllText(Application.dataPath + "/GameData/GameConfig.json", contents);
			UnityEditor.AssetDatabase.Refresh();
			Debug.Log("Config updated");
#else
			Debug.LogError("Config can only be updated in editor");
#endif
		}
		
#if UNITY_EDITOR
		[UnityEditor.MenuItem("Data/Update Config")]
		public static void UpdateGameConfigMenuItem()
		{
			string[] assetPaths = UnityEditor.AssetDatabase.FindAssets("t: ConfigUpdater");
			if (assetPaths.Length > 0)
			{
				ConfigUpdater instance = UnityEditor.AssetDatabase.LoadAssetAtPath<ConfigUpdater>(assetPaths[0]);
				instance.UpdateGameConfig();
			}
		}
		#endif
	}
}
