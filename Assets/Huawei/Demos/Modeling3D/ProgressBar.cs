using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private int minimum;
    [SerializeField] private int maximum;
    [SerializeField] public int current;
    [SerializeField] private Image mask;
    [SerializeField] public Text text;
    [HideInInspector] public string progressType;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill() {
        current = current > maximum ? maximum : current;
        current = current < minimum ? minimum : current;
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;

        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;
        text.text = string.Format("{0} {1}%", progressType,(fillAmount * 100));
        
    }
}
