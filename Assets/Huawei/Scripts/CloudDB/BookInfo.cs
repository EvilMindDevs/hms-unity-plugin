using HuaweiMobileServices.CloudDB;
using HuaweiMobileServices.Utils;
using UnityEngine;

using System;

namespace HmsPlugin
{
	public class BookInfo : JavaObjectWrapper, ICloudDBZoneObject
	{
		public BookInfo() : base("selam") { }
		public BookInfo(AndroidJavaObject javaObject) : base(javaObject) { }
		private int id;
		private string bookName;
		private string author;
		private double price;
		private string publisher;
		private DateTime publishTime;
		private bool shadowFlag;

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
			get { return Call<AndroidJavaObject>("getPrice").Call<double>("doubleValue"); }
			set { Call("setPrice", new AndroidJavaObject("java.lang.Double", value)); }
		}

		public string Publisher
		{
			get { return Call<string>("getPublisher"); }
			set { Call("setPublisher", value); }
		}

		public DateTime PublishTime
		{
			get { return new DateTime(Call<AndroidJavaObject>("getPublishTime").Call<long>("getTime")); }
			set { Call("setPublishTime", new AndroidJavaObject("java.util.Date", value.Ticks)); }
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
