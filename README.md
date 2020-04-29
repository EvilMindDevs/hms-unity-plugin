# Huawei Mobile Services Plugin

The HMS Unity plugin helps you integrate all the power of Huawei Mobile Services in your Unity game:

* Huawei Account Kit
* In App purchases


## Connect your game Huawei Mobile Services in 5 easy steps


1. Register your app at Huawei Developer
2. Import the Plugin to your Unity project
3. Configure your manifest
4. Connect your game with the HMS Manager
5. Connect the HMS Callback Handler with your game

### 1 - Register your app at Huawei Developer

Register at [Huawei Developer](https://developer.huawei.com/consumer/en/)
____

### 2 - Import the plugin to your Unity Project

To import the plugin:

1. Download the [.unitypackage](http://evil-mind.com/huawei/downloads/HMSPlugin.unitypackage)
2. Open your game in Unity
3. Choose Assets> Import Package> Custom
![Import Package](http://evil-mind.com/huawei/images/importCustomPackage.png "Import package")
4. In the file explorer select the downloaded HMS Unity plugin. The Import Unity Package dialog box will appear, with all the items in the package pre-checked, ready to install.
![Import Dialog](http://evil-mind.com/huawei/images/unityImport.png "Import dialog")
5. Select Import and Unity will deploy the Unity plugin into your Assets Folder
____

### 3 - Configure your Manifest

In order for the plugin to work you need to add some information to your Android's Manifest. Make sure you have this information before proceeding.

* App ID. The app's unique ID.
* CPID. The developer's unique ID.
* Package Name

Get all this info from [Huawei Developer](https://developer.huawei.com/consumer/en/). Open the developers console go to My Services > HUAWEI IAP, and click on your apps name to enter the Detail page.

![Detail page](http://evil-mind.com/huawei/images/appInfo.png "Detail page")
____

#### How to configure the Manifest

1. Open Unity and choose **Huawei> App Gallery> Configure** The manifest configuration dialog will appear.

    ![Editor Tool](http://evil-mind.com/huawei/images/unityMenu.png "Editor tool")

2. Fill out the fields: AppID, CPID and package name.
3. Click Configure Manifest
    The plugin will include all the necessary information inside the Android Manifest
    * Permissions
    * Meta Data
    * Providers
And your manifest should look now like these:

``` xml
<?xml version="1.0" encoding="utf-8"?>
    <manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.unity3d.player" xmlns:tools="http://schemas.android.com/tools" android:installLocation="preferExternal">
        <supports-screens android:smallScreens="true" android:normalScreens="true" android:largeScreens="true" android:xlargeScreens="true" android:anyDensity="true" />
        <application android:theme="@style/UnityThemeSelector" android:icon="@mipmap/app_icon" android:label="@string/app_name">
        <activity android:name="com.unity3d.player.UnityPlayerActivity" android:label="@string/app_name">
            <intent-filter>
            <action android:name="android.intent.action.MAIN" />
            <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
        </activity>
        <meta-data android:name="com.huawei.hms.client.appid" android:value="appid=9999" />
        <meta-data android:name="com.huawei.hms.client.cpid" android:value="cpid=1234567890" />
        <meta-data android:name="com.huawei.hms.version" android:value="2.6.1" />
        <provider android:name="com.huawei.hms.update.provider.UpdateProvider" android:authorities="com.yourco.huawei.hms.update.provider" android:exported="false" android:grantUriPermissions="true" />
        <provider android:name="com.huawei.updatesdk.fileprovider.UpdateSdkFileProvider" android:authorities="com.yourco.huawei.updateSdk.fileProvider" android:exported="false" android:grantUriPermissions="true" />
        </application>
        <uses-permission android:name="com.huawei.appmarket.service.commondata.permission.GET_COMMON_DATA" />
        <uses-permission android:name="android.permission.REQUEST_INSTALL_PACKAGES" />
        <uses-permission android:name="android.permission.INTERNET" />
        <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
        <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
        <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
        <uses-permission android:name="android.permission.READ_PHONE_STATE" />
    </manifest>
```
____
### 4 Connect your game with the HMS Manager

In order for the plugin to work, you need to deploy HMSManager prefab inside your scene.

1. In Unity's project view, locate the plugins prefab folder
2. Drag and drop the HMS Manager to your scene
3. Select the HMS Manager object in the hierarchy view
4. Configure the HMS Manager by selecting it and filling out the field in the Inspector
    * Key: Input your game's key
    * Set your game's products
        * Input the amount of products you created at developer's portal
        * Fill out the productID with the products ID you set on the developer's portal
        * Check the is Consumable checkbox if your product is consumable
    ![HMS Manager](http://evil-mind.com/huawei/images/HMSManagerConfig.png "HMS Manager")

Now you need your game to call the HMSManager from your game. You can do this by code or as a UI event. See below for further instructions.
    
#### Call the HMS by code

First, get the reference to the HMSManager

```csharp
private HMSManager hmsManager =  GameObject.Find("HMSManager").GetComponent<HMSManager>();
```
##### Account Kit (login)
Call login method in order to open the login dialog
```csharp
hmsManager.Login();
```

##### In App Purchases
You can retrieve a products information from App Gallery:
* Name
* Description
* Price

``` csharp
GetProductDetail(string productID);
```

Open the Purchase dialog by calling to BuyProduct method
```csharp
BuyProduct(string productID)
```

#### Call the HMS from your UI

1. Select you button and open the inspector
2. Find the On Click () section and drag and drop the HMS Manager object to the object selector
![On Click Event configuration](http://evil-mind.com/huawei/images/onClick.png "On Click Event configuration")
3. Select the method you want to call from the dropdown list:
    * Login
    * BuyProduct
    * GetProductDetail
    * GetPurchaseInfo
    * CheckForUpdates

If you are not sure how to do this, search the demo folder and open the sample scene.

![Sample store](http://evil-mind.com/huawei/images/demo.jpg "Sample store")
____

### 5 Connect the HMS Callbacks Handler with you game

The Callbacks Handler is a component inside the HMS Manager that will help you deal with all the responses from HMS Servers.

1. Open the HMSCallbacksHandler.cs file inside Plugins/HuaweiMbileServices/Scripts
This script is divided in two parts. The products help methods and the event methods:

    #### Products Help

    Gives you information about the products registered inside App Gallery Connect. Call them when needed.

    ```csharp
    public List GetProductList(){}

    public string GetProductName(string productID) {}

    public string GetProductDescription(string productID) {}

    public string GetProductPrice(string productID){}
    ```

    #### Event Methods

    Are the methods fired when an answer arrives form server. Override or implement these to make your game respond to successfull or error callbacks.
    (i.e: add 100 golden coins to player when On Purchase Success event is received )

    ##### Login 
    
    ```csharp
    public void OnLoginSuccess ( GameUserData gameUserData ){}

    public void OnLoginError ( int resultCode ){}
    ```
    ##### Product's Details
    ```csharp
    public void OnProductDetailSuccess ( ProductDetailsResponse response )
    public void OnProductDetailError ( int resultCode )
    ```
    ##### Buying a Product
    ```csharp
    public void OnPurchaseSuccess ( ProductPayReponse response )

    public void OnPurchaseError (int resultCode)
    ```


______

## License

This project is licensed under the MIT License




