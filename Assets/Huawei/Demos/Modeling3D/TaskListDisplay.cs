using System.Collections;
using System.Collections.Generic;
using HmsPlugin;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskListDisplay : MonoBehaviour, IPointerClickHandler
{
    #region Definations
    private const string TAG = "[HMS] TaskListDisplay ";
    private const string TASK_LIST_PREFS_KEY = "3dTaskList";
    [SerializeField] public Text Name;
    [SerializeField] public Text Status;
    [SerializeField] public Image Image;
    [SerializeField] public RawImage RawImage;
    [SerializeField] public Button PreviewButton;
    [SerializeField] public Button DownloadButton;
    [SerializeField] public Text HasDownloaded;
    [HideInInspector] public string TaskId;

    private readonly PlayerPrefsJsonDatabase<Modeling3dDTO> modeling3dTaskEntity = new PlayerPrefsJsonDatabase<Modeling3dDTO>(TASK_LIST_PREFS_KEY);
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(TAG + "Start");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DeleteTask()
    {
        modeling3dTaskEntity.Delete(TaskId);
        var modeling3dDemoManager = FindObjectOfType<Modeling3dDemoManager>();
        modeling3dDemoManager.TaskList();
        modeling3dDemoManager.DeleteTask(TaskId);
        Destroy(gameObject);
    }

    public void PreviewTask()
    {
        var modeling3dDemoManager = FindObjectOfType<Modeling3dDemoManager>();
        modeling3dDemoManager.PreviewFile(TaskId);
    }

    public void DownloadTask()
    {
        var modeling3dDemoManager = FindObjectOfType<Modeling3dDemoManager>();
        modeling3dDemoManager.DownloadFile(TaskId);
    }
    public void RefreshAllTaskAndOpenList()
    {
        var modeling3dDemoManager = FindObjectOfType<Modeling3dDemoManager>();
        RefreshAllTask();
        modeling3dDemoManager.TaskList();
    }
    public void RefreshAllTask()
    {
        var modeling3dDemoManager = FindObjectOfType<Modeling3dDemoManager>();
        var allTask = modeling3dTaskEntity.GetAll();
        if (allTask?.Count > 0)
            AndroidToast.MakeText("Updating All Task...").Show();
        RefreshTaskOperation(allTask, modeling3dDemoManager);
    }
    private void RefreshTaskOperation(List<Modeling3dDTO> allTask, Modeling3dDemoManager modeling3dDemoManager)
    {
        foreach (var task in allTask)
        {
            var result = UpdateTaskStatus(task, modeling3dDemoManager);
        }

    }
    private Modeling3dReconstructQueryResult UpdateTaskStatus(Modeling3dDTO task, Modeling3dDemoManager modeling3dDemoManager)
    {
        var queryResult = modeling3dDemoManager.QueryTask(task.TaskId);
        if (queryResult != null)
        {
            task.Status = $"Status: {queryResult.Status} Code: {queryResult.RetCode} \n Message: {queryResult.RetMessage}";
            if (!string.IsNullOrWhiteSpace(queryResult.ReconstructFailMessage))
                task.Status += $" \n FailMessage: {queryResult.ReconstructFailMessage}";
            modeling3dTaskEntity.Update(task);
        }
        return queryResult;
    }

    // When this button is clicked, copy the Task ID to the clipboard
    public void OnPointerClick(PointerEventData eventData)
    {
        GUIUtility.systemCopyBuffer = TaskId;
        AndroidToast.MakeText($"Copy to Clipboard TaskId: {TaskId}").Show();
    }
    #endregion
}
