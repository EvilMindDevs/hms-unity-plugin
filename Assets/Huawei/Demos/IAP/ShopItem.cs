//using HuaweiMobileServices.IAP;
//using UnityEngine;
//using UnityEngine.UI;

//public class ShopItem : MonoBehaviour
//{


//    public string productID;

//    private IapManager iapManager;

//    private Sprite itemImage;

//    private Image img;
//    private Text nameText, priceText, descText;

//    bool update;

//    private void Start()
//    {
//        iapManager = GameObject.Find("IapManager").GetComponent<IapManager>();

//        img = transform.Find("ItemImage").GetComponent<Image>();
//        nameText = transform.Find("ItemName").GetComponent<Text>();
//        priceText = transform.Find("ItemCost").GetComponent<Text>();
//        descText = transform.Find("ItemDesc").GetComponent<Text>();

//        img.sprite = itemImage;

//        nameText.text = "";
//        priceText.text = "";
//        descText.text = "";

//        update = false;
//    }

//    void Update()
//    {
//        if (update)
//        {
//            update = false;

//            itemImage = Resources.Load<Sprite>(productID);
//            img.sprite = itemImage;
//            Debug.Log("1");
//            ProductInfo productInfo = iapManager.GetProductInfo(productID);
//            Debug.Log("2"+ productInfo.ProductName);
//            nameText.text = productInfo.ProductName;
//            Debug.Log("3");
//            priceText.text = productInfo.Price;
//            Debug.Log("4");
//            descText.text = productInfo.ProductDesc;

//            Debug.Log("5");


//        }
//    }

//    public void Load()
//    {
//        Debug.Log("[HMSPlugin]: Loading item " + productID);
//        update = true;
//    }

//    public void Buy()
//    {
//        iapManager.BuyProduct(productID);
//    }
//}
