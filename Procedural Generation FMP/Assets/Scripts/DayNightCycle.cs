using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D sun;
    public List<Light2D> spotLights;
    public TimeOfDay[] times;
    public Weather[] weathers;
    public float fadeTime;
    int currentTime;
    int currentWeather;

    float timer;

    public float timeScale = 1;

    Vector3 lightChangeVelocity;
    float intensityChangeVelocity;

    private void Update()
    {
        timer += (Time.deltaTime / 60f) * timeScale;

        foreach(var light in spotLights)
        {
            light.intensity = 1 - sun.intensity;
        }

        if(timer >= times[currentTime].length)
        {
            currentTime++;

            int chance = Random.Range(0, TotalWeatherChance());

            int tally = 0;

            if(weathers[currentWeather].particle != null)
                weathers[currentWeather].particle.Stop();

            for (int i = 0; i < weathers.Length; i++)
            {
                tally += weathers[i].chance;
                Debug.Log($"Chance: {chance} Tally: {tally}");
                if (chance <= tally)
                {
                    currentWeather = i;
                    break;
                }
            }

            if (currentTime == times.Length)
                currentTime = 0;

            timer = 0;

            StopAllCoroutines();
            intensityChangeVelocity = 0;
            lightChangeVelocity = Vector3.zero;

            Debug.Log(times[currentTime].name + ": " + weathers[currentWeather].name);

            if (!weathers[currentWeather].overrideTimeSettings)
            {
                StartCoroutine(FadeToNewTime(times[currentTime].lightColor, times[currentTime].intensity));

                if (weathers[currentWeather].particle != null)
                    weathers[currentWeather].particle.Play();
            }
            else
            {
                float lightLevel = Mathf.Min(weathers[currentWeather].intensity, times[currentTime].intensity);

                StartCoroutine(FadeToNewTime(weathers[currentWeather].lightColor, lightLevel));

                if (weathers[currentWeather].particle != null)
                {
                    weathers[currentWeather].particle.Play();
                    
                }

                Debug.Log("Playing " + weathers[currentWeather].particle);
            }
        }
    }

    IEnumerator FadeToNewTime(Color newColour, float newIntensity)
    {
        while (sun.intensity != times[currentTime].intensity)
        {
            sun.color = (Vector4)Vector3.SmoothDamp((Vector4)sun.color, (Vector4)newColour, ref lightChangeVelocity, fadeTime);
            sun.intensity = Mathf.SmoothDamp(sun.intensity, newIntensity, ref intensityChangeVelocity, fadeTime);
            yield return null;
        }
    }

    int TotalWeatherChance()
    {
        int total = 0;

        foreach(var weather in weathers)
        {
            total += weather.chance;
        }

        return total;
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


[System.Serializable]
public struct Weather
{
    public string name;
    public int chance;
    public bool overrideTimeSettings;
    public Color lightColor;
    public float intensity;
    public ParticleSystem particle;
}
