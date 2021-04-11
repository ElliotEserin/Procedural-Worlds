using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D sun;
    public TimeOfDay[] times;
    public float fadeTime;
    int currentTime;

    float timer;

    public float timeScale = 1;

    Vector3 lightChangeVelocity;
    float intensityChangeVelocity;

    private void Update()
    {
        timer += (Time.deltaTime / 60f) * timeScale;

        if(timer >= times[currentTime].length)
        {
            currentTime++;

            if (currentTime == times.Length)
                currentTime = 0;

            timer = 0;

            StopCoroutine(FadeToNewTime());
            StartCoroutine(FadeToNewTime());
        }
    }

    IEnumerator FadeToNewTime()
    {
        while (sun.color != times[currentTime].lightColor)
        {
            sun.color = (Vector4)Vector3.SmoothDamp((Vector4)sun.color, (Vector4)times[currentTime].lightColor, ref lightChangeVelocity, fadeTime);
            sun.intensity = Mathf.SmoothDamp(sun.intensity, times[currentTime].intensity, ref intensityChangeVelocity, fadeTime);
            yield return null;
        }
    }
}

[System.Serializable]
public struct TimeOfDay
{
    public string name;
    public int length;
    public Color lightColor;
    public float intensity;
}
