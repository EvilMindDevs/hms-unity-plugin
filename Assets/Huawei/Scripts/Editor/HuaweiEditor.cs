using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;


namespace HmsPlugin
{
    public class HuaweiEditor : EditorWindow
    {

        //#if UNITY_EDITOR
        bool appPackageEntered;
        bool appIDEntered;
        bool cpIDEntered;
        float labelWidth = 100f;


        string APPID = "1234567890";
        string PACKAGE = "com.yourcompany.yourgame";
        string CPID = "1234567890";

        String regexPackage = "^([A-Za-z]{1}[A-Za-z\\d_]*\\.)+[A-Za-z][A-Za-z\\d_]*$";
        String regexIDs = "^([0.9\\d_])";

        private GUIStyle redGUIStyle;

        // System.Xml.Linq.XNamespace android = "http://schemas.android.com/apk/res/android";


        //Add Huawei Menu
        [MenuItem("Huawei/App Gallery")]
        static void init()
        {

            HuaweiEditor window = (HuaweiEditor)EditorWindow.GetWindow((typeof(HuaweiEditor)));
            window.Show();
        }

        /// <summary>
        /// Creates the Huawei Menu GUI
        /// </summary>
        void OnGUI()
        {
            //Styles
            redGUIStyle = new GUIStyle();
            redGUIStyle.normal.textColor = Color.red;
            redGUIStyle.padding = new RectOffset(10, 0, 0, 0);

            GUIContent guiContentPackage = new GUIContent("Package", "your bundle identifier");


            // CREATE FORM
            GUILayout.Label("Welcome to Huawei Tools", EditorStyles.boldLabel);
            GUILayout.Label("v0.1");
            GUILayout.Space(5f);

            //Application Package
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(guiContentPackage, GUILayout.MinWidth(labelWidth));
            PACKAGE = EditorGUILayout.TextField(PACKAGE);
            GUILayout.FlexibleSpace();
            appPackageEntered = EditorGUILayout.Toggle(appPackageEntered);
            appPackageEntered = Regex.IsMatch(PACKAGE, regexPackage) && !PACKAGE.Equals("com.yourcompany.yourgame");
            EditorGUILayout.EndHorizontal();


            //APP ID 
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("App ID", GUILayout.MinWidth(labelWidth));
            APPID = EditorGUILayout.TextField(APPID);
            GUILayout.FlexibleSpace();
            appIDEntered = EditorGUILayout.Toggle(appIDEntered);
            appIDEntered = Regex.IsMatch(APPID, regexIDs) && !APPID.Equals("1234567890");
            EditorGUILayout.EndHorizontal();

            //CPID 
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("CPID", GUILayout.MinWidth(labelWidth));
            CPID = EditorGUILayout.TextField(CPID);
            GUILayout.FlexibleSpace();
            cpIDEntered = EditorGUILayout.Toggle(cpIDEntered);
            cpIDEntered = Regex.IsMatch(CPID, regexIDs) && !CPID.Equals("1234567890");
            EditorGUILayout.EndHorizontal();



            GUILayout.Space(10f);

            if (appPackageEntered && appIDEntered && cpIDEntered)
            {
                if (GUILayout.Button("Configure Manifest"))
                {
                    prepareManifest();
                }

            }
            else
            {
                // ERROR MESSAGES
                if (!appPackageEntered)
                {
                    GUILayout.Label("Please enter a valid package name", redGUIStyle);
                }
                if (!appIDEntered)
                {
                    GUILayout.Label("Please enter a valid app ID", redGUIStyle);
                }
                if (!cpIDEntered)
                {
                    GUILayout.Label("Please enter a valid CPI", redGUIStyle);
                }


            }

        }

        /// <summary>
        /// Adds the necesary information to Android Manifest
        /// If no manifest present gets Unity's default
        /// If present manifest, edits
        /// </summary>
        void prepareManifest()
        {

            string androidPluginsPath = Path.Combine(Application.dataPath, "Plugins");
            androidPluginsPath = Path.Combine(androidPluginsPath, "Android");

            string appManifestPath = Path.Combine(Application.dataPath, "Plugins");
            appManifestPath = Path.Combine(appManifestPath, "Android");
            appManifestPath = Path.Combine(appManifestPath, "AndroidManifest.xml");



            string defaultManifestPath = Path.Combine(Directory.GetParent(EditorApplication.applicationPath).FullName, "Data");
#if UNITY_EDITOR_OSX
            defaultManifestPath = Directory.GetParent(EditorApplication.applicationPath).FullName;
#endif
        defaultManifestPath = Path.Combine(defaultManifestPath, "PlaybackEngines");
        defaultManifestPath = Path.Combine(defaultManifestPath, "AndroidPlayer");
        defaultManifestPath = Path.Combine(defaultManifestPath, "Apk");

        if ( Int32.Parse(Application.unityVersion.Substring(0, 4)) >= 2019){
            
            defaultManifestPath = Path.Combine(defaultManifestPath, "UnityManifest.xml");
        }
        else
        {
            defaultManifestPath = Path.Combine(defaultManifestPath, "AndroidManifest.xml");
        }



            Debug.Log("preparing");

            Debug.Log(defaultManifestPath);

            // Check if user has already created AndroidManifest.xml file in its location.
            // If not, use default Unity AndroidManifest.xml.
            if (!File.Exists(appManifestPath))
            {
                if (!Directory.Exists(androidPluginsPath))
                {
                    Directory.CreateDirectory(androidPluginsPath);
                }


                File.Copy(defaultManifestPath, appManifestPath);

                Debug.Log("[HMS]: User defined AndroidManifest.xml file not found in Plugins/Android folder.");
                Debug.Log("[HMS]: Creating default app's AndroidManifest.xml from Unity's default file.");
            }
            else
            {
                // manifestFound = true;
                Debug.Log("[HMS]: AndroidManifest.xml file already exists in Plugins/Android folder.");
            }


            // OPEN MANIFEST FILE
            XmlDocument manifestFile = new XmlDocument();
            manifestFile.Load(appManifestPath);

            // Add needed permissions if they are missing.
            AddPermissions(manifestFile);

            // Add meta data
            AddMetaData(manifestFile);

            //Add provider
            AddProvider(manifestFile);

            // Save the changes.
            manifestFile.Save(appManifestPath);


            Debug.Log("[HMS]: App's AndroidManifest.xml ready");


        }

        void AddPermissions(XmlDocument manifest)
        {

            // The Huawei SDK needs permissions to be added to you app's manifest file:
            //<uses-permission android:name="com.huawei.appmarket.service.commondata.permission.GET_COMMON_DATA"/>
            //<uses-permission android:name="android.permission.REQUEST_INSTALL_PACKAGES"/>
            //<uses-permission android:name="android.permission.INTERNET"/>
            //<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE"/>
            //<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
            //<uses-permission android:name="android.permission.ACCESS_WIFI_STATE"/>
            //<uses-permission android:name="android.permission.READ_PHONE_STATE"/>

            bool hasCommonDataPermission = false;
            bool hasInstallPackagesPermission = false;
            bool hasInternetPermission = false;
            bool hasExternalStoragePermission = false;
            bool hasAccessNetworkStatePermission = false;
            bool hasAccessWifiStatePermission = false;
            bool hasReadPhonePermission = false;

            XmlElement manifestRoot = manifest.DocumentElement;

            // Check if permissions are already there.
            foreach (XmlNode node in manifestRoot.ChildNodes)
            {
                if (node.Name == "uses-permission")
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Value.Contains("com.huawei.appmarket.service.commondata.permission.GET_COMMON_DATA"))
                        {
                            hasCommonDataPermission = true;
                        }
                        if (attribute.Value.Contains("android.permission.REQUEST_INSTALL_PACKAGES"))
                        {
                            hasInstallPackagesPermission = true;
                        }
                        if (attribute.Value.Contains("android.permission.INTERNET"))
                        {
                            hasInternetPermission = true;
                        }
                        if (attribute.Value.Contains("android.permission.WRITE_EXTERNAL_STORAGE"))
                        {
                            hasExternalStoragePermission = true;
                        }
                        if (attribute.Value.Contains("android.permission.ACCESS_WIFI_STATE"))
                        {
                            hasAccessWifiStatePermission = true;
                        }
                        if (attribute.Value.Contains("android.permission.ACCESS_NETWORK_STATE"))
                        {
                            hasAccessNetworkStatePermission = true;
                        }
                        else if (attribute.Value.Contains("android.permission.READ_PHONE_STATE"))
                        {
                            hasReadPhonePermission = true;
                        }
                    }
                }
            }

            // Adding CommonData Permission
            if (!hasCommonDataPermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "com.huawei.appmarket.service.commondata.permission.GET_COMMON_DATA");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: COMMON_DATA permission added to manifest.");
            }
            else
            {
                UnityEngine.Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains COMMON DATA permission.");
            }

            // Adding INSTALL PACKAGES Permission
            if (!hasInstallPackagesPermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.REQUEST_INSTALL_PACKAGES");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: INSTALL_PACKAGES permission added to manifest.");
            }
            else
            {
                UnityEngine.Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains Install Packages permission.");
            }

            // Adding Internet Permission
            if (!hasInternetPermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.INTERNET");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: android.permission.INTERNET permission added to manifest.");
            }
            else
            {
                Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains android.permission.INTERNET permission.");
            }

            // Adding Storage Permission
            if (!hasExternalStoragePermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.WRITE_EXTERNAL_STORAGE");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: WRITE_EXTERNAL_STORAGE permission added to manifest.");
            }
            else
            {
                Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains WRITE_EXTERNAL_STORAGE permission.");
            }

            // Adding Network Permission
            if (!hasAccessNetworkStatePermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.ACCESS_NETWORK_STATE");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: ACCESS_NETWORK_STATE permission added to manifest.");
            }
            else
            {
                Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains ACCESS_NETWORK_STATE permission.");
            }

            // Adding WiFi Permission
            if (!hasAccessWifiStatePermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.ACCESS_WIFI_STATE");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]: ACCESS_WIFI_STATE permission added to manifest.");
            }
            else
            {
                Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains ACCESS_WIFI_STATE permission.");
            }

            // Adding Phone State Permission
            if (!hasReadPhonePermission)
            {
                XmlElement element = manifest.CreateElement("uses-permission");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.permission.READ_PHONE_STATE");
                manifestRoot.AppendChild(element);
                Debug.Log("[HMS]:  READ_PHONE_STATE permission added to manifest.");
            }
            else
            {
                Debug.Log("[HMS]: Your app's AndroidManifest.xml file already contains  READ_PHONE_STATE permission.");
            }

        }

        void AddProvider(XmlDocument manifest)
        {

            // NEED TO ADD 
            // <provider android:name="com.huawei.hms.update.provider.UpdateProvider" 
            // android:authorities="xxx.xxx.xxx.hms.update.provider" 
            // android:exported="false" 
            // android:grantUriPermissions="true" > 
            // </provider>

            bool hasUpdateProvider = false;
            bool hasUpdateSDKFileProvider = false;
            XmlNode updateProviderNode = null;
            XmlNode updateSDKFileNode = null;


            XmlElement manifestRoot = manifest.DocumentElement;
            XmlNode applicationNode = manifestRoot.SelectSingleNode("application");

            foreach (XmlNode node in applicationNode.ChildNodes)
            {
                if (node.Name == "provider")
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Value.Contains("com.huawei.hms.update.provider.UpdateProvider"))
                        {
                            hasUpdateProvider = true;
                            updateProviderNode = node;
                        }
                        if (attribute.Value.Contains("com.huawei.hms.update.provider.UpdateSdkFileProvider"))
                        {
                            hasUpdateSDKFileProvider = true;
                            updateSDKFileNode = node;
                        }
                    }

                }

            }

            if (!hasUpdateProvider)
            {
                // Create new provider UpdateProvider
                XmlElement element = manifest.CreateElement("provider");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "com.huawei.hms.update.provider.UpdateProvider");
                element.SetAttribute("authorities", "http://schemas.android.com/apk/res/android", PACKAGE + ".hms.update.provider");
                element.SetAttribute("exported", "http://schemas.android.com/apk/res/android", "false");
                element.SetAttribute("grantUriPermissions", "http://schemas.android.com/apk/res/android", "true");
                applicationNode.AppendChild(element);
                Debug.Log("[HMS]: Update Provider added to manifest.");

            }
            else
            {
                // Edit provider with package name
                updateProviderNode.Attributes["android:authorities"].Value = PACKAGE + ".hms.update.provider";
                Debug.Log("[HMS]: Update Provider edited in manifest.");


            }

            if (!hasUpdateSDKFileProvider)
            {
                //Create new provider UpdateSDKFileProvider
                XmlElement elementSDK = manifest.CreateElement("provider");
                elementSDK.SetAttribute("name", "http://schemas.android.com/apk/res/android", "com.huawei.updatesdk.fileprovider.UpdateSdkFileProvider");
                elementSDK.SetAttribute("authorities", "http://schemas.android.com/apk/res/android", PACKAGE + ".updateSdk.fileProvider");
                elementSDK.SetAttribute("exported", "http://schemas.android.com/apk/res/android", "false");
                elementSDK.SetAttribute("grantUriPermissions", "http://schemas.android.com/apk/res/android", "true");
                applicationNode.AppendChild(elementSDK);
                Debug.Log("[HMS]: SDK Update Provider added to manifest.");
            }
            else
            {
                // Edit provider with package name
                updateSDKFileNode.Attributes["authorities", "http://schemas.android.com/apk/res/android"].Value = PACKAGE + ".hms.update.provider";
                Debug.Log("[HMS]: SDK Update Provider edited in manifest.");

            }
        }

        void AddMetaData(XmlDocument manifest)
        {

            // NEED TO ADD 
            // <meta-data
            //     android:name="com.huawei.hms.client.appid"
            //     android:value="appid=101282883">
            // </meta-data>

            // <meta-data
            //     android:name="com.huawei.hms.client.cpid"
            //     android:value="cpid=890034000004105619">
            // </meta-data>


            bool hasAppID = false;
            bool hasCPID = false;

            XmlNode appIDNode = null;
            XmlNode cPIDNode = null;


            XmlElement manifestRoot = manifest.DocumentElement;

            XmlNode applicationNode = manifestRoot.SelectSingleNode("application");

            foreach (XmlNode node in applicationNode.ChildNodes)
            {
                if (node.Name == "meta-data")
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Value.Contains("com.huawei.hms.client.appid"))
                        {
                            hasAppID = true;
                            appIDNode = node;
                        }
                        if (attribute.Value.Contains("com.huawei.hms.client.cpid"))
                        {
                            hasCPID = true;
                            cPIDNode = node;
                        }

                    }

                }

            }



            if (!hasAppID)
            {
                XmlElement element = manifest.CreateElement("meta-data");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "com.huawei.hms.client.appid");
                element.SetAttribute("value", "http://schemas.android.com/apk/res/android", "appid=" + APPID);
                applicationNode.AppendChild(element);
                Debug.Log("[HMS]: App ID added to manifest.");
            }
            else
            {
                // Edit meta-data with package name
                appIDNode.Attributes["value", "http://schemas.android.com/apk/res/android"].Value = "appid=" + APPID;
                Debug.Log("[HMS]: App ID updated in manifest.");
            }

            if (!hasCPID)
            {
                XmlElement element = manifest.CreateElement("meta-data");
                element = manifest.CreateElement("meta-data");
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", "com.huawei.hms.client.cpid");
                element.SetAttribute("value", "http://schemas.android.com/apk/res/android", "cpid=" + CPID);
                applicationNode.AppendChild(element);
                UnityEngine.Debug.Log("[HMS]: App CPID added to manifest.");
            }
            else
            {
                // Edit meta-data with package name
                cPIDNode.Attributes["value", "http://schemas.android.com/apk/res/android"].Value = "cpid=" + CPID;
                Debug.Log("[HMS]: CPID updated in manifest.");

            }


        }
        //#endif
    }
}