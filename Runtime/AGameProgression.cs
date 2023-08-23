using UnityEngine;

namespace JackSParrot.Data
{
    public abstract class AGameProgression
    {
        public abstract string SaveName { get; }

        protected bool dirty       = false;
        protected bool savesLocked = false;

        public bool IsDirty() => dirty;
        public void LockSaves() => savesLocked = true;

        public bool IsLocked => savesLocked;

        public void Save() => dirty = !savesLocked;

        public void Load(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Reset();
                return;
            }

            ResetData();
            LoadData(data);
        }

        public void UnlockSaves()
        {
            savesLocked = false;
            dirty = true;
        }

        public virtual string GetSerializedData()
        {
            dirty = false;
            return JsonUtility.ToJson(this);
        }

        protected virtual void LoadData(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
        }

        protected abstract void ResetData();

        public void Reset()
        {
            ResetData();
            savesLocked = false;
            Save();
        }
    }
}