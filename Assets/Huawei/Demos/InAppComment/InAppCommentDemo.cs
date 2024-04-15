using HuaweiMobileServices.InAppComment;
using UnityEngine;
using UnityEngine.UI;

public class InAppCommentDemo : MonoBehaviour
{

    private Button Btn_CreateAppLinkingText;
    private const string TAG = "[HMS] InAppCommentDemo";
    private void Awake()
    {
        Btn_CreateAppLinkingText = GameObject.Find("InAppComment").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_CreateAppLinkingText.onClick.AddListener(ShowInAppComment);
    }

    private void OnDisable()
    {
        Btn_CreateAppLinkingText.onClick.RemoveListener(ShowInAppComment);
    }

    public void ShowInAppComment()
    {
        Debug.Log($"{TAG} ShowInAppComment");
        InAppComment.ShowInAppComment(onResult);
    }

    private void onResult(int result)
    {
        Debug.Log($"{TAG} onResult:{result}");
        CheckResultMeans(result);
    }

    private void CheckResultMeans(int result)
    {
        switch (result)
        {
            case 101:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The app has not been released on AppGallery.");
                break;
            case 102:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : Rating submitted.");
                break;
            case 103:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : Comment submitted.");
                break;
            case 104:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The HUAWEI ID sign-in status is invalid.");
                break;
            case 105:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The user does not meet the conditions for displaying the comment pop-up.");
                break;
            case 106:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The commenting function is disabled.");
                break;
            case 107:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The in-app commenting service is not supported. (Apps released in the Chinese mainland do not support this service.)");
                break;
            case 108:
                Debug.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The user canceled the comment.)");
                break;
            default:
                Debug.Log($"{TAG} CheckResultMeans Default ResultCode:{result}");
                break;
        }
    }

}
