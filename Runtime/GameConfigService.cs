using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.JSON;
using JackSParrot.Services;
using UnityEngine;

namespace JackSParrot.Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "GameConfigService", menuName = "JackSParrot/Services/GameConfigService")]
    public class GameConfigService : AService
    {
        public AGameDataConverter GameDataConverter;
        
        [NonSerialized]
        private Dictionary<Type, AGameConfig> _configs = new Dictionary<Type, AGameConfig>();
        
        [NonSerialized]
        private List<AGameConfig> _configsList = new List<AGameConfig>();

        [SerializeField]
        private AReadOnlyDataPersistanceProvier dataPersistanceProvider;

        [SerializeField]
        private AGameConfigProvider gameConfigsProvider;

        public void Load()
        {
            foreach (AGameConfig config in _configsList)
            {
                config.Load(dataPersistanceProvider.Load(config.Name));
            }
        }

        public string Serialize()
        {
            JSONObject serialized = new JSONObject();
            foreach (AGameConfig config in _configsList)
            {
                serialized.Add(config.Name, JSON.JSON.LoadString(JsonUtility.ToJson(config)));
            }

            return serialized.ToString();
        }

        public override void Cleanup()
        {
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            gameConfigsProvider.GetConfigs().ForEach(AddConfig);
            Load();
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        public T Get<T>() where T : AGameConfig
        {
            Debug.Assert(_configs.ContainsKey(typeof(T)));
            return _configs[typeof(T)] as T;
        }

        private void AddConfig<T>(T config) where T : AGameConfig
        {
            Type configType = config.GetType();
            Debug.Assert(!_configs.ContainsKey(configType));
            _configs.Add(configType, config);
            _configsList.Add(config);
        }
    }
}