using HuaweiMobileServices.CloudDB;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public class BookInfo : JavaObjectWrapper, ICloudDBZoneObject
	{
		public BookInfo() : base("com.clouddbdemo.kb.huawei.BookInfo") { }
		public BookInfo(AndroidJavaObject javaObject) : base(javaObject) { }

		private int _id;
		private string _bookName;
		private string _author;
		private double _price;
		private string _publisher;
		private Date _publishTime;
		private bool _shadowFlag = true;

		public int Id
		{
			get { return Call<AndroidJavaObject>("getId").Call<int>("intValue"); }
			set { Call("setId", new AndroidJavaObject("java.lang.Integer", value)); }
		}
		public string BookName
		{
			get { return Call<string>("getBookName"); }
			set { Call("setBookName", value); }
		}
		public string Author
		{
			get { return Call<string>("getAuthor"); }
			set { Call("setAuthor", value); }
		}
		public double Price
		{
			get { return Call<AndroidJavaObject>("getPrice").Call<int>("doubleValue"); }
			set { Call("setPrice", new AndroidJavaObject("java.lang.Double", value)); }
		}

		public string Publisher
		{
			get { return Call<string>("getPublisher"); }
			set { Call("setPublisher", value); }
		}

		public Date PublishTime
		{
			get { return Call<Date>("getPublishTime"); }
			set { Call("setPublishTime", value); }
		}

		public bool ShadowFlag
		{
			get { return Call<bool>("getShadowFlag"); }
			set { Call("setShadowFlag", value); }
		}

        public AndroidJavaObject GetObj() => base.JavaObject;
        public void SetObj(AndroidJavaObject arg0) => base.JavaObject = arg0; 

    }
}
