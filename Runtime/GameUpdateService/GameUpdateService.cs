﻿using System;
using System.Collections;
using System.Collections.Generic;
using JackSparrot.Services;
using UnityEngine;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "RemoteConfigUpdateService", menuName = "JackSParrot/Services/RemoteConfigUpdateService")]
    public class RemoteConfigUpdateService : AGameUpdateService
    {
        public event Action<bool> OnShowUpdatePopup = force => { };
        
        private ARemoteConfigService _remoteConfig;

        public override void TryShowUpdate()
        {
            string currentVersion = Application.version;
            string remoteVersion = _remoteConfig.GetString("version", currentVersion);
            bool forceUpdate = _remoteConfig.GetBool("forceUpgrade", false);
            if (CompareVersion(currentVersion, remoteVersion) > 0)
            {
                OnShowUpdatePopup(forceUpdate);
            }
        }

        public override void Cleanup()
        {
            
        }

        public override List<Type> GetDependencies()
        {
            return new List<Type>{typeof(ARemoteConfigService)};
        }

        public override IEnumerator Initialize()
        {
            _remoteConfig = ServiceLocator.GetService<ARemoteConfigService>();
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        public static int CompareVersion(string versionA, string versionB)
        {
            string[] partsA = versionA.Split('.');
            string[] partsB = versionB.Split('.');
            if (!int.TryParse(partsA[0], out int maa) || !int.TryParse(partsB[0], out int mab))
            {
                Debug.LogError($"Error parsing versions A:{versionA} B:{versionB}");
                return 0;
            }
            if (mab != maa)
            {
                return maa > mab ? -1 : 1;
            }
            if (!int.TryParse(partsA[1], out int mia) || !int.TryParse(partsB[1], out int mib))
            {
                Debug.LogError($"Error parsing versions A:{versionA} B:{versionB}");
                return 0;
            }
            if (mib != mia)
            {
                return mia > mib ? -1 : 1;
            }
            if (!int.TryParse(partsA[2], out int pa) || !int.TryParse(partsB[2], out int pb))
            {
                Debug.LogError($"Error parsing versions A:{versionA} B:{versionB}");
                return 0;
            }
            if (pb != pa)
            {
                return pa > pb ? -1 : 1;
            }

            return 0;
        }
    }
}