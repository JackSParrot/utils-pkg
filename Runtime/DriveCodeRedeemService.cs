using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "DriveCodeRedeemService", menuName = "JackSParrot/Services/DriveCodeRedeemService")]
    public class DriveCodeRedeemService : ACodeRedeemService
    {
        [SerializeField]
        private string url = null;

        public override void Cleanup()
        {
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        public override void RedeemCode(string code, Action<string> response)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("Missing url in DriveCodeRedeemService");
                response?.Invoke(string.Empty);
                return;
            }
            UnityWebRequest req = new UnityWebRequest($"{url}?code={code}", "GET", new DownloadHandlerBuffer(), null);
            req.SendWebRequest().completed += o => ResponseReceived(o, response);
        }

        private void ResponseReceived(UnityEngine.AsyncOperation obj, Action<string> response)
        {
            UnityWebRequestAsyncOperation req = obj as UnityWebRequestAsyncOperation;
            if (req == null || req.webRequest == null)
            {
                response?.Invoke(string.Empty);
                return;
            }

            UnityWebRequest resp = req.webRequest;
            if (resp.result != UnityWebRequest.Result.Success)
            {
                response?.Invoke(string.Empty);
                return;
            }

            string[] data = resp.downloadHandler.text.Split(';');
            if (data.Length < 2)
            {
                response?.Invoke(string.Empty);
                return;
            }

            response?.Invoke(data[1]);
        }
    }
}