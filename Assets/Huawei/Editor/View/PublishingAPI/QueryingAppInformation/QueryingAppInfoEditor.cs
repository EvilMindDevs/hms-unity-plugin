using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace HmsPlugin.PublishingAPI
{
    public class QueryingAppInfoEditor : VerticalSequenceDrawer
    {
        private Label.Label ReleaseState;
        private Label.Label AppName;
        private CategoryLevel categoryLevel;
        private Label.Label parentTypeString;
        private Label.Label childTypeString;
        private Label.Label grandChildTypeString;
        private Toggle.Toggle uploadPackage;

        public QueryingAppInfoEditor()
        {
            InitializeCategoryLevels();

            ReleaseState = new Label.Label();
            AppName = new Label.Label();
            parentTypeString = new Label.Label("Error - Not Assigned");
            childTypeString = new Label.Label("Error - Not Assigned");
            grandChildTypeString = new Label.Label("Error - Not Assigned");
            uploadPackage = new Toggle.Toggle("Upload After Build", HMSConnectAPISettings.Instance.Settings.GetBool(HMSConnectAPISettings.UploadAfterBuild, false), onUploadAfterBuildChecked);

            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("App Name:"), new Spacer(), AppName, new Space(10)));
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Release State:"), new Spacer(), ReleaseState, new Space(10)));
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Parent Type:"), new Spacer(), parentTypeString, new Space(10)));
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Child Type:"), new Spacer(), childTypeString, new Space(10)));
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Grand Child Type:"), new Spacer(), grandChildTypeString, new Space(10)));
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(uploadPackage, new Spacer()));
            AddDrawer(new Space(5));
            AddDrawer(new HelpBoxEnablingApp());
            AddDrawer(new Space(5));

            RequestAppInfo();
        }

        private void onUploadAfterBuildChecked(bool state)
        {
            HMSConnectAPISettings.Instance.Settings.SetBool(HMSConnectAPISettings.UploadAfterBuild, state);
        }

        [PostProcessBuildAttribute(1)]
        public async static void AfterBuildNotification(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.Android && HMSConnectAPISettings.Instance.Settings.GetBool(HMSConnectAPISettings.UploadAfterBuild))
            {
                if (!AskBeforeUploadProcess(pathToBuiltProject))
                {
                    return;
                }
                await GetUploadUrl(pathToBuiltProject);
            }
        }

        private static bool AskBeforeUploadProcess(string filePath)
        {
            float bytesToMegabytes= 1024f * 1024f;
            float megabytesToGigabytes = 1024f;
            float fileSize = (new FileInfo(filePath).Length) / bytesToMegabytes;
            bool isPackageAAB = UnityEditor.EditorUserBuildSettings.buildAppBundle;

            if (!isPackageAAB && fileSize >= 4 * megabytesToGigabytes ||
                isPackageAAB && fileSize >= 150 )
            {
                if (isPackageAAB)
                {
                    EditorUtility.DisplayDialog("Cannot upload AAB", "Your AAB file exceeds file size limit.\nLimit: 150MB for AAB and 4GB for APK", "Okay");
                    return false;

                }
                else
                {
                    EditorUtility.DisplayDialog("Cannot upload APK", "Your APK file exceeds file size limit.\nLimit: 150MB for AAB and 4GB for APK", "Okay");
                    return false;
                }
            }

            return EditorUtility.DisplayDialog("Upload After Build?","Are you sure uploading package to AGC after build?\nFile Size: " +
               fileSize + " MB", "Ok", "Cancel");
        }

        private static void UploadAnAppPackage(string filePath, string uploadUrl, string authCode)
        {
            int fileCount = 1;
            string fileName = Path.GetFileName(filePath);
            int parseType = 0;
            byte[] fileByte = UnityEngine.Windows.File.ReadAllBytes(Path.Combine("", filePath));
            string contentTypeHeader = "multipart/form-data";
            MultipartFormFileSection file = new MultipartFormFileSection("file", fileByte, fileName, contentTypeHeader);
            HMSWebRequestHelper.Instance.PostFormRequest(uploadUrl, file, authCode, fileCount.ToString(), parseType.ToString(), UploadAnAppPackageRes, "Uploading The Package", "Uploading Package to URL...");
        }

        private static void UpdatingAppFileInfoRes(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<UpdateFileInfoRes>(response.downloadHandler.text);

            if (responseJson.ret.code == 0)
            {
                string versionLog = "";
                if (responseJson.pkgVersion != null)
                {
                    foreach (string version in responseJson.pkgVersion)
                    {
                        versionLog += version + ", ";
                    }
                }
                EditorUtility.ClearProgressBar();
                Debug.Log("[HMS ConnectAPI] UpdatingAppFile Successful, retCode: " + responseJson.ret.code + ", retMessage " + responseJson.ret.msg + ", Versions: " + versionLog);
            }
            else
            {
                Debug.LogError("[HMS ConnectAPI] UpdatingAppFile Failed, retCode: " + responseJson.ret.code + ", retMessage " + responseJson.ret.msg);
                EditorUtility.ClearProgressBar();
            }
        }


        private static async void UploadAnAppPackageRes(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<UploadAppPackage>(response.downloadHandler.text);

            if (int.Parse(responseJson.result.resultCode) == 0)
            {
                int size = responseJson.result.UploadFileRsp.fileInfoList[0].size;
                string disposableUrl = responseJson.result.UploadFileRsp.fileInfoList[0].disposableURL;
                string fileDestUrl = responseJson.result.UploadFileRsp.fileInfoList[0].fileDestUlr;
                
                UpdateFileInfo fileInfo = new UpdateFileInfo();
                fileInfo.files = new Files();
                fileInfo.files.fileName = PlayerSettings.productName + ((UnityEditor.EditorUserBuildSettings.buildAppBundle) ? ".aab" : ".apk");
                fileInfo.files.fileDestUrl = fileDestUrl;
                fileInfo.fileType = 5;
                string jsonValue = JsonUtility.ToJson(fileInfo);
                string accessToken = await HMSWebUtils.GetAccessTokenAsync();

                HMSWebRequestHelper.Instance.PutRequest("https://connect-api.cloud.huawei.com/api/publish/v2/app-file-info?appId=" + HMSEditorUtils.GetAGConnectConfig().client.app_id,
                    jsonValue, new Dictionary<string, string>()
                    {
                        {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID)},
                        {"Authorization","Bearer " + accessToken}
                    }, UpdatingAppFileInfoRes);
                EditorUtility.DisplayProgressBar("Uploading The Package", "Uploading Package to AGC...", 0.3f);
            }
            else
            {
                Debug.LogError($"[HMS ConnectAPI] GetUploadURL failed. Error Code: {responseJson.result.resultCode}.");
            }
        }

        private static async Task GetUploadUrl(string filePath)
        {
            string accessToken = await HMSWebUtils.GetAccessTokenAsync();
            string suffix = (UnityEditor.EditorUserBuildSettings.buildAppBundle) ? "aab" : "apk";
            HMSWebRequestHelper.Instance.GetRequest("https://connect-api.cloud.huawei.com/api/publish/v2/upload-url?appId=" + HMSEditorUtils.GetAGConnectConfig().client.app_id + "&suffix=" + suffix,
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization", "Bearer " + accessToken }
                }, (res) => {
                    EditorUtility.DisplayProgressBar("Uploading The Package", "Getting Upload URL...", 0.3f);
                    onGetUploadUrl(res, filePath);
                });
        }

        private async void RequestAppInfo()
        {
            string accessToken = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.Instance.GetRequest("https://connect-api.cloud.huawei.com/api/publish/v2/app-info?appId=" + HMSEditorUtils.GetAGConnectConfig().client.app_id,
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization", "Bearer " + accessToken }
                }, OnQueryingInfoResponse);

        }

        private static void onGetUploadUrl(UnityWebRequest response, string filePath)
        {
            var responseJson = JsonUtility.FromJson<UploadUrl>(response.downloadHandler.text);

            if (responseJson.ret.code == 0)
            {
                Debug.Log($"[HMS ConnectAPI] GetUploadURL Succeed. Trying to upload the package now...");
                UploadAnAppPackage(filePath, responseJson.uploadUrl, responseJson.authCode);
                EditorUtility.ClearProgressBar();
            }
            else
            {
                Debug.LogError($"[HMS ConnectAPI] GetUploadURL failed. Error Code: {responseJson.ret.code}, Error Message: {responseJson.ret.msg}.");
                EditorUtility.ClearProgressBar();
            }
        }

        private void OnQueryingInfoResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<QueryingAppInfoResJson>(response.downloadHandler.text);

            if (responseJson.ret.code == 0)
            {

                if (responseJson.languages.Count > 0)
                {
                    AppName.SetText(responseJson.languages[0].appName);
                }

                Debug.Log("[HMS ConnectAPI] QueryingAppInfo parsed successfully.");
                ReleaseState.SetText(((ReleaseStates)responseJson.appInfo.releaseState).ToString().Replace("_", " "));

                foreach (var category in categoryLevel.Level1)
                {
                    if (category.Key == responseJson.appInfo.parentType)
                    {
                        parentTypeString.SetText(category.Value);
                        break;
                    }
                }

                foreach (var category in categoryLevel.Level2)
                {
                    if (category.Key == responseJson.appInfo.childType)
                    {
                        childTypeString.SetText(category.Value);
                        break;
                    }
                }

                foreach (var category in categoryLevel.Level3)
                {
                    if (category.Key == responseJson.appInfo.grandChildType)
                    {
                        grandChildTypeString.SetText(category.Value);
                        break;
                    }
                }

            }
            else
            {
                Debug.LogError($"[HMS ConnectAPI] QueryingAppInfo failed. Error Code: {responseJson.ret.code}, Error Message: {responseJson.ret.msg}.");
            }
        }
        #region JsonStuff

        [Serializable]
        private class UpdateFileInfo
        {
            public Files files;
            public int fileType;
        }

        [Serializable]
        private class UpdateFileInfoRes
        {
            public Ret ret;
            public string[] pkgVersion;
        }

        [Serializable]
        private class Files
        {
            public string fileName;
            public string fileDestUrl;
        }

        [Serializable]
        private class FileInfoList
        {
            public string disposableURL;
            public string fileDestUlr;
            public int size;
        }

        [Serializable]
        private class UploadFileRsp
        {
            public List<FileInfoList> fileInfoList;
            public int ifSuccess;
        }

        [Serializable]
        private class Result
        {
            public UploadFileRsp UploadFileRsp;
            public string resultCode;
        }

        [Serializable]
        private class UploadAppPackage
        {
            public Result result;
        }


        [Serializable]
        private class Ret
        {
            public int code;
            public string msg;
        }

        [Serializable]
        private class QueryingAppInfoResJson
        {
            public Ret ret;
            public AppInfo appInfo;
            public List<Languages> languages;
        }

        [Serializable]
        private class UploadUrl
        {
            public Ret ret;
            public string uploadUrl;
            public string chunkUploadUrl;
            public string authCode;
        }

        [Serializable]
        private class AppInfo
        {
            public int releaseState;
            public int parentType;
            public int childType;
            public int grandChildType;
            public string contentRate;
        }

        [Serializable]
        private class Languages
        {
            public string appName;
        }

        private enum ReleaseStates
        {
            released,
            release_rejected,
            removed,
            releasing,
            reviewing,
            updating,
            removal_requested,
            draft,
            update_rejected,
            removal_rejected,
            removed_by_developer,
            release_canceled
        }

        private class CategoryLevel
        {
            public List<KeyValuePair<int, string>> Level1;
            public List<KeyValuePair<int, string>> Level2;
            public List<KeyValuePair<int, string>> Level3;
        }

        private void InitializeCategoryLevels()
        {
            categoryLevel = new CategoryLevel();

            categoryLevel.Level1 = new List<KeyValuePair<int, string>>();
            categoryLevel.Level1.Add(new KeyValuePair<int, string>(-1, "Not Assigned"));
            categoryLevel.Level1.Add(new KeyValuePair<int, string>(0, "Not Assigned"));
            categoryLevel.Level1.Add(new KeyValuePair<int, string>(2, "Game"));
            categoryLevel.Level1.Add(new KeyValuePair<int, string>(13, "App"));

            categoryLevel.Level2 = new List<KeyValuePair<int, string>>();
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(-1, "Not Assigned"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(0, "Not Assigned"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(15, "Puzzle & casual"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(16, "Strategy"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(18, "Action"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(20, "Role-playing"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(21, "Card & board"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(22, "Sports games"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(23, "Entertainment"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(24, "Tools"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(25, "Finance"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(26, "Social"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(27, "Lifestyle"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(28, "Navigation & transport"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(29, "Personalized themes"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(30, "Education"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(31, "Sports & health"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(33, "Photo & video"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(345, "News & reading"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(358, "Shopping"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(359, "Food & drink"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(360, "Cars"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(361, "Travel"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(362, "Business"));
            categoryLevel.Level2.Add(new KeyValuePair<int, string>(363, "Kids"));

            categoryLevel.Level3 = new List<KeyValuePair<int, string>>();
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(-1, "Not Assigned"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(0, "Not Assigned"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10043, "Casual"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10044, "Puzzle"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10046, "Tile-matching"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10047, "Music"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10048, "Mystery"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10113, "IO"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10062, "Tower defense"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10122, "Historical strategy"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10123, "Modern strategy"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10063, "Business management"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10064, "Life simulation"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10065, "MOBA"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10053, "Fighting"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10054, "Running"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10055, "Shooting"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10056, "Flying"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10118, "Xianxia"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10120, "Legend"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10121, "Fantasy"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10119, "Wuxia"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10057, "Adventure"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10058, "Action"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10059, "Turn-based strategy"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10060, "Online multiplayer"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10061, "Trading cards"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10114, "Survival games"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10115, "Incremental games"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10108, "Fight the landlord"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10109, "Mahjong"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10110, "Board & chess"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10111, "Cards"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10500, "Fishing"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10049, "Racing"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10050, "Basketball"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10051, "Soccer"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10052, "Sports"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10066, "Music"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10067, "Videos"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10068, "TV"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10069, "Radio"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10070, "Karaoke"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10071, "Streaming"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10019, "Browsers"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10020, "Keyboards"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10021, "Wi-Fi"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10022, "Tools"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10023, "Alarms"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10024, "Security"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10034, "Banking"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10035, "Equity funds"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10036, "Personal finance"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10037, "Accounting"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10038, "Lottery"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10112, "Loans"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10039, "Chatting"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10040, "Community"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10041, "Communication"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10042, "Love & marriage"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10079, "Housekeeping"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10080, "Local life"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10081, "Rent and buy houses"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10082, "House refurbishment"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10083, "Movie tickets"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10084, "Weather & calendars"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10014, "Maps"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10015, "Navigation"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10016, "Transport & tickets"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10017, "Cars"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10018, "Public transport"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10001, "Ringtones"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10002, "Screen locks"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10003, "Wallpapers"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10004, "Launchers"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10095, "Learning"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10096, "English"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10097, "Translation"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10098, "Exams"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10005, "Health"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10006, "Well-being"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10007, "Sports"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10013, "Healthcare"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10025, "Photography"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10026, "Short videos"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10027, "Beautification"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10028, "AV editors"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10029, "Photo galleries"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10085, "Books"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10086, "News"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10087, "Anime & comics"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10088, "Audiobooks"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10089, "Magazines"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10090, "Humor"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10091, "Sports"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10501, "Directories"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10030, "Malls"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10116, "Shopping guides"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10031, "Group purchasing"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10032, "International shopping"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10117, "Deliveries and packages"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10033, "Discounts"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10075, "Recipes"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10076, "Take-out"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10077, "Catering"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10078, "Fresh food"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10099, "Car care"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10100, "Traffic violations"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10101, "Car info"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10102, "Driving test"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10072, "Travel"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10073, "Accommodation"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10074, "Travel assistance"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10103, "Business software"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10104, "Efficiency"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10105, "Notes"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10106, "Email"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10107, "Jobs"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10092, "Early learning"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10093, "Nursery rhymes"));
            categoryLevel.Level3.Add(new KeyValuePair<int, string>(10094, "Mom & baby"));
        }
#endregion
    }
}
