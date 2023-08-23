using System.Collections;
using UnityEngine;

namespace JackSParrot.Data
{
    public abstract class AReadOnlyDataPersistanceProvier : ScriptableObject
    {
        public abstract IEnumerator Initialize();
        public abstract string Load(string key);
    }
}