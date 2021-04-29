# Huawei Mobile Services Plugin

The HMS Unity plugin helps you integrate all the power of Huawei Mobile Services in your Unity game:

* Huawei Account Kit
* In App purchases: Consumable, non consumables and Subscriptions.
* Ads: Interstitial, rewarded videos and banner
* Push notifications
* Game leaderboards and achievements
* Huawei Anayltics kit
* Crash Service
* Remote Config
* Auth Service
* Cloud DB
* Drive Kit
* Nearby Service
* App Messaging

## Requirements
Android SDK min 21
Net 4.x

## Important
This plugin supports:
* Unity version 2019/2020 - Developed in Master Branch
* Unity version 2018 - Developed in 2018 Branch

**Make sure to download the corresponding unity package for the Unity version you are using from the release section**

## Troubleshooting
Please check our [wiki page](https://github.com/EvilMindDevs/hms-unity-plugin/wiki/Troubleshooting)

## Status
This is an ongoing project, currently WIP. Feel free to contact us if you'd like to collaborate and use Github issues for any problems you might encounter. We'd try to answer in no more than a working day.

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

### 3 - Update your agconnect-services.json file.

In order for the plugin to work, some kits are in need of agconnect-json file. Please download your latest config file from AGC and import into Assets/StreamingAssets folder.
![image](https://user-images.githubusercontent.com/6827857/113585485-f488bd80-9634-11eb-8b1e-6d0b5e06ecf0.png)
____

### 4 Connect your game with any HMS Manager

In order for the plugin to work, you need to deploy the needed Manager prefab inside your scene.

1. In Unity's project view, locate the plugins prefab folder
2. Drag and drop the Manager to your scene

Now you need your game to call the Manager from your game. You can do this by code or as a UI event. See below for further instructions.
    
#### Call the HMS by code

First, get the reference to the Manager

```csharp
private AccountManager accountManager =  GameObject.Find("AccountManager").GetComponent<AccountManager>();
```
### Account Kit (login)
Call login method in order to open the login dialog
```csharp
accountManager.Login();
```

## Analytics kit
 
1. Enable Analtics kit from AGC
2. Update ...Assets\StreamingAssets\agconnect-services.json file
 
 Send analytics function:
 
``` csharp
HMSAnalyticsManager.Instance.SendEventWithBundle(eventId, key, value);
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
  4. Analytics kit should be initialized in "onCreate()" for that we use ...Assets\StreamingAssets\agconnect-services.json file.
  
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
6. Analytics
7. Remote Config
8. Crash
9. Cloud DB
10. Auth Service
11. Drive Kit
12. Nearby Service
13. App Messaging

### Account

Official Documentation on Account Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050048870)

### In App Purchases

Official Documentation on IAP Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050033062)

### Ads

Official Documentation on Ads Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/publisher-service-introduction-0000001070671805)

### Push

Official Documentation on Push Kit: [Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/service-introduction-0000001050040060)

### Game

Official Documentation on Game Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050121216)

### Analytics

Official Documentation on Analytics Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050745149)

### Remote Config

Official Documentation on Remote Config: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-get-started)

### Crash

Official Documentation on Crash Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-crash-getstarted-0000001055260538)

### Cloud DB

Official Documentation on Cloud DB: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-clouddb-introduction)

### Auth Service

Official Documentation on Auth Service: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050048870)

### Drive Kit

Official Documentation on Drive Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/introduction-0000001050039630)

### Nearby Service

Official Documentation on Nearby Service: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/contactshield--0000001057494465)

### App Messaging

Official Documentation on App Messaging: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-appmessage-introduction-0000001071884501)
______

## License

This project is licensed under the MIT License




