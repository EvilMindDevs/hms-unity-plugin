using UnityEngine;
using UnityEngine.UI;
public class TextToSpeechDemoScene : MonoBehaviour
{
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject wordCountText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CalculateInputWordCount()
    {
        string inputText = inputField.GetComponent<InputField>().text;
        wordCountText.GetComponent<Text>().text = $"Entered: {inputText.ToCharArray().Length} / 500";
    }
}
