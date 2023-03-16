using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int inGameMonth = 1;
    public int inGameYear = 1;

    public float realTimeToYear = 10 * 60; // 10 minutes in seconds
    public float timeScale = 1.0f;

    private float elapsedTime = 0;

    void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;

        if (elapsedTime >= realTimeToYear)
        {
            elapsedTime = 0;
            inGameYear++;
            inGameMonth = 1;
        }
        else
        {
            inGameMonth = Mathf.Clamp(Mathf.CeilToInt((elapsedTime / realTimeToYear) * 12), 1, 12);
        }
    }

    public void PauseTime()
    {
        timeScale = 0;
    }

    public void ResumeTime()
    {
        timeScale = 1;
    }

    public void SpeedUp2x()
    {
        timeScale = 2;
    }

    public void SpeedUp3x()
    {
        timeScale = 3;
    }
}
