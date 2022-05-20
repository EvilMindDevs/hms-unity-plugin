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
* Drive Kit
* Nearby Service
* App Messaging


## Requirements
Android SDK min 21
Net 4.x

## Important
This plugin supports:
* Unity version 2019, 2020, 2021 - Developed in master Branch
* Unity version 2018 - Developed in 2.0-2018 Branch

**Make sure to download the corresponding unity package for the Unity version you are using from the release section**

## Troubleshooting
Please check our [wiki page](https://github.com/EvilMindDevs/hms-unity-plugin/wiki/Troubleshooting)

## Status
This is an ongoing project, currently WIP. Feel free to contact us if you'd like to collaborate and use Github issues for any problems you might encounter. We'd try to answer in no more than a working day.

## Connect your game Huawei Mobile Services in 5 easy steps

1. Register your app at Huawei Developer
2. Import the Plugin to your Unity project
3. Connect your game with the HMS Kit Managers

### 1 - Register your app at Huawei Developer

#### 1.1-  Register at [Huawei Developer](https://developer.huawei.com/consumer/en/)

#### 1.2 - Create an app in AppGallery Connect.
During this step, you will create an app in AppGallery Connect (AGC) of HUAWEI Developer. When creating the app, you will need to enter the app name, app category, default language, and signing certificate fingerprint. After the app has been created, you will be able to obtain the basic configurations for the app, for example, the app ID and the CPID.

1. Sign in to Huawei Developer and click **Console**.
2. Click the under **Ecosystem services**, click on **App Services**.
3. Click on the **AppGallery Connect** under Distribution and Promotion.
4. Click **My apps**.
5. On the displayed **My apps** page, click **New app** on top right.
6. Enter the App name, select App category (Game), and select Default language as needed.
7. Upon successful app creation, the App information page will automatically display. There you can find the App ID that is assigned by the system to your app.

#### 1.3 Add Package Name
Set the package name of the created application on the AGC.

1. In app information page, there is a label at top saying **"My Apps"**. Mouse hover on it and select **My Project**. This will lead you to the project information of your application
2. You should see a pop up asking about your package name for the application. Select **Manually enter a package name**
3. Fill in the application package name in the input box and click save.

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

1. In AppGallery Connect, go to **My Project** and select your project.
2. Go to the App information section, click on **+** button and enter the SHA-256 fingerprint that you generated earlier.
3. Click √ to save the fingerprint.

____

### 2 - Import the plugin to your Unity Project

To import the plugin:

1. Download the [.unitypackage](https://github.com/EvilMindDevs/hms-unity-plugin/releases)
2. Open your game in Unity
3. Choose Assets> Import Package> Custom
![Import Package](http://evil-mind.com/huawei/images/importCustomPackage.png "Import package")
4. In the file explorer select the downloaded HMS Unity plugin. The Import Unity Package dialog box will appear, with all the items in the package pre-checked, ready to install.
![image](https://user-images.githubusercontent.com/6827857/113576269-e8e2ca00-9627-11eb-9948-e905be1078a4.png)
5. Select Import and Unity will deploy the Unity plugin into your Assets Folder
____

### 3 - Update your agconnect-services.json file.

In order for the plugin to work, some kits are in need of agconnect-json file. Please download your latest config file from AGC and import into Assets/StreamingAssets folder.
![image](https://user-images.githubusercontent.com/6827857/113585485-f488bd80-9634-11eb-8b1e-6d0b5e06ecf0.png)
____

### 4 - Connect your game with any HMS Kit

In order for the plugin to work, you need to select the needed kits Huawei > Kit Settings.
![image](https://user-images.githubusercontent.com/6827857/142605459-04ca144e-6b34-4018-8f81-44a5ed67cbf3.png)

It will automaticly create the GameObject for you and it has DontDestroyOnLoad implemented so you don't need to worry about reference being lost.

Now you need your game to call the Kit Managers from your game. See below for further instructions.
    
## Account Kit (Sign In)
Call login method in order to open the login dialog. Be sure to have AccountKit enabled in Huawei > Kit Settings.

```csharp
HMSAccountManager.Instance.SignIn();
```

## Analytics kit
 
1. Enable Analtics kit from AGC
2. Update ...Assets\StreamingAssets\agconnect-services.json file
 
 Send analytics function:
 
``` csharp
HMSAnalyticsManager.Instance.SendEventWithBundle(eventId, key, value);
  ```
  
## In App Purchases
Register your products via custom editor under Huawei > Kit Settings > IAP tab.
![image](https://user-images.githubusercontent.com/6827857/113579431-f8184680-962c-11eb-9bfd-13ec69402536.png)
Write your product identifier that is in AGC and select product type.

If you check "Initialize On Start" checkbox, it'll automaticly retrieve registered products on Start.
If you want to initialize the IAP by yourself, call the function mentioned in below. You can also set callbacks as well.

``` csharp
HMSIAPManager.Instance.CheckIapAvailability();

HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

private void OnCheckIapAvailabilityFailure(HMSException ex)
    {
        
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        
    }
```

Open the Purchase dialog by calling to BuyProduct method. You can set callbacks and check which product was purchased.
```csharp
HMSIAPManager.Instance.BuyProduct(string productID)

HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;

private void OnBuyProductSuccess(PurchaseResultInfo result)
    {
        if (result.InAppPurchaseData.ProductId == "removeAds")
        {
            // Write your remove ads logic here.
        }
    }
```

Restore purchases that have been bought by user before.
```csharp
 HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            //restoredProducts contains all products that has been restored.
        });
```

You can also use "Create Constant Classes" button to create a class called HMSIAPConstants which will contain all products as constants and you can call it from your code. Such as;
```csharp
HMSIAPManager.Instance.BuyProduct(HMSIAPConstants.testProduct);
```

## Ads kit
There is a custom editor in Huawei > Kit Settings > Ads tab.
![image](https://user-images.githubusercontent.com/6827857/142604639-57b5856d-f001-492f-9014-eb670eb50e8f.png)

You can enable/disable ads that you want in your project.
Insert your Ad Id into these textboxes in the editor.
If you want to use test ads, you can check UseTestAds checkbox that'll overwrite all ad ids with test ads.

If you want to know more details about Splash Ad and its configuration, please check this article written by @sametguzeldev [here](https://medium.com/huawei-developers-tr/extend-your-advertisement-with-hms-ads-kit-splash-ad-in-unity-3e13f38f3abe).

Then you can call certain functions such as
```csharp
    HMSAdsKitManager.Instance.ShowBannerAd();
    HMSAdsKitManager.Instance.HideBannerAd();
    HMSAdsKitManager.Instance.ShowInterstitialAd();
    
    HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
    HMSAdsKitManager.Instance.ShowRewardedAd();
    
    public void OnRewarded(Reward reward)
    {
       
    }
```
**For Using the Consent SDK, please refer to [Consent](https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/publisher-service-consent-settings-0000001075342977).**

## Game kit
There is a custom editor in Huawei > Kit Settings > Game Service tab.
![image](https://user-images.githubusercontent.com/6827857/114309121-cef52b80-9aee-11eb-93d8-e69fda402ee3.png)

Check "Initialize on Start" checkbox to initialize the Game Service Kit on Start or call HMSGameManager.Instance.Init() in your custom logic.

```csharp
   HMSGameManager.Instance.Init();
```

You can use "Create Constant Classes" button to create a class called HMSLeaderboardConstants or HMSAchievementConstants which will contain all achievements and leaderboards as constants and you can call it from your code. Such as;
```csharp
    HMSLeaderboardManager.Instance.SubmitScore(HMSLeaderboardConstants.topleaderboard,50);
    HMSAchievementsManager.Instance.RevealAchievement(HMSAchievementConstants.firstshot);
```

You can call native calls to list achievements or leaderboards.
```csharp
  HMSAchievementsManager.Instance.ShowAchievements();
  HMSLeaderboardManager.Instance.ShowLeaderboards();
```

# App Update in Game Service

There is a method in Game Service called CheckAppUpdate that will trigger the update mechanism of HMS to detect if there is newer version in AppGallery.
It triggers OnAppUpdateInfo inside HMSGameManager that is returning status,rtnCode,rtnMessage,isExit,buttonStatus. This callback gets called after CheckAppUpdate is done.
If you want to receive this callback, please subscribe into it before calling CheckAppUpdate.

It requires two booleans;
showAppUpdate: Making this true will prompt a native UI that will show the user there is a newer version if there is an update.
forceAppUpdate: Making this true will remove the cancel button from the UI and forcing user to update.
```csharp
    HMSGameManager.Instance.OnAppUpdateInfo = OnAppUpdateInfo;
    HMSGameManager.Instance.CheckAppUpdate(showAppUpdate,forceAppUpdate);
```

# Connect API
2.1.0 version comes with Connect API features!
Right now we've implemented Publishing API and PMS API. To be able to use these APIs, you need to create an API Client through AppGallery Connect.

After selecting your project on AGC, please go to Users and Permissions section.
Find API key section on the left side and click Connect API.
On the right side, you will see a button called "Create". Click on it to create an API Client for Connect API.
![image](https://user-images.githubusercontent.com/6827857/146188264-f423b51c-c5eb-4de1-a230-3afd6f2f39ad.png)

After creating your key, please copy Client ID and Key section.
![image](https://user-images.githubusercontent.com/6827857/146189847-6d49f155-472b-4259-b3e2-760599662ae4.png)

Paste your Client ID to Client ID section, Key to Client Secret section in Token Obtainer Editor.
![image](https://user-images.githubusercontent.com/6827857/146190339-db5f98b9-7419-46aa-b660-1deb35f7183e.png)


## Publishing API
This API here to help you to publish your apk or aab after a successfull build. You can access this API by going 
>Huawei>Connect API>Publishing API>Querying App Information

From Querying App Information, you can check you app name, app category and your release state.
But most fun part starts after those information. Cause those informations there just for letting you know "I can communicate with AppGallery".

After informations there are a checkbox called "Upload After Build". If you select this checkbox, than Plugin will ask you everytime you do a successfull build "Should I send this apk/aab to AppGallery Connect?". If you select yes, than sending work will be started and you can check it from console or from progress bar. After uploading, you can check your apk/abb from the App Gallery Connect.

***Note: If you are using AAB, you should consider reading the warning after enabling the checkbox. 
"Please Check the App Signing Feature Enabled on AppGallery Connect For Uploading AAB Packages"***

![readmePhotoCensored](https://user-images.githubusercontent.com/16370078/145428901-ba2150ca-995d-443d-9498-24c4e64e6760.png)

## PMS API
This API here to help you to manage your products. You can access this API by going 
>Huawei>Connect API>PMS API

***Query IAP Products***, You can view all of your products with or without filtering by Product ID and Product name.

You can create a product with ***Create a Product*** or import your products with ***Create Products***.

***Note: You can not edit your deleted products.
Note: You can not change your products' purchase type which you created.***

![readmePhotoCensored](https://user-images.githubusercontent.com/39373386/145815616-c2abf3d2-e303-41df-b000-bb4fe953a86f.png)

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

In order for Nearby Service demo to work we have to declare specific permissions in the AndroidManifest.xml file.

Adding Permissions for Nearby Service Documentation: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/system-Guides/nearbyservice-add-access-0000001142548835)

```groovy
<!-- Permission to check Wi-Fi status -->
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<!-- Permission to change Wi-Fi status -->
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />
<!-- Permission to obtain a coarse device location -->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<!-- Permission to obtain the accurate device location -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<!--Permission to read external storage -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"/>
<!-- Permission to write data to external storage -->
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<!-- Bluetooth permissions -->
<uses-permission android:name="android.permission.BLUETOOTH" android:maxSdkVersion="30" />    
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" android:maxSdkVersion="30"/>    
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />    
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />    
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
```
**Note:** The **ACCESS_FINE_LOCATION** , **WRITE_EXTERNAL_STORAGE**, and **READ_EXTERNAL_STORAGE** permissions are dangerous system permissions, so you need to dynamically apply for these permissions. If your app does not have these permissions, Nearby Service cannot enable broadcast or scanning for your app.

### App Messaging

Official Documentation on App Messaging: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-appmessage-introduction-0000001071884501)
______

## License

This project is licensed under the MIT License




