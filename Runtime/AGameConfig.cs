using System;
using UnityEngine;

namespace JackSParrot.Data
{
    [Serializable]
    public abstract class AGameConfig
    {
        public abstract string Name { get; }

        public virtual void Load(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
        }
    }
}