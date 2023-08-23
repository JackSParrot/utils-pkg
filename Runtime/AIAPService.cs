using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    [Serializable]
    public class IAPProduct
    {
        public readonly string Id;
        public readonly string LocalizedName;
        public readonly string LocalizedPrize;
        public IAPProduct(string id, string name, string price)
        {
            Id = id;
            LocalizedName = name;
            LocalizedPrize = price;
        }
    }
    
    [Serializable]
    public class StoreId
    {
        public string name;
        public string id;
    }

    [Serializable]
    public class IAPProdutConfig
    {
        public enum EProductType
        {
            OneTime,
            Consumable
        }
        public string Id;
        public EProductType ProductType;
        //store name -> store id // GooglePlay -> product_id
        public List<StoreId> StoresIds = new List<StoreId>();
    }
    
    public abstract class AIAPService : AService
    {
        public Action<string> OnProductRestored = delegate(string s) { };
        public abstract void BuyProduct(string productId, Action<bool> callback = null);
        public abstract void BuyProduct(IAPProduct product, Action<bool> callback = null);
        public abstract List<IAPProduct> GetProducts();
        public abstract IAPProduct GetProduct(string productId);
    }
}