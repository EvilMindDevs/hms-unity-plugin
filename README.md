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


## Requirements
Android SDK min 21
Net 4.x

## Important
This plugin supports:
* Unity version 2019, 2020 - Developed in Master Branch
* Unity version 2018 - Developed in 2018 Branch

**Make sure to download the corresponding unity package for the Unity version you are using from the release section**

## Troubleshooting
Please check our [wiki page](https://github.com/EvilMindDevs/hms-unity-plugin/wiki/Troubleshooting)

## Status
This is an ongoing project, currently WIP. Feel free to contact us if you'd like to collaborate and use Github issues for any problems you might encounter. We'd try to answer in no more than a working day.

### Expected soon features
* Native Ads Integration

## Connect your game Huawei Mobile Services in 5 easy steps

1. Register your app at Huawei Developer
2. Import the Plugin to your Unity project
3. Connect your game with the HMS Kit Managers
4. 

### 1 - Register your app at Huawei Developer

#### 1.1-  Register at [Huawei Developer](https://developer.huawei.com/consumer/en/)

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
![image](https://user-images.githubusercontent.com/6827857/113576269-e8e2ca00-9627-11eb-9948-e905be1078a4.png)
5. Select Import and Unity will deploy the Unity plugin into your Assets Folder
____

### 3 - Update your agconnect-services.json file.

In order for the plugin to work, some kits are in need of agconnect-json file. Please download your latest config file from AGC and import into Assets/StreamingAssets folder.
![image](https://user-images.githubusercontent.com/6827857/113585485-f488bd80-9634-11eb-8b1e-6d0b5e06ecf0.png)
____

### 4 - Connect your game with any HMS Kit

In order for the plugin to work, you need to select the needed kits Huawei > Kit Settings.

![image](https://user-images.githubusercontent.com/6827857/113576579-7b836900-9628-11eb-89a5-724c7188c819.png)

It will automaticly create the GameObject for you and it has DontDestroyOnLoad implemented so you don't need to worry about reference being lost.

Now you need your game to call the Kit Managers from your game. See below for further instructions.
    
##### Account Kit (Sign In)
Call login method in order to open the login dialog. Be sure to have AccountKit enabled in Huawei > Kit Settings.

```csharp
HMSAccountManager.Instance.SignIn();
```

##### Analytics kit
 
1. Enable Analtics kit from AGC
2. Update ...Assets\StreamingAssets\agconnect-services.json file
 
 Send analytics function:
 
``` csharp
HMSAnalyticsManager.Instance.SendEventWithBundle(eventId, key, value);
  ```
  
##### In App Purchases
Register your products via custom editor under Huawei > Kit Settings > IAP tab.
![image](https://user-images.githubusercontent.com/6827857/113579431-f8184680-962c-11eb-9bfd-13ec69402536.png)
Write your product identifier that is in AGC and select product type.

If you check "Initialize On Start" checkbox, it'll automaticly retrieve registered products on Start.
If you want to initialize the IAP by yourself, call the function mentioned in below. You can also set callbacks as well.

``` csharp
HMSIAPManager.Instance.CheckIapAvailability();

HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

private void OnCheckIapAvailabilityFailure(HMSException obj)
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

private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        if(obj.InAppPurchaseData.ProductId == "removeAds")
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

##### Ads kit
There is a custom editor in Huawei > Kit Settings > Ads tab.
![image](https://user-images.githubusercontent.com/6827857/113583224-0ae14a00-9632-11eb-83c3-a45ab2699e4f.png)

You can enable/disable ads that you want in your project.
Insert your Ad Id into these textboxes in the editor.
If you want to use test ads, you can check UseTestAds checkbox that'll overwrite all ad ids with test ads. 

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

##### Game kit
There is a custom editor in Huawei > Kit Settings > Game Service tab.
![image](https://user-images.githubusercontent.com/6827857/113587013-e6d43780-9636-11eb-8621-8fc4d0fdb433.png)

You can also use "Create Constant Classes" button to create a class called HMSLeaderboardConstants or HMSAchievementConstants which will contain all achievements and leaderboards as constants and you can call it from your code. Such as;
```csharp
    HMSLeaderboardManager.Instance.ReportScore(HMSLeaderboardConstants.topleaderboard,50);
    HMSAchievementsManager.Instance.RevealAchievement(HMSAchievementConstants.firstshot);
```

You can call native calls to list achievements or leaderboards.
```csharp
  HMSAchievementsManager.Instance.ShowAchievements();
  HMSLeaderboardManager.Instance.ShowLeaderboards();
```

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

Official Documentation on Analytics Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-get-started)

### Crash

Official Documentation on Analytics Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-crash-getstarted-0000001055260538)

### Cloud DB

Official Documentation on Analytics Kit: [ Documentation](https://developer.huawei.com/consumer/en/doc/development/AppGallery-connect-Guides/agc-clouddb-introduction)
______

## License

This project is licensed under the MIT License




