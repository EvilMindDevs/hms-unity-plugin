using HuaweiMobileServices.InAppComment;

using UnityEngine;
using UnityEngine.UI;

public class InAppCommentDemo : MonoBehaviour
{

    private Button Btn_CreateAppLinkingText;

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
        Debug.Log("ShowInAppComment");

        InAppComment.ShowInAppComment();
    }
}
