using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public interface IIAPProductManipulator : ICollectionManipulator
    {
        IEnumerable<HMSIAPProductEntry> GetAllProducts();
        void RemoveProduct(HMSIAPProductEntry product);
        AddIAPProductValueResult AddProduct(string identifier, HMSIAPProductType type);
    }


    public class IAPProductManipulator : IIAPProductManipulator
    {
        public event Action OnRefreshRequired;

        private HMSSettings _settings;
        private List<HMSIAPProductEntry> _productList;

        public IAPProductManipulator(HMSSettings settings)
        {
            _settings = settings;
            _productList = new List<HMSIAPProductEntry>();
            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                _productList.Add(new HMSIAPProductEntry(_settings.Keys.ElementAt(i), (HMSIAPProductType)Enum.Parse(typeof(HMSIAPProductType), _settings.Values.ElementAt(i))));
            }
        }

        public AddIAPProductValueResult AddProduct(string identifier, HMSIAPProductType type)
        {
            identifier = identifier.PreprocessValue();
            var canAdd = CanAdd(identifier);
            if (canAdd == AddIAPProductValueResult.OK)
            {
                _productList.Add(new HMSIAPProductEntry(identifier, type));
                _settings.Set(identifier, type.ToString());
                RequireRefresh();
            }

            return canAdd;
        }

        private void RequireRefresh()
        {
            OnRefreshRequired.InvokeSafe();
        }

        private AddIAPProductValueResult CanAdd(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) return AddIAPProductValueResult.Invalid;

            foreach (var products in _productList)
            {
                if (products.Identifier.Equals(identifier))
                {
                    return AddIAPProductValueResult.AlreadyExists;
                }
            }
            return AddIAPProductValueResult.OK;
        }

        public void Dispose()
        {
            OnRefreshRequired = null;
        }

        public IEnumerable<HMSIAPProductEntry> GetAllProducts()
        {
            return _productList;
        }

        public int GetProductCount()
        {
            return _productList.Count;
        }

        public void RemoveProduct(HMSIAPProductEntry product)
        {
            Debug.Assert(_productList.Contains(product), "Failed to find " + product.Identifier + " in IAP Product Settings file!");
            _productList.Remove(product);
            _settings.Remove(product.Identifier);
            RequireRefresh();
        }

        public void ClearAllProducts()
        {
            _productList.Clear();
            _settings.Clear();
            RequireRefresh();
        }
    }

    public enum AddIAPProductValueResult
    {
        OK,
        AlreadyExists,
        Invalid
    }
}
