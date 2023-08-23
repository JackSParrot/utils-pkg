using UnityEngine.AddressableAssets;

namespace Services.AddressableUtils
{
    [System.Serializable]
    public class AssetReferenceScene : AssetReference
    {
        public override bool ValidateAsset(string path)
        {
            return path.EndsWith(".unity");
        }
    }
}
