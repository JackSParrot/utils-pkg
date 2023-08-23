/*
using JackSParrot.JSON;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayFabService : IDisposable
    {
        bool _loggedIn = false;

        public PlayFabService()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = "3A295";
            }
            RegisterExceptionHandling();
        }

        public void LogIn(string userId, Action<bool> onLoggedIn)
        {
            Action<PlayFabError> onLoginFailure = e =>
            {
                Debug.LogError(e.GenerateErrorReport());
                _loggedIn = false;
                onLoggedIn(_loggedIn);
            };
            Action<LoginResult> onSuccess = r =>
            {
                _loggedIn = true;
                onLoggedIn(_loggedIn);
            };
            var request = new LoginWithCustomIDRequest { CustomId = userId, CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onLoginFailure);
        }

        public void SaveUserData(JSON data, Action<bool> onDone)
        {
            if (!_loggedIn) return;
            
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName =  "SaveUserData",
                FunctionParameter = new Dictionary<string, string>{["data"] = JSON.DumpString(data)}
            },
            r => onDone(true),
            e => onDone(false)
            );
        }

        public void LoadUserData(Action<JSON> onDone)
        {
            if (!_loggedIn)
            {
                onDone(null);
                return;
            }

            void onSuccess(ExecuteCloudScriptResult e)
            {
                var parser = new JSONParser();
                JSONObject userData = null;
                string res = e.FunctionResult?.ToString();
                if (!string.IsNullOrEmpty(res))
                {
                    var data = parser.Parse(res);
                    userData = data["Data"].AsObject();
                }
                onDone(userData);
            }
            void onError(PlayFabError e)
            {
                Debug.LogError(e.GenerateErrorReport());
                onDone(null);
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "GetUserData"
            }, onSuccess, onError);
            //PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onSuccess, onError);
        }

        #region Analytics
        public void TrackEvent(string name, Dictionary<string, object> parameters = null)
        {
            if (!_loggedIn)  return;
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
            {
                Body = parameters ?? new Dictionary<string, object>(),
                EventName = name
            },
            result => { },
            error => Debug.LogError(error.GenerateErrorReport()));
        }
        #endregion

        public void GetGameConfig(Action<JSONObject> onDone)
        {
            if (!_loggedIn) return;

            Action<GetTitleDataResult> onSuccess = result =>
            {
                var parser = new JSONParser();
                JSONObject config = new JSONObject(new Dictionary<string, JSON>
                {
                    ["Units"] = parser.Parse(result.Data["Units"]),
                    ["SummonSkills"] = parser.Parse(result.Data["SummonSkills"]),
                    ["SingleHealSkills"] = parser.Parse(result.Data["SingleHealSkills"]),
                    ["Factions"] = parser.Parse(result.Data["Factions"]),
                    ["Levels"] = parser.Parse(result.Data["Levels"]),
                    ["Packs"] = parser.Parse(result.Data["Packs"]),
                    ["Settings"] = parser.Parse(result.Data["Settings"])
                });
                onDone(config);
            };
            Action<PlayFabError> onError = e =>
            {
                Debug.LogError(e.GenerateErrorReport());
                onDone(null);
            };
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onSuccess, onError);
        }

        public void GetGameData(Dictionary<string, string> currentVersions, Action<string> callback)
        {
            if (!_loggedIn) return;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "GetConfigs",
                FunctionParameter = currentVersions,
                GeneratePlayStreamEvent = false

            },
            result =>
            {
                callback(result.FunctionResult != null ? result.FunctionResult.ToString() : string.Empty);
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
        }
        public void RegisterExceptionHandling()
        { 
            Application.logMessageReceived += HandleException;
        }

        void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                TrackEvent("Exception", new Dictionary<string, object>
                {
                    ["type"] = condition.Substring(0, Mathf.Min(condition.Length, 80)),
                    ["stack"] = stackTrace.Substring(0, Mathf.Min(stackTrace.Length, 80))
                });
            }
        }

        public void Dispose() { }
    }
}
*/