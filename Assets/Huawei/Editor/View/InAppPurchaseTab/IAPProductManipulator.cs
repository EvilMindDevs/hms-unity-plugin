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
        IEnumerable<IAPProductEntry> GetAllProducts();
        void RemoveProduct(IAPProductEntry product);
        AddIAPProductValueResult AddProduct(string identifier, IAPProductType type);
    }


    public class IAPProductManipulator : IIAPProductManipulator
    {
        public event Action OnRefreshRequired;

        private Settings _settings;
        private List<IAPProductEntry> _productList;

        public IAPProductManipulator(Settings settings)
        {
            _settings = settings;
            _productList = new List<IAPProductEntry>();
            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                _productList.Add(new IAPProductEntry(_settings.Keys.ElementAt(i), (IAPProductType)Enum.Parse(typeof(IAPProductType), _settings.Values.ElementAt(i))));
            }
        }

        public AddIAPProductValueResult AddProduct(string identifier, IAPProductType type)
        {
            identifier = identifier.PreprocessValue();
            var canAdd = CanAdd(identifier);
            if (canAdd == AddIAPProductValueResult.OK)
            {
                _productList.Add(new IAPProductEntry(identifier, type));
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

        public IEnumerable<IAPProductEntry> GetAllProducts()
        {
            return _productList;
        }

        public void RemoveProduct(IAPProductEntry product)
        {
            Debug.Assert(_productList.Contains(product), "Failed to find " + product.Identifier + " in IAP Product Settings file!");
            _productList.Remove(product);
            _settings.Remove(product.Identifier);
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
