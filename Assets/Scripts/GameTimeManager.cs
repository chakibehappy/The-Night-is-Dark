using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameTimeManager : MonoBehaviour
{
    public TextMeshProUGUI txtTime, txtDay;
    public List<int> skyColorTime;
    public Color[] skyColor;
    int currentSkyColor = 0;

    public string[] dayname = new string[] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
    public int day = 1;
    public int totalHour = 20;
    public int hour = 8;
    public int minute = 0;
    public string timeTag = "PM";
    bool timeIsTicking = true;

    public int blinkingCount = 10;
    int currentblinkingCount;
    public float tenMinuteDelay = 5f;
    public float realSecHourTime;
    public Sequence skySequence;

    IEnumerator timeCoroutine;
    bool isPaused = false;

    private void Start()
    {
        Debug.LogWarning("Read this note :");
        // Save each hour color time on new list
        // Function to Adjust Everything by totalHour (display time, sky, etc)

        currentblinkingCount = blinkingCount;
        realSecHourTime = tenMinuteDelay * 6;
        currentSkyColor = GetSkyColor();
        Camera.main.backgroundColor = skyColor[currentSkyColor];
        ChangeSkyColor();
        TickingTime();
    }

    public void ChangeSkyColor()
    {
        if (totalHour == skyColorTime[currentSkyColor])
        {
            currentSkyColor++;
            if (currentSkyColor >= skyColorTime.Count)
                currentSkyColor = 0;

            int nextColorIndex = currentSkyColor + 1;
            if (nextColorIndex >= skyColorTime.Count)
                nextColorIndex = 0;

            float nextHours = skyColorTime[nextColorIndex];
            if(skyColorTime[currentSkyColor] > nextHours)
            {
                nextHours += 24;
            }

            skySequence = DOTween.Sequence();
            skySequence.Append(
                Camera.main.DOColor(skyColor[currentSkyColor], realSecHourTime * (nextHours - skyColorTime[currentSkyColor]))
            );
        }
    }

    public void TickingTime()
    {
        StopTime();
        timeIsTicking = true;
        timeCoroutine = TickingTimeIE();
        StartCoroutine(timeCoroutine);
        if (skySequence != null)
        {
            skySequence.Play();
        }
    }

    public void StopTime()
    {
        if (timeCoroutine != null)
        {
            timeIsTicking = false;
            StopCoroutine(timeCoroutine);
        }
        if (skySequence != null)
        {
            skySequence.Pause();
        }
    }
    
    public void ResumeTime(bool isPlay = true)
    {
        isPaused = !isPlay;
    }

    IEnumerator PauseIE()
    {
        while (isPaused)
        {
            yield return null;
        }
    }

    public IEnumerator TickingTimeIE()
    {
        while (timeIsTicking)
        {
            while (currentblinkingCount > 0)
            {
                yield return StartCoroutine(PauseIE());
                DisplayTime();
                yield return new WaitForSeconds(tenMinuteDelay / (blinkingCount * 2));

                yield return StartCoroutine(PauseIE());
                DisplayTime(" ");
                yield return new WaitForSeconds(tenMinuteDelay / (blinkingCount * 2));
                
                currentblinkingCount--;
            }

            currentblinkingCount = blinkingCount;
            minute += 10;

            if(minute >= 60)
            {
                minute = 0;
                totalHour++;
                hour++;
                if(hour > 12)
                {
                    hour = 1;
                }

                if(totalHour >= 24)
                {
                    totalHour = hour = 0;
                    day++;
                }
                yield return StartCoroutine(PauseIE());
                DisplayTime();
                ChangeSkyColor();
            }
        }
    }

    void DisplayTime(string contenation = ":")
    {
        timeTag = totalHour < 12 ? "AM" : "PM";
        txtDay.text = dayname[day];
        string h = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        string m = minute < 10 ? "0" + minute.ToString() : minute.ToString();
        txtTime.text = h + contenation + m + "." + timeTag; 
    }

    int GetSkyColor()
    {
        for(int i = 0; i < skyColorTime.Count; i++)
        {
            if (totalHour <= skyColorTime[i])
                return i;
        }
        return 0;
    }
}
