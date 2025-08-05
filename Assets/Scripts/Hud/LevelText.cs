using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    private Text levelText;

    void Start()
    {
        levelText = GetComponent<Text>();
    }

    public void SetLevel(int level)
    {
        levelText.text = "Level " + level.ToString();
    }
}
