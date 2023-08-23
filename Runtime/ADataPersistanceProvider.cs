using System.Collections;
using UnityEngine;

namespace JackSParrot.Data
{
    public abstract class ADataPersistanceProvider : ScriptableObject
    {
        public abstract IEnumerator Initialize();
        public abstract  void Save(string key, string data);
        public abstract  string Load(string key);
        public abstract  void DeleteAll();
        public abstract  void Flush();
    }
}