using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int inGameMonth = 1;
    public int inGameYear = 1;

    public float realTimeToYear = 10 * 60; // 10 minutes in seconds
    public float timeScale = 1.0f;

    public float elapsedTime = 0;

    public event Action MonthElapsed;
    public event Action YearElapsed;

    public TimeState timeState;

    void Update()
    {
        AdvanceTime(Time.deltaTime);
    }

    public void PauseTime()
    {
        timeScale = 0;
        timeState = TimeState.Pause;
    }

    public void ResumeTime()
    {
        timeScale = 1;
        timeState = TimeState.Resume;
    }

    public void SpeedUp2x()
    {
        timeScale = 2;
        timeState = TimeState.SpeedUp2x;
    }

    public void SpeedUp3x()
    {
        timeScale = 3;
        timeState = TimeState.SpeedUp3x;
    }

    public void AdvanceTime(float delta)
    {
        float realTimePerMonth = realTimeToYear / 12f;
        elapsedTime += delta * timeScale;

        if (elapsedTime >= realTimePerMonth)
        {
            int elapsedMonths = Mathf.FloorToInt(elapsedTime / realTimePerMonth);
            elapsedTime -= elapsedMonths * realTimePerMonth;

            inGameMonth += elapsedMonths;
            if (inGameMonth > 12)
            {
                inGameYear += inGameMonth / 12;
                inGameMonth %= 12;

                YearElapsed?.Invoke();
            }

            MonthElapsed?.Invoke();
        }
    }

}

public enum TimeState {
    Pause,
    Resume,
    SpeedUp2x,
    SpeedUp3x
}