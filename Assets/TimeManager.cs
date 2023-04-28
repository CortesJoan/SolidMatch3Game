using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float hours;
    public float minutes;

    private float timeSpeed = 1f;

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        minutes += Time.deltaTime * timeSpeed;

        if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }

        if (hours >= 24)
        {
            hours = 0;
        }
    }
}