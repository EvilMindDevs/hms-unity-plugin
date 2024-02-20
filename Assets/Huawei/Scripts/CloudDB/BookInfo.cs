using HuaweiMobileServices.CloudDB;
using HuaweiMobileServices.Utils;
using UnityEngine;
using System;

namespace HmsPlugin
{
    public class BookInfo : JavaObjectWrapper, ICloudDBZoneObject
    {
        public BookInfo() : base("selam") { } // Aleyküm selam :D
        public BookInfo(AndroidJavaObject javaObject) : base(javaObject) { }

        public int Id
        {
            get => Call<AndroidJavaObject>("getId").Call<int>("intValue");
            set => Call("setId", new AndroidJavaObject("java.lang.Integer", value));
        }

        public string BookName
        {
            get => Call<string>("getBookName");
            set => Call("setBookName", value);
        }

        public string Author
        {
            get => Call<string>("getAuthor");
            set => Call("setAuthor", value);
        }

        public double Price
        {
            get => Call<AndroidJavaObject>("getPrice").Call<double>("doubleValue");
            set => Call("setPrice", new AndroidJavaObject("java.lang.Double", value));
        }

        public string Publisher
        {
            get => Call<string>("getPublisher");
            set => Call("setPublisher", value);
        }

        public DateTime PublishTime
        {
            get => new DateTime(Call<AndroidJavaObject>("getPublishTime").Call<long>("getTime"));
            set => Call("setPublishTime", new AndroidJavaObject("java.util.Date", value.Ticks));
        }

        public bool ShadowFlag
        {
            get => Call<bool>("getShadowFlag");
            set => Call("setShadowFlag", value);
        }

        public AndroidJavaObject GetObj() => base.JavaObject;
        public void SetObj(AndroidJavaObject arg0) => base.JavaObject = arg0;
    }
}