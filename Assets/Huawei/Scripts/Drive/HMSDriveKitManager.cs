using HuaweiMobileServices.Base;
using HuaweiMobileServices.Drive;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSDriveKitManager : HMSManagerSingleton<HMSDriveKitManager>
    {
        private readonly string TAG = "[HMS] HMSDriveKitManager ";
        private Drive drive;
        private File recentlyCreatedFile;


        public HMSDriveKitManager()
        {
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }       

        private void OnAwake()
        {
            drive = BuildDrive();
        }
        public Drive BuildDrive()
        {
            Drive service = new Drive.Builder(CredentialManager.GetInstance().GetCredential()).Build();
            return service;
        }

        //https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/client-drive-files-create-0000001050125997#section38411940183416
        public File CreateFiles(String type, String filePath)
        {
            File file = null;
            if (filePath == null)
            {
                Debug.LogError(TAG + "createFile error, filePath is null");
                return file;
            }
            else if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError(TAG + "createFile error, filePath notExists. FilePath:" + filePath);
                return file;
            }

            try
            {
                // Create or upload a file.
                FileContent fileContent = new FileContent(type, new HuaweiMobileServices.Utils.java.io.File(filePath));
                File fileInfo = new File().SetFileName("testFile.txt");
                file = drive.files().create(fileInfo, fileContent).Execute();
                Debug.Log(TAG + "CreateFiles success mFile.GetId()" + file.GetId());
                recentlyCreatedFile = file;
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "createFiles error:" + e.Message);
            }
            return file;
        }

        public void CreateFolder(string folderName = "testDir") 
        {
            // Create a folder.
            File dir = new File().SetFileName(folderName)
                .SetMimeType("application/vnd.huawei-apps.folder");
            File Directory = drive.files().create(dir).Execute();
        }

        public List<Comment> ListComments(string fileId = "")
        {
            List<Comment> commentArrayList = new List<Comment>();

            if (fileId == "") 
            {
                if(recentlyCreatedFile != null) 
                {
                    fileId = recentlyCreatedFile.GetId();// Use recently created file's ID
                }
                else 
                {
                    return commentArrayList;
                }
            }

            try
            {
                Drive.Comments.List request = drive.comments().list(fileId);
                CommentList commentList = request.SetPageSize(100).SetFields("*").Execute();
                string nextCursor = commentList.GetNextCursor();

                do
                {
                    if (nextCursor != null)
                    {
                        request.SetCursor(nextCursor);
                    }

                    List<Comment> comments = (List<Comment>)commentList.GetComments();
                    if (comments == null)
                    {
                        break;
                    }
                    commentArrayList.AddRange(comments);
                    nextCursor = commentList.GetNextCursor();
                } while (!string.IsNullOrEmpty(nextCursor));
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "comments list error: " + e.ToString());
            }
            Debug.Log(TAG + "ListComments Success commentArrayList.Count" + commentArrayList.Count);
            return commentArrayList;
        }

        public Comment CreateComments(string fileId = "")
        {
            Comment comment = null;

            if (fileId == "")
            {
                if (recentlyCreatedFile != null)
                {
                    fileId = recentlyCreatedFile.GetId(); // Use recently created file's ID
                }
                else
                {
                    return comment;
                }
            }
            
            try
            {
                Comment content = new Comment();
                content.SetDescription("interface test");
                var date = System.DateTime.Now.Ticks;
                content.SetCreatedTime(new HuaweiMobileServices.Drive.DateTime(date));
                comment = drive.comments().create(fileId, content).SetFields("*").Execute();
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "createComments error: " + e.ToString());
            }

            return comment;
        }

        public FileList ListFile()
        {
            try
            {
                FileList list = drive.files().list().Execute();
                string nextCursor = list.GetNextCursor();
                Debug.Log(TAG + "ListFile nextCursor string.IsNullOrEmpty(nextCursor):" + string.IsNullOrEmpty(nextCursor));
                while (!string.IsNullOrEmpty(nextCursor)) 
                {
                    nextCursor = list.GetNextCursor();
                }
                return list;
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + " ListFile error:" + e.Message);
                return null;
            }
        }

        public File GetFile(string fileId = "")
        {
            File file = null;
            if (fileId == "")
            {
                if (recentlyCreatedFile != null)
                {
                    fileId = recentlyCreatedFile.GetId(); // Use recently created file's ID
                }
                else
                {
                    return file;
                }
            }

            try
            {
                string fileName = "testFileDowloaded.txt";
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
                HuaweiMobileServices.Utils.java.io.File f = new HuaweiMobileServices.Utils.java.io.File(filePath + fileName);

                // Obtain file metadata.
                Drive.Files.Get request = drive.files().get(fileId);
                request.SetFields("*");
                file = request.Execute();

                // Download the entity file.
                Drive.Files.Get get = drive.files().get(fileId);
                get.SetForm("content");
                MediaHttpDownloader downloader = get.GetMediaHttpDownloader();
                downloader.SetContentRange(0, file.GetSize() - 1).SetDirectDownloadEnabled(true);
                get.ExecuteContentAndDownloadTo(new HuaweiMobileServices.Utils.java.io.FileOutputStream(f));
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "GetFile error: " + e.Message);
            }

            return file;
        }

        public File CreateDirectory(string dirName = "")
        {
            File directory = null;
            try
            {
                IMap<string, string> appProperties = new HashMap<string, string>();
                appProperties.Put("appProperties", "property");
                System.DateTime time = System.DateTime.UtcNow;
                dirName = (string.IsNullOrEmpty(dirName)) ? time.ToString("yyyyMMdd_HHmmss_SSS") : dirName;
                File file = new File();
                file.SetFileName(dirName)
                    .SetAppSettings(appProperties)
                    .SetMimeType("application/vnd.huawei-apps.folder");
                directory = drive.files().create(file).Execute();
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "createDirectory error: " + e.Message);
            }
            return directory;
        }

        public About GetAbout()
        {
            About response = null;
            try
            {
                Drive.About about = drive.about();
                response = about.get().SetFields("*").Execute();
                string res = response.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "GetAbout error: " + e.ToString());
            }
            return response;
        }

    }
}
