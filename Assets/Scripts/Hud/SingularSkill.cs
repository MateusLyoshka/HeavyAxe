using UnityEngine;
using UnityEngine.UI;

public class SingularSkill : MonoBehaviour
{
    private Slider slider;
    private TimerText timerText;
    private SkillIconSetter iconSetter;

    private float coolDown;
    private float timer;
    private bool timerStart;

    void Awake()
    {
        slider = GetComponent<Slider>();
        timerText = GetComponentInChildren<TimerText>();
        iconSetter = GetComponentInChildren<SkillIconSetter>();
    }

    public void SetSkillIcon(Sprite icon)
    {
        iconSetter.SetIcon(icon);
    }

    public void StartTimer(float time)
    {
        timerText.SetText(timer);
        coolDown = time;
        timer = time;
        timerStart = true;
    }

    void Update()
    {
        if (timerStart)
        {
            timer -= Time.deltaTime;
            float sliderValue = Mathf.Clamp01(timer / coolDown);
            slider.value = sliderValue;
            timerText.SetText(timer);
            if (timer <= 0)
            {
                timerText.SetText((int)timer - 1);
                timer = 0;
                timerStart = false;
            }
        }
    }
}
