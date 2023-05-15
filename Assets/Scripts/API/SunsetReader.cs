using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SunsetReader : MonoBehaviour
{
    string url = "https://api.sunrise-sunset.org/json?lat=41.0151370&lng=-28.979530";

    string mockJsonData = @"{
    ""results"": {
        ""sunrise"": ""6:00:00 AM"",
        ""sunset"": ""8:00:00 PM"",
        ""solar_noon"": ""1:00:00 PM"",
        ""day_length"": ""14:00:00"",
        ""civil_twilight_begin"": ""5:30:00 AM"",
        ""civil_twilight_end"": ""8:30:00 PM"",
        ""nautical_twilight_begin"": ""5:00:00 AM"",
        ""nautical_twilight_end"": ""9:00:00 PM"",
        ""astronomical_twilight_begin"": ""4:30:00 AM"",
        ""astronomical_twilight_end"": ""9:30:00 PM""
    },
    ""status"": ""OK""
}";

    void Start()
    {
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {
        string currentString = "";
        DateTime sunsetUtc = DateTime.Now;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {

            yield return webRequest.SendWebRequest();
            string selectedJsonData = webRequest.result == UnityWebRequest.Result.ConnectionError ? mockJsonData : webRequest.downloadHandler.text;

            SunriseSunsetResponse response = JsonUtility.FromJson<SunriseSunsetResponse>(selectedJsonData);

            sunsetUtc = DateTime.Parse(response.results.sunset).ToLocalTime();

            DateTime now = DateTime.Now;

            if (now > sunsetUtc && now < DateTime.Today.AddDays(1))
            {
                currentString += "Night Pinball Game! ";
            }
            else
            {
                currentString += "Day Pinball Game! ";
            }
        }
        currentString += $"Sunset Starts At {sunsetUtc:HH:mm}";
        EventManager.TriggerEvent(EventKeys.OnAPICallCompleted, new object[] { currentString });
    }
}


[Serializable]
public class SunriseSunsetData
{
    public string sunrise;
    public string sunset;
    public string solar_noon;
    public string day_length;
    public string civil_twilight_begin;
    public string civil_twilight_end;
    public string nautical_twilight_begin;
    public string nautical_twilight_end;
    public string astronomical_twilight_begin;
    public string astronomical_twilight_end;
}

[Serializable]
public class SunriseSunsetResponse
{
    public SunriseSunsetData results;
    public string status;
}


