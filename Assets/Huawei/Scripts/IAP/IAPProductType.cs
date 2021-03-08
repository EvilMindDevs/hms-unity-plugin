using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class IAPProductEntry
    {
        public string Identifier { get; set; }
        public IAPProductType Type { get; set; }

        public IAPProductEntry(string identifier, IAPProductType type)
        {
            Identifier = identifier;
            Type = type;
        }
    }

    public enum IAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2
    }
}
