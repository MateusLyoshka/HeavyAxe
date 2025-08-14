using UnityEngine;
using UnityEngine.UI;

public class StatusBarText : MonoBehaviour
{
    private Text statusText;
    void Awake()
    {
        statusText = GetComponent<Text>();

    }

    public void SetStatusText(string text)
    {
        statusText.text = text;
    }
}
