using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public float maxTime = 10f;
    private float currentTime;

    public Image timerImage; 
    public GameManager gameManager; 

    private bool timerRunning = false;

    public void StartTimer()
    {
        currentTime = maxTime;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;
        float fill = Mathf.Clamp01(currentTime / maxTime);
        timerImage.fillAmount = fill;

        if (currentTime <= 0f)
        {
            timerRunning = false;
            gameManager.TriggerGameOverByTimer(); 
        }

        if (currentTime <= 10f)
        {
    
            timerImage.color = Color.red;
        }
        else
        {

            timerImage.color = Color.white; 
        }

    }
}
