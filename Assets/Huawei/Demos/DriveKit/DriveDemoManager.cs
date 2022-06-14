using System;
using System.Collections.Generic;
using HmsPlugin;
using HuaweiMobileServices.Drive;
using HuaweiMobileServices.Id;
using UnityEngine;
using UnityEngine.UI;

public class DriveDemoManager : MonoBehaviour
{

    private static int DIRECT_UPLOAD_MAX_SIZE = 20 * 1024 * 1024;
    private static int DIRECT_DOWNLOAD_MAX_SIZE = 20 * 1024 * 1024;

    public Text description;

    void Start()
    {
        description.text = string.Empty;
        GetScopeList();
    }

    public void GetAboutOnClick()
    {
        description.text = string.Empty;
        GetAbout();
    }

    public void CreateDirectoryOnClick()
    {
        description.text = string.Empty;
        createDirectory();
    }

    public void CreateFileOnClick()
    {
        description.text = string.Empty;
        createFile(@"testFile.txt", "test", null, "test");
    }

    public void ListCommentsOnClick()
    {
        description.text = string.Empty;
        listComments("test");
    }

    public void CreateCommentsOnClick()
    {
        description.text = string.Empty;
        createComments("test");
    }

    public HuaweiIdAuthParams GetScopeList()
    {
        List<Scope> scopeList = new List<Scope>();
        scopeList.Add(new Scope(Drive.DriveScopes.SCOPE_DRIVE_FILE));
        scopeList.Add(new Scope(Drive.DriveScopes.SCOPE_DRIVE_APPDATA));
        HuaweiIdAuthParams authParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DEFAULT_AUTH_REQUEST_PARAM)
            .SetAccessToken()
            .SetIdToken()
            .SetScopeList(scopeList)
            .CreateParams();

        description.text = "authParams is null -> " + (authParams == null).ToString() + "\n";
        return authParams;
    }

    public Drive BuildDrive()
    {
        Drive service = new Drive.Builder(CredentialManager.GetInstance().GetCredential()).Build();
        description.text = "service is null -> " + (service == null).ToString() + "\n";
        return service;
    }

    public void createFile(string filePath, string parentId, byte[] thumbnailImageBuffer, string thumbnailMimeType)
    {

        try
        {
            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                Debug.Log("createFile error, filePath is null");
                return;
            }

            System.IO.FileInfo info = new System.IO.FileInfo(filePath);

            FileContent content = new FileContent(null, info);

            File file = new File()
                .SetFileName(info.FullName);

            Drive drive = BuildDrive();
            Drive.Files.Create request = drive.files().create(file, content);
            bool isDirectUpload = false;

            if (info.Length < DIRECT_UPLOAD_MAX_SIZE)
            {
                isDirectUpload = true;
            }

            request.GetMediaHttpUploader().SetDirectUploadEnabled(isDirectUpload);
            request.Execute();
        } catch (Exception e)
        {
            Debug.Log("createFile exception: " + filePath + "\nMessage: " + e.Message);
            description.text = "createFile exception: " + filePath + "\nMessage: " + e.Message + "\n";
        }
    }

    public File createDirectory()
    {
        File directory = null;
        try
        {
            Drive drive = BuildDrive();
            Dictionary<string, string> appProperties = new Dictionary<string, string>();
            appProperties.Add("appProperties", "property");
            System.DateTime time = System.DateTime.UtcNow;
            string dirName = time.ToString("yyyyMMdd_HHmmss_SSS");

            File file = new File();
            file.SetFileName(dirName)
                .SetAppSettings(appProperties)
                .SetMimeType("application/vnd.huawei-apps.folder");

            directory = drive.files().create(file).Execute();
        }
        catch (Exception e)
        {
            Debug.Log("createDirectory error: " + e.Message);
            description.text = "createDirectory error: " + e.Message + "\n";
        }
        return directory;
    }

    public About GetAbout()
    {
        About response = null;
        try
        {
            Drive drive = BuildDrive();
            Drive.About about = drive.about();
            response = about.get().SetFields("*").Execute();
            string res = response.ToString();
        }
        catch (Exception e)
        {
            Debug.Log("[HMS]: Drive getInfo error: " + e.ToString());
            description.text = "[HMS]: Drive getInfo error: " + e.ToString() + "\n";
        }
        return response;
    }

    public List<Comment> listComments(string fileId)
    {
        Drive drive = BuildDrive();
        List<Comment> commentArrayList = new List<Comment>();
        string nextCursor = null;
        try
        {
            Drive.Comments.List request = drive.comments().list(fileId);
            do
            {
                if (nextCursor != null)
                {
                    request.SetCursor(nextCursor);
                }
                CommentList commentList = request.SetPageSize(100).SetFields("*").Execute();
                List<Comment> comments = (List<Comment>)commentList.GetComments();
                if (comments == null)
                {
                    break;
                }
                commentArrayList.AddRange(comments);
                nextCursor = commentList.GetNextCursor();
            } while (!string.IsNullOrEmpty(nextCursor));
        } catch (Exception e)
        {
            Debug.Log("comments list error: " + e.ToString());
            description.text = "comments list error: " + e.ToString() + "\n";

        }
        return commentArrayList;
    }

    public Comment createComments(string fileId)
    {
        Comment comment = null;
        Drive drive = BuildDrive();
        try
        {
            Comment content = new Comment();
            content.SetDescription("interface test");
            content.SetCreatedTime(new HuaweiMobileServices.Drive.DateTime(System.DateTime.Now.Millisecond.ToString()));
            comment = drive.comments().create(fileId, content).SetFields("*").Execute();
        } catch (Exception e)
        {
            Debug.Log("createComments error: " + e.ToString());
            description.text = "createComments error: " + e.ToString() + "\n";
        }

        return comment;
    }


}
