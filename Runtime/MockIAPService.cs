using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.Services;

namespace Game.Services.IAP
{
    public class MockIAPService : AIAPService
    {
        public override void BuyProduct(string productId, Action<bool> callback = null)
        {
            if (callback == null)
            {
                OnProductRestored(productId);
            }
            else
            {
                callback?.Invoke(true);
            }
        }

        public override void BuyProduct(IAPProduct product, Action<bool> callback = null)
        {
            BuyProduct(product.Id, callback);
        }

        public override IAPProduct GetProduct(string productId)
        {
            return new IAPProduct("0", "", "0.01 USD");
        }

        public override List<IAPProduct> GetProducts()
        {
            return new List<IAPProduct> { GetProduct("") };
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
            Status = EServiceStatus.Initialized;
            yield return null;
        }
    }
}