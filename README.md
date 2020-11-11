# Huawei Mobile Services Plugin

The HMS Unity plugin helps you integrate all the power of Huawei Mobile Services in your Unity game:

* Huawei Account Kit
* In App purchases: Consumable, non consumables and Subscriptions.
* Ads: Interstitial, rewarded videos and banner
* Push notifications
* Game leaderboards and achievements
* Huawei Anayltics kit


## Requirements
Android SDK min 21
Net 4.x

## Important
This plugin supports:
* Unity version 2019 - Developed in Master Branch
* Unity version 2018 - Developed in 2018 Branch


**If analytics kit will not used, delete "agconnect-credential-1.0.0.300.aar" and  "hianalytics-5.0.3.300.aar" from "...Assets\Plugins\Android"**

**Make sure to download the corresponding unity package for the Unity version you are using from the release section**

## Troubleshooting
Please check our [wiki page](https://github.com/EvilMindDevs/hms-unity-plugin/wiki/Troubleshooting)

## Status
This is an ongoing project, currently WIP. Feel free to contact us if you'd like to collaborate and use Github issues for any problems you might encounter. We'd try to answer in no more than a working day.

### Expected soon features
* Analytics integration

## Connect your game Huawei Mobile Services in 5 easy steps

1. Register your app at Huawei Developer
2. Import the Plugin to your Unity project
3. Configure your manifest
4. Connect your game with the HMS Managers
5. Connect the HMS Callbacks with your game

### 1 - Register your app at Huawei Developer

#### 1.1-  Register at [Huawei Developer](https://developer.huawei.com/consumer/en/)

![Huawei Developer](http://evil-mind.com/huawei/images/huaweiDeveloper.png "Huawei Developer")

#### 1.2 - Create an app in AppGallery Connect.
During this step, you will create an app in AppGallery Connect (AGC) of HUAWEI Developer. When creating the app, you will need to enter the app name, app category, default language, and signing certificate fingerprint. After the app has been created, you will be able to obtain the basic configurations for the app, for example, the app ID and the CPID.

1. Sign in to Huawei Developer and click **Console**.
2. Click the HUAWEI AppGallery card and access AppGallery Connect.
3. On the **AppGallery Connect** page, click **My apps**.
4. On the displayed **My apps** page, click **New**.
5. Enter the App name, select App category (Game), and select Default language as needed.
6. Upon successful app creation, the App information page will automatically display. There you can find the App ID and CPID that are assigned by the system to your app.

#### 1.3 Add Package Name
Set the package name of the created application on the AGC.

1. Open the previously created application in AGC application management and select the **Develop TAB** to pop up an entry to manually enter the package name and select **manually enter the package name**.
2. Fill in the application package name in the input box and click save.

> Your package name should end in .huawei in order to release in App Gallery

#### Generate a keystore.

Create a keystore using Unity or Android Tools. make sure your Unity project uses this keystore under the **Build Settings>PlayerSettings>Publishing settings**


#### Generate a signing certificate fingerprint.

During this step, you will need to export the SHA-256 fingerprint by using keytool provided by the JDK and signature file.

1. Open the command window or terminal and access the bin directory where the JDK is installed.
2. Run the keytool command in the bin directory to view the signature file and run the command.

    ``keytool -list -v -keystore D:\Android\WorkSpcae\HmsDemo\app\HmsDemo.jks``
3. Enter the password of the signature file keystore in the information area. The password is the password used to generate the signature file.
4. Obtain the SHA-256 fingerprint from the result. Save for next step.


#### Add fingerprint certificate to AppGallery Connect
During this step, you will configure the generated SHA-256 fingerprint in AppGallery Connect.

1. In AppGallery Connect, click the app that you have created and go to **Develop> Overview**
2. Go to the App information section and enter the SHA-256 fingerprint that you generated earlier.
3. Click √ to save the fingerprint.

____

### 2 - Import the plugin to your Unity Project

To import the plugin:

1. Download the [.unitypackage](https://github.com/EvilMindDevs/hms-unity-plugin/releases)
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
        <provider android:name="org.m0skit0.android.hms.unity.provider.AnalyticsContentProvider" android:authorities="org.m0skit0.android.hms.unity.activity.HMSContentProvider" android:exported="false" android:grantUriPermissions="true"/>
 
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
### 4 Connect your game with any HMS Manager

In order for the plugin to work, you need to deploy the needed HMS Manager prefab inside your scene.

1. In Unity's project view, locate the plugins prefab folder
2. Drag and drop the HMS Manager to your scene

Now you need your game to call the HMS Manager from your game. You can do this by code or as a UI event. See below for further instructions.
    
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

#### Analytics kit
 
1. Enable Analtics kit from AGC
2. Update ...Assets\Plugins\Android\assets\agconnect-services.json file
3. Add this provider to AndroidManifest.xml

```xml
<provider android:name="org.m0skit0.android.hms.unity.provider.AnalyticsContentProvider" android:authorities="org.m0skit0.android.hms.unity.activity.HMSContentProvider" android:exported="false" android:grantUriPermissions="true"/>
 ```
 
 Invoke analytics funtions:
 
``` csharp
AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

HiAnalyticsTools.EnableLog();
instance = HiAnalytics.GetInstance(activity);
instance.SetAnalyticsEnabled(true);
Bundle bundleUnity = new Bundle();
bundleUnity.PutString(key, value);
instance.OnEvent(eventID, bundleUnity);
  ```
  4. Analytics kit should be initialized in "onCreate()" for that we use ...Assets\Plugins\Android\assets\agconnect-services.json file.
  
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

### 5 Connect the HMS Callbacks with you game
In order to receive the callback information from Huawei Mobile Services, you need to set the callbacks that will control the information retrieved from Huawei Servers.

## Kits Specification
Find below the specific information on the included functionalities in this plugin

1. Account
2. In App Purchases
3. Ads
4. Push notifications
5. Game

### Account

Official Documentation on Account Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/account-introduction-v4)


### In App Purchases

Official Documentation on IAP Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/iap-service-introduction-v4)

### Ads

Official Documentation on Ads Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ads-sdk-introduction)

### Push

Official Documentation on Push Kit: [Documentation](https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/push-introduction)

### Game

Official Documentation on Game Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/game-introduction-v4)


### Analytics

Official Documentation on Analytics Kit: [ Documentation](https://developer.huawei.com/consumer/en/hms/huawei-analyticskit)
______

## License

This project is licensed under the MIT License




