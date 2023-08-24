using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace JackSParrot.Services
{
	public class UnityIAPService: AIAPService, IStoreListener
	{
		[SerializeField]
		private List<IAPProdutConfig> products = new List<IAPProdutConfig>();

		private IStoreController _controller = null;
		private List<IAPProduct> _products   = new List<IAPProduct>();

		private Dictionary<string, Action<bool>> _purchaseCallbacks = new Dictionary<string, Action<bool>>();

		public override List<IAPProduct> GetProducts() => _products;
		public override IAPProduct GetProduct(string productId) => _products.Find(p => p.Id.Equals(productId));

		public override void BuyProduct(IAPProduct product, Action<bool> callback = null) => BuyProduct(product.Id, callback);

		public override void BuyProduct(string productId, Action<bool> callback = null)
		{
			if (_controller == null || _purchaseCallbacks.ContainsKey(productId))
				return;
			Product product = _controller.products.WithID(productId);

			if (product != null && product.availableToPurchase)
			{
				if (callback != null)
				{
					_purchaseCallbacks.Add(productId, callback);
				}

				Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
				_controller.InitiatePurchase(product);
			}
			else
			{
				callback?.Invoke(false);
				Debug.Log($"BuyProductID {productId}: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogError(error);
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			Debug.LogError(error);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			string productId = args.purchasedProduct.definition.id;
			Debug.Log($"ProcessPurchase: PASS. Product: '{productId}' transaction: '{args.purchasedProduct.transactionID}'");

			if (_purchaseCallbacks.ContainsKey(productId))
			{
				_purchaseCallbacks[productId](true);
				_purchaseCallbacks.Remove(productId);
			}
			else
			{
				OnProductRestored?.Invoke(productId);
			}

			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			Debug.LogError($"product {i.definition.id} failed with error {p}");
			if (_purchaseCallbacks.ContainsKey(i.definition.id))
			{
				_purchaseCallbacks[i.definition.id](false);
				_purchaseCallbacks.Remove(i.definition.id);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			_controller = controller;
			_products.Clear();
			foreach (Product product in _controller.products.all)
			{
				if (product.availableToPurchase)
				{
					_products.Add(new IAPProduct(product.definition.id,
												 product.metadata.localizedTitle,
												 product.metadata.localizedPriceString));
				}

				Debug.Log($"ProductInStore: {product.metadata.localizedTitle} -> {product.metadata.localizedPriceString}");
			}
		}

		public override void Cleanup()
		{
			OnProductRestored = delegate(string a) { };
		}

		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			foreach (IAPProdutConfig product in products)
			{
				IDs ids = new IDs();
				foreach (StoreId store in product.StoresIds)
				{
					ids.Add(store.storeId, store.productId);
				}

				ProductType type = product.ProductType == IAPProdutConfig.EProductType.Consumable
					? ProductType.Consumable
					: product.ProductType == IAPProdutConfig.EProductType.OneTime
						? ProductType.NonConsumable
						: ProductType.Subscription;
				configurationBuilder.AddProduct(product.Id, type, ids);
			}

			UnityPurchasing.Initialize(this, configurationBuilder);
			Status = EServiceStatus.Initialized;
			yield return null;
		}
	}
}
