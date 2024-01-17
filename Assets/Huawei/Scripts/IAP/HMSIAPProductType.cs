namespace HmsPlugin
{
    public class HMSIAPProductEntry
    {
        public string Identifier { get; set; }
        public HMSIAPProductType Type { get; set; }

        public HMSIAPProductEntry(string identifier, HMSIAPProductType type)
        {
            Identifier = identifier;
            Type = type;
        }
    }

    public enum HMSIAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2
    }
}
